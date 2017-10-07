namespace RaspberryLEDCube.CanonicalSchema.Schema
{
    public sealed class Color3
    {
        public Color3(byte r, byte g, byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
    }
}
