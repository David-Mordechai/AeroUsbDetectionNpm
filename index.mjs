import { usb } from 'usb';
import EventEmitter from 'events';

export default class UsbEventsContorller extends EventEmitter {

    constructor() {

    }

    async startListing() {
        usb.on('attach', () => {
            console.log('attach');
        });

        usb.on('detach', () => {
            console.log('detach');
        });
    }

}
