using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    private bool captured;
    public float captureProgress;
    private float captureTimer;

    private TeamManager tm;

    public float CaptureFunction;

    [SerializeField]
    private float captureTimeRequired = 2.5f;

    public int TotalShips = 0;

    float spawning = 1;

    void Awake()
    {
        tm = GetComponent<TeamManager>();
    }

    void Update()
    {
        captureTimer += Time.deltaTime;
        if (TotalShips > 0)
        {
            captureProgress = 0; 
        }

        if (TotalShips < 1)
        {
            captureProgress += 0.1f;
        }

        if (captureProgress == 1)
        {
            if (captureTimer >= captureTimeRequired)
            {
                var planetrenderer = GetComponent<Renderer>();
                planetrenderer.material.color = tm.getTeamColor();
                captured = true;
                spawning = 1f;
            }
        }
    }


    //Giving planet info to EnemyAi using a collider
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (captureProgress == -1)
            {
                tm.currentTeam = 2;
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }

            if ((captureProgress > -1) && (captureProgress < 1))
            {
                tm.currentTeam = 0;
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }

            if (captureProgress == 1)
            {
                tm.currentTeam = 1;
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (captureProgress == -1)
            {
                string Type = "Enemy";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }

            if ((captureProgress > -1) && (captureProgress < 1))
            {

                string Type = "Empty";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject);

            }

            if (captureProgress == 1)
            {
                string Type = "Allied";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }
        }
    }
}

