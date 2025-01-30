using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTracker : MonoBehaviour
{
    public static LapTracker instance;

    private int lap = 1;
    public int totalLaps = 3;
   // public GameObject UIManager;
   // public RaceManager raceManager;

    public Transform gates;

    public Transform activeGate;
    public Transform precedingGate;
    public Transform leadingGate;

    public float lapTime;
    public float lap1Time;
    public float lap2Time;
    private System.TimeSpan ts;

    public bool raceStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    private int countdownCurrent = 3;

    public bool raceOver;

    public AudioSource raceStartBeep;

    void Awake()
    {
        instance = this;
        // initial gate set to the start/finish line
        activeGate = precedingGate = GetGate(0);
        // next gate set to the second gate
        leadingGate = GetGate(1);

        raceStartBeep.pitch = 0.5f;
        raceStartBeep.volume = 0.25f;

    }

    private void Start()
    {
        raceStarting = true;
        raceOver = false;
        startCounter = timeBetweenStartCount;

        UIManager.instance.lapCounter.text = "Lap#" + lap;
        UIManager.instance.countDownText.text = countdownCurrent + "";
    }

    private void Update()
    {

        if (raceStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countDownText.text = countdownCurrent + "";
                raceStartBeep.Stop();
                raceStartBeep.Play();

                if (countdownCurrent == 0)
                {

                    raceStarting = false;
                    UIManager.instance.countDownText.gameObject.SetActive(false);
                    raceStartBeep.pitch = 1f; 
                    raceStartBeep.Play();
                    UIManager.instance.goTex.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (!raceOver)
            { 
            lapTime += Time.deltaTime;
            ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.lapTimeText.text = string.Format("{0:00}m{1:00}.{2:00}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
            }
        }

    }

    int GetGateIndex(Transform gate)
    {
        for (int i = 0; i < gates.childCount; i++)
        {
            Transform currentGate = gates.GetChild(i);
            if (gate == currentGate)
            {
                return i;
            }
        }
        return -1;
    }

    Transform GetGate(int index)
    {
        return gates.GetChild((gates.childCount + index) % gates.childCount);
    }


    void OnGateEnterOrExit(Transform enteredGate)
    {

        // if it's the active gate, do nothing
        if (enteredGate == activeGate) return;


        // if this is the leading gate and we last passed the preceding gate, then have cleared the next gate along the track
        if (enteredGate == leadingGate && activeGate == precedingGate)
        {

            // update the active gate
            activeGate = leadingGate;

            int newGateIndex = GetGateIndex(enteredGate);
            // update the preceding gate
            precedingGate = GetGate(newGateIndex);
            // update the leading gate
            leadingGate = GetGate(newGateIndex + 1);

            
            if (newGateIndex == 0)
            {
                /*
                if (lap == raceManager.totalLaps)
                {
                    raceManager.SendMessage("OnRaceComplete");
                }
                else
                {
                    lap++;
                    UIManager.SendMessage("OnLapChanged", lap);
                }
                */
                LapComplete();
              //  Debug.Log(lap);
            }
            
            

        }
        else
        {
            activeGate = enteredGate;
        }
 


    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Checkpoint") return;
        OnGateEnterOrExit(other.transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Checkpoint") return;
        OnGateEnterOrExit(other.transform);
    }
    void LapComplete()
    {
        if (lap == 1)
        {
            UIManager.instance.lap1TimeText.text = string.Format("lap1 - {0:00}m{1:00}.{2:00}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
        if (lap == 2) 
        {
            UIManager.instance.lap2TimeText.text = string.Format("lap2 - {0:00}m{1:00}.{2:00}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
        if (lap == 3)
        {
            UIManager.instance.lap3TimeText.text = string.Format("lap3 - {0:00}m{1:00}.{2:00}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        lap++;
        raceStartBeep.pitch = 2f;
        raceStartBeep.volume = 0.1f;
        raceStartBeep.Play();
        lapTime = 0f;

        if (lap == 4)
        {
            //raceStarting = true;
            raceOver = true;
            lap -= lap;
            UIManager.instance.raceOverText.gameObject.SetActive(true);

        }

        UIManager.instance.lapCounter.text = "Lap#" + lap;
        lapTime = 0f;
        
    }
}
