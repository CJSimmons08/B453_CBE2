using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float Speed;
    public bool IsContollerTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsContollerTarget)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            var deltaPos = new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime;
            gameObject.transform.Translate(deltaPos);
        }
    }
}
