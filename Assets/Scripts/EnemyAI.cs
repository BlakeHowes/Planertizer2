using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    List<GameObject> EnemyPlanets = new List<GameObject>();
    [SerializeField]
    List<GameObject> NearbyEmpyPlanets = new List<GameObject>();
    [SerializeField]
    List<GameObject> AlliedPlanets = new List<GameObject>();

    public void AddPlanet(GameObject Planet,string Type)
    {
        if (Type == "Enemy")
        {
            EnemyPlanets.Add(Planet);
        }

        if (Type == "Empty")
        {
            NearbyEmpyPlanets.Add(Planet);
        }

        if (Type == "Allied")
        {
            AlliedPlanets.Add(Planet);
        }
    }

    public void RemovePlanet(GameObject Planet, string Type)
    {
        if (Type == "Enemy")
        {
            EnemyPlanets.Remove(Planet);
        }

        if (Type == "Empty")
        {
            NearbyEmpyPlanets.Remove(Planet);
        }

        if (Type == "Allied")
        {
            AlliedPlanets.Remove(Planet);
        }
    }
}
