import UsbEventsContorller from "../aero-usb-detection/index.mjs"

var usbController = new UsbEventsContorller();

usbController.on('attach', (device) =>{
    console.log(device);
})

usbController.on('detach', (device) =>{
    console.log(device);
})