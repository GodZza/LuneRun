using UnityEngine;
using com.playchilla.runner.track;
using shared.math;

namespace com.playchilla.runner.track.segment
{
    public class HoleSegment : Segment
    {
        private int _parts;

        public HoleSegment(Part connectPart, Vec3Const direction, int parts, int levelId) 
            : base(connectPart, "HoleSegment", levelId)
        {
            _parts = parts;
            
            // Simplified: create hole parts without geometry
            for (int i = 0; i < parts - 1; i++)
            {
                Part newPart = new Part(this, new Vec3(i * 10, 0, 0), (Vec3)direction, 
                                       new Vec3(0, 1, 0), null, i, 0);
                AddPart(newPart);
            }

            UnityEngine.Vector3 x;
        }

        public override int GetNumberOfParts()
        {
            return _parts;
        }
    }
}