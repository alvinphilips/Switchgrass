using System;
using System.Collections.Generic;
using Switchgrass.Track;
using Switchgrass.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Switchgrass.AI
{
    public class AICarController: CarController
    {
        public TrackNode seekingTrackNode;

        [Header("Agent Settings")]
        [SerializeField] private List<SteeringBehaviour> steeringBehaviours = new();
        
        [Header("Driving Settings")]
        [SerializeField, Range(1f, 10f)] private float lookaheadDistance = 5f;
        [SerializeField] private float turnInputMultiplier = 1000f;
        [SerializeField] private AnimationCurve accelerationBySteeringAngle;

        private void OnEnable()
        {
            RaceManager.Instance.RegisterAIRacer(this);
        }

        private void OnDisable()
        {
            if (RaceManager.IsValid)
            {
                RaceManager.Instance.DeregisterAIRacer(this);
            }
        }

        protected override void GetControlInput()
        {
            var target = seekingTrackNode.GetRacingLinePoint();
            
            if (Vector3.Distance(target, transform.position) < lookaheadDistance && seekingTrackNode.TryGetNextNode(out var nextNode))
            {
                seekingTrackNode = nextNode;
                target = seekingTrackNode.GetRacingLinePoint();
            }

            var dir = (target - transform.position).Flatten().normalized;

            TurnInput = Mathf.Clamp(dir.Cross(transform.forward.Flatten().normalized * turnInputMultiplier), -1, 1);
            SpeedInput = fowardAccel * accelerationBySteeringAngle.Evaluate(Mathf.Abs(TurnInput));
        }

        private void OnDrawGizmosSelected()
        {
            if (seekingTrackNode is null) return;
            
            // Draw our 'target' node, the one we're aiming for
            var target = seekingTrackNode.transform.position + seekingTrackNode.transform.right *
                seekingTrackNode.width / 2 * seekingTrackNode.racingLine;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(target, 0.5f);
        }
    }
}