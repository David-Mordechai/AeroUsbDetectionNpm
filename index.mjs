import { usb } from 'usb';
import EventEmitter from 'events';
import edge from 'edge-js'

export default class UsbEventsContorller extends EventEmitter {

    constructor() {
        super()
        this.dll = edge.func('./RemovableUsbInfo.dll');
    }

    async startListing() {
        usb.on('attach', (device) => {
            console.log('attach');
            var payload = {
                ActionType: 'Attach',
                ProductId: device.deviceDescriptor.idProduct,
                VendorId: device.deviceDescriptor.idVendor 
            }
            this.dll(payload, function (error, result) { 
                if(error) console.error(error)
                console.log(result);
            });
        });

        usb.on('detach', (device) => {
            console.log('detach');
            var payload = {
                ActionType: 'Detach',
                ProductId: device.deviceDescriptor.idProduct,
                VendorId: device.deviceDescriptor.idVendor 
            }
            this.dll(payload, function (error, result) { 
                if(error) console.error(error)
                console.log(result);
            });
        });
    }
}

const a = new UsbEventsContorller();
a.startListing();