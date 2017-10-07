using RaspberryLEDCube.CanonicalSchema.Enums;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using RaspberryLEDCube.CanonicalSchema.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class LEDCubeController
    {
        private readonly LEDController _ledController;
        private readonly PSUController _psuController;
        private readonly ProtocolColorBuffer[] _cubeColorBuffers;
        private readonly byte _length;
        private readonly byte _width;
        private readonly byte _height;

        public LEDCubeController(byte length, byte width, byte height)
        {
            _length = length;
            _width = width;
            _height = height;

            var layers = height;
            var ledsPerLayer = length * width;

            _cubeColorBuffers = Enumerable.Range(0, layers).Select(l => new ProtocolColorBuffer(ledsPerLayer, (byte)l)).ToArray();

            _psuController = new PSUController(24, 23);
            _ledController = new LEDController(ChipSelectLines.ChipSelectPin24);
        }

        public async Task InitializeAsync()
        {
            await _psuController.InitializeAsync();
            await _ledController.InitializeAsync();
        }

        public void StartLEDCube()
        {
            if (_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already started.");
            }

            Clear();
            _psuController.TurnPowerOn();
            Draw();
        }

        public void ShutdownLEDCube()
        {
            if (!_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already shutdown.");
            }

            _psuController.TurnPowerOff();
        }

        public void Clear()
        {
            Fill(new Color3(0, 0, 0));
        }

        public void Fill(Color3 fillColor)
        {
            for (byte x = 0; x < _length; x++)
            {
                for (byte y = 0; y < _width; y++)
                {
                    for (byte z = 0; z < _height; z++)
                    {
                        SetLEDColor(x, y, z, fillColor);
                    }
                }
            }
        }

        public void Draw()
        {
            if (!_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is not yet started.");
            }

            foreach (var colorBuffer in _cubeColorBuffers)
            {
                _ledController.WriteColorBuffer(colorBuffer);
            }
        }

        public void SetLEDColor(byte x, byte y, byte z, Color3 color)
        {
            int index = GetLEDBufferIndex(x, y);
            SetLEDColorInBuffer(z, index, color);
        }

        public Color3 GetLEDColor(byte x, byte y, byte z)
        {
            int index = GetLEDBufferIndex(x, y);
            return GetLEDColorFromBuffer(z, index);
        }

        private void SetLEDColorInBuffer(byte layer, int index, Color3 color)
        {
            _cubeColorBuffers[layer][index].Red = color.Red;
            _cubeColorBuffers[layer][index].Green = color.Green;
            _cubeColorBuffers[layer][index].Blue = color.Blue;
        }

        private Color3 GetLEDColorFromBuffer(byte layer, int index)
        {
            Color3 color = _cubeColorBuffers[layer][index];
            return new Color3(color.Red, color.Green, color.Blue);
        }

        private int GetLEDBufferIndex(byte x, byte y)
        {
            int index;

            var isYEven = (y % 2) == 0;

            if (isYEven)
            {
                index = x + (y * _length);
            }
            else
            {
                index = (_length - x - 1) + (y * _length);
            }

            return index;
        }


    }
}
