using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetClean : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private GameObject Ring;

    void Update()
    {
        TakeOverCheck();

        if(TakeOverCheck() == true)
        {
            Dictionary<int, int> SubTeamList = new Dictionary<int, int>();
            Collider[] AirSpace = Physics.OverlapSphere(transform.position, radius);
            foreach (var Ship in AirSpace)
            {
              
            }
        }
    }

    private bool TakeOverCheck()
    {
        int FirstTeamFound = -1;
        Collider[] AirSpace = Physics.OverlapSphere(transform.position, radius);
        foreach (var Ship in AirSpace)
        {
            if (FirstTeamFound == -1)
            {
                FirstTeamFound = Ship.GetComponent<ShipClean>().GetTeam();
            }

            if (Ship.GetComponent<ShipClean>().GetTeam() != FirstTeamFound)
            {
                return false;
            }
        }
        return true;
    }

    public void HighlightRing()
    {
        
    }
}