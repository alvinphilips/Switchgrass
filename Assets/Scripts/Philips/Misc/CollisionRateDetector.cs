using UnityEngine;

namespace Switchgrass.Misc
{
    public class CollisionRateDetector : MonoBehaviour
    {
        [SerializeField] private string tagToCheck = "Wall";
        [SerializeField] private int notifyEverySeconds = 60;
        
        private float _elapsedTime;
        private int _hitCount;

        private void Start()
        {
            _elapsedTime = 1f; // 'hack' to not get annoying messages all the time
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            
            if (Mathf.RoundToInt(_elapsedTime) % (notifyEverySeconds + 1) == 0)
            {
                Debug.Log($"Collision Hit Rate: {_hitCount/(_elapsedTime - 1)}");
                _elapsedTime += 1;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(tagToCheck))
            {
                _hitCount++;
            }
        }
    }
}
