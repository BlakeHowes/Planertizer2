

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    [SerializeField]
    private float ShipSpeed;
    [SerializeField]
    private float HeightMultiplyer;
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
    private GameObject ShipMesh1;
    [SerializeField]
    private GameObject ShipMesh2;

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

    private bool search;
    private float searchtimer;
    private GameObject NewTarget;
    private float AltitudeTemp;
    [SerializeField]
    private LayerMask layermask;
    private bool attackingplanet;
    private GameObject PlanetToAttack;



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

        float randomizer = Random.Range(0, 2);

        if (WhatIAttack == "ENEMY")
        {
            Renderer rend = ShipMesh1.GetComponent<Renderer>();
            rend.material.color = AllyColor;
        }

        if (WhatIAttack == "ALLIES")
        {
            Renderer rend = ShipMesh1.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = EnemyColor;
            }
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
                    target.transform.position = collider.transform.position;
                    target.transform.SetParent(collider.transform);
                    AltitudeFromPlanet = collider.transform.gameObject.GetComponent<CaptureManager>().Altitude * HeightMultiplyer;
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
            if (collider != null)
            {
                if (collider.tag == ("PLANET"))
                {
                    if (IsRegistered == false)
                    {
                        if (WhatIAttack == "ENEMY")
                        {
                            CurrentPlanet = collider.gameObject;
                            collider.GetComponent<CaptureManager>().CaptureFunction += 1f;
                            if(collider.GetComponent<CaptureManager>().AmICaptured != 1 &&(EnemysInRange.Count == 0))
                            {
                                attackingplanet = true;
                                PlanetToAttack = collider.transform.gameObject;
                            }
                        }

                        if (WhatIAttack == "ALLIES")
                        {
                            CurrentPlanet = collider.gameObject;
                            collider.GetComponent<CaptureManager>().CaptureFunction -= 1f;
                            if (collider.GetComponent<CaptureManager>().AmICaptured != -1 && (EnemysInRange.Count == 0))
                            {
                                attackingplanet = true;
                                PlanetToAttack = collider.transform.gameObject;
                            }
                        }

                        collider.GetComponent<CaptureManager>().TotalShips += 1f;
                        IsRegistered = true;
                    }
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

    //Upgraded Move Position
    public void MoveTarget(GameObject NewTargetPosition, float Altitude)
    {
        NewTarget = NewTargetPosition;
        AltitudeTemp = Altitude;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, NewTargetPosition.transform.position - transform.position, out hit, 10000f, layermask))
        {
            if(hit.transform != null)
            {
                if (hit.transform == NewTargetPosition.transform)
                {
                    target.transform.position = NewTargetPosition.transform.position;
                    target.transform.SetParent(NewTargetPosition.transform);
                    AltitudeFromPlanet = Altitude * HeightMultiplyer;
                }

                if (hit.transform != NewTargetPosition.transform)
                {
                    target.transform.position = hit.transform.position;
                    target.transform.SetParent(NewTargetPosition.transform);

                    if (hit.transform.tag == "PLANET")
                    {
                        AltitudeFromPlanet = hit.transform.gameObject.GetComponent<CaptureManager>().Altitude * HeightMultiplyer;
                    }
                    if (hit.transform.tag == "OBJECT")
                    {
                        AltitudeFromPlanet = hit.transform.gameObject.transform.localScale.x + 6f + Random.Range(-0.5f, 2f);
                    }
                        search = true;
                }
            }
        }
    }

    public void MoveTargetToSpace(Vector3 NewTargetPosition, float Altitude)
    {
        target.transform.position = NewTargetPosition;
        target.transform.SetParent(null);
        AltitudeFromPlanet = Altitude * HeightMultiplyer;
    }

    public void Highlight()
    {
        Renderer rend = ShipMesh1.GetComponent<Renderer>();
        rend.material.color = Color.yellow;
    }

    public void RemoveHighlight()
    {
        Renderer rend = ShipMesh1.GetComponent<Renderer>();
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

        if ((EnemysInRange.Count == 0) &&(attackingplanet == false))
        {
            Gun.SetPosition(0, transform.position);
            Gun.SetPosition(1, transform.position);
        }

        if(attackingplanet == true)
        {
            Gun.SetPosition(0, gunstartpos.transform.position);
            Gun.SetPosition(1, PlanetToAttack.transform.position);

            if ((PlanetToAttack.GetComponent<CaptureManager>().AmICaptured == 1) && (WhatIAttack == "ENEMY"))
            {
                attackingplanet = false;
            }

            if ((PlanetToAttack.GetComponent<CaptureManager>().AmICaptured == -1) &&(WhatIAttack == "ALLIES"))
            {
                attackingplanet = false;
            }
        }

        if (search == true) // Search function if ray didnt hit target
        {
            searchtimer += Time.deltaTime;
            if(searchtimer > 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, NewTarget.transform.position - transform.position, out hit, 10000f, layermask))
                {
                    if (hit.transform != null)
                    {
                        if (hit.transform == NewTarget.transform)
                        {
                            target.transform.position = NewTarget.transform.position;
                            target.transform.SetParent(NewTarget.transform);
                            AltitudeFromPlanet = AltitudeTemp * HeightMultiplyer;

                            search = false;
                        }

                        if (hit.transform != NewTarget.transform)
                        {
                            target.transform.position = hit.transform.position;
                            target.transform.SetParent(NewTarget.transform);
                            if (hit.transform.tag == "PLANET")
                            {
                                AltitudeFromPlanet = hit.transform.gameObject.GetComponent<CaptureManager>().Altitude * HeightMultiplyer;
                            }

                            searchtimer = 0f;
                        }
                    }
                }
            }
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