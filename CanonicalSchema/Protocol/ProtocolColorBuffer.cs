using RaspberryLEDCube.CanonicalSchema.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RaspberryLEDCube.CanonicalSchema.Protocol
{


    public class ProtocolColorBuffer : IEnumerable<byte>
    {
        private const byte ESCAPE_BYTE = 16;
        private const byte START_BYTE = 2;
        private const byte END_BYTE = 3;
        private const byte CLEAR_BYTE = 4;

        List<Color3> _colors;
        private byte _channel;
        private readonly int _numberOfColors;

        public byte Channel
        {
            get
            {
                return _channel;
            }

            set
            {
                _channel = value;
            }
        }

        public int NumberOfColors
        {
            get
            {
                return _numberOfColors;
            }
        }

        public ProtocolColorBuffer(int numberOfColors, byte channel)
        {
            _numberOfColors = numberOfColors;
            _colors = new List<Color3>();
            for (int i = 0; i < numberOfColors; i++)
            {
                _colors.Add(new Color3(0, 0, 0));
            }

            _channel = channel;
        }

        public Color3[] AsColorArray()
        {
            return _colors.ToArray();
        }

        private void AddDataByte(List<byte> bytes, byte b)
        {
            if (b == ESCAPE_BYTE)
            {
                bytes.Add(ESCAPE_BYTE);
            }

            bytes.Add(b);
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return GetBytes().ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetBytes().GetEnumerator();
        }

        public Color3 this[int index]
        {
            get { return _colors[index]; }
            //set { _colors[index] = value; }
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ESCAPE_BYTE);
            bytes.Add(START_BYTE);

            AddDataByte(bytes, _channel);

            foreach (var color in _colors)
            {
                AddDataByte(bytes, color.Red);
                AddDataByte(bytes, color.Green);
                AddDataByte(bytes, color.Blue);
            }

            bytes.Add(ESCAPE_BYTE);
            bytes.Add(END_BYTE);

            return bytes.ToArray();
        }
    }
}
