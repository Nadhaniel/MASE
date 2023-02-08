using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private float speedMultiplier;
    private Transform tr;
    public Movement(float SpeedMultiplier, Transform Tr)
    {
        this.speedMultiplier = SpeedMultiplier;
        this.tr = Tr;
    }

    public void MoveForward()
    {
        Vector3 pos = tr.position;
        pos.x += 0.1f * speedMultiplier;
        tr.position = pos;
    }

    public void MoveBackward()
    {
        Vector3 pos = tr.position;
        pos.x += -(0.1f * speedMultiplier);
        tr.position = pos;
    }

    public void MoveLeft()
    {
        Vector3 pos = tr.position;
        pos.z += 0.1f * speedMultiplier;
        tr.position = pos;
    }

    public void MoveRight()
    {
        Vector3 pos = tr.position;
        pos.z += -(0.1f * speedMultiplier);
        tr.position = pos;
    }


}
