﻿using System.Collections;
using System.Collections.Generic;
using Switchgrass;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target;
    private Vector3 offsetDir;
    public float minDistance;
    public float maxDistance;
    private float activeDistance;

    // Start is called before the first frame update
    void Start()
    {
        offsetDir = transform.position - target.transform.position;

        activeDistance = minDistance;

        offsetDir.Normalize();

    }

    // Update is called once per frame
    void Update()
    {
        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude / target.maxSpeed));
        transform.position = target.transform.position + (offsetDir * activeDistance) ;
    }
}
