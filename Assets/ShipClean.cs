using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipClean : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField]
    private float radius;
    [SerializeField]
    private int Team;
    [SerializeField]
    private int SubTeam;
    [SerializeField]
    private float LaserDuration;
    [SerializeField]
    private float LaserCoolDown;
    [SerializeField]
    private float Damage;
    [Space(10)]

    [Header("Flight Settings")]
    [SerializeField]
    public bool Boss;
    [SerializeField]
    private float ShipSpeed;
    [SerializeField]
    private float TurnSpeed;
    [SerializeField]
    private float OrbitRange;
    [SerializeField]
    private float AltitudeFromPlanet;
    [Space(10)]

    [Header("Components")]
    [SerializeField]
    private LineRenderer LaserBeam;
    [SerializeField]
    private GameObject Target;
    [SerializeField]
    private Renderer ShipMesh;

    private Rigidbody rb;
    private float ShootTimer;
    private float turn;
    private Vector3 UniqueSpin;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        turn = TurnSpeed;
        UniqueSpin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        ShipMesh.material.color = GetTeamColour();

        //Set Target parent to spawn
        Collider[] StartPlanet = Physics.OverlapSphere(transform.position, radius);
        Target.transform.position = StartPlanet[0].transform.position;
        Target.transform.SetParent(StartPlanet[0].transform);

    }

    private void FixedUpdate()
    {
        Shoot();
        Orbiting();
    }

    private GameObject EnemysICanShoot()
    {
        Collider[] OtherShips = Physics.OverlapSphere(transform.position, radius);

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var Ship in OtherShips)
        {
            var sqrdistance = (transform.position - Ship.transform.position).sqrMagnitude;
            if (sqrdistance < nearestDistance)
            {
                if (Ship.GetComponent<ShipClean>().GetTeam() != Team)
                {
                    nearestDistance = sqrdistance;
                    nearestEnemy = Ship.gameObject;
                }
            }
        }
        return nearestEnemy;
    }

    private void Shoot()
    {
        if (EnemysICanShoot() != null)
        {
            GameObject Enemy = EnemysICanShoot();
            ShootTimer += Time.deltaTime;
            LaserBeam.SetPosition(0, transform.position);

            if (ShootTimer < LaserDuration)
            {
                LaserBeam.enabled = true;
                LaserBeam.SetPosition(1, Enemy.transform.position);
                Enemy.GetComponent<ShipStats>().RemoveHealth(Damage);
            }

            if (ShootTimer > LaserDuration)
            {
                LaserBeam.enabled = false;

                if (ShootTimer > LaserDuration + LaserCoolDown)
                {
                    ShootTimer = 0;
                }
            }
        }
        else
        {
            LaserBeam.enabled = false;
        }
    }

    public void MoveTarget(GameObject NewTarget, float Altitude)
    {
        RaycastHit hit;
        LayerMask layermask = LayerMask.GetMask("Planet");
        if (Physics.Raycast(transform.position, NewTarget.transform.position - transform.position, out hit, 10000f, layermask))
        {
            if (hit.transform != null)
            {
                if (hit.transform == NewTarget.transform)
                {
                    Target.transform.position = NewTarget.transform.position;
                    Target.transform.SetParent(NewTarget.transform);
                    if (Boss == true)
                    {
                        Altitude = Altitude * 2;
                    }
                    AltitudeFromPlanet = Altitude;
                }
            }
        }
    }

    void Orbiting()
    {
        float distancetotarget = Vector3.Distance(Target.transform.position, transform.position);
        Vector3 targetDirection = Target.transform.position - transform.position;

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

    private Color GetTeamColour()
    {
        Color TeamColour = Color.white;
        if (SubTeam == 1)
        {
            TeamColour = Color.blue;
        }
        if (SubTeam == 2)
        {
            TeamColour = Color.red;
        }
        if (SubTeam == 3)
        {
            TeamColour = Color.magenta;
        }
        if (SubTeam == 4)
        {
            TeamColour = Color.green;
        }
        return TeamColour;
    }

    public void Highlight(bool lightup)
    {
        if(lightup == true)
        {
            ShipMesh.material.color = Color.yellow;
        }
        if (lightup == false)
        {
            ShipMesh.material.color = GetTeamColour();
        }
    }

    public int GetTeam()
    {
        return Team;
    }

    public int GetSubTeam()
    {
        return SubTeam;
    }
}