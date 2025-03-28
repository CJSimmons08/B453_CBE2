using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunLaser : MonoBehaviour
{
    public Vector3 Velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Velocity * Time.deltaTime, Space.World);
        CheckIfOffScreen();
    }

    private void CheckIfOffScreen()
    {
        if (transform.position.y > GlobalManager.Instance.GetTopOfScreen())
        {
            Destroy(gameObject);
        }
    }

    //checks if the collider it his was the robot, if so, call StartStunDuration() and destroy stun laser.
    //also checks if it hit a platform, if it did, destroy the laser but not the platform
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            GlobalManager.Instance.StartStunDuration();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}

