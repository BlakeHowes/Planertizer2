using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    [SerializeField]
    private float MaxShipNumber;
    [SerializeField]
    public float CaptureFunction;
    [SerializeField]
    public float TotalShips;
    [SerializeField]
    private float TimeTakenToCapture;
    [SerializeField]
    public float CaptureValue;
    private bool Captured;
    private bool NoLongerEmpty;
    [SerializeField]
    private float CaptureTimer;
    private float Spawning; //1 = spawn allies, -1 = spawn enemys
    [SerializeField]
    private float SpawnRate;
    [SerializeField]
    private GameObject Spawnpoint;
    [SerializeField]
    private GameObject AllyShipPrefab;
    [SerializeField]
    private GameObject EnemyShipPrefab;
    [SerializeField]
    private float SpawnTimer;
    [SerializeField]
    private bool TellEnemyAIThisIsAEnemyPlanet;
    private bool TellEnemyAIThisIsAAlliedPlanet;
    void Update()
    {
        CaptureTimer += Time.deltaTime;
        if (TotalShips > 0)
        {
            CaptureValue = CaptureFunction / TotalShips;
        }

        if (TotalShips < 1)
        {
            CaptureValue = 0f;
        }

        if (CaptureValue == 1)
        {
                if(CaptureTimer >= TimeTakenToCapture)
                {
                    var planetrenderer = GetComponent<Renderer>();
                    planetrenderer.material.color = Color.blue;
                    Captured = true;
                    NoLongerEmpty = true;
                    Spawning = 1f;

                    TellEnemyAIThisIsAAlliedPlanet = true;
                }
        }

        if (CaptureValue == -1)
        {
            if (CaptureTimer >= TimeTakenToCapture)
            {
                var planetrenderer = GetComponent<Renderer>();
                planetrenderer.material.color = Color.red;
                Captured = true;
                NoLongerEmpty = true;
                Spawning = -1f;

                TellEnemyAIThisIsAEnemyPlanet = true;
            }

        }

        if((CaptureValue > -1) && (CaptureValue < 0))
        {
            Spawning = 0f;
            SpawnTimer = 0f;
        }


        if ((CaptureValue > 0) && (CaptureValue < 1))
        {
            Spawning = 0f;
            SpawnTimer = 0f;
        }

        if ((CaptureValue != 1) & (CaptureValue >= 0))
        {
            CaptureTimer = 0f;
            Captured = false;
        }

        if ((CaptureValue != -1) & (CaptureValue < 0))
        {
            CaptureTimer = 0f;
            Captured = false;
        }

        if(Spawning == 1f)
        {
            SpawnTimer += Time.deltaTime;
            if (TotalShips <= MaxShipNumber)
            {
                if (SpawnTimer > SpawnRate)
                {
                    Instantiate(AllyShipPrefab, Spawnpoint.transform.position, new Quaternion());
                    SpawnTimer = 0f;
                }
            }
        }

        if (Spawning == -1f)
        {
            SpawnTimer += Time.deltaTime;
            if (TotalShips <= MaxShipNumber)
            {
                if (SpawnTimer > SpawnRate)
                {
                    Instantiate(EnemyShipPrefab, Spawnpoint.transform.position, new Quaternion());
                    SpawnTimer = 0f;
                }
            }
        }

        if(TellEnemyAIThisIsAEnemyPlanet == true)
        {
            string Type = "MyPlanets";
            GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
            EnemyAi.GetComponent<EnemyAI>().AddPlanet(gameObject);
            TellEnemyAIThisIsAEnemyPlanet = false;

            if (EnemyAi.GetComponent<EnemyAI>().NearbyNeutralPlanets.Contains(gameObject))
            {
                Type = "Empty";
                EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }
        }

        if (TellEnemyAIThisIsAAlliedPlanet == true)
        {
            string Type = "MyPlanets";
            GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
            EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            TellEnemyAIThisIsAAlliedPlanet = false;

            if (EnemyAi.GetComponent<EnemyAI>().NearbyNeutralPlanets.Contains(gameObject))
            {
                Type = "Empty";
                EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }
        }
    }

    //Giving planet info to EnemyAi using a collider
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (CaptureValue == -1)
            {
                string Type = "Enemy";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }

            if ((CaptureValue > -1) && (CaptureValue < 1))
            {
                string Type = "Empty";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }

            if (CaptureValue == 1)
            {
                string Type = "Allied";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (CaptureValue == -1)
            {
                string Type = "Enemy";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }

            if ((CaptureValue > -1) && (CaptureValue < 1))
            {
                if (NoLongerEmpty == false)
                {
                    string Type = "Empty";
                    col.GetComponent<EnemyAI>().RemovePlanet(gameObject);
                }
            }

            if (CaptureValue == 1)
            {
                string Type = "Allied";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject);
            }
        }
    }
}

