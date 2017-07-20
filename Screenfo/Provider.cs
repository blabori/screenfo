using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Screenfo
{
    public static class Provider
    {
        /// <summary>
        /// Reads the EDID block from Windows registry for the given device.
        /// </summary>
        /// <returns>A byte array containing the found EDID block.</returns>
        private static byte[] GetMonitorEDID(IntPtr deviceInfo, Win32.SP_DEVINFO_DATA deviceInfoData)
        {

            // Open device specific registry key
            IntPtr deviceRegistryKey = Win32.SetupDiOpenDevRegKey(deviceInfo, ref deviceInfoData, 0x00000001, 0, 0x00000001, Win32.KEY_QUERY_VALUE);

            if (deviceRegistryKey == IntPtr.Zero)
            {
                // No suitable registry key found
                throw new Exception("Failed to open registry key for device");
            }

            // Read EDID into buffer
            IntPtr buf = Marshal.AllocHGlobal((int)256);
            try
            {
                RegistryValueKind regKeyType = RegistryValueKind.Binary;
                int length = 256;
                uint result = Win32.RegQueryValueEx(deviceRegistryKey, "EDID", 0, ref regKeyType, buf, ref length);
                if (result != 0)
                {
                    throw new Exception("Error when reading EDID for device " + deviceInfoData.classGuid);
                }
            }
            finally
            {
                Win32.RegCloseKey(deviceRegistryKey);
            }

            // Copy EDID in return buffer
            byte[] returnBuf = new byte[256];
            Marshal.Copy(buf, returnBuf, 0, 256);
            Marshal.FreeHGlobal(buf);
            return returnBuf;
        }


        /// <summary>
        /// Returns a list (of type Monitor) containing all found devices.
        /// </summary>
        /// <returns>The list of found devices (can be empty). Else null.</returns>
        public static List<Monitor> GetConnectedScreens()
        {
            List<Monitor> listOfConnectedMonitors = new List<Monitor>();

            Guid deviceGuid = new Guid(Win32.GUID_DEVINTERFACE_MONITOR);

            // Get all devices that satisfy the DEVINTERFACE_MONITOR class
            IntPtr deviceInfo = Win32.SetupDiGetClassDevs(ref deviceGuid, IntPtr.Zero, IntPtr.Zero, (uint)(Win32.DIGCF_FLAG.DIGCF_PRESENT | Win32.DIGCF_FLAG.DIGCF_DEVICEINTERFACE));

            if (deviceInfo.ToInt64() != Win32.INVALID_HANDLE_VALUE)
            {
                bool successful = true;
                uint i = 0;

                while (successful)
                {
                    Win32.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new Win32.SP_DEVICE_INTERFACE_DATA();
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                    successful = Win32.SetupDiEnumDeviceInterfaces(deviceInfo, IntPtr.Zero, ref deviceGuid, i, ref deviceInterfaceData);

                    if (successful)
                    {
                        Win32.SP_DEVINFO_DATA deviceInfoData = new Win32.SP_DEVINFO_DATA();
                        deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                        Win32.SP_DEVICE_INTERFACE_DETAIL_DATA detail = new Win32.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        if (IntPtr.Size == 8)
                        {
                            detail.cbSize = 8;  // x86_64
                        }
                        else
                        {
                            detail.cbSize = 4 + Marshal.SystemDefaultCharSize; // x86
                        }

                        // get buffer size
                        uint bufferSize = 0;
                        Win32.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

                        // retrieve detailed device information
                        Win32.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref deviceInterfaceData, ref detail, bufferSize, IntPtr.Zero, ref deviceInfoData);
                        IntPtr instanceBuf = Marshal.AllocHGlobal((int)bufferSize);
                        Win32.CM_Get_Device_ID(deviceInfoData.devInst, instanceBuf, (int)bufferSize, 0);
                        string instanceID = Marshal.PtrToStringAuto(instanceBuf);

                        // get EDID block to parse VESA information
                        byte[] edid = { 0, 0, 0, 0 };
                        edid = GetMonitorEDID(deviceInfo, deviceInfoData);

                        // create search keys
                        string sSerFind = new string(new char[] { (char)00, (char)00, (char)00, (char)0xff });
                        string sModFind = new string(new char[] { (char)00, (char)00, (char)00, (char)0xfc });

                        string[] sDescriptor = new string[4];
                        sDescriptor[0] = Encoding.Default.GetString(edid, 0x36, 18);
                        sDescriptor[1] = Encoding.Default.GetString(edid, 0x48, 18);
                        sDescriptor[2] = Encoding.Default.GetString(edid, 0x5A, 18);
                        sDescriptor[3] = Encoding.Default.GetString(edid, 0x6C, 18);

                        string sSerial = "";
                        string sModel = "";
                        var horRes = ((edid[54 + 4] >> 4) << 8) | edid[54 + 2];
                        var vertRes = ((edid[54 + 7] >> 4) << 8) | edid[54 + 5];

                        // do the search
                        foreach (string sDesc in sDescriptor)
                        {
                            if (sDesc.Contains(sSerFind))
                            {
                                sSerial = sDesc.Substring(4).Replace("\0", "").Trim();
                            }

                            if (sDesc.Contains(sModFind))
                            {
                                sModel = sDesc.Substring(4).Replace("\0", "").Trim();
                            }
                        }

                        listOfConnectedMonitors.Add(new Monitor(sModel, sSerial, horRes, vertRes));

                        Marshal.FreeHGlobal(instanceBuf);

                    }

                    i++;
                }

            }

            Win32.SetupDiDestroyDeviceInfoList(deviceInfo);
            return listOfConnectedMonitors;
        }
    }
}
