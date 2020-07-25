﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    public List<GameObject> SelectedShips = new List<GameObject>();
    public List<GameObject> ShipsToRemove = new List<GameObject>();
    void Update()

    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                foreach (GameObject Ship in SelectedShips)
                {
                    if (Ship == null)
                    {
                        ShipsToRemove.Add(Ship);
                        continue;
                    }

                    if (Ship.gameObject.tag == "ALLIES")
                    {
                        Vector3 NewTargetPosition = hit.transform.position;
                        Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition);
                    }
                }

                foreach (GameObject Ship in ShipsToRemove)
                {
                    SelectedShips.Remove(Ship);
                }
            }
        }
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