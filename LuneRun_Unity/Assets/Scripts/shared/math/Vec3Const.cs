using System;

namespace shared.math
{
    public class Vec3Const
    {
        protected double _x;
        protected double _y;
        protected double _z;
        private const double EpsilonSqr = 1e-10;
        
        public static readonly Vec3Const forwardVector = new Vec3Const(0, 0, 1);

        public Vec3Const(double x = 0, double y = 0, double z = 0)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public double x { get { return _x; } }
        public double y { get { return _y; } }
        public double z { get { return _z; } }

        public Vec3 clone()
        {
            return new Vec3(_x, _y, _z);
        }
        
        public static Vec3 FromN(Vec3Const v)
        {
            return new Vec3(v.x, v.y, v.z);
        }

        // Convert to Unity Vector3 (if needed)
        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3((float)_x, (float)_y, (float)_z);
        }

        public Vec3 add(Vec3Const other)
        {
            return new Vec3(_x + other._x, _y + other._y, _z + other._z);
        }

        public Vec3 addXYZ(double x, double y, double z)
        {
            return new Vec3(_x + x, _y + y, _z + z);
        }

        public Vec3 sub(Vec3Const other)
        {
            return new Vec3(_x - other._x, _y - other._y, _z - other._z);
        }

        public Vec3 subXYZ(double x, double y, double z)
        {
            return new Vec3(_x - x, _y - y, _z - z);
        }

        public Vec3 mul(Vec3Const other)
        {
            return new Vec3(_x * other._x, _y * other._y, _z * other._z);
        }

        public Vec3 mulXYZ(double x, double y, double z)
        {
            return new Vec3(_x * x, _y * y, _z * z);
        }

        public Vec3 div(Vec3Const other)
        {
            return new Vec3(_x / other._x, _y / other._y, _z / other._z);
        }

        public Vec3 divXYZ(double x, double y, double z)
        {
            return new Vec3(_x / x, _y / y, _z / z);
        }

        public Vec3 scale(double s)
        {
            return new Vec3(_x * s, _y * s, _z * s);
        }

        public Vec3 rescale(double newLength)
        {
            double len = length();
            if (len == 0) return new Vec3();
            double factor = newLength / len;
            return new Vec3(_x * factor, _y * factor, _z * factor);
        }

        public Vec3 normalize()
        {
            double len = length();
            if (len == 0) return new Vec3();
            double inv = 1.0 / len;
            return new Vec3(_x * inv, _y * inv, _z * inv);
        }

        public double dot(Vec3Const other)
        {
            return _x * other._x + _y * other._y + _z * other._z;
        }

        public double length()
        {
            return Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }

        public double lengthSqr()
        {
            return _x * _x + _y * _y + _z * _z;
        }

        public double distance(Vec3Const other)
        {
            double dx = _x - other._x;
            double dy = _y - other._y;
            double dz = _z - other._z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double distanceXYZ(double x, double y, double z)
        {
            double dx = _x - x;
            double dy = _y - y;
            double dz = _z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double distanceSqr(Vec3Const other)
        {
            double dx = _x - other._x;
            double dy = _y - other._y;
            double dz = _z - other._z;
            return dx * dx + dy * dy + dz * dz;
        }

        public double distanceXYZSqr(double x, double y, double z)
        {
            double dx = _x - x;
            double dy = _y - y;
            double dz = _z - z;
            return dx * dx + dy * dy + dz * dz;
        }

        public bool equals(Vec3Const other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public bool equalsXYZ(double x, double y, double z)
        {
            return _x == x && _y == y && _z == z;
        }

        public bool isNormalized()
        {
            return Math.Abs(_x * _x + _y * _y + _z * _z - 1) < EpsilonSqr;
        }

        public override string ToString()
        {
            return string.Format("Vec3Const({0}, {1}, {2})", _x, _y, _z);
        }
    }
}