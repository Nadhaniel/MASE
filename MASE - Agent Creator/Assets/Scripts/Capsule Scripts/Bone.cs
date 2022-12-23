using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    private float size;

    public Bone() { }
    public Bone(Vector3 position, Quaternion rotation, float size)
    {
        this.position = position;
        this.rotation = rotation;
        this.size = size;
    }

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Quaternion Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    public float Size
    {
        get { return size; }
        set { size = value; }
    }


}
