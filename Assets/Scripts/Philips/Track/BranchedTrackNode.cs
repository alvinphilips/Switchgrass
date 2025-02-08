using UnityEngine;

namespace Switchgrass.Track
{
    public class BranchedTrackNode : TrackNode
    {
        [SerializeField] private TrackNode alternate;

        // Access the Next node, with no side effects
        public override TrackNode Next => _useAlternate ? alternate : next;
        
        private bool _useAlternate;
        private readonly Vector3[] _debugPathPoints = new Vector3[ShowPathLength];
        
        private const int ShowPathLength = 4;
        private static readonly Color BranchPathColor = Color.yellow;

        public void SetUseAlternate(bool useAlternate)
        {
            _useAlternate = useAlternate;
            
            // Used for debugging
            TrackNode targetNode = this;
            for (var i = 0; i < ShowPathLength; i++)
            {
                _debugPathPoints[i] = targetNode.GetRacingLinePoint() + Vector3.up * DisplayOffset;
                targetNode = targetNode.Next;
                if (targetNode is null) break;
            }
        }

        public override bool TryGetNextNode(out TrackNode nextNode)
        {
            if (!_useAlternate || alternate is null)
            {
                return base.TryGetNextNode(out nextNode);
            }
            
            nextNode = alternate;
            return true;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = BranchPathColor;
            Gizmos.DrawLineStrip(_debugPathPoints, false);
        }
    }
}
