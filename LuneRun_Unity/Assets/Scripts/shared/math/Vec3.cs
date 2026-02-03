using UnityEngine;

namespace shared.math
{
    public class Vec3 : Vec3Const
    {
        public Vec3(double x = 0, double y = 0, double z = 0) : base(x, y, z)
        {
        }

        public new double x
        {
            get { return _x; }
            set { _x = value; }
        }

        public new double y
        {
            get { return _y; }
            set { _y = value; }
        }

        public new double z
        {
            get { return _z; }
            set { _z = value; }
        }
        public Vec3 Copy(Vec3Const other) => copy(other);
        public Vec3 copy(Vec3Const other)
        {
            _x = other.x;
            _y = other.y;
            _z = other.z;
            return this;
        }

        public Vec3 setTo(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            return this;
        }

        public Vec3 zero()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            return this;
        }

        public Vec3 addSelf(Vec3Const other)
        {
            _x += other.x;
            _y += other.y;
            _z += other.z;
            return this;
        }

        public Vec3 addXYZSelf(double x, double y, double z)
        {
            _x += x;
            _y += y;
            _z += z;
            return this;
        }

        public Vec3 subSelf(Vec3Const other)
        {
            _x -= other.x;
            _y -= other.y;
            _z -= other.z;
            return this;
        }

        public Vec3 subXYZSelf(double x, double y, double z)
        {
            _x -= x;
            _y -= y;
            _z -= z;
            return this;
        }

        public Vec3 mulSelf(Vec3Const other)
        {
            _x *= other.x;
            _y *= other.y;
            _z *= other.z;
            return this;
        }

        public Vec3 mulXYZSelf(double x, double y, double z)
        {
            _x *= x;
            _y *= y;
            _z *= z;
            return this;
        }

        public Vec3 divSelf(Vec3Const other)
        {
            _x /= other.x;
            _y /= other.y;
            _z /= other.z;
            return this;
        }

        public Vec3 divXYZSelf(double x, double y, double z)
        {
            _x /= x;
            _y /= y;
            _z /= z;
            return this;
        }

        public Vec3 scaleSelf(double s)
        {
            _x *= s;
            _y *= s;
            _z *= s;
            return this;
        }

        public Vec3 normalizeSelf()
        {
            double len = length();
            if (len > 0)
            {
                _x /= len;
                _y /= len;
                _z /= len;
            }
            return this;
        }

        public Vec3 crossSelf(Vec3Const other)
        {
            double nx = _y * other.z - _z * other.y;
            double ny = _z * other.x - _x * other.z;
            double nz = _x * other.y - _y * other.x;
            _x = nx;
            _y = ny;
            _z = nz;
            return this;
        }

        public Vec3 cross(Vec3Const other)
        {
            double nx = _y * other.z - _z * other.y;
            double ny = _z * other.x - _x * other.z;
            double nz = _x * other.y - _y * other.x;

            return new Vec3(nx,ny, nz);
        }

        public Vec3 lerp(Vec3Const other, double alpha)
        {
            return new Vec3(
                _x * (1 - alpha) + other.x * alpha,
                _y * (1 - alpha) + other.y * alpha,
                _z * (1 - alpha) + other.z * alpha
            );
        }

        public override string ToString()
        {
            return string.Format("Vec3({0}, {1}, {2})", _x, _y, _z);
        }

        public static implicit operator Vector3(Vec3 v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }

        public static implicit operator Vec3(Vector3 v)
        {
            return new Vec3((float)v.x, (float)v.y, (float)v.z);
        }
    }
}