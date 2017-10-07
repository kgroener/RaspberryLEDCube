using RaspberryLEDCube.CanonicalSchema.Enums;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using RaspberryLEDCube.CubeControl.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RaspberryLEDCube
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            LEDCubeController controller = new LEDCubeController(8, 8, 8);

            await controller.InitializeAsync();

            controller.StartLEDCube();

            while (true)
            {
                controller.Clear();
                controller.Draw();
                
                await Task.Delay(1000);
            }

            //_deferral.Complete();
        }
    }
}
