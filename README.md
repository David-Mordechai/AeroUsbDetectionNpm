# aero-win-usb-detection

`aero-win-usb-detection` allows you to listen for attach/detach events of USB devices and get device path and label.

## Install

```sh
npm i aero-win-usb-detection
```

## Usage

```js
import UsbEventsContorller from "aero-win-usb-detection";

var usbController = new UsbEventsContorller();

usbController.startListening();

usbController.on("attach", (device) => {
  console.log(device);
});

usbController.on("detach", (device) => {
  console.log(device);
});

// To stop listening
// usbController.stopListening();
```
