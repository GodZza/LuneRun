using UnityEngine;
using com.playchilla.runner.track;
using shared.math;

namespace com.playchilla.runner.track.segment
{
    // ============================================================================
    // HoleSegment - ¿Õ¶´¹ìµÀ¶Î
    // ============================================================================
    public class HoleSegment : Segment
    {
        private int _parts;

        public HoleSegment(Part connectPart, Vector3 direction, int parts, int levelId) 
            : base(connectPart, "HoleSegment", levelId)
        {
            _parts = parts;

            var normalizedDirection = direction;
            normalizedDirection.y = 0;
            normalizedDirection.Normalize();

            var currentPosition = connectPart.GetPos();
            currentPosition.y = 0;

            var directionStep = normalizedDirection * Part.Length;
            

            for (int i = 0; i < parts - 1; i++)
            {
                currentPosition = currentPosition + directionStep;
                var newPart = new Part(this, currentPosition, normalizedDirection, Vector3.up, null, GetParts().Count, 0);
                AddPart(newPart);
            }
        }

        public override int GetNumberOfParts()
        {
            return _parts;
        }
    }
}