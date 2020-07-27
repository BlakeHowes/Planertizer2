﻿

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
    [SerializeField]
    private Color AllyColor;
    [SerializeField]
    private Color EnemyColor;

    private Rigidbody rb;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private LineRenderer Gun;
    private float turn;
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
    private bool IamNew;


    public State state = State.ORBITING;
    public enum State
    {
        ORBITING,
        CHASING
    }

    void OnEnable()
    {
        IamNew = true;
        rb = GetComponent<Rigidbody>();
        turn = TurnSpeed;
        UniqueSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        AltitudeFromPlanet += (Random.Range(-0.5f, 2f));

        Gun.SetPosition(0, transform.position);
        Gun.SetPosition(1, transform.position);

        if (WhatIAttack == "ENEMY")
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = AllyColor;
        }

        if (WhatIAttack == "ALLIES")
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = EnemyColor;
        }

        
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider != null)
        {
            if (IamNew == true)
            {
                if (collider.gameObject.tag == "PLANET")
                {
                    MoveTarget(collider.gameObject, collider.gameObject.GetComponent<CaptureManager>().Altitude);
                    IamNew = false;
                }
            }
        }

        if (collider != this)
        {
            if (collider.tag == (WhatIAttack))
            {
                if (collider != null)
                {
                    EnemysInRange.Add(collider.attachedRigidbody.gameObject);
                }
            }

            if (collider.tag == ("PLANET"))
            {
                if(IsRegistered == false)
                {
                    if (WhatIAttack == "ENEMY")
                    {
                        CurrentPlanet = collider.gameObject;
                        collider.GetComponent<CaptureManager>().CaptureFunction += 1f;
                    }

                    if (WhatIAttack == "ALLIES")
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
                if (WhatIAttack == "ALLIES")
                {
                    GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
                    EnemyAi.GetComponent<EnemyAI>().AddShip(gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == (WhatIAttack))
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
            if(WhatIAttack == "ALLIES")
            {
                GameObject EnemyAi = GameObject.FindGameObjectWithTag("ENEMYAI");
                EnemyAi.GetComponent<EnemyAI>().RemoveShip(gameObject);
            }
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

    public void MoveTarget(GameObject NewTargetPosition, float Altitude)
    {
        target.transform.position = NewTargetPosition.transform.position;
        target.transform.SetParent(NewTargetPosition.transform);
        AltitudeFromPlanet = Altitude;
    }

    public void Highlight()
    {
        Renderer rend = ShipMesh.GetComponent<Renderer>();
        rend.material.color = Color.yellow;
    }

    public void RemoveHighlight()
    {
        Renderer rend = ShipMesh.GetComponent<Renderer>();
        rend.material.color = AllyColor;
    }

    private void Update()
    {
        nearestEnemy = null;
        nearestDistance = Mathf.Infinity;
        List<GameObject> ShipsToRemove = new List<GameObject>();
        foreach (GameObject Ship in EnemysInRange)
        {
            if (Ship != this)
            {
                if(Ship == null)
                {
                    ShipsToRemove.Add(Ship);
                    continue;
                }

                if (Vector3.Distance(transform.position, Ship.transform.position) < nearestDistance)
                {
                    nearestDistance = Vector3.Distance(transform.position, Ship.transform.position);
                    nearestEnemy = Ship;
                }
            }
            if (EnemysInRange.Count > 0)
            {
                Gun.SetPosition(0, gunstartpos.transform.position);
                Gun.SetPosition(1, nearestEnemy.transform.position);
                Ship.GetComponent<ShipStats>().Health -= Time.deltaTime * Damage;
                
            }
        }
        foreach (GameObject Ship in ShipsToRemove)
        {
            EnemysInRange.Remove(Ship);
        }

        if (EnemysInRange.Count == 0)
        {
            Gun.SetPosition(0, transform.position);
            Gun.SetPosition(1, transform.position);
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