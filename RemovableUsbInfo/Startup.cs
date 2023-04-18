using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace RemovableUsbInfo
{
    public class Startup
    {
        private static readonly Dictionary<string, string> ConnectedDevices = new Dictionary<string, string>();

        public Task<object> Start(object input)
        {
            Debugger.Launch();
            var allDrives = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable);
            foreach (var drive in allDrives)
            {
                ConnectedDevices.Add(drive.RootDirectory.FullName, drive.VolumeLabel);
            }
            return Task.FromResult((object)true);
        }

        public Task<object> Attach(object input)
        {
            System.Console.WriteLine(ConnectedDevices.Count);
            var allDrives = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable);
            var result = string.Empty;
            foreach (var drive in allDrives)
            {
                if(ConnectedDevices.ContainsKey(drive.RootDirectory.FullName))
                    continue;

                ConnectedDevices.Add(drive.RootDirectory.FullName, drive.VolumeLabel);
                result = $"{{ Path: {drive.RootDirectory.FullName}, Label: {drive.VolumeLabel} }}";
            }
            return Task.FromResult((object)result);
        }

        public Task<object> Detach(object input)
        {
            System.Console.WriteLine(ConnectedDevices.Count);
            var allDrives = DriveInfo.GetDrives()
                .Where(x => x.DriveType == DriveType.Removable)
                .Select(x => x.RootDirectory.FullName)
                .ToList();

            var result = string.Empty;
            var keyToRemove = string.Empty;

            foreach(var key in ConnectedDevices.Keys)
            {
                if(allDrives.Contains(key))
                    continue;

                var label = ConnectedDevices[key];
                keyToRemove = key;
                result = $"{{ Path: {key}, Label: {label} }}";
            }

            if (!string.IsNullOrEmpty(keyToRemove))
            {
                ConnectedDevices.Remove(keyToRemove);
            }
            
            return Task.FromResult((object)result);
        }
    }

    public static class Helper
    {
        public static string FindPath(string pattern)
        {
            var usbObjects = new List<string>();
            var deviceId = "*none*";

            foreach (var o in new ManagementObjectSearcher(
                         $"select * from Win32_USBHub Where DeviceID Like '%{pattern}%'").Get())
            {
                var entity = (ManagementObject)o;
                deviceId = entity["DeviceID"].ToString();

                foreach (var managementBaseObject in entity.GetRelated("Win32_USBController"))
                {
                    var controller = (ManagementObject)managementBaseObject;
                    foreach (var o1 in new ManagementObjectSearcher(
                                 "ASSOCIATORS OF {Win32_USBController.DeviceID='"
                                 + controller["PNPDeviceID"] + "'}").Get())
                    {
                        var obj = (ManagementObject)o1;
                        if (obj.ToString().Contains("DeviceID"))
                            usbObjects.Add(obj["DeviceID"].ToString());
                    }
                }
            }

            var vIdpIdPosition = usbObjects.IndexOf(deviceId);
            for (var i = vIdpIdPosition; i <= usbObjects.Count; i++)
            {
                if (usbObjects[i].Contains("USBSTOR"))
                {
                    return usbObjects[i];
                }
            }

            return "*none*";
        }

        public static Tuple<string, string> GetDriveLetter(string device)
        {
            foreach (var managementBaseObject in new ManagementObjectSearcher("select * from Win32_DiskDrive").Get())
            {
                var drive = (ManagementObject)managementBaseObject;
                if (drive["PNPDeviceID"].ToString() != device) continue;

                foreach (var o1 in drive.GetRelated("Win32_DiskPartition"))
                {
                    var o = (ManagementObject)o1;
                    foreach (var managementBaseObject1 in o.GetRelated("Win32_LogicalDisk"))
                    {
                        var i = (ManagementObject)managementBaseObject1;
                        return new Tuple<string, string>(i["Name"].ToString(), i["VolumeName"].ToString());
                    }
                }
            }
            return null;
        }
    }
}
