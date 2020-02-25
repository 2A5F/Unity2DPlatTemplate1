using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWing : MonoBehaviour
{
    public GameObject Wing;

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F))
        {
            Wing.SetActive(true);
        } else
        {
            Wing.SetActive(false);
        }
    }
}
