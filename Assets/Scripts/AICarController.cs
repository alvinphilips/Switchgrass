using System;
using Switchgrass.Track;
using Switchgrass.Utils;
using UnityEngine;

namespace Switchgrass
{
    public class AICarController: CarController
    {
        public TrackNode currentTrackNode;

        [Header("AI Settings")]
        [SerializeField, Range(1f, 10f)] private float lookaheadDistance = 5f;
        [SerializeField] private float turnInputMultiplier = 1000f;
        [SerializeField] private AnimationCurve accelerationBySteeringAngle;
        
        protected override void GetControlInput()
        {
            var target = currentTrackNode.GetRacingLinePoint();
            
            if (Vector3.Distance(target, transform.position) < lookaheadDistance && currentTrackNode.next is not null)
            {
                currentTrackNode = currentTrackNode.next;
                target = currentTrackNode.GetRacingLinePoint();
            }

            var dir = (target - transform.position).Flatten().normalized;

            TurnInput = Mathf.Clamp(dir.Cross(transform.forward.Flatten().normalized * turnInputMultiplier), -1, 1);
            SpeedInput = fowardAccel * accelerationBySteeringAngle.Evaluate(Mathf.Abs(TurnInput));
        }

        private void OnDrawGizmosSelected()
        {
            if (currentTrackNode is null) return;
            
            var target = currentTrackNode.transform.position + currentTrackNode.transform.right *
                currentTrackNode.width / 2 * currentTrackNode.racingLine;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(target, 0.5f);
        }
    }
}