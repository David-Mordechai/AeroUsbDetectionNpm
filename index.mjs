import { usb } from 'usb';
import EventEmitter from 'events';
import edge from 'edge-js'

export default class UsbEventsContorller extends EventEmitter {

    drivesCache = {};

    constructor() {
        super()
        this.dll = edge.func('./RemovableUsbInfo.dll');
    }

    async startListing() {
        usb.on('attach', async (device) => {
            await this.handleAttach(device)
            
        });

        usb.on('detach', (device) => {
            console.log('detach');

            var driveKey = `${device.deviceDescriptor.idProduct}_${device.deviceDescriptor.idVendor}`;
            var result = this.drivesCache[driveKey];

            if (result) {
                console.log(`detached ${result.Path}, ${result.Label}`);
                delete this.drivesCache[driveKey];
            }
        });
    }

    async handleAttach(device){
        console.log('attach');
        var payload = {
            ActionType: 'Attach',
            ProductId: device.deviceDescriptor.idProduct,
            VendorId: device.deviceDescriptor.idVendor
        }
        var _this = this;
        this.dll(payload, function (error, result) {
            if (error) {
                console.error(error);
                return;
            }

            if(result == null)
                return;
                
            var driveKey = `${device.deviceDescriptor.idProduct}_${device.deviceDescriptor.idVendor}`;
            _this.drivesCache[driveKey] = result;
            console.log(`attached ${result.Path}, ${result.Label}`);
        });
    }
}

const a = new UsbEventsContorller();
a.startListing();