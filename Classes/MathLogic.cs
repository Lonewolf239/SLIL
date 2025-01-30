using System;

namespace SLIL.Classes
{
    internal class TPoint
    {
        internal double X { get; set; }
        internal double Y { get; set; }

        internal TPoint(double x, double y) { X = x; Y = y; }
    }

    internal class MathLogic
    {
        internal static double NormalizeAngle(double a)
        {
            a %= (2 * Math.PI);
            if (a > Math.PI) a -= 2 * Math.PI;
            else if (a < -Math.PI) a += 2 * Math.PI;
            return a;
        }

        internal static bool PointInFOV(double pX, double pY, double pA, double distance, TPoint point, double FOV = Math.PI / 3)
        {
            pA = NormalizeAngle(pA);
            double halfFOV = FOV / 2;
            double leftAngle = pA - halfFOV;
            double rightAngle = pA + halfFOV;
            TPoint leftFar = new TPoint(pX + distance * Math.Cos(leftAngle), pY + distance * Math.Sin(leftAngle));
            TPoint rightFar = new TPoint(pX + distance * Math.Cos(rightAngle), pY + distance * Math.Sin(rightAngle));
            return IsPointInRectangle(new TPoint(pX, pY), leftFar, rightFar, point);
        }

        private static bool IsPointInRectangle(TPoint playerPos, TPoint leftFar, TPoint rightFar, TPoint point)
        {
            TPoint v1 = new TPoint(leftFar.X - playerPos.X, leftFar.Y - playerPos.Y);
            TPoint v2 = new TPoint(rightFar.X - playerPos.X, rightFar.Y - playerPos.Y);
            TPoint vPoint = new TPoint(point.X - playerPos.X, point.Y - playerPos.Y);
            double dot1 = vPoint.X * v1.X + vPoint.Y * v1.Y;
            double dot2 = vPoint.X * v2.X + vPoint.Y * v2.Y;
            return dot1 >= 0 && dot1 <= (v1.X * v1.X + v1.Y * v1.Y) &&
                   dot2 >= 0 && dot2 <= (v2.X * v2.X + v2.Y * v2.Y);
        }

        internal static double GetDistance(TPoint point1, TPoint point2)
        {
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}