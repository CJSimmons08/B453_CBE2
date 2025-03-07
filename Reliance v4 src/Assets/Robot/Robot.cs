using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField]
    Transform PatrolLeftPosition, PatrolRightPosition;
    [SerializeField]
    float RobotSpeed;
    bool moveright = true;
    // Update is called once per frame
    void Update()
    {
        Move();
        TurnAround();
    }

    private void TurnAround()
    {
        if (transform.position.x > PatrolRightPosition.position.x) moveright = false;
        if (transform.position.x < PatrolLeftPosition.position.x) moveright = true;
    }

    private void Move()
    {
        var Direction = moveright ? 1 : -1;
        transform.Translate(new Vector3(Direction, 0, 0) * RobotSpeed * Time.deltaTime);

        var pos = transform.position;
        float targetY = GlobalManager.Instance.GetTopOfScreen() - 2;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 2);
        transform.position = pos;
    }
}
