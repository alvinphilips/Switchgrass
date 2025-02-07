using System;
using UnityEngine;
using UnityEngine.Events;

namespace Switchgrass.Misc
{
    // Adapted from my own work on Furlastic Duo, MIT: https://github.com/PeonyKitten/FurlasticDuo/blob/dev/Assets/Game/Scripts/Misc/DoSomethingOnEvent.cs
    public class DoSomethingOnEvent : MonoBehaviour
    {
        [Serializable]
        public enum ListenableEvent
        {
            None,
            ObjectStart,
            ObjectDestroy,
            ObjectUpdate,
            TriggerEnter,
            TriggerExit,
            CollisionEnter,
            CollisionExit,
        }

        [SerializeField] private ListenableEvent performActionOnEvent = ListenableEvent.None;
        [SerializeField] private bool checkTag = true;
        [SerializeField] private string tagToCheck = "Player";

        public UnityEvent onAction;

        private void Start()
        {
            if (performActionOnEvent != ListenableEvent.ObjectStart) return;
            
            onAction?.Invoke();
        }

        private void Update()
        {
            if (performActionOnEvent != ListenableEvent.ObjectUpdate) return;
            
            onAction?.Invoke();
        }

        private void OnDestroy()
        {
            if (performActionOnEvent != ListenableEvent.ObjectDestroy) return;
            
            onAction?.Invoke();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (performActionOnEvent != ListenableEvent.TriggerEnter) return;
            if (checkTag && !other.gameObject.CompareTag(tagToCheck)) return;
            
            onAction?.Invoke();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (performActionOnEvent != ListenableEvent.TriggerExit) return;
            if (checkTag && !other.gameObject.CompareTag(tagToCheck)) return;
            
            onAction?.Invoke();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (performActionOnEvent != ListenableEvent.CollisionEnter) return;
            if (checkTag && !other.gameObject.CompareTag(tagToCheck)) return;
            
            onAction?.Invoke();
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (performActionOnEvent != ListenableEvent.CollisionExit) return;
            if (checkTag && !other.gameObject.CompareTag(tagToCheck)) return;
            
            onAction?.Invoke();
        }
    }
}