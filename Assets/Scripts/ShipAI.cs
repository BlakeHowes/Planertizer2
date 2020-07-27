

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    [SerializeField]
    private float ShipSpeed;
    [SerializeField]
    private float AltitudeFromPlanet;
    [SerializeField]
    private float TurnSpeed;
    [SerializeField]
    private float OrbitRange;
    [SerializeField]
    private float Damage;
    [SerializeField]
    private string WhatIAttack; //"ENEMY" for allied ships and "ALLIES" for enemys
    [SerializeField]
    private GameObject ShipMesh;
    /*
    [SerializeField]
    private float persuitRange;
    */

    private Rigidbody rb;
    [SerializeField]
    public GameObject target;
    [SerializeField]
    private LineRenderer Gun;
    private Vector3 UniqueSpin;
    private float distancetotarget;

    public List<GameObject> EnemysInRange = new List<GameObject>();
    public Collider other;
    private float nearestDistance;
    private GameObject nearestEnemy;
    private bool IsRegistered;
    private GameObject CurrentPlanet;
    [SerializeField]
    private GameObject gunstartpos;
    private TeamManager teamManager;
    private float turn;

    public State state = State.ORBITING;
    public enum State
    {
        ORBITING,
        CHASING
    }

    void OnEnable()
    {

        rb = GetComponent<Rigidbody>();
        teamManager = GetComponent<TeamManager>();
        turn = TurnSpeed;
        UniqueSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        AltitudeFromPlanet = (Random.Range(-0.5f, 2f));

        Gun.enabled = false;

        if (teamManager.currentTeam == 1)
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = Color.blue;
        }

        if (teamManager.currentTeam == 2)
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = Color.red;
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<TeamManager>().currentTeam != teamManager.currentTeam && collider.GetComponent<TeamManager>().currentTeam != 0 && collider.tag == "SHIP")
        {
            EnemysInRange.Add(collider.attachedRigidbody.gameObject);
        }

        if (collider.tag == ("PLANET"))
        {
            if(IsRegistered == false)
            {
                  
                if (teamManager.currentTeam == 1)
                {
                    CurrentPlanet = collider.gameObject;
                    collider.GetComponent<CaptureManager>().CaptureFunction += 1f;
                }

                if (teamManager.currentTeam == 2)
                {
                    CurrentPlanet = collider.gameObject;
                    collider.GetComponent<CaptureManager>().CaptureFunction -= 1f;
                }

                collider.GetComponent<CaptureManager>().TotalShips += 1f;
                IsRegistered = true;
            }
        }

        if(collider.tag == "ENEMYAI")
        {
            if (teamManager.currentTeam == 2)
            {
                GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
                EnemyAi.GetComponent<EnemyAI>().ShipsICanMove.Add(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "SHIP")
        {
            if (EnemysInRange.Count > 0)
            {
                EnemysInRange.Remove(collider.attachedRigidbody.gameObject);
            }

        }
        if (collider.tag == ("PLANET"))
        {
            CurrentPlanet = null;
            if (WhatIAttack == "ENEMY")
            {
                collider.GetComponent<CaptureManager>().CaptureFunction -= 1f;
            }

            if (WhatIAttack == "ALLIES")
            {
                collider.GetComponent<CaptureManager>().CaptureFunction += 1f;
            }

            collider.GetComponent<CaptureManager>().TotalShips -= 1f;
            IsRegistered = false;
        }

        if (collider.tag == "ENEMYAI")
        {
            collider.GetComponent<EnemyAI>().ShipsICanMove.Remove(gameObject);
        }
    }

    public void RemoveFromPlanet()
    {
        if (this != null)
        {
            if (CurrentPlanet != null)
            {
                CurrentPlanet.GetComponent<CaptureManager>().TotalShips -= 1f;

                if (WhatIAttack == "ENEMY")
                {
                    CurrentPlanet.GetComponent<CaptureManager>().CaptureFunction -= 1f;
                }

                if (WhatIAttack == "ALLIES")
                {
                    CurrentPlanet.GetComponent<CaptureManager>().CaptureFunction += 1f;
                }
            }
        }
    }

    public void MoveTarget(Vector3 NewTargetPosition)
    {
        target.transform.position = NewTargetPosition;
    }

    public void Highlight()
    {
        Renderer rend = ShipMesh.GetComponent<Renderer>();
        rend.material.color = Color.yellow;
    }

    public void RemoveHighlight()
    {
        Renderer rend = ShipMesh.GetComponent<Renderer>();
        rend.material.color = Color.blue;
    }

    private void Update()
    {
        nearestEnemy = null;
        nearestDistance = Mathf.Infinity;
        List<GameObject> ShipsToRemove = new List<GameObject>();
        foreach (GameObject Ship in EnemysInRange)
        {


            if (Vector3.Distance(transform.position, Ship.transform.position) < nearestDistance)
            {
                nearestDistance = Vector3.Distance(transform.position, Ship.transform.position);
                nearestEnemy = Ship;
            }

            if (EnemysInRange.Count > 0)
            {

                Gun.SetPosition(0, gunstartpos.transform.position);
                Gun.SetPosition(1, nearestEnemy.transform.position);
                Gun.enabled = true;
                Ship.GetComponent<ShipStats>().Health -= Time.deltaTime * Damage;
                
            }
        }

        if (EnemysInRange.Count == 0)
        {
            Gun.enabled = false;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.ORBITING:
                Orbiting();
                break;

            case State.CHASING:
                Chasing();
                break;
        }
    }
    void Orbiting()
    {

        distancetotarget = Vector3.Distance(target.transform.position, transform.position);
        Vector3 targetDirection = target.transform.position - transform.position;

        var rotation = targetDirection + UniqueSpin;
        float sign = Mathf.Sign(Vector3.Angle(targetDirection, transform.forward) - 90);


        //Towards target
        if (distancetotarget > AltitudeFromPlanet + OrbitRange)
        {
            sign = 1;
            TurnSpeed = turn / 6;
        }
        //At correct altitude
        if (distancetotarget < AltitudeFromPlanet + OrbitRange && distancetotarget > AltitudeFromPlanet - OrbitRange)
        {
            TurnSpeed = turn / 15f;
        }
        //Approaching planet
        if (distancetotarget < AltitudeFromPlanet - OrbitRange)
        {
            TurnSpeed = turn / 3;
        }
        //Anti-crash
        if (distancetotarget < AltitudeFromPlanet / 2)
        {
            sign = -1;
            TurnSpeed = turn;
        }

        Quaternion newRotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, rotation * sign, TurnSpeed, 0.0f));
        rb.rotation = newRotation;
        rb.velocity = transform.forward * ShipSpeed;

    }


    void Chasing()
    {
        /*
            rb.velocity = transform.forward * speed;
            Vector3 RotateTowardsEmemy = Vector3.RotateTowards(transform.forward, enemyDirection, turnSpeed, 0.0f);

            if (distancetotarget > (flightHeight + persuitRange))
            {
                state = State.ORBITING;
            }
        */
    }
}