using System;
using System.Threading.Tasks;

namespace RemovableUsbInfo
{
    public class Startup
    {
        public Task<object> Invoke(dynamic input)
        {
            //Debugger.Launch();
            try
            {
                var vendorId = (int)input.VendorId;
                var productId = (int)input.ProductId;
                var hexProductId = productId.ToString("X4");
                var hexVendorId = vendorId.ToString("X4");
                var pattern = $"VID_{hexVendorId}&PID_{hexProductId}";

                var device = ManagementObjectHelper.FindDevice(pattern);
                var outputDto = ManagementObjectHelper.GetDriveLetterAndLabel(device);
                return Task.FromResult<object>(outputDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
