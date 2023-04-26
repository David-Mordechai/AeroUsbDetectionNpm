# Install

```sh
npm i aero-win-usb-detection
```

## Usage

```js
import UsbEventsContorller from "aero-win-usb-detection";

var usbController = new UsbEventsContorller();

usbController.startListing();

usbController.on("attach", (device) => {
  console.log(device);
});

usbController.on("detach", (device) => {
  console.log(device);
});

// To stop listening
// usbController.stopListening();
```
