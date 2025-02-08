using Switchgrass.Track;
using UnityEngine;

namespace Switchgrass
{
    public abstract class CarController : MonoBehaviour
    {
        public Rigidbody theRB;
        public float maxSpeed;

        public float fowardAccel, reverseAccel;
        public float turnStrength = 180;

        protected float SpeedInput = 0F;
        protected float TurnInput;

        private bool grounded;
        private float dragOnGround;

        public Transform groundRayPoint, groundRayPoint2;
        public LayerMask whatIsGround;
        public float groundRayLength = 0.75f;
        public float gravityMod = 10f;
        public float carDragOnGround = 3f;
        public float angularDragOnGround = 3f;

        public Transform leftFrontWheel, rightFrontWheel;
        public float maxWheelTurn = 30f;

        public ParticleSystem[] dustTrail;
        public float maxEmission = 50f, emissionFadeSpeed = 30f;
        private float emissionRate;

        public AudioSource engineSound;
        public AudioSource skidSound;
        public float skidFade = 2f;
    
        public TrackNode currentSector;
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            theRB.transform.parent = null; // unparents the physics and model
        }

        protected abstract void GetControlInput();
    
        private void Update()
        {
            if (LapTracker.instance.raceStarting) return;

            UpdateCurrentTrackSector();
            GetControlInput();

            UpdateFrontWheelModelRotations();
            UpdateTireParticleEmissions();
            UpdateEngineAndSkidSounds();
        }

        private void UpdateCurrentTrackSector()
        {
            if (currentSector.SectorContains(transform.position)) return;

            if (currentSector.Next is not null)
            {
                currentSector = currentSector.Next;
            }
            else
            {
                Debug.LogWarning("Lost Track");
            }
        }

        private void UpdateTireParticleEmissions()
        {
            emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

            if (grounded && (Mathf.Abs(TurnInput) > 0.5f || (theRB.velocity.magnitude < maxSpeed * 0.5f && theRB.velocity.magnitude != 0)))
            {
                emissionRate = maxEmission;
            }

            if (theRB.velocity.magnitude <= 1)
            {
                emissionRate = 0;
            }

            foreach (var t in dustTrail)
            {
                var emissionModule = t.emission;
                emissionModule.rateOverTime = emissionRate;
            }
        }

        private void UpdateEngineAndSkidSounds()
        {
            if (engineSound)
            {
                engineSound.pitch = 1f + theRB.velocity.magnitude / maxSpeed * 2.5f;
            }

            if (skidSound)
            {
                skidSound.volume = Mathf.Abs(TurnInput) > 0.5f ? 0.25f : Mathf.MoveTowards(skidSound.volume, 0f, skidFade * Time.deltaTime);
            }
        }

        private void UpdateFrontWheelModelRotations()
        {
            var leftFrontWheelRotation = Mathf.MoveTowardsAngle(leftFrontWheel.localRotation.eulerAngles.y, TurnInput * maxWheelTurn - 180, 0.5f);
            var rightFrontWheelRotation = Mathf.MoveTowardsAngle(rightFrontWheel.localRotation.eulerAngles.y, TurnInput * maxWheelTurn, 0.5f);
        
            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, leftFrontWheelRotation, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, rightFrontWheelRotation, rightFrontWheel.localRotation.eulerAngles.z);
        }

        private void FixedUpdate()
        {
            grounded = false;

            var normalTarget = Vector3.zero;

            if (Physics.Raycast(groundRayPoint.position, -transform.up, out var hit, groundRayLength, whatIsGround))
            {
                grounded = true;
                normalTarget = hit.normal;
            }


            if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                grounded = true;
                normalTarget = (normalTarget + hit.normal) / 2;
            }


            if (grounded)
            {
                transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
            }
     

            // This accelerates the car 

            if ((grounded) & !LapTracker.instance.raceOver)
            {
                theRB.drag = carDragOnGround;
                theRB.angularDrag = angularDragOnGround;
                theRB.AddForce(transform.forward * (SpeedInput * 100f));
            }
            else
            {
                theRB.drag = 0.1f;
                theRB.AddForce(-Vector3.up * (gravityMod * 100f));
            }

            if (theRB.velocity.magnitude > maxSpeed)
            {
                theRB.velocity = theRB.velocity.normalized;
            }


            //Debug.Log(theRB.velocity.magnitude);
            //Debug.Log(grounded);

            //Move the Model 

            transform.position = theRB.position;

            if (grounded && TurnInput != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, TurnInput * turnStrength * Time.deltaTime * Mathf.Sign(SpeedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
            }
        }

        public float GetTrackDistance()
        {
            // Total length up to the next TrackNode
            var sectorLength = currentSector.length;
        
            // How far we've yet to go
            var distanceToTrackNode = Vector3.Distance(currentSector.GetRacingLinePoint(), transform.position);
        
            return sectorLength - distanceToTrackNode;
        }

        protected void ResetToTrack()
        {
            var pointToGoTo = LapTracker.instance.precedingGate;
        
            transform.position = pointToGoTo.position;
            transform.rotation = LapTracker.instance.precedingGate.rotation;
     
            theRB.transform.position = pointToGoTo.position;
            theRB.velocity = Vector3.zero;
            SpeedInput = 0f;
            TurnInput = 0F;
        }
    }
}
