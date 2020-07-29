
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCon : MonoBehaviour
{
    private float timer;
    public SpriteRenderer Startt;
    public SpriteRenderer Instructions;
    public SpriteRenderer Exit;
    public bool startbutton;
    [SerializeField]
    public bool startinstructions;
    public bool startExit;
    public Button Startbutton;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool gotomenu;
    private float speedset;
    private float reset;
    [SerializeField]
    private float timetaken;
    void Start()
    {
        Startt.enabled = false;
        Instructions.enabled = false;
        Exit.enabled = false;
        speedset = speed;
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if(timer > 1)
        {
            if(Startt != null)
            {
                Startt.enabled = true;
            }
            if (Instructions != null)
            {
                Instructions.enabled = true;
            }
            if (Exit != null)
            {
                Exit.enabled = true;
            }
        }
        if (startbutton == true)
        {
            startbutton = false;
            SceneManager.LoadScene("InGame 1");
        }

        

        if (startExit == true)
        {
            startExit = false;
            Application.Quit();
        }

        if (startinstructions == true)
        {
            Vector3 newpos = new Vector3(-534, 11, -615);
            camera.transform.position = Vector3.Lerp(camera.transform.position, newpos,(speed/10));
            Quaternion endrotation = Quaternion.Euler(0, -90, 0);
            reset += Time.deltaTime;

            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, endrotation,speed);

            if(reset > timetaken)
            {
                startinstructions = false;
                speed = speedset;
                reset = 0f;
            }
        }

        if (gotomenu == true)
        {
            Vector3 startpos = new Vector3(0f, 0f, 0f);
            Vector3 newpos = new Vector3(-534, 11, -615);
            camera.transform.position = Vector3.Lerp(camera.transform.position, startpos,(speed / 10));
            Quaternion startrotation = Quaternion.Euler(0, 0, 0);
            reset += Time.deltaTime;

            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, startrotation,speed);
            if (reset > timetaken)
            {
                gotomenu = false;
                speed = speedset;
                reset = 0f;
            }
        }

    }
    public void StartGame()
    {
        startbutton = true;
    }
    public void StartInstructions()
    {
        startinstructions = true;
        speed = speedset;
        reset = 0f;
    }

    public void StartExit()
    {
        startExit = true;
    }

    public void GoToMenu()
    {
        gotomenu = true;
        speed = speedset;
        reset = 0f;
    }
}
