using System.Collections.Generic;
using Switchgrass.Patterns;
using Switchgrass.Track;
using UnityEngine;

// Handles
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Switchgrass.AI
{
    public class RaceManager : Singleton<RaceManager>
    {
        [SerializeField] private TrackNode startNode;
        
        [Header("Shortcut Settings")]
        [SerializeField] private BranchedTrackNode shortcutNode;
        [SerializeField] private float playerLeadDistance = 10f;
        [SerializeField] private bool useShortcutOverride;
        [SerializeField] private bool shortcutOverride;
        
        private PlayerCarController _player;
        private readonly List<AICarController> _aiCars = new();
        private readonly Dictionary<CarController, int> _laps = new ();

        private bool _enableShortcut;
        private float _totalTrackDistance;

        private void Start()
        {
            _player = FindObjectOfType<PlayerCarController>();
            if (_player is null)
            {
                Debug.LogError("Could not find Player Car Controller!");
                return;
            }

            // Since a sector is defined as the area between the nodes in front and back, based on the front node, we
            // have our first sector be the node immediately following the Start node.
            var startSector = startNode.next;
            
            // Use the last node to calculate the length of the track
            // NOTE: Because of how TrackNodes work, length is defined as the cumulative distance up to the current Node
            // Since the Start node has a length of 0, we calculate this ourselves.
            _totalTrackDistance = startNode.prev.length 
                                  + Vector3.Distance(startNode.prev.GetRacingLinePoint(), startNode.GetRacingLinePoint());

            _player.currentSector = startSector;
            _laps.Add(_player, 0);
            
            foreach (var aiCar in _aiCars)
            {
                aiCar.currentSector = startSector;
                aiCar.seekingTrackNode = startSector;
                _laps.Add(aiCar, 0);
            }

            if (useShortcutOverride)
            {
                shortcutNode.SetUseAlternate(shortcutOverride);
            }
        }

        private float GetTotalCarTrackDistance(CarController car)
        {
            return _laps[car] * _totalTrackDistance + car.GetTrackDistance();
        }

        public void Update()
        {
            var playerDistance = GetTotalCarTrackDistance(_player);

            // Disable AI shortcuts on the last lap :P
            var enableShortcut = useShortcutOverride ? shortcutOverride : LapTracker.instance.CurrentLap < LapTracker.instance.totalLaps;

            if (enableShortcut && !useShortcutOverride)
            {
                foreach (var aiCar in _aiCars)
                {
                    // We're too close to the Player, disable shortcut
                    if (playerDistance - GetTotalCarTrackDistance(aiCar) < playerLeadDistance)
                    {
                        enableShortcut = false;
                        break;
                    }
                }
            }

            // The shortcut acts like a railroad switch
            if (_enableShortcut != enableShortcut)
            {
                _enableShortcut = enableShortcut;
                shortcutNode.SetUseAlternate(enableShortcut);
                Debug.Log($"Shortcut status: {(enableShortcut ? "Open" : "Closed")}");
            }
        }

        public void RegisterAIRacer(AICarController racer)
        {
            _aiCars.Add(racer);
        }

        public void DeregisterAIRacer(AICarController racer)
        {
            _aiCars.Remove(racer);
        }

        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            var position = transform.position;
            
            // Draw little 'threads' to our player and all of our AI racers
            if (_player is not null)
            {
                var carPosition = _player.transform.position;
                var halfHeight = Vector3.up * (carPosition.y - position.y) * 0.5f;
                Handles.DrawBezier(position, carPosition, position + halfHeight, carPosition - halfHeight, Color.green, Texture2D.whiteTexture, 1f);
            }
            
            foreach (var aiCar in _aiCars)
            {
                var carPosition = aiCar.transform.position;
                var halfHeight = Vector3.up * (carPosition.y - position.y) * 0.5f;
                Handles.DrawBezier(position, carPosition, position + halfHeight, carPosition - halfHeight, Color.white, Texture2D.whiteTexture, 1f);
            }
        }
        #endif
    }
}