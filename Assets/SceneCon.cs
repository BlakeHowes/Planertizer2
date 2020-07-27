
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
    public bool startinstructions;
    public bool startExit;
    public Button Startbutton;
    void Start()
    {
        Startt.enabled = false;
        Instructions.enabled = false;
        Exit.enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 3)
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
            SceneManager.LoadScene("InGame");
        }

        if (startinstructions == true)
        {
            startinstructions = false;
            SceneManager.LoadScene("Instructions");
        }

        if (startExit == true)
        {
            startExit = false;
            Application.Quit();
        }
    }
    public void StartGame()
    {
        startbutton = true;
    }
    public void StartInstructions()
    {
        startinstructions = true;
    }

    public void StartExit()
    {
        startExit = true;
    }
}
