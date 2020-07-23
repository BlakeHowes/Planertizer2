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
    private float persuitRange;

    private Rigidbody rb;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float turn;
    private Vector3 UniqueSpin;
    private float distancetotarget;

    public List<GameObject> EnemysInRange = new List<GameObject>();
    public Collider other;
    private float nearestDistance;
    private GameObject nearestEnemy;

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
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider != this)
        {
            if (collider.tag == ("ENEMY"))
            {
                EnemysInRange.Add(collider.attachedRigidbody.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == ("ENEMY"))
        {
            EnemysInRange.Remove(collider.attachedRigidbody.gameObject);
        }
    }

    private void Update()
    {
        nearestEnemy = null;
        nearestDistance = Mathf.Infinity;
        foreach (GameObject Ship in EnemysInRange)
        {
            if (Ship != this)
            {
                if (Vector3.Distance(transform.position, Ship.transform.position) < nearestDistance)
                {
                    nearestDistance = Vector3.Distance(transform.position, Ship.transform.position);
                    nearestEnemy = Ship;
                }
            }
            if (EnemysInRange.Count > 0)
            {
                Debug.DrawLine(transform.position, nearestEnemy.transform.position, Color.red);
                nearestEnemy.GetComponent<>
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

        var rotation = (targetDirection) + (UniqueSpin);
        Vector3 RotateTowardsTargetDirection = Vector3.RotateTowards(transform.forward, rotation, TurnSpeed, 0.0f);
        Vector3 RotateAwayFromTargetDirection = Vector3.RotateTowards(transform.forward, -rotation, TurnSpeed, 0.0f);


        Vector3 RotateParallelTargetDirection = Vector3.RotateTowards(transform.forward, rotation, TurnSpeed, 0.0f);

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
            TurnSpeed = turn / 5f;
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
        //this stops them crashing into the planet
        if (distancetotarget < (AltitudeFromPlanet - OrbitRange))
        {
            transform.rotation = Quaternion.LookRotation(RotateAwayFromTargetDirection);
            rb.velocity = transform.forward * ShipSpeed;
            TurnSpeed = turn / 6;
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