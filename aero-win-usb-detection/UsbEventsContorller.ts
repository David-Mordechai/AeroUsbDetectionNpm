import { usb } from "usb";
import EventEmitter from "events";
import edge from "edge-js";
import path from "path";
import { fileURLToPath } from "url";

export default class UsbEventsContorller extends EventEmitter {
  drivesCache = {};
  dll: edge.Func<unknown, unknown>;

  constructor() {
    super();
    const __filename = fileURLToPath(import.meta.url);
    const __dirname = path.dirname(__filename);
    const rootPath = path.join(__dirname, "/RemovableUsbInfo.dll");
    this.dll = edge.func(rootPath);
  }

  async startListening() {
    usb.on("attach", (device) => {
      console.log("attach");
      var payload = {
        ProductId: device.deviceDescriptor.idProduct,
        VendorId: device.deviceDescriptor.idVendor,
      };
      var _this = this;
      this.dll(payload, function (error, result) {
        if (error) {
          console.error(error);
          return;
        }

        if (result == null) return;

        var driveKey = `${device.deviceDescriptor.idProduct}_${device.deviceDescriptor.idVendor}`;
        _this.drivesCache[driveKey] = result;
        _this.emit("attach", result);
      });
    });

    usb.on("detach", (device) => {
      console.log("detach");

      var driveKey = `${device.deviceDescriptor.idProduct}_${device.deviceDescriptor.idVendor}`;
      var result = this.drivesCache[driveKey];

      if (result) {
        delete this.drivesCache[driveKey];
        this.emit("detach", result);
      }
    });
  }

  stopListening(){
    usb.removeAllListeners();
  }
}
