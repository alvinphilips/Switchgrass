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
        protected AICarController Agent;

        public virtual void Init(AICarController agent)
        {
            Agent = agent;
        }
        
        public abstract CarControlInput GetControlInput();
        
        public virtual void DrawGizmos() {}
    }
}