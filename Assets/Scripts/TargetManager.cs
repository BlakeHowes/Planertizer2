using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    public List<GameObject> SelectedShips = new List<GameObject>();
    public List<GameObject> ShipsToRemove = new List<GameObject>();
    [SerializeField]
    private LayerMask layermask;
    private bool paused;
    void Update()
    {
        foreach (GameObject Ship in SelectedShips)
        {
            if (Ship == null)
            {
                ShipsToRemove.Add(Ship);
            }
        }

        foreach (GameObject Ship in ShipsToRemove)
        {
            SelectedShips.Remove(Ship);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,Mathf.Infinity,layermask))
            {
                foreach (GameObject Ship in SelectedShips)
                {
                    if (Ship.gameObject.tag == "ALLIES")
                    {
                        if (hit.transform.tag == "PLANET")
                        {
                            hit.transform.gameObject.GetComponent<CaptureManager>().Highlight();
                            GameObject NewTargetPosition = hit.transform.gameObject;
                            float Altitude = NewTargetPosition.GetComponent<CaptureManager>().Altitude + Random.Range(-0.5f, 2f);
                            Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition,Altitude);
                        }

                        if (hit.transform.tag == "SPACE")
                        {
                            GameObject NewTargetPosition = hit.transform.gameObject;
                            float Altitude = 1 + Random.Range(-0.5f, 10f);
                            Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition, Altitude);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }

        if(paused == true)
        {
            Time.timeScale = 0f;
        }

        if (paused == false)
        {
            Time.timeScale = 1f;
        }

        ShipsToRemove.Clear();
    }
    public void ClearSelection()
    {
        foreach (GameObject Ship in SelectedShips)
        {
            Ship.GetComponent<ShipAI>().RemoveHighlight();
        }

        SelectedShips.Clear();
    }
}