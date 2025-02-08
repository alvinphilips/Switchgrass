using UnityEngine;
using UnityEngine.InputSystem;

namespace Switchgrass
{
    public class PlayerCarController: CarController
    {
        // We switch to using Gamepad input if we detect any (PlayerInput send us a message)
        private bool _useJoystick;
        
        protected override void GetControlInput()
        {
            if (_useJoystick) return;
            
            SpeedInput = 0f;

            if (Input.GetAxis("Vertical") > 0)
            {
                SpeedInput = Input.GetAxis("Vertical") * fowardAccel;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                SpeedInput = Input.GetAxis("Vertical") * reverseAccel;
            }

            TurnInput = Input.GetAxis("Horizontal");
        
            // Reset Car to Track
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetToTrack();
            }
        }

        private void OnSteering(InputValue steering)
        {
            _useJoystick = true;
            
            TurnInput = steering.Get<float>();
        }

        private void OnAcceleration(InputValue acceleration)
        {
            _useJoystick = true;
            
            var vertical = acceleration.Get<float>();

            SpeedInput = vertical > 0 ? vertical * fowardAccel : vertical * reverseAccel;
        }
    }
}