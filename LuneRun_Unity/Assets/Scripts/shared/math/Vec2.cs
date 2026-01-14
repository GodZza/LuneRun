using System;

namespace shared.math
{
    public class Vec2Const
    {
        protected double _x;
        protected double _y;

        public Vec2Const(double x = 0, double y = 0)
        {
            _x = x;
            _y = y;
        }

        public double x => _x;
        public double y => _y;

        public virtual Vec2 clone()
        {
            return new Vec2(_x, _y);
        }

        public virtual Vec2 Add(Vec2Const other)
        {
            return new Vec2(_x + other._x, _y + other._y);
        }

        public virtual Vec2 AddXY(double dx, double dy)
        {
            return new Vec2(_x + dx, _y + dy);
        }

        // Additional methods can be added as needed
    }

    public class Vec2 : Vec2Const
    {
        public Vec2(double x = 0, double y = 0) : base(x, y) { }

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

        public Vec2 Copy(Vec2Const source)
        {
            _x = source.x;
            _y = source.y;
            return this;
        }

        public Vec2 CopyXY(double nx, double ny)
        {
            _x = nx;
            _y = ny;
            return this;
        }

        public Vec2 Zero()
        {
            _x = 0;
            _y = 0;
            return this;
        }

        public Vec2 AddSelf(Vec2Const other)
        {
            _x += other.x;
            _y += other.y;
            return this;
        }

        public Vec2 AddXYSelf(double dx, double dy)
        {
            _x += dx;
            _y += dy;
            return this;
        }

        // Override clone to return Vec2
        public override Vec2 clone()
        {
            return new Vec2(_x, _y);
        }

        // Override Add to return Vec2
        public override Vec2 Add(Vec2Const other)
        {
            return new Vec2(_x + other.x, _y + other.y);
        }

        public override Vec2 AddXY(double dx, double dy)
        {
            return new Vec2(_x + dx, _y + dy);
        }
    }
}