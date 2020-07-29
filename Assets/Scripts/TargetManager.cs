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
    private SpriteRenderer PauseMenu;
    [SerializeField]
    private SpriteRenderer ResumeMenu;
    [SerializeField]
    private SpriteRenderer MainMenu;
    [SerializeField]
    private SpriteRenderer ExitMenu;
    [SerializeField]
    private Canvas Buttons;
    private void Awake()
    {
        PauseMenu.enabled = false;
        ResumeMenu.enabled = false;
        MainMenu.enabled = false;
        ExitMenu.enabled = false;
        Buttons.enabled = false;
    }
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

                        if(hit.transform.tag == "OBJECT")
                        {
                            GameObject NewTargetPosition = hit.transform.gameObject;
                            float Altitude = NewTargetPosition.transform.localScale.x + Random.Range(-0.5f, 2f);
                            Ship.GetComponent<ShipAI>().MoveTarget(NewTargetPosition, Altitude);
                        }

                        if (hit.transform.tag == "SPACE")
                        {
                            Vector3 pos = hit.point;
                            float Altitude = 1 + Random.Range(-0.5f, 10f);
                            Ship.GetComponent<ShipAI>().MoveTargetToSpace(pos, Altitude);
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
            PauseMenu.enabled = true;
            ResumeMenu.enabled = true;
            MainMenu.enabled = true;
            ExitMenu.enabled = true;
            Buttons.enabled = true;
        }

        if (paused == false)
        {
            Time.timeScale = 1f;
            PauseMenu.enabled = false;
            ResumeMenu.enabled = false;
            MainMenu.enabled = false;
            ExitMenu.enabled = false;
            Buttons.enabled = false;
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
        paused = !paused;
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