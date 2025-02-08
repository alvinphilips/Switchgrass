using UnityEngine;

namespace Switchgrass.Track
{
    public class ProbabilisticTrackNode : BranchedTrackNode
    {
        [SerializeField, Range(0, 1)] private float alternateProbability = 0.5f;
        
        public override bool TryGetNextNode(out TrackNode nextNode)
        {
            // When asked for the next node, pick either the 'regular' one or alternate, based on a set probability.
            SetUseAlternate(Random.value < alternateProbability);
            return base.TryGetNextNode(out nextNode);
        }
    }
}