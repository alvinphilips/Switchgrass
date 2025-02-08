using System;
using System.Collections.Generic;
using Switchgrass.Utils;
using UnityEngine;

namespace Switchgrass.Track
{
    [Serializable]
    public class TrackData
    {
        public float[] racingLineValues;
    }
    
    public class TrackManager: MonoBehaviour
    {
        [SerializeField] private TrackNode startNode;
        
        [Header("Recording Data")]
        [SerializeField] private CarController car;
        [SerializeField] private bool recordRun;
        
        [Header("Debugging")]
        [SerializeField] private bool useRacingLineValues;
        [SerializeField]
        [TextArea] private string constructRacingLineValues; // Quick and dirty 'persistence'

        private Vector3 _prevCarPosition;
        private Vector3 _currentCarPosition;
        private TrackNode _currentSector;
        
        // List of Racing Line values, starting from the starting node.
        private readonly List<float> _racingLineValues = new();

        private bool _collectData;

        private void OnDrawGizmos()
        {
            if (_currentSector is null || !_collectData) return;
            
            Gizmos.color = Color.magenta;
            if (_currentSector.SectorContains(car.transform.position))
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawSphere(car.transform.position + Vector3.up * 2, 0.5f);
        }

        private void Start()
        {
            _currentSector = startNode.next;
            if (recordRun)
            {
                _collectData = true;
            }
        }

        // Hey, it works :P
        private void OnValidate()
        {
            if (!useRacingLineValues) return;
            
            _currentSector = startNode;
            var data = (TrackData) JsonUtility.FromJson(constructRacingLineValues, typeof(TrackData));
            _racingLineValues.Clear();
            _racingLineValues.AddRange(data.racingLineValues);
            
            foreach (var racingLineValue in _racingLineValues)
            {
                if (_currentSector is null) break;
                _currentSector.racingLine = racingLineValue;
                _currentSector = _currentSector.next;
            }
            
            _currentSector = startNode;
            useRacingLineValues = false;
        }

        private void Update()
        {
            if (!_collectData) return;
            CollectData();
        }

        private void CollectData()
        {
            _prevCarPosition = _currentCarPosition;
            _currentCarPosition = car.transform.position;

            if (_currentSector.SectorContains(_currentCarPosition)) return;

            if (_currentSector.next is null)
            {
                Debug.Log("Reached the end of the racetrack.");
                _collectData = false;
                return;
            }

            if (!_currentSector.next.SectorContains(_currentCarPosition))
            {
                Debug.Log("Lost Racer :(");
                _collectData = false;
                return;
            }

            // Thank you, https://en.wikipedia.org/wiki/Line-line_intersection
            
            var p1 = _currentSector.transform.position.Flatten();
            var p2 = p1 + (_currentSector.transform.right * _currentSector.width / 2).Flatten();
            var p3 = _prevCarPosition.Flatten();
            var p4 = _currentCarPosition.Flatten();

            var det = (p1 - p2).Cross(p3 - p4);

            if (!Mathf.Approximately(det, 0))
            {
                var racingLine = (p1 - p3).Cross(p3 - p4) / det;
                _racingLineValues.Add(racingLine);
                _currentSector.racingLine = racingLine;
            }
            else
            {
                // This is to prevent us from losing data if we choose to rebuild the track with this ruh
                _racingLineValues.Add(_currentSector.racingLine);
            }
            
            _currentSector = _currentSector.next;
        }

        private void OnApplicationQuit()
        {
            // Do not print racing line values if we did not record any
            if (_racingLineValues.Count == 0) return;
            
            var container = new TrackData { racingLineValues = _racingLineValues.ToArray() };
            Debug.Log(JsonUtility.ToJson(container));
        }
    }
}