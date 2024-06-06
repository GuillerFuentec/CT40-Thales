using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace IDBridge.Services
{
    public class DeviceService
    {
        public DeviceService()
        {
            ListConnectedDevices();
        }

        private async void ListConnectedDevices()
        {
            try
            {
                Debug.WriteLine("Initializing DeviceService...");

                // Obtiene el selector de dispositivos para todos los dispositivos presentes
                string[] deviceSelectors = new string[]
                {
                    "System.Devices.InterfaceClassGuid:=\"{50dd5230-ba8a-11d1-bf5d-0000f805f530}\"", // USB Devices
                    "System.Devices.InterfaceClassGuid:=\"{4d36e97b-e325-11ce-bfc1-08002be10318}\"", // Disk Drives
                    "System.Devices.InterfaceClassGuid:=\"{4d36e968-e325-11ce-bfc1-08002be10318}\"", // Storage Controllers
                    "System.Devices.InterfaceClassGuid:=\"{4d36e97d-e325-11ce-bfc1-08002be10318}\""  // Smart Card Readers
                };

                var nonPeripheralDevices = new List<DeviceInformation>();

                foreach (var selector in deviceSelectors)
                {
                    var deviceInformationCollection = await DeviceInformation.FindAllAsync(selector);
                    foreach (var deviceInfo in deviceInformationCollection)
                    {
                        if (!IsPeripheralDevice(deviceInfo))
                        {
                            nonPeripheralDevices.Add(deviceInfo);
                        }
                    }
                }

                Debug.WriteLine($"Found {nonPeripheralDevices.Count} non-peripheral devices.");

                foreach (var device in nonPeripheralDevices)
                {
                    Debug.WriteLine($"Device: {device.Name}, Id: {device.Id}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        private bool IsPeripheralDevice(DeviceInformation deviceInfo)
        {
            // Lista de identificadores de periféricos comunes
            string[] peripheralNames = { "Mouse", "Keyboard", "Monitor", "Display", "Speaker", "Audio" };
            foreach (var name in peripheralNames)
            {
                if (deviceInfo.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
