using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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
    private int NumberOfShipsUsedToColonize;
    private float TurnTimer;
    private GameObject CurrentPlanet;

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
        TurnTimer += Time.deltaTime;
        if (TurnTimer >= AiSpeed)
        {
            foreach (GameObject Planet in TotalEnemyPlanets)
            {
                transform.position = Planet.transform.position;
                CurrentPlanet = Planet;
                DecideWhatToDo();
            }

            TurnTimer = 0f;
        }
    }

    void DecideWhatToDo()
    {
        if (NearbyEmpyPlanetsInRange.Count > 0)
        {
            Colonize();
        }

        if ((NearbyEmpyPlanetsInRange.Count == 0) && (AlliedPlanetsInRange.Count > 0))
        {
            Attack();
        }

        if((NearbyEmpyPlanetsInRange.Count == 0) && (AlliedPlanetsInRange.Count == 0) && (EnemyPlanetsInRange.Count > 1))
        {
            MoveShipsTowardsAllies();
        }
    }

    void Colonize()
    {
        if (CurrentPlanet.GetComponent<CaptureManager>().AmICaptured == -1)
        {
            foreach (GameObject Planet in NearbyEmpyPlanetsInRange)
            {
                if (CurrentPlanet.GetComponent<CaptureManager>().TotalShips > DefensiveFactor)
                {
                    if (ShipsICanMove.Count > DefensiveFactor)
                    {
                        float SendShips = 0f;
                        foreach (GameObject Ship in ShipsICanMove)
                        {
                            if (SendShips < NumberOfShipsUsedToColonize)
                            {
                                if (CurrentPlanet.GetComponent<CaptureManager>().TotalShips > DefensiveFactor)
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
    }

    void Attack()
    {

    }

    void MoveShipsTowardsAllies()
    {
        
    }
}
