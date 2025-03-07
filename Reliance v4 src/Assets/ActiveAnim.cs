using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAnim : MonoBehaviour
{
    public GameObject StickContainer;
    public GameObject[] Sticks;
    public AnimationCurve AlphaCurve;
    public AnimationCurve PosCurve;
    public float AnimTime;
    public Transform Target;

    Coroutine currentAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(Transform target)
    {
        if (currentAnim != null)
        {
            StopCoroutine(currentAnim);
        }
        Target = target;
        currentAnim = StartCoroutine(ActivateCoroutine());
    }

    protected IEnumerator ActivateCoroutine()
    {
        Debug.Log("Playing activate coroutine");
        float startTime = Time.time;
        StickContainer.SetActive(true);
        while (true)
        {
            float t = (Time.time - startTime) / AnimTime;
            if (t > 1)
            {
                StickContainer.SetActive(false);
                break;
            }

            for (int i = 0; i < Sticks.Length; i++)
            {
                var rot = Quaternion.Euler(0, 0, i * 45);
                var right = rot * Vector3.right;

                var stick = Sticks[i];
                stick.transform.right = right;
                var posOffset = right * PosCurve.Evaluate(t);
                stick.transform.position = posOffset + Target.position;

                float alpha = AlphaCurve.Evaluate(t);
                stick.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            }

            yield return null;
        }
        currentAnim = null;
    }
}
