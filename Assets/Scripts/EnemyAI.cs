using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private int i;

    [SerializeField]
    List<GameObject> TotalEnemyPlanets = new List<GameObject>();
    [SerializeField]
    List<GameObject> EnemyPlanetsInRange = new List<GameObject>();
    [SerializeField]
    public List<GameObject> NearbyEmpyPlanetsInRange = new List<GameObject>();
    [SerializeField]
    List<GameObject> AlliedPlanetsInRange = new List<GameObject>();
    [SerializeField]
    public List<GameObject> ShipsICanMove = new List<GameObject>();


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
    GameObject remeberme;
    private int SeekFarWayPlanetsChecker;
    private float radiusonstart;
    private int ifixed;

    private void Awake()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        radiusonstart = col.radius;
    }

    public void AddPlanet(GameObject Planet,string Type)
    {
        if (Type == "MyPlanets")
        {
            TotalEnemyPlanets.Add(Planet);
        }

        if (Type == "Enemy")
        {
            EnemyPlanetsInRange.Add(Planet);
        }

        if (Type == "Empty")
        {
            NearbyEmpyPlanetsInRange.Add(Planet);
        }

        if (Type == "Allied")
        {
            AlliedPlanetsInRange.Add(Planet);
        }
    }

    public void RemovePlanet(GameObject Planet, string Type)
    {
        if (Type == "MyPlanets")
        {
            TotalEnemyPlanets.Remove(Planet);
        }

        if (Type == "Enemy")
        {
            EnemyPlanetsInRange.Remove(Planet);
        }

        if (Type == "Empty")
        {
            NearbyEmpyPlanetsInRange.Remove(Planet);
        }

        if (Type == "Allied")
        {
            AlliedPlanetsInRange.Remove(Planet);
        }
    }

    public void AddShip(GameObject Ship)
    {
        ShipsICanMove.Add(Ship);
    }

    public void RemoveShip(GameObject Ship)
    {
        ShipsICanMove.Remove(Ship);
    }

    private void Update()
    {
        List<GameObject> ShipsToRemove = new List<GameObject>();
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
        }

        ShipsToRemove.Clear();

        TurnTimer += Time.deltaTime;
        if (TurnTimer >= AiSpeed)
        {
            if (AlliedPlanetsInRange.Count > 0)
            {
                if(i > 0)
                {
                    ifixed = i - 1;
                    remeberme = TotalEnemyPlanets[ifixed];
                }
            }

            if (i >= TotalEnemyPlanets.Count)
            {
                i = 0;
                SeekFarWayPlanetsChecker = 0;
            }

            if (TotalEnemyPlanets.Count > 0)
            {
                if((NearbyEmpyPlanetsInRange.Count == 0) && (AlliedPlanetsInRange.Count == 0))
                {
                    SeekFarWayPlanetsChecker += 1;
                }

                transform.position = TotalEnemyPlanets[i].transform.position;

                if(i < TotalEnemyPlanets.Count)
                {
                    i += 1;
                }

                if (SeekFarWayPlanetsChecker == TotalEnemyPlanets.Count)
                {
                    SphereCollider Col = GetComponent<SphereCollider>();
                    Col.radius = Col.radius * 1.1f;
                    remeberme = null;

                }
            }

            foreach (GameObject Planet in TotalEnemyPlanets)
            {
                CurrentPlanet = Planet;
                if(Planet.GetComponent<CaptureManager>().AmICaptured > -1)

                {
                    DefendPlanet();
                }

                DecideWhatToDo();
            }

            TurnTimer = 0f;
            reset = true;
        }

        if (TurnTimer > 0.1f)
        {
            if(CurrentPlanet != null)
            {
  
            }
        }
    }
    void DecideWhatToDo()
    {
        if (NearbyEmpyPlanetsInRange.Count > 0)
        {
            Colonize();
            SphereCollider col = GetComponent<SphereCollider>();
            col.radius = radiusonstart;
        }

        if ((NearbyEmpyPlanetsInRange.Count == 0) && (AlliedPlanetsInRange.Count > 0))
        {
            Attack();
            SphereCollider col = GetComponent<SphereCollider>();
            col.radius = radiusonstart;
        }

        if((NearbyEmpyPlanetsInRange.Count == 0) && (AlliedPlanetsInRange.Count == 0) && (EnemyPlanetsInRange.Count > 1))
        {
            MoveShipsTowardsAllies();
        }
    }

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
        resetpos();
    }

    void Colonize()
    {
        if (CurrentPlanet.GetComponent<CaptureManager>().AmICaptured == -1)
        {
            foreach (GameObject Planet in NearbyEmpyPlanetsInRange)
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
        foreach (GameObject Planet in AlliedPlanetsInRange)
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
        float actioncount = 0f;
        foreach (GameObject Planet in EnemyPlanetsInRange)
        {

                foreach (GameObject Ship in ShipsICanMove)
                {
                    if (actioncount < (CurrentPlanet.GetComponent<CaptureManager>().TotalShips) - DefensiveFactor)
                    {
                    if (remeberme != null)
                    {
                        Vector3 MoveTarget = remeberme.transform.position;
                        Ship.GetComponent<ShipAI>().MoveTarget(MoveTarget);
                        actioncount += 1;
                    }
                    }
                }
                ShipsICanMove.Clear();
        }
    }

    void resetpos()
    {


        Vector3 resetcolpos = new Vector3(0f, 200f, 0f);
        transform.position = resetcolpos;
        EnemyPlanetsInRange.Clear();
        AlliedPlanetsInRange.Clear();
        NearbyEmpyPlanetsInRange.Clear();
        reset = false;

        if (TurnTimer > 0.1)
        {
            transform.position = TotalEnemyPlanets[ifixed].transform.position;
        }
    }
}

