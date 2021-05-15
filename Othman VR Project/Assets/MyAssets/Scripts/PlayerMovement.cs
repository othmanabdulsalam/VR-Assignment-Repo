using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    Transform camera;
    public bool prevLookingForward = false, lookingForward = false,
    isMoving = false;
    public bool toggleForwardMotion, startLookingForward;
    public float speed = 1.0f;
    float toggleAngle = 30.0f;

    //public GameObject lookBar;
    float timeToMove = 2.0f; // time before making a start
    float moveTimer = 0f;
    float timeToStop = 0.5f; // time before making a stop
    float stopTimer = 0f;

    public MeshRenderer reticle;

    // Use this for initialization
    void Start()
    {
        camera = GameObject.Find("Main Camera").transform;
        characterController = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }


    void handleMovement()
    {
        prevLookingForward = lookingForward;
        if (camera.transform.eulerAngles.x >= 15 && camera.transform.eulerAngles.x < 100)
        {
            //if (stopTimer < timeToStop)
            //{
            //    stopTimer += Time.deltaTime; // add to time count
            //    updateLookBar(stopTimer,timeToMove);
            //}
            //else 
            //{
            //    lookBar.GetComponentInChildren<Image>().fillAmount = 0;
            //    stopTimer = 0f; // reset timer to 0
            //    lookingForward = false;
            //}
            lookingForward = false;
        }
        else
        {
            lookingForward = true;
        }

        if (lookingForward == true && prevLookingForward == false)
        {
            startLookingForward = true;
        }
        else
        {
            startLookingForward = false;
        }
        if (startLookingForward)
        {
            toggleForwardMotion = !toggleForwardMotion; // swaps to whatever bool value it was not
        }

        if (lookingForward && toggleForwardMotion)
        {
            isMoving = true;
        }
        else
        {
            //Debug.Log("You have stopped");

            isMoving = false;
        }

        if (isMoving)
        {
            // set reticle colour to indicate movement
            reticle.material.SetColor("_Color",Color.white);

            //timer = 0f; // reset the time
            //lookBar.GetComponentInChildren<Image>().fillAmount = 0f;

            Vector3 forward = camera.TransformDirection(Vector3.forward);
            characterController.SimpleMove(forward * speed);
            return;
        }

        // set reticle colour to indicate pause
        reticle.material.SetColor("_Color", Color.black);
    }
    void updateLookBar(float timer,float timeAmount)
    {
        //lookBar.GetComponentInChildren<Image>().fillAmount = barAmount(timer,timeAmount);
    }
    float barAmount(float timer,float timeAmount)
    {
        float barAmount = timer / timeAmount;
        return barAmount;
    }
}

