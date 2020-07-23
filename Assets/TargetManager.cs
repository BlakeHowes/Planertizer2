using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    private List<GameObject> SelectedShips = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (GameObject Ship in SelectedShips)
            {
                if (Ship.gameObject.tag == "ALLIES")
                {
                    target = Ship.transform.GetChild(0);

                    if (Physics.Raycast(ray, out hit))
                    {
                        target.transform.position = hit.transform.position;
                    }
                }
            }
        }
    }
}