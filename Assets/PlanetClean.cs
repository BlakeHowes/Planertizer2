using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetClean : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private GameObject Ring;

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

    void Update()
    {
        TakeOverCheck();

        if(TakeOverCheck() == true)
        {
            Dictionary<int, int> SubTeamList = new Dictionary<int, int>();
            Collider[] AirSpace = Physics.OverlapSphere(transform.position, radius);
            foreach (var Ship in AirSpace)
            {
                int SubTeamValue = Ship.GetComponent<ShipClean>().GetSubTeam();
                if (!SubTeamList.ContainsKey(SubTeamValue))
                {
                    SubTeamList[SubTeamValue] = 0;
                }
                SubTeamList[SubTeamValue] += 1;
            }

            int SubTeamWithHighestCount = 0;
            int HighestCount = 0;
            foreach (int SubTeam in SubTeamList.Keys)
            {
                if(SubTeamList[SubTeam] > HighestCount)
                {
                    HighestCount = SubTeamList[SubTeam];
                    SubTeamWithHighestCount = SubTeam;
                }
            }
            Debug.Log(SubTeamWithHighestCount);
        }
    }

    public void HighlightRing()
    {
        
    }
}