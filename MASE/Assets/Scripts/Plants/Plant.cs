using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float health = 100f;
    private void Update()
    {
        if (health < 1)
        {
            Destroy(this.transform.gameObject);
        }
    }
}
