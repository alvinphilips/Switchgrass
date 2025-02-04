using Switchgrass.Utils;
using UnityEngine;

namespace Switchgrass.Track
{
    /// <summary>
    /// A Single 'Node' in the Track. Represented as a GameObject.
    /// Stores references to the previous node (if any, the START node has no previous node) and next node(s).
    /// </summary>
    /// <remarks>This could be easily stored as plain data in a struct for efficiency. The current approach makes it
    /// easier to access the Transforms, though.</remarks>
    public class TrackNode : MonoBehaviour
    {
        public float width = 10; // How 'wide' the track is at this position
        [Range(-1, 1)]
        public float racingLine; // Where the 'ideal' racing line is on the horizontal (X) axis
        public TrackNode next;
        public TrackNode prev;
        
        private float length = -1;
        
        // Display 'Settings' for ease-of-use
        public const float DisplayOffset = 0.2f;
        private const bool UseStartNodeColor = true;
        private const bool UseEndNodeColor = true;
        private static readonly Color StartNodeColor = Color.green;
        private static readonly Color EndNodeColor = Color.red;
        private static readonly Color RacingLineColor = Color.blue;
        private static readonly Color RacingLineNodeColor = Color.blue;
        
        public void JoinNode(TrackNode nextNode)
        {
            next = nextNode;
            next.prev = this;
        }

        /// <summary>
        /// Recursively calculate the length of the piecewise linear path until this node.
        /// </summary>
        public void CalculateLength()
        {
            // Start Node
            if (prev is null)
            {
                length = 0;
                return;
            }

            // Need to (re)calculate the length of previous nodes
            if (prev.length < 0)
            {
                prev.CalculateLength();
            }
            
            var toPrev = Vector3.Distance(transform.position, prev.transform.position);
            length = prev.length + toPrev;
        }

        /// <summary>
        /// Check if a point is contained within the 'sector' formed by the current node and the previous one, projected
        /// onto a 2D plane. This assumes the sector is convex.
        /// </summary>
        /// <param name="testPosition">Point to check</param>
        /// <returns>Whether the point is inside the given sector.</returns>
        public bool SectorContains(Vector3 testPosition)
        {
            // The 'start' node has no 'area'
            if (prev is null) return false;
            
            // Good old-fashioned custom collision test
            // It's been a while, so I'm going to wing this
            
            var position = transform.position;
            var offset = transform.right * width / 2;
            
            var prevPosition = prev.transform.position;
            var prevOffset = prev.transform.right * prev.width / 2;

            // Using a Math Extension (see Utils/MathExtensions) to clean this up
            // Points that form the bounds, projected onto a 2D plane
            var tl2D = (position - offset).Flatten();
            var tr2D = (position + offset).Flatten();
            var bl2D = (prevPosition - prevOffset).Flatten();
            var br2D = (prevPosition + prevOffset).Flatten();

            // Point to test.
            var test2D = testPosition.Flatten();

            var left = tl2D - bl2D;
            var right = tr2D - br2D;
            var top = tr2D - tl2D;
            var bottom = br2D - bl2D;
            
            return left.Cross(test2D - bl2D) < 0 
                   && right.Cross(test2D - tr2D) > 0 
                   && top.Cross(test2D - tr2D) < 0 
                   && bottom.Cross(test2D - br2D) > 0;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position + Vector3.up * DisplayOffset;
            var offset = transform.right * width / 2;
            var racingLinePoint = position + offset * racingLine;

            if (UseStartNodeColor && !prev)
            {
                Gizmos.color = StartNodeColor;
            }

            if (UseEndNodeColor && !next)
            {
                Gizmos.color = EndNodeColor;
            }
            Gizmos.DrawLine(position - offset, position + offset);
            
            Gizmos.color = RacingLineNodeColor;
            Gizmos.DrawSphere(racingLinePoint, 0.1f);
            Gizmos.color = Color.white;
            
            if (!prev) return;

            var prevPosition = prev.transform.position + Vector3.up * DisplayOffset;
            var prevOffset = prev.transform.right * prev.width / 2;
            
            // Left and Right lines
            Gizmos.DrawLine(position - offset, prevPosition - prevOffset);
            Gizmos.DrawLine(position + offset, prevPosition + prevOffset);
            
            Gizmos.color = RacingLineColor;
            Gizmos.DrawLine(racingLinePoint, prevPosition + prevOffset * prev.racingLine);
        }
    }
}
