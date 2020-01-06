﻿using LEDCube.Animations.Animations;
using LEDCube.Animations.Animations.Trigonometry;
using LEDCube.Animations.Controllers;
using LEDCube.Animations.Helpers;
using RaspberryLEDCube.CubeControl.Controllers;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RaspberryLEDCube
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private PSUController _psuController;
        private LEDCubeController _cubeController;
        private AnimationController _animationController;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            
            _psuController = new PSUController(24, 23);
            await _psuController.InitializeAsync();
            
            var ledController = new LEDController(CanonicalSchema.Enums.ChipSelectLines.ChipSelectPin24);
            await ledController.InitializeAsync();

            _cubeController = new LEDCubeController(ledController);

            _animationController = new AnimationController(_cubeController);

            await StartLEDCubeAsync();

            _animationController.RequestAnimation<DropletWaveAnimation>(LEDCube.Animations.Enums.AnimationPriority.Normal);
            _animationController.RequestAnimation<SineAnimation>(LEDCube.Animations.Enums.AnimationPriority.Normal);
            _animationController.RequestAnimation<GameOfLifeAnimation>(LEDCube.Animations.Enums.AnimationPriority.Normal);
            _animationController.Start();

            //var hue = 0.0;

            //while (true)
            //{
            //    var color = ColorHelper.HSVToColor(hue, 1, 0.1);

            //    _cubeController.Fill(Color.FromArgb(color.R, color.G, color.B));
            //    await _cubeController.DrawAsync();
            //    await Task.Delay(50);

            //    hue += 1;
            //}

            //_deferral.Complete();
        }

        private Task StartLEDCubeAsync()
        {
            if (_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already started.");
            }

            _cubeController.Clear();
            _psuController.TurnPowerOn();
            return _cubeController.DrawAsync();
        }

        private void ShutdownLEDCube()
        {
            if (!_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already shutdown.");
            }

            _psuController.TurnPowerOff();
        }
    }
}
