using Switchgrass.Game;
using Switchgrass.Track;
using Switchgrass.Utils;
using UnityEngine;

namespace Switchgrass.AI
{
    [CreateAssetMenu(menuName = "Steering Behaviours/Follow Track Nodes", fileName = "FollowTrackNodes")]
    public class FollowTrackNodesSB: SteeringBehaviour
    {
        [Header("Driving Settings")]
        [SerializeField, Range(1f, 10f)] private float lookaheadDistance = 5f;
        [SerializeField] private float turnInputMultiplier = 1000f;
        [SerializeField] private AnimationCurve accelerationBySteeringAngle;

        private TrackNode _currentTrackNode;
        
        public override CarControlInput GetControlInput()
        {
            CarControlInput result;
            
            var target = _currentTrackNode.GetRacingLinePoint();
            
            // Move on to the next node
            if (Vector3.Distance(target, Agent.transform.position) < lookaheadDistance && _currentTrackNode.TryGetNextNode(out var nextNode))
            {
                _currentTrackNode = nextNode;
                target = _currentTrackNode.GetRacingLinePoint();
            }
            
            var dir = (target - Agent.transform.position).Flatten().normalized;
            
            // Use the wedge product to figure out which way to turn
            result.TurnInput = Mathf.Clamp(dir.Cross(Agent.transform.forward.Flatten().normalized * turnInputMultiplier), -1, 1);
            
            // Set our speed based on turning angle
            result.SpeedInput = Agent.fowardAccel * accelerationBySteeringAngle.Evaluate(Mathf.Abs(result.TurnInput));

            return result;
        }

        public override void Init(AICarController agent)
        {
            base.Init(agent);
            if (!RaceManager.IsValid) return;
            _currentTrackNode = RaceManager.Instance.GetStartingSectorNode();
        }

        protected void OnEnable()
        {
            if (!RaceManager.IsValid) return;
            _currentTrackNode = RaceManager.Instance.GetStartingSectorNode();
        }

        protected void OnDisable()
        {
            _currentTrackNode = null;
        }

        public override void DrawGizmos()
        {
            if (!RaceManager.IsValid || _currentTrackNode is null) return;
            
            // Draw our 'target' node, the one we're aiming for
            var target = _currentTrackNode.GetRacingLinePoint();
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(target, 0.5f);
        }
    }
}