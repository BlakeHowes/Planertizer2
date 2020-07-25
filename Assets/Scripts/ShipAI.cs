

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

    public State state = State.ORBITING;
    public enum State
    {
        ORBITING,
        CHASING
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        turn = TurnSpeed;
        UniqueSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        AltitudeFromPlanet += (Random.Range(-0.5f, 2f));

        Gun.SetPosition(0, transform.position);
        Gun.SetPosition(1, transform.position);

        if (WhatIAttack == "ENEMY")
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = Color.blue;
        }

        if (WhatIAttack == "ALLIES")
        {
            Renderer rend = ShipMesh.GetComponent<Renderer>();
            rend.material.color = Color.red;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
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

        var rotation = (targetDirection) + (UniqueSpin);
        Vector3 RotateTowardsTargetDirection = Vector3.RotateTowards(transform.forward, rotation, TurnSpeed, 0.0f);
        Vector3 RotateAwayFromTargetDirection = Vector3.RotateTowards(transform.forward, -rotation, TurnSpeed, 0.0f);

        //This Causes the ships to fly towards the target
        if (distancetotarget > (AltitudeFromPlanet + OrbitRange))
        {
            transform.rotation = Quaternion.LookRotation(RotateTowardsTargetDirection);
            rb.velocity = transform.forward * ShipSpeed;
            TurnSpeed = turn / 6;
        }


        if (distancetotarget < (AltitudeFromPlanet + OrbitRange) && (distancetotarget > (AltitudeFromPlanet - OrbitRange)))
        {
            //this is at the correct flight Height distance
            float angle = Vector3.Angle(targetDirection, transform.forward);

            rb.velocity = transform.forward * ShipSpeed;
            TurnSpeed = turn / 15f;
            //this circularises their movement so they fly parallel to the target
            if (angle < 90)
            {
                transform.rotation = Quaternion.LookRotation(RotateAwayFromTargetDirection);
            }
            if (angle > 90)
            {
                transform.rotation = Quaternion.LookRotation(RotateTowardsTargetDirection);
            }
        }
        //this attempts to cirularise the orbit as it approches the planet to stop bouncing
        if (distancetotarget < (AltitudeFromPlanet - OrbitRange))
        {
            float angle = Vector3.Angle(targetDirection, transform.forward);
            if (angle < 90)
            {
                transform.rotation = Quaternion.LookRotation(RotateAwayFromTargetDirection);
            }
            if (angle > 90)
            {
                transform.rotation = Quaternion.LookRotation(RotateTowardsTargetDirection);
            }
            rb.velocity = transform.forward * ShipSpeed;
            TurnSpeed = turn / 3;
        }
        //this stops them crashing into the planet
        if (distancetotarget < AltitudeFromPlanet / 2)
        {
            transform.rotation = Quaternion.LookRotation(RotateAwayFromTargetDirection);
            TurnSpeed = turn;
        }
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