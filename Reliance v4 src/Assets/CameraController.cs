using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed;
    public Girl Girl;
    public Demon Demon;
    public Gradient SkyColors;
    public float ElevationColorStretch;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // The position between the girl and the demon
        float targetY = (Girl.transform.position.y + Demon.transform.position.y) / 2;
        var pos = transform.position;
        pos.y = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * Speed);
        transform.position = pos;

        cam.backgroundColor = SkyColors.Evaluate(pos.y / ElevationColorStretch);
    }
}
