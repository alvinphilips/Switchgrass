using System;
using System.Collections.Generic;
using UnityEngine;

namespace Switchgrass.AI
{
    [CreateAssetMenu(menuName = "Steering Behaviours/Avoid Obstacles", fileName = "AvoidObstacles")]
    public class AvoidObstaclesSB : SteeringBehaviour
    {
        [SerializeField] private List<Feeler> feelers;
        [SerializeField] private LayerMask obstacleLayerMask;
        [SerializeField] private float obstacleAvoidanceForce;
        [SerializeField] private bool visualizeHits;

        protected virtual Color FeelerDebugColor => Color.yellow;
        
        private float _turnInput;
        
        public override CarControlInput GetControlInput()
        {
            CarControlInput result;
            result.SpeedInput = 0;
            result.TurnInput = 0;
            
            var transform = Agent.transform;
            
            foreach (var feeler in feelers)
            {
                var position = transform.TransformPoint(feeler.offset);
                var forward = transform.TransformDirection(feeler.direction);
                var ray = new Ray(position, forward);
                
                // Check if this 'feeler' hit anything
                if (!Physics.Raycast(ray, out var hitInfo, feeler.length, obstacleLayerMask.value,
                        QueryTriggerInteraction.Ignore)) continue;
                
                #if UNITY_EDITOR
                // Visualize hit point
                if (visualizeHits)
                {
                    Debug.DrawRay(hitInfo.point, Vector3.up * 10, Color.red, 0.2f);
                }
                #endif
                
                result.TurnInput += -feeler.rotation * obstacleAvoidanceForce;
            }

            result.TurnInput = Mathf.Clamp(result.TurnInput, -1, 1);
            // 'smooth' turning input. Technically we could do better, there's a Freya Holmer talk about this.
            result.TurnInput = Mathf.Lerp(_turnInput, result.TurnInput, 0.5f);
            _turnInput = result.TurnInput;
            
            return result;
        }

        private void OnValidate()
        {
            // Recalculate the direction vectors
            foreach (var feeler in feelers)
            {
                feeler.CalculateDirection();
            }
        }

        public override void DrawGizmos()
        {
            if (Agent is null) return;
            
            var transform = Agent.transform;
            
            foreach (var feeler in feelers)
            {
                var position = transform.TransformPoint(feeler.offset);
                var forward = transform.TransformDirection(feeler.direction);
                Gizmos.color = FeelerDebugColor;
                Gizmos.DrawRay(position, forward * feeler.length);
            }
        }
        
        /// <summary>
        /// Simple helper class to define 'feelers' for obstacle detection.
        /// </summary>
        [Serializable]
        public class Feeler
        {
            public Vector3 offset;
            public float rotation;
            public float length;
            [HideInInspector]
            public Vector3 direction;

            public void CalculateDirection()
            {
                direction = Quaternion.Euler(0, rotation, 0) * Vector3.forward;
            }
        }
    }
}