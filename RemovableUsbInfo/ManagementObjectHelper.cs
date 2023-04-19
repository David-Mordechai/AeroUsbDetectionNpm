using System.Management;

namespace RemovableUsbInfo
{
    public static class ManagementObjectHelper
    {
        public static string FindDevice(string pattern)
        {
            using var winUsbHubsCollection = new ManagementObjectSearcher(
                         $"select * from Win32_USBHub Where DeviceID Like '%{pattern}%'");

            foreach (var o in winUsbHubsCollection.Get())
            {
                var entity = (ManagementObject)o;

                foreach (var managementBaseObject in entity.GetRelated("Win32_USBController"))
                {
                    var controller = (ManagementObject)managementBaseObject;
                    using var winUsbControllersCollection =new ManagementObjectSearcher(
                                 "ASSOCIATORS OF {Win32_USBController.DeviceID='" + controller["PNPDeviceID"] + "'}");
                    foreach (var o1 in winUsbControllersCollection.Get())
                    {
                        var obj = (ManagementObject)o1;
                        if (obj.ToString().Contains("DeviceID") && obj["DeviceID"].ToString().Contains("USBSTOR"))
                            return obj["DeviceID"].ToString();
                    }
                }
            }
            return "*none*";
        }

        public static OutputDto GetDriveLetterAndLabel(string device)
        {
            using var collection = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (var managementBaseObject in collection.Get())
            {
                var drive = (ManagementObject)managementBaseObject;
                if (drive["PNPDeviceID"].ToString() != device) continue;

                foreach (var o1 in drive.GetRelated("Win32_DiskPartition"))
                {
                    var o = (ManagementObject)o1;
                    foreach (var managementBaseObject1 in o.GetRelated("Win32_LogicalDisk"))
                    {
                        var i = (ManagementObject)managementBaseObject1;
                        return new OutputDto {Path = i["Name"].ToString(), Label = i["VolumeName"].ToString()};
                    }
                }
            }
            return null;
        }
    }
}