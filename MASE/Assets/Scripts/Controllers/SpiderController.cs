using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderController : MonoBehaviour
{
    public float speed = 1f;

    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float multiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            multiplier = 2f;
        }

        if (rigidbody.velocity.magnitude < speed * multiplier)
        {
            float value = Input.GetAxis("Vertical");
            if (value != 0)
            {
                rigidbody.AddForce(0, 0, value * Time.fixedDeltaTime * 1000f);
            }
            value = Input.GetAxis("Horizontal");
            if (value != 0)
            {
                rigidbody.AddForce(value * Time.fixedDeltaTime * 1000f, 0f, 0f);
            }
        }
    }
}
