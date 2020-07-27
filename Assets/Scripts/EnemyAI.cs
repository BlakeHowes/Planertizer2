using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private int currentPlanetIndex = 0;

    [SerializeField]
    public List<GameObject> ShipsICanMove = new List<GameObject>();
    List<GameObject> ShipsToRemove = new List<GameObject>();


    [SerializeField]
    private float AiSpeed; //this will determine the rate at which the AI performs actions
    private float turnTimer = 0f;

    private Vector3 startPos;
    private float sphereRadiusDefault = 2.5f;

    public List<GameObject> NearbyNeutralPlanets = new List<GameObject>();
    public List<GameObject> NearbyAllyPlanets = new List<GameObject>();
    public List<GameObject> NearbyEnemyPlanets = new List<GameObject>();
    public List<GameObject> NearbyPlanets = new List<GameObject>();
    public List<GameObject> NearbyAllyShips = new List<GameObject>();




    private void Start()
    {
        startPos = transform.position;
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
                NearbyNeutralPlanets.Add(planet);
                break;
            case -1:
                NearbyAllyPlanets.Add(planet);
                break;
            case 1:
                NearbyEnemyPlanets.Add(planet);
                break;

        }

        NearbyPlanets.Add(planet);
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
                NearbyNeutralPlanets.Remove(planet);
                break;
            case -1:
                NearbyAllyPlanets.Remove(planet);
                break;
            case 1:
                NearbyEnemyPlanets.Remove(planet);
                break;

        }

    }

     private void Update()
    {

        foreach (GameObject Ship in ShipsICanMove)
        {
            if (Ship == null)
            {
                ShipsICanMove.Remove(Ship);
                Debug.Log("Removed Ship at index: " + ShipsICanMove.IndexOf(Ship));
            }
        }


        //turn
        turnTimer += Time.deltaTime;

        if (turnTimer >= AiSpeed)
        {
            currentPlanetIndex++;

            transform.position = NearbyPlanets[currentPlanetIndex].transform.position;
            CheckRadius();
            DecideWhatToDo();
        }

    }

    //Check for nearrby planets/ships
    void CheckRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadiusDefault);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "PLANET")
            {
                AddPlanet(hitCollider.gameObject);
            }
            if (hitCollider.tag == "SHIP")
            {
                if (hitCollider.gameObject.GetComponent<TeamManager>().currentTeam == GetComponent<TeamManager>().currentTeam)
                {
                    NearbyAllyShips.Add(hitCollider.gameObject);
                }

            }

        }
    }


        //Determine next action
    void DecideWhatToDo()
    {
        GameObject CurrentPlanet = NearbyPlanets[currentPlanetIndex];
        //Defend Planet
        if (CurrentPlanet.GetComponent<CaptureManager>().CaptureValue > -1)
        {
            Defend(CurrentPlanet);
        }
        //If nearby planet, reset radius and coloniz
        if (NearbyNeutralPlanets.Count > 0)
        {
            Colonize();

        }
        //If no nearby empty, and more than 0 enemy planets, attack
        if (NearbyNeutralPlanets.Count == 0 && NearbyEnemyPlanets.Count > 0)
        {
            Attack();
        }

        //If no enemy/neutral planets, move in ally direction/Scout
        if (NearbyNeutralPlanets.Count == 0 && NearbyEnemyPlanets.Count == 0 && NearbyAllyPlanets.Count > 1)
        {
            ScoutForward();
        }

    }

    void Attack()
    {

    }

    void Defend(GameObject planet)
    {
        foreach(GameObject Ship in NearbyAllyShips)
        {
            Ship.GetComponent<TargetManager>().target.position = planet.transform.position;
        }
    }

    void Colonize()
    {

    }

    void ScoutForward()
    {

    }

}

