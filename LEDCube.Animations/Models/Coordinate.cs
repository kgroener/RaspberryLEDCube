using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Models
{
    public class AbsoluteCoordinate : IEquatable<AbsoluteCoordinate>
    {
        public AbsoluteCoordinate()
        {
        }

        public AbsoluteCoordinate(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public static bool operator !=(AbsoluteCoordinate left, AbsoluteCoordinate right) => !(left == right);

        public static bool operator ==(AbsoluteCoordinate left, AbsoluteCoordinate right) => EqualityComparer<AbsoluteCoordinate>.Default.Equals(left, right);

        public override bool Equals(object other)
        {
            return Equals(other as AbsoluteCoordinate);
        }

        public bool Equals(AbsoluteCoordinate other)
        {
            return other != null &&
                   X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public bool Equals(int x, int y, int z)
        {
            return X == x && Y == y && Z == z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            hashCode = (hashCode * -1521134295) + Z.GetHashCode();
            return hashCode;
        }
    }

    public class AbsoluteLEDCoordinate
    {
        public Color Color { get; set; }
        public AbsoluteCoordinate Coordinate { get; set; }
    }

    public class Coordinate
    {
        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class LEDCoordinate
    {
        public Color Color { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}