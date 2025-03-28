using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public float Speed;
    public bool IsContollerTarget;
    public Transform LeftWall;
    public Transform RightWall;

    public GameObject Platform;

    AudioSource audioSource;
    
    //Variables added by Connor Simmons:
    private bool platformOverlappingCharacter = false;
    [SerializeField] Girl girlCharacter; 


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsContollerTarget)
        {
            if (Input.GetButtonDown("KeySubmit"))
            {
                PlacePlatform();
            }
        }
        else
        {
            if (Input.GetButtonDown("JoySubmit"))
            {
                PlacePlatform();
            }
        }
    }

    private void FixedUpdate()
    {
        float vertical;
        float horizontal;
        if (IsContollerTarget)
        {
            vertical = Input.GetAxis("KeyVertical");
            horizontal = Input.GetAxis("KeyHorizontal");
        }
        else
        {
            vertical = Input.GetAxis("JoyVertical");
            horizontal = Input.GetAxis("JoyHorizontal");
        }
        var deltaPos = new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime;
        gameObject.transform.Translate(deltaPos);

        var pos = gameObject.transform.position;
        pos.x = Mathf.Max(LeftWall.position.x + 2, pos.x);
        pos.x = Mathf.Min(RightWall.position.x - 2, pos.x);
        gameObject.transform.position = pos;
    }
    private void PlacePlatform()
    {
        if (platformOverlappingCharacter)
        {
            //still place platform but also push girl character down
            girlCharacter.PushCharacterDown();
        }

        audioSource.Play();
        var offset = new Vector3(0, 2, 0);
        var platformPos = transform.position + offset;
        Instantiate(Platform, platformPos, Quaternion.identity);
    }

    //Functions Added by Connor Simmons
    
    //upon platform collider collision check if collided with the girl character, if so, set bool to true
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GirlChar"))
        {
            platformOverlappingCharacter = true;
        }
    }

    //upon another collider exiting the platforms, check if it was the girl character and if bool is true, if
    //so, set bool back to false
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GirlChar") && platformOverlappingCharacter)
        {
            platformOverlappingCharacter = false;
        }
    }
}
