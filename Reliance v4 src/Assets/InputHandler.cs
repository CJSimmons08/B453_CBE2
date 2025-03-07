using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Girl Girl;
    public Demon Demon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SwitchChar"))
        {
            if (Girl.IsContollerTarget)
            {
                Girl.IsContollerTarget = false;
                Demon.IsContollerTarget = true;
                GetComponentInChildren<ActiveAnim>().Activate(Demon.transform);
            }
            else
            {
                Girl.IsContollerTarget = true;
                Demon.IsContollerTarget = false;
                GetComponentInChildren<ActiveAnim>().Activate(Girl.transform);
            }
        }
    }
}
