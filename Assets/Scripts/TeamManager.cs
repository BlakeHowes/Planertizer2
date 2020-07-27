using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public int currentTeam = 0;

    public List<Color> teamColors = new List<Color>{Color.white, Color.blue, Color.red, Color.green, Color.yellow};

    public Color getTeamColor()
    {
        return teamColors[currentTeam];
    }
}
