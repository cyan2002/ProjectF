using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBootstrapper : MonoBehaviour
{
    [SerializeField] private AudioManager audioManagerPrefab;

    private void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }
    }
}
