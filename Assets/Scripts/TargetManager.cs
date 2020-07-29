using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TargetManager : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    public List<GameObject> SelectedShips = new List<GameObject>();
    public List<GameObject> ShipsToRemove = new List<GameObject>();
    [SerializeField]
    private LayerMask layermask;
    private bool paused;
    [SerializeField]
    private GameObject Buttons;
    [SerializeField]
    private GameObject targetring;
    private bool removering;
    private float ringtimer;

    private void Awake()
    {
        targetring.SetActive(false);
        Buttons.SetActive(false);
    }
    void Update()
    {
        if(removering == true)
        {
            ringtimer += Time.deltaTime;
            if(ringtimer > 1)
            {
                removering = false;
                targetring.SetActive(false);
                ringtimer = 0;
            }
        }

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

                        if(hit.transform.tag == "OBJECT")
                        {
                            GameObject NewTargetPosition = hit.transform.gameObject;
                            float Altitude = NewTargetPosition.transform.localScale.x + 6f + Random.Range(-0.5f, 2f);
                            Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition, Altitude);
                        }

                        if (hit.transform.tag == "SPACE")
                        {
                            Vector3 pos = hit.point;
                            float Altitude = 1 + Random.Range(-0.5f, 10f);
                            Ship.GetComponent<ShipAI>().MoveTargetToSpace(pos, Altitude);

                            targetring.SetActive(true);
                            targetring.transform.position = hit.point;
                            removering = true;
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
            Buttons.SetActive(true);
        }

        if (paused == false)
        {
            Time.timeScale = 1f;
            Buttons.SetActive(false);
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

    public void resume()
    {
        paused = false;
    }

    public void MainMenu1()
    {
        paused = !paused;
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}