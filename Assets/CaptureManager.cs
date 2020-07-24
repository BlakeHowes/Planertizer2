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
                    Spawning = 1f;
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
                    Spawning = -1f;
                }
            }
        }

        if ((AmICaptured != 1) & (AmICaptured >= 0))
        {
            CaptureTimer = 0f;
            Captured = false;
            Spawning = 0f;
            SpawnTimer = 0f;
        }

        if ((AmICaptured != -1) & (AmICaptured < 0))
        {
            CaptureTimer = 0f;
            Captured = false;
            Spawning = 0f;
            SpawnTimer = 0f;
        }

        if(Spawning == 1f)
        {
            SpawnTimer += Time.deltaTime;
            if (SpawnTimer > SpawnRate)
            {
                Instantiate(AllyShipPrefab, Spawnpoint.transform.position, new Quaternion());
                SpawnTimer = 0f;
            }
        }

        if (Spawning == -1f)
        {
            SpawnTimer += Time.deltaTime;
            if (SpawnTimer > SpawnRate)
            {
                Instantiate(EnemyShipPrefab, Spawnpoint.transform.position, new Quaternion());
                SpawnTimer = 0f;
            }
        }
    }
}
