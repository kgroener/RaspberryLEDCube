using RaspberryLEDCube.CanonicalSchema.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RaspberryLEDCube.CanonicalSchema.Protocol
{


    public class ProtocolBulkColorBuffer : IEnumerable<byte>
    {
        private const byte ESCAPE_BYTE = 16;
        private const byte BULK_START_BYTE = 1;
        private const byte END_BYTE = 3;
        private const byte CLEAR_BYTE = 4;
        private readonly IEnumerable<ProtocolColorBuffer> _buffers;

        public ProtocolBulkColorBuffer(IEnumerable<ProtocolColorBuffer> buffers)
        {
            _buffers = buffers;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return GetBytes().ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetBytes().GetEnumerator();
        }

        private void AddDataByte(List<byte> bytes, byte b)
        {
            if (b == ESCAPE_BYTE)
            {
                bytes.Add(ESCAPE_BYTE);
            }

            bytes.Add(b);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ESCAPE_BYTE);
            bytes.Add(BULK_START_BYTE);

            foreach (var buffer in _buffers)
            {
                foreach(var color in buffer.AsColorArray())
                {
                    AddDataByte(bytes, color.Red);
                    AddDataByte(bytes, color.Green);
                    AddDataByte(bytes, color.Blue);
                }
            }

            bytes.Add(ESCAPE_BYTE);
            bytes.Add(END_BYTE);

            return bytes.ToArray();
        }
    }
}
