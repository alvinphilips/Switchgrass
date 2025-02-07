using System;
using UnityEngine;

namespace Switchgrass.Patterns
{
    public class Singleton<T> : MonoBehaviour where T: Behaviour
    {
        [SerializeField] public bool keepPersistant;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = FindObjectOfType<T>();
                _instance ??= new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();

                return _instance;
            }
        }

        public static bool IsValid => _instance is not null;

        private void Awake()
        {
            if (_instance is not null && _instance != this) DestroyImmediate(this);
            _instance = this as T;

            if (keepPersistant)
            {
                DontDestroyOnLoad(this);
            }
        }

        private void OnDisable()
        {
            _instance = null;
        }
    }
}