using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenMusic : MonoBehaviour
{
    public SoundData sound;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(sound);
    }
}
