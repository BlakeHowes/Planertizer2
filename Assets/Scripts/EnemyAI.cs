using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private int currentPlanetIndex;

    [SerializeField]
    List<GameObject> TotalEnemyPlanets = new List<GameObject>();
    [SerializeField]
    List<GameObject> AllyPlanetsInRange = new List<GameObject>();
    [SerializeField]
    public List<GameObject> NearbyEmptyPlanetsInRange = new List<GameObject>();
    [SerializeField]
    List<GameObject> EnemyPlanetsInRange = new List<GameObject>();
    [SerializeField]
    public List<GameObject> ShipsICanMove = new List<GameObject>();
    List<GameObject> ShipsToRemove = new List<GameObject>();


    [SerializeField]
    private float AiSpeed; //this will determine the rate at which the AI performs actions
    [SerializeField]
    private int DefensiveFactor;
    [SerializeField]
    private int PartySize;
    [SerializeField]
    private float TurnTimer;
    private GameObject CurrentPlanet;

    private bool reset;
    [SerializeField]
    GameObject AllyPlanetNearEnemies;
    private int SeekFarWayPlanetsChecker;
    private float radiusonstart;
    private int ifixed;

    private Vector3 startPos;



    private void Start()
    {
        startPos = transform.position;
    }

    private void Awake()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        radiusonstart = col.radius;
    }

    //Add planet to list based on type
    public void AddPlanet(GameObject planet)
    {
        int planet_team = planet.GetComponent<TeamManager>().currentTeam;
        int self_team = GetComponent<TeamManager>().currentTeam;
        int relationship = 0;


        relationship = (self_team != planet_team && planet_team != 0) ? -1 : 1;


        switch (relationship)
        {
            case 0:
                NearbyEmptyPlanetsInRange.Add(planet);
                break;
            case -1:
                AllyPlanetsInRange.Add(planet);
                break;
            case 1:
                EnemyPlanetsInRange.Add(planet);
                break;

        }
    }

    //Remove based on type
    public void RemovePlanet(GameObject planet)
    {
        int planet_team = planet.GetComponent<TeamManager>().currentTeam;
        int self_team = GetComponent<TeamManager>().currentTeam;
        int relationship = 0;


        relationship = (self_team != planet_team && planet_team != 0) ? -1 : 1;


        switch (relationship)
        {
            case 0:
                NearbyEmptyPlanetsInRange.Remove(planet);
                break;
            case -1:
                AllyPlanetsInRange.Remove(planet);
                break;
            case 1:
                EnemyPlanetsInRange.Remove(planet);
                break;

        }

    }

            //Remove ship from list
    public void RemoveShip(GameObject Ship)
    {
        ShipsICanMove.Remove(Ship);
    }

    private void Update()
    {

        foreach (GameObject Ship in ShipsICanMove)
        {
            if (Ship == null)
            {
                ShipsToRemove.Add(Ship);
            }
        }

        foreach (GameObject Ship in ShipsToRemove)
        {
            ShipsICanMove.Remove(Ship);
            ShipsToRemove.Remove(Ship);
        }

        TurnTimer += Time.deltaTime;
        if (TurnTimer >= AiSpeed)
        {
            if (AllyPlanetsInRange.Count > 0)
            {
                if(currentPlanetIndex > 0)
                {
                    if (currentPlanetIndex-1 < TotalEnemyPlanets.Count)
                    {
                        AllyPlanetNearEnemies = TotalEnemyPlanets[currentPlanetIndex-1];
                    }

                }
            }

            if (currentPlanetIndex >= TotalEnemyPlanets.Count)
            {
                currentPlanetIndex = 0;
                SeekFarWayPlanetsChecker = 0;
            }

            if (TotalEnemyPlanets.Count > 0)
            {
                if((NearbyEmptyPlanetsInRange.Count == 0) && (AllyPlanetsInRange.Count == 0))
                {
                    SeekFarWayPlanetsChecker += 1;
                }

                transform.position = TotalEnemyPlanets[currentPlanetIndex].transform.position;

                if(currentPlanetIndex < TotalEnemyPlanets.Count)
                {
                    currentPlanetIndex += 1;
                }

                if (SeekFarWayPlanetsChecker == TotalEnemyPlanets.Count)
                {
                    SphereCollider Col = GetComponent<SphereCollider>();
                    Col.radius = Col.radius * 1.1f;
                    AllyPlanetNearEnemies = null;

                }
            }

            foreach (GameObject Planet in TotalEnemyPlanets)
            {
                CurrentPlanet = Planet;
                DecideWhatToDo();
            }

            TurnTimer = 0f;
        }
    }

    //Determine next actuin
    void DecideWhatToDo()
    {
        //Defend Planet
        if (CurrentPlanet.GetComponent<CaptureManager>().CaptureValue > -1)
        {
            DefendPlanet();
        }
        //If nearby planet, reset radius and coloniz
        if (NearbyEmptyPlanetsInRange.Count > 0)
        {
            Colonize();
            SphereCollider col = GetComponent<SphereCollider>();
            col.radius = radiusonstart;
        }
        //If no nearby empty, and more than 0 enemy planets, attack
        if ((NearbyEmptyPlanetsInRange.Count == 0) && (EnemyPlanetsInRange.Count > 0))
        {
            Attack();
            SphereCollider col = GetComponent<SphereCollider>();
            col.radius = radiusonstart;
        }

        //If no enemy/neutral planets, move in ally direction/Scout
        if((NearbyEmptyPlanetsInRange.Count == 0) && (EnemyPlanetsInRange.Count == 0) && (AllyPlanetsInRange.Count > 1))
        {
            MoveShipsTowardsAllies();
        }

    }

    //Planet defense
    void DefendPlanet()
    {
        float actioncount = 0f;
        foreach (GameObject Ship in ShipsICanMove)
        {
            if (actioncount < (PartySize + CurrentPlanet.GetComponent<CaptureManager>().TotalShips))
            {
                Vector3 MoveTarget = CurrentPlanet.transform.position;
                Ship.GetComponent<ShipAI>().MoveTarget(MoveTarget);
                actioncount += 1;
            }
        }
    }

    //Colonize planets
    void Colonize()
    {
        if (CurrentPlanet.GetComponent<CaptureManager>().CaptureValue == -1)
        {
            foreach (GameObject Planet in NearbyEmptyPlanetsInRange)
            {
                if (CurrentPlanet.GetComponent<CaptureManager>().TotalShips > 1)
                {
                    if (ShipsICanMove.Count > DefensiveFactor)
                    {
                        float SendShips = 0f;
                        foreach (GameObject Ship in ShipsICanMove)
                        {
                            if (SendShips < PartySize)
                            {
                                if (CurrentPlanet.GetComponent<CaptureManager>().TotalShips > 1)
                                {
                                    Vector3 MoveTarget = Planet.transform.position;
                                    Ship.GetComponent<ShipAI>().MoveTarget(MoveTarget);
                                }
                            }
                            SendShips += 1;
                        }
                    }
                }
            }
        }
        resetpos();
    }

    void Attack()
    {
        foreach (GameObject Planet in EnemyPlanetsInRange)
        {
            if (ShipsICanMove.Count > Planet.GetComponent<CaptureManager>().TotalShips)
            {
                foreach (GameObject Ship in ShipsICanMove)
                {
                    Vector3 MoveTarget = Planet.transform.position;
                    Ship.GetComponent<ShipAI>().MoveTarget(MoveTarget);
                }
            }
        }
        resetpos();
    }


    
    void MoveShipsTowardsAllies()
    {
        float shipsSent = 0f;
        foreach (GameObject Planet in AllyPlanetsInRange)
        {

                foreach (GameObject Ship in ShipsICanMove)
                {
                    if (shipsSent < (CurrentPlanet.GetComponent<CaptureManager>().TotalShips) - DefensiveFactor)
                    {
                        if (AllyPlanetNearEnemies != null)
                        {
                            Vector3 MoveTarget = AllyPlanetNearEnemies.transform.position;
                            Ship.GetComponent<ShipAI>().MoveTarget(MoveTarget);
                            shipsSent += 1;
                        }
                    }
                }
                ShipsICanMove.Clear();
        }
        resetpos();
    }



    void resetpos()
    {
        transform.position = startPos;
        AllyPlanetsInRange.Clear();
        EnemyPlanetsInRange.Clear();
        NearbyEmptyPlanetsInRange.Clear();

    }

    public void AddShip(GameObject Ship)
    {
        ShipsICanMove.Add(Ship);
    }
}

