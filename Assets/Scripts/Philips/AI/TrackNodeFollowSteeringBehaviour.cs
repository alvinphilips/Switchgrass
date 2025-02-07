using Switchgrass.Track;
using Switchgrass.Utils;
using UnityEngine;

namespace Switchgrass.AI
{
    public class TrackNodeFollowSteeringBehaviour: SteeringBehaviour
    {
        [Header("Driving Settings")]
        [SerializeField, Range(1f, 10f)] private float lookaheadDistance = 5f;
        [SerializeField] private float turnInputMultiplier = 1000f;
        [SerializeField] private AnimationCurve accelerationBySteeringAngle;

        private TrackNode _currentTrackNode;
        
        public override CarControlInput CalculateForce()
        {
            CarControlInput result;
            
            var target = _currentTrackNode.GetRacingLinePoint();
            
            if (Vector3.Distance(target, agent.transform.position) < lookaheadDistance && _currentTrackNode.next is not null)
            {
                _currentTrackNode = _currentTrackNode.next;
                target = _currentTrackNode.GetRacingLinePoint();
            }

            var dir = (target - agent.transform.position).Flatten().normalized;


            result.TurnInput = Mathf.Clamp(dir.Cross(agent.transform.forward.Flatten().normalized * turnInputMultiplier), -1, 1);
            result.SpeedInput = agent.fowardAccel * accelerationBySteeringAngle.Evaluate(Mathf.Abs(result.TurnInput));

            return result;
        }

        public override void Init()
        {
            _currentTrackNode = agent.seekingTrackNode;
        }
    }
}