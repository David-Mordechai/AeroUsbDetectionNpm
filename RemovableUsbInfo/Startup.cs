using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RemovableUsbInfo
{
    public class Startup
    {
        private static readonly Dictionary<string, OutputDto> ConnectedDevices = new Dictionary<string, OutputDto>();

        public Task<object> Invoke(dynamic input)
        {
            //Debugger.Launch();
            
            var actionType = (string)input.ActionType;
            var vendorId = (int)input.VendorId;
            var productId = (int)input.ProductId;
            if ((actionType is "Attach" || actionType is "Detach") is false) return null;
            
            var hexProductId = productId.ToString("X4");
            var hexVendorId = vendorId.ToString("X4");
            var pattern = $"VID_{hexVendorId}&PID_{hexProductId}";

            return actionType is "Attach" ? 
                HandleAttach(pattern) : HandleDetach(pattern);
        }

        private static Task<object> HandleAttach(string pattern)
        {
            var stopWatch = Stopwatch.StartNew();
            var device = ManagementObjectHelper.FindDevice(pattern);
            stopWatch.Stop();
            Console.WriteLine($"FindDevice: {stopWatch.Elapsed}");

            stopWatch.Restart();
            var outputDto = ManagementObjectHelper.GetDriveLetterAndLabel(device);
            stopWatch.Stop();
            Console.WriteLine($"GetDriveLetterAndLabel: {stopWatch.Elapsed}");

            ConnectedDevices.Add(pattern, outputDto);
            return Task.FromResult<object>(outputDto);
        }

        private static Task<object> HandleDetach(string pattern)
        {
            if (ConnectedDevices.TryGetValue(pattern, out var outputDto) is false) return Task.FromResult<object>(null);
            ConnectedDevices.Remove(pattern);
            return Task.FromResult<object>(outputDto);
        }
    }
}
