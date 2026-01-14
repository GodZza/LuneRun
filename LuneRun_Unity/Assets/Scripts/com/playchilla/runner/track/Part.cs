using UnityEngine;
using shared.math;
using com.playchilla.runner.track.segment;

namespace com.playchilla.runner.track
{
    public class Part
    {
        public Segment segment;
        public Part next;
        public Part previous;
        public Vec3 pos;
        public Vec3 dir;
        public Vec3 normal;
        public GameObject visual;
        public int index;
        public double zRot;

        public Part(Segment segment, Vec3 pos, Vec3 dir, Vec3 normal, GameObject visual, int index, double zRot)
        {
            this.segment = segment;
            this.pos = pos;
            this.dir = dir;
            this.normal = normal;
            this.visual = visual;
            this.index = index;
            this.zRot = zRot;
        }

        public Vec3 GetPos()
        {
            return pos;
        }
    }
}