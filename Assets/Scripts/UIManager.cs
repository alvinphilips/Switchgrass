using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    //public int lapHere = 1;
    public TMP_Text lapCounter, lapTimeText, lap1TimeText, lap2TimeText, lap3TimeText;
    public TMP_Text countDownText, goTex, raceOverText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(lapHere);
    }
}
