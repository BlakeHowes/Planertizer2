using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{

    private Vector2 screenPos1 = Vector2.zero;
    private Vector3 dragPos1 = Vector3.zero;

    private Vector2 screenPos2 = Vector2.zero;
    private Vector3 dragPos2 = Vector3.zero;

    private Vector2 bottomLeft = Vector2.zero;
    private Vector2 topRight = Vector2.zero;

    private bool isDragging = false;


    [SerializeField]
    private float depth = 2;
    [SerializeField]
    private Image uiBox;

    private Plane plane = new Plane(Vector3.up, Vector3.zero);


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            gameObject.GetComponent<TargetManager>().ClearSelection(); //Selection clear, its kinda buggy, might need rework

            screenPos1 = Input.mousePosition;
            dragPos1 = ScreenCast(screenPos1);
            dragPos2 = dragPos1;

            uiBox.rectTransform.position = bottomLeft;
            uiBox.enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            dragPos2 = ScreenCast(screenPos2);
            uiBox.enabled = false;

            if (dragPos1 != Vector3.zero || dragPos2 != Vector3.zero)
            {
                CheckBounds();
            }

        }

        if (uiBox.enabled == true)
        {
            screenPos2 = Input.mousePosition;
            bottomLeft = new Vector2(Mathf.Min(screenPos1.x, screenPos2.x), Mathf.Min(screenPos1.y, screenPos2.y));
            topRight = new Vector2(Mathf.Max(screenPos1.x, screenPos2.x), Mathf.Max(screenPos1.y, screenPos2.y));

            uiBox.rectTransform.position = bottomLeft;
            
            uiBox.rectTransform.sizeDelta = topRight - bottomLeft;
        }

    }
    Vector3 VecAbs(Vector3 inVec)
    {
        Vector3 outVec = new Vector3(Mathf.Abs(inVec.x), Mathf.Abs(inVec.y), Mathf.Abs(inVec.z));

        return outVec;
    }

    Vector3 ScreenCast(Vector2 pos)
    {
        float enter;
        Vector3 hitPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (plane.Raycast(ray, out enter))
        {
            hitPoint = ray.GetPoint(enter);
        }
        return hitPoint;
        

            
    }

    void CheckBounds()
    {
        Vector3 size = VecAbs(dragPos1 - dragPos2);
        size /= 2;
        size.y = depth;

        LayerMask lm = LayerMask.GetMask("Ships");

        Collider[] inBounds = Physics.OverlapBox((dragPos1 + dragPos2)/2, size, Quaternion.identity, lm);
        int i = 0;
        Debug.Log("Checked Bounds");
        while (i < inBounds.Length)
        {
            /*Debug.Log("Hit : " + inBounds[i].name + " at index: " + i);*/
            gameObject.GetComponent<TargetManager>().SelectedShips.Add(inBounds[i].gameObject);
            GameObject Ship = (inBounds[i].gameObject);
            Ship.GetComponent<ShipAI>().Highlight();
            i++;
        }

    }


}
