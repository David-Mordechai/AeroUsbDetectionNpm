using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace RemovableUsbInfo
{
    public static class ManagementObjectHelper
    {
        public static string FindDevice(string pattern)
        {
            foreach (var o in new ManagementObjectSearcher(
                         $"select * from Win32_USBHub Where DeviceID Like '%{pattern}%'").Get())
            {
                var entity = (ManagementObject)o;

                foreach (var managementBaseObject in entity.GetRelated("Win32_USBController"))
                {
                    var controller = (ManagementObject)managementBaseObject;
                    foreach (var o1 in new ManagementObjectSearcher(
                                 "ASSOCIATORS OF {Win32_USBController.DeviceID='" + controller["PNPDeviceID"] + "'}").Get())
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
            Console.WriteLine("GetDriveLetterAndLabel");
            foreach (var managementBaseObject in new ManagementObjectSearcher("select * from Win32_DiskDrive").Get())
            {
                Console.WriteLine("Win32_DiskDrive");
                var drive = (ManagementObject)managementBaseObject;
                if (drive["PNPDeviceID"].ToString() != device) continue;

                foreach (var o1 in drive.GetRelated("Win32_DiskPartition"))
                {
                    Console.WriteLine("Win32_DiskPartition");
                    var o = (ManagementObject)o1;
                    foreach (var managementBaseObject1 in o.GetRelated("Win32_LogicalDisk"))
                    {
                        Console.WriteLine("Win32_LogicalDisk");

                        var i = (ManagementObject)managementBaseObject1;
                        return new OutputDto {Path = i["Name"].ToString(), Label = i["VolumeName"].ToString()};
                    }
                }
            }
            Console.WriteLine("GetDriveLetterAndLabel did not find a label and drive latter");
            return null;
        }
    }
}