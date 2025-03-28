﻿using System;

namespace SLIL.Classes
{
    internal class TPoint
    {
        internal double X { get; set; }
        internal double Y { get; set; }

        internal TPoint(double x, double y) { X = x; Y = y; }
    }

    internal class ML
    {
        internal static double NormalizeAngle(double a)
        {
            a %= (2 * Math.PI);
            if (a > Math.PI) a -= 2 * Math.PI;
            else if (a < -Math.PI) a += 2 * Math.PI;
            return a;
        }

        internal static double GetDistance(TPoint point1, TPoint point2)
        {
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        internal static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
        internal static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
        internal static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));

        internal static bool OutOfLimits(double value, double max) => OutOfLimits(value, 0, max);
        internal static bool OutOfLimits(float value, float max) => OutOfLimits(value, 0, max);
        internal static bool OutOfLimits(int value, int max) => OutOfLimits(value, 0, max);

        internal static bool OutOfLimits(double value, double min, double max) => value < min || value > max;
        internal static bool OutOfLimits(float value, float min, float max) => value < min || value > max;
        internal static bool OutOfLimits(int value, int min, int max) => value < min || value > max;

        internal static bool WithinOne(double value) => value >= 0 && value <= 1;
        internal static bool WithinOne(float value) => value >= 0 && value <= 1;
        internal static bool WithinOne(int value) => value >= 0 && value <= 1;
    }
}