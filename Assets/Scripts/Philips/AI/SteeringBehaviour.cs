using UnityEngine;

namespace Switchgrass.AI
{
    public struct CarControlInput
    {
        public float SpeedInput;
        public float TurnInput;
    }
    
    public abstract class SteeringBehaviour: ScriptableObject
    {
        public float weight = 1;
        public AICarController agent;

        public abstract void Init();
        public abstract CarControlInput CalculateForce();
    }
}