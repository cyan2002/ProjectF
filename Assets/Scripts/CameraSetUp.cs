using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

//finds the player in the start of a scene and connects to it
public class CameraSetUp : MonoBehaviour
{
    void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
        else
        {
            Debug.Log("Player not found!");
        }
    }
}