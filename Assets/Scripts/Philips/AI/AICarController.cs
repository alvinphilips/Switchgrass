using System.Collections.Generic;
using Switchgrass.Game;
using UnityEngine;

namespace Switchgrass.AI
{
    public class AICarController: CarController
    {
        [Header("Agent Settings")]
        [SerializeField] private List<SteeringBehaviour> steeringBehaviours = new();

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

        protected override void Start()
        {
            base.Start();
            
            foreach (var steeringBehaviour in steeringBehaviours)
            {
                steeringBehaviour.Init(this);
            }
        }

        private void OnValidate()
        {
            foreach (var steeringBehaviour in steeringBehaviours)
            {
                steeringBehaviour.Init(this);
            }
        }

        protected override void GetControlInput()
        {
            TurnInput = 0;
            SpeedInput = 0;
            
            // Accumulate control inputs
            foreach (var steeringBehaviour in steeringBehaviours)
            {
                var controlInput = steeringBehaviour.GetControlInput();
                TurnInput += controlInput.TurnInput * steeringBehaviour.weight;
                SpeedInput += controlInput.SpeedInput * steeringBehaviour.weight;
            }

            // Clamp controls to their range
            TurnInput = Mathf.Clamp(TurnInput, -1, 1);
            SpeedInput = Mathf.Clamp(SpeedInput, reverseAccel, fowardAccel);
        }

        private void OnDrawGizmos()
        {
            foreach (var steeringBehaviour in steeringBehaviours)
            {
                steeringBehaviour.DrawGizmos();
            }
        }
    }
}