using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    [SerializeField]
    public float CaptureFunction;
    [SerializeField]
    public float TotalShips;
    [SerializeField]
    private float TimeTakenToCapture;
    [SerializeField]
    private float AmICaptured;
    private bool Captured;
    [SerializeField]
    private float CaptureTimer;
    void Update()
    {
        CaptureTimer += Time.deltaTime;
        if (TotalShips > 0f)
        {
            AmICaptured = CaptureFunction / TotalShips;
        }

        if (TotalShips < 1f)
        {
            AmICaptured = 0f;
        }

        if (AmICaptured == 1)
        {
            if (Captured == false)
            {
                if(CaptureTimer >= TimeTakenToCapture)
                {
                    var planetrenderer = GetComponent<Renderer>();
                    planetrenderer.material.color = Color.blue;
                    Captured = true;
                }
            }
        }

        if (AmICaptured == -1)
        {
            if (Captured == false)
            {
                if (CaptureTimer >= TimeTakenToCapture)
                {
                    var planetrenderer = GetComponent<Renderer>();
                    planetrenderer.material.color = Color.red;
                    Captured = true;
                }
            }
        }

        if ((AmICaptured != 1) & (AmICaptured >= 0))
        {
            CaptureTimer = 0f;
            Captured = false;
        }

        if ((AmICaptured != -1) & (AmICaptured < 0))
        {
            CaptureTimer = 0f;
            Captured = false;
        }
    }
}
