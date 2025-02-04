using System;
using System.Collections.Generic;
using Switchgrass.Utils;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private CarController car;

        [Header("Debugging")]
        [SerializeField] private bool useRacingLineValues;
        [SerializeField]
        [TextArea] private string constructRacingLineValues;

        private Vector3 prevCarPosition;
        private Vector3 currentCarPosition;
        private TrackNode currentSector;
        
        // List of Racing Line values, starting from the starting node.
        private readonly List<float> racingLineValues = new();

        private bool stopCollectingData;

        private void OnDrawGizmos()
        {
            if (currentSector is null) return;
            
            Gizmos.color = Color.magenta;
            if (currentSector.SectorContains(car.transform.position))
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawSphere(car.transform.position + Vector3.up * 2, 0.5f);
        }

        private void Start()
        {
            currentSector = startNode;
        }

        private void OnValidate()
        {
            if (!useRacingLineValues) return;
            
            currentSector = startNode;
            var data = (TrackData) JsonUtility.FromJson(constructRacingLineValues, typeof(TrackData));
            racingLineValues.Clear();
            racingLineValues.AddRange(data.racingLineValues);
            
            foreach (var racingLineValue in racingLineValues)
            {
                if (currentSector is null) break;
                currentSector.racingLine = racingLineValue;
                currentSector = currentSector.next;
            }
            
            currentSector = startNode;
            useRacingLineValues = false;
        }

        private void Update()
        {
            if (stopCollectingData) return;
            CollectData();
        }

        private void CollectData()
        {
            prevCarPosition = currentCarPosition;
            currentCarPosition = car.transform.position;

            if (currentSector.SectorContains(currentCarPosition)) return;

            if (currentSector.next is null)
            {
                Debug.Log("Reached the end of the racetrack.");
                stopCollectingData = true;
                return;
            }

            if (!currentSector.next.SectorContains(currentCarPosition))
            {
                Debug.Log("Lost Racer :(");
                stopCollectingData = true;
                return;
            }

            // Thank you, https://en.wikipedia.org/wiki/Line-line_intersection
            
            var p1 = currentSector.transform.position.Flatten();
            var p2 = p1 + (currentSector.transform.right * currentSector.width / 2).Flatten();
            var p3 = prevCarPosition.Flatten();
            var p4 = currentCarPosition.Flatten();

            var det = (p1 - p2).Cross(p3 - p4);

            if (!Mathf.Approximately(det, 0))
            {
                var racingLine = (p1 - p3).Cross(p3 - p4) / det;
                racingLineValues.Add(racingLine);
                currentSector.racingLine = racingLine;
            }
            
            currentSector = currentSector.next;
        }

        private void OnApplicationQuit()
        {
            var container = new TrackData { racingLineValues = racingLineValues.ToArray() };
            Debug.Log(JsonUtility.ToJson(container));
        }
    }
}