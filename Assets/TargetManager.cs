using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    public List<GameObject> SelectedShips = new List<GameObject>();

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
                    if (Ship.gameObject.tag == "ALLIES")
                    {
                        Vector3 NewTargetPosition = hit.transform.position;
                        Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition);
                    }
                }
            }
        }
    }
    public void ClearSelection()
    {
        SelectedShips.Clear();
    }
}