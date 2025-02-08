using UnityEngine;

namespace Switchgrass.AI
{
    [CreateAssetMenu(menuName = "Steering Behaviours/Shunt Player", fileName = "ShuntPlayer")]
    public class ShuntPlayerSB : AvoidObstaclesSB
    {
        [SerializeField] private float shuntDelay;
        
        protected override Color FeelerDebugColor => Color.magenta;
        
        private float _shuntTimer;
        
        public override CarControlInput GetControlInput()
        {
            _shuntTimer -= Time.deltaTime;
            
            // We've already shunted the player recently
            if (_shuntTimer > 0)
            {
                return new CarControlInput();;
            }
            
            var result = base.GetControlInput();

            // We want to move in the direction of the Player here
            result.TurnInput *= -1;
            _shuntTimer = shuntDelay;
            
            return result;
        }
    }
}