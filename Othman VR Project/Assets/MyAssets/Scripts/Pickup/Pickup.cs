using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pickup : MonoBehaviour
{
    public float rotationSpeed = 100.0f;// rotation speed of the pickup
    // Update is called once per frame
    void Update()
    {
        rotateObject();
    }

    void rotateObject()
    {
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }


     void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") // player has collected the object, they've finished the maze
        {
            // load the menu
            SceneManager.LoadScene("Menu");
        }
    }
}
