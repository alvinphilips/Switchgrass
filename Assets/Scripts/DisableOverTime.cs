using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    public float timeToDisable;
    private float timeDisabling;
    
    // Start is called before the first frame update
    void Start()
    {
        timeDisabling = timeToDisable;
    }

    // Update is called once per frame
    void Update()
    {
        timeDisabling -= Time.deltaTime;
        if (timeDisabling <= 0)
        {
            gameObject.SetActive(false);
            timeDisabling = timeToDisable;
        }
    }
}
