using UnityEngine;

namespace shared.math
{
    public class Conv
    {
        private const double _d2r = Mathf.PI / 180.0;
        private const double _r2d = 180.0 / Mathf.PI;

        public Conv()
        {
        }

        public static double Deg2Rad(double degrees)
        {
            return degrees * _d2r;
        }

        public static double Rad2Deg(double radians)
        {
            return radians * _r2d;
        }
    }
}