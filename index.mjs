import { usb } from 'usb';
import EventEmitter from 'events';
import edge from 'edge-js'

export default class UsbEventsContorller extends EventEmitter {

    constructor() {
        super()
        this.startMethod = edge.func({assemblyFile: './RemovableUsbInfo.dll', methodName: 'Start'});
        this.startMethod("", function(error, result){
            if(error) console.error(error)
        })

        this.attachMethod = edge.func({assemblyFile: './RemovableUsbInfo.dll', methodName: 'Attach'});
        this.detachMethod = edge.func({assemblyFile: './RemovableUsbInfo.dll', methodName: 'Detach'});
    }

    async startListing() {
        usb.on('attach', (device) => {
            console.log(device);
            console.log('attach');
            this.attachMethod("", function (error, result) { 
                if(error) console.error(error)
                console.log(result);
            });
        });

        usb.on('detach', () => {
            console.log('detach');
            this.detachMethod("", function (error, result) { 
                if(error) console.error(error)
                console.log(result);
            });
        });
    }

}

const a = new UsbEventsContorller();
a.startListing();