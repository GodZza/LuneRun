using UnityEngine;
using com.playchilla.runner.track;
using shared.math;

namespace com.playchilla.runner.track.segment
{
    public class HoleSegment : Segment
    {
        private int _parts;

        public HoleSegment(Part connectPart, Vector3 direction, int parts, int levelId) 
            : base(connectPart, "HoleSegment", levelId)
        {
            _parts = parts;

            var loc1 = direction;
            loc1.y = 0;
            loc1.Normalize();

            var loc2 = (Vector3)connectPart.GetPos();
            loc2.y = 0;

            var loc3 = loc1 * Part.Length;
            

            for (int i = 0; i < parts - 1; i++)
            {
                loc2 = loc2 + loc3;
                var newPart = new Part(this, loc2, loc1, Vector3.up, null, GetParts().Count, 0);
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