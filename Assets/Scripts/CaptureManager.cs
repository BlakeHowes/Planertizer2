using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CaptureManager : MonoBehaviour
{
    [SerializeField]
    public float Altitude;
    [SerializeField]
    private float MaxShipNumber;
    [SerializeField]
    public float CaptureFunction;
    [SerializeField]
    public float TotalShips;
    [SerializeField]
    private float TimeTakenToCapture;
    [SerializeField]
    public float AmICaptured;
    private bool Captured;
    private bool NoLongerEmpty;
    [SerializeField]
    private float CaptureTimer;
    public float Spawning; //1 = spawn allies, -1 = spawn enemys
    [SerializeField]
    private float SpawnRate;
    [SerializeField]
    private GameObject Spawnpoint;
    [SerializeField]
    private GameObject LevelManager;
    [SerializeField]
    private GameObject AllyShipPrefab;
    [SerializeField]
    private GameObject EnemyShipPrefab;
    [SerializeField]
    private float SpawnTimer;
    [SerializeField]
    private bool TellEnemyAIThisIsAEnemyPlanet;
    private bool TellEnemyAIThisIsAAlliedPlanet;
    [SerializeField]
    private GameObject Ring;
    private bool ChangeColourBack;
    private bool blue;
    private bool red;
    private float timer2;

    private void Awake()
    {

    }

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
                    var planetrenderer = Ring.GetComponent<Renderer>();
                    planetrenderer.material.color = Color.blue;
                    Captured = true;
                    NoLongerEmpty = true;
                    Spawning = 1f;
                    blue = true;
                    TellEnemyAIThisIsAAlliedPlanet = true;
                }
            }
        }

        if (AmICaptured == -1)
        {
            if (Captured == false)
            {
                if (CaptureTimer >= TimeTakenToCapture)
                {
                    var planetrenderer = Ring.GetComponent<Renderer>();
                    planetrenderer.material.color = Color.red;
                    Captured = true;
                    NoLongerEmpty = true;
                    Spawning = -1f;
                    red = true;
                    TellEnemyAIThisIsAEnemyPlanet = true;

                   

                }
            }
        }

        if((AmICaptured > -1) && (AmICaptured < 0))
        {
            Spawning = 0f;
            SpawnTimer = 0f;
        }


        if ((AmICaptured > 0) && (AmICaptured < 1))
        {
            Spawning = 0f;
            SpawnTimer = 0f;
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
            EnemyAi.GetComponent<EnemyAI>().AddPlanet(gameObject, Type);
            TellEnemyAIThisIsAEnemyPlanet = false;


            if (EnemyAi.GetComponent<EnemyAI>().NearbyEmpyPlanetsInRange.Contains(gameObject))
            {
                Type = "Empty";
                EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
            }
        }

        if (TellEnemyAIThisIsAAlliedPlanet == true)
        {
            string Type = "MyPlanets";
            GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
            EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
            TellEnemyAIThisIsAAlliedPlanet = false;



            if (EnemyAi.GetComponent<EnemyAI>().NearbyEmpyPlanetsInRange.Contains(gameObject))
            {
                Type = "Empty";
                EnemyAi.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
            }
        }

        if(ChangeColourBack == true)
        {
            timer2 += Time.deltaTime;
            if(timer2 > 1)
            {
                if(blue == true)
                {
                    var planetrenderer = Ring.GetComponent<Renderer>();
                    planetrenderer.material.color = Color.blue;
                    ChangeColourBack = false;
                }
                if (red == true)
                {
                    var planetrenderer = Ring.GetComponent<Renderer>();
                    planetrenderer.material.color = Color.red;
                    ChangeColourBack = false;
                }
                if((blue != true) && (red != true))
                {
                    var planetrenderer = Ring.GetComponent<Renderer>();
                    planetrenderer.material.color = Color.white;
                }
            }
        }
    }

    //Giving planet info to EnemyAi using a collider
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (AmICaptured == -1)
            {
                string Type = "Enemy";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject, Type);
            }

            if ((AmICaptured > -1) && (AmICaptured < 1))
            {
                string Type = "Empty";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject, Type);
            }

            if (AmICaptured == 1)
            {
                string Type = "Allied";
                col.GetComponent<EnemyAI>().AddPlanet(gameObject, Type);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "ENEMYAI")
        {
            if (AmICaptured == -1)
            {
                string Type = "Enemy";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
            }

            if ((AmICaptured > -1) && (AmICaptured < 1))
            {
                if (NoLongerEmpty == false)
                {
                    string Type = "Empty";
                    col.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
                }
            }

            if (AmICaptured == 1)
            {
                string Type = "Allied";
                col.GetComponent<EnemyAI>().RemovePlanet(gameObject, Type);
            }
        }
    }

    public void Highlight()
    {
        var planetrenderer = Ring.GetComponent<Renderer>();
        planetrenderer.material.color = Color.yellow;
        ChangeColourBack = true;
    }
}

