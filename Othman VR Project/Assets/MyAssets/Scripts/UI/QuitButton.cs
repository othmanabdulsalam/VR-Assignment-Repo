using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuitButton : MonoBehaviour
{
    public Image circle;
    public UnityEvent quitButtonClick;
    public float totalTime = 2;
    bool quitButtonStatus;
    public float quitButtonTimer = 0;


    void Update()
    {
        if (quitButtonStatus)
        {
            //Debug.Log("It works!!!");
            // update the timer count until it reaches time to activate button
            quitButtonTimer += Time.deltaTime;

            // reflect progress with circle fill amount
            circle.fillAmount = quitButtonTimer / totalTime;
        }
        if (quitButtonTimer > totalTime)
        {
            quitButtonClick.Invoke();
        }
    }



    public void buttonOn()
    {
        quitButtonStatus = true;
    }

    public void buttonOff()
    {
        quitButtonStatus = false;
        quitButtonTimer = 0;
        circle.fillAmount = 0;
    }


    public void quitApp()
    {
        Application.Quit();
        Debug.Log("Quit the application");
    }
}
