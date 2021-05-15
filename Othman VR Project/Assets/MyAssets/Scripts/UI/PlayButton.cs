using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public Image circle;
    public UnityEvent playButtonClick;
    public float totalTime = 2;
    bool playButtonStatus;
    public float playButtonTimer = 0;

    // Update is called once per frame
    void Update()
    {
        if(playButtonStatus)
        {
            // update the timer count until it reaches time to activate button
            playButtonTimer += Time.deltaTime;

            // reflect progress with circle fill amount
            circle.fillAmount = playButtonTimer / totalTime;
        }

        if (playButtonTimer > totalTime)
        {
            playButtonClick.Invoke();
        }
    }

    public void buttonOn()
    {
        playButtonStatus = true;
    }

    public void buttonOff()
    {
        circle.fillAmount = 0;
        playButtonStatus = false;
        playButtonTimer = 0;
        //circle.fillAmount = 0;
    }

    public void loadMaze()
    {
        circle.fillAmount = 0;
        SceneManager.LoadScene("Maze");
    }

}
