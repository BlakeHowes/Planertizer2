using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSceneCOn : MonoBehaviour
{
    public List<GameObject> PlanetScore = new List<GameObject>();
    public int controlfunction;
    private float checktimer;
    void Awake()
    {
        GameObject[] Planet = GameObject.FindGameObjectsWithTag("PLANET");
        PlanetScore.AddRange(Planet);
    }

    public void Update()
    {
        checktimer += Time.deltaTime;
        if (checktimer > 5)
        {
            foreach (GameObject Planet in PlanetScore)
            {
                if (Planet.GetComponent<CaptureManager>() != null)
                {
                    if (Planet.GetComponent<CaptureManager>().Spawning == 1)
                    {
                        controlfunction += 1;
                    }

                    if (Planet.GetComponent<CaptureManager>().Spawning == -1)
                    {
                        controlfunction += -1;
                    }
                }
            }

            if (controlfunction == PlanetScore.Count)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

            if ((controlfunction * -1) == PlanetScore.Count)
            {
                SceneManager.LoadScene("MainMenu");
            }

            controlfunction = 0;
            checktimer = 0f;
        }

    }
}
