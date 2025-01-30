using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{

    public AudioSource collisionSound;
    public int groundLayer = 8;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != groundLayer)
        {
            collisionSound.Stop();
            collisionSound.pitch = Random.Range(0.5f, 1.5f);
            collisionSound.Play();
        }
    }

}
