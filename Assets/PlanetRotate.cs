using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    public float Xspeed;
    public float Yspeed;
    public float Zspeed;
    void FixedUpdate()
    {
        transform.Rotate((1f* Xspeed) * Time.deltaTime, (1f * Yspeed) * Time.deltaTime, (1f * Zspeed) * Time.deltaTime);
    }
}
