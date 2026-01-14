using shared.math;

namespace shared.algorithm.spatial
{
    public class SpatialHashValue
    {
        public double x1, y1, x2, y2;
        public int cx1, cy1, cx2, cy2;
        public uint timeStamp = 0;

        public SpatialHashValue(double x1, double y1, double x2, double y2)
        {
            Update(x1, y1, x2, y2);
        }

        public bool Update(double x1, double y1, double x2, double y2)
        {
            if (x1 == this.x1 && y1 == this.y1 && x2 == this.x2 && y2 == this.y2)
                return false;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            return true;
        }

        public void DeltaMove(double dx, double dy)
        {
            x1 += dx;
            y1 += dy;
            x2 += dx;
            y2 += dy;
        }

        public void SetExclude()
        {
            timeStamp = uint.MaxValue;
        }

        public void SetInclude()
        {
            timeStamp = 0;
        }

        public Vec2 GetMin()
        {
            return new Vec2(x1, y1);
        }

        public Vec2 GetMax()
        {
            return new Vec2(x2, y2);
        }

        public Vec2 GetCenter()
        {
            return new Vec2(x1 + (x2 - x1) * 0.5, y1 + (y2 - y1) * 0.5);
        }

        public Vec2 GetSize()
        {
            return new Vec2(x2 - x1, y2 - y1);
        }
    }
}