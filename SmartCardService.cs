using System;
using Windows.Devices.SmartCards;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace IDBridge.Services
{
    public class SmartCardService
    {
        private SmartCardReader _reader;

        public SmartCardService()
        {
            InitializeSmartCardReader();
        }

        private async void InitializeSmartCardReader()
        {
            try
            {
                // Obtiene el selector de dispositivos para los lectores de tarjetas inteligentes.
                var deviceSelector = SmartCardReader.GetDeviceSelector();
                var deviceInformation = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(deviceSelector);

                if (deviceInformation.Count > 0)
                {
                    // Busca el dispositivo Gemalto CT40.
                    foreach (var deviceInfo in deviceInformation)
                    {
                        if (deviceInfo.Name.Contains("CT40"))
                        {
                            _reader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                            break;
                        }
                    }

                    if (_reader != null)
                    {
                        _reader.CardAdded += CardAdded;
                        NotifyStatus("CT40 device is successfully added.");
                    }
                    else
                    {
                        NotifyStatus("CT40 is not found.");
                    }
                }
                else
                {
                    NotifyStatus("No smart card readers found.");
                }
            }
            catch (Exception ex)
            {
                NotifyStatus($"Error initializing smart card reader: {ex.Message}");
            }
        }

        private async void CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            // Actualiza la UI en el hilo de la UI.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = (Frame)Windows.UI.Xaml.Window.Current.Content;
                if (frame.Content is MainPage mainPage)
                {
                    mainPage.UpdateStatus("Se detectó una tarjeta.");
                }
            });
        }

        private async void NotifyStatus(string message)
        {
            // Actualiza la UI en el hilo de la UI.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = (Frame)Windows.UI.Xaml.Window.Current.Content;
                if (frame.Content is MainPage mainPage)
                {
                    mainPage.UpdateStatus(message);
                }
            });
        }
    }
}
