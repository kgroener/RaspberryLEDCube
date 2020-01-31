using LEDCube.Animations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Mathematics
{
    public class RotationMatrix
    {
        public RotationMatrix(double angleX, double angleY, double angleZ)
        {
            AngleX = angleX;
            AngleY = angleY;
            AngleZ = angleZ;
        }

        public double AngleX { get; }
        public double AngleY { get; }
        public double AngleZ { get; }

        public static Coordinate Rotate(Coordinate coordinate, Coordinate origin, RotationMatrix matrix)
        {
            var inputMatrix = new double[] { coordinate.X - origin.X, coordinate.Y - origin.Y, coordinate.Z - origin.Z, 1 };

            var rotationMatrix = new double[4, 4];

            var UVW = new[] {
                new { U = 1, V = 0, W = 0 },
                new { U = 0, V = 1, W = 0 },
                new { U = 0, V = 0, W = 1 },
            };
            var rotations = new[] { matrix.AngleX, matrix.AngleY, matrix.AngleZ }.Zip(UVW, (a, b) => new { Angle = a, b.U, b.V, b.W });

            foreach (var rotation in rotations)
            {
                var outputMatrix = new double[4];

                var angleRadians = rotation.Angle * Math.PI / 180.0;

                double u = rotation.U;
                double v = rotation.V;
                double w = rotation.W;
                double u2 = u * u;
                double v2 = v * v;
                double w2 = w * w;
                double L = u2 + v2 + w2;

                rotationMatrix[0, 0] = (u2 + ((v2 + w2) * Math.Cos(angleRadians))) / L;
                rotationMatrix[0, 1] = ((u * v * (1 - Math.Cos(angleRadians))) - (w * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[0, 2] = ((u * w * (1 - Math.Cos(angleRadians))) + (v * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[0, 3] = 0.0;

                rotationMatrix[1, 0] = ((u * v * (1 - Math.Cos(angleRadians))) + (w * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[1, 1] = (v2 + ((u2 + w2) * Math.Cos(angleRadians))) / L;
                rotationMatrix[1, 2] = ((v * w * (1 - Math.Cos(angleRadians))) - (u * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[1, 3] = 0.0;

                rotationMatrix[2, 0] = ((u * w * (1 - Math.Cos(angleRadians))) - (v * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[2, 1] = ((v * w * (1 - Math.Cos(angleRadians))) + (u * Math.Sqrt(L) * Math.Sin(angleRadians))) / L;
                rotationMatrix[2, 2] = (w2 + ((u2 + v2) * Math.Cos(angleRadians))) / L;
                rotationMatrix[2, 3] = 0.0;

                rotationMatrix[3, 0] = 0.0;
                rotationMatrix[3, 1] = 0.0;
                rotationMatrix[3, 2] = 0.0;
                rotationMatrix[3, 3] = 1.0;

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        outputMatrix[i] += rotationMatrix[j, i] * inputMatrix[j];
                    }
                }

                inputMatrix = outputMatrix;
            }

            return new Coordinate(inputMatrix[0] + origin.X, inputMatrix[1] + origin.Y, inputMatrix[2] + origin.Z);
        }
    }
}