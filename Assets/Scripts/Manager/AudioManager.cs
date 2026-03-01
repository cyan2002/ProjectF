using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The audio manager works by having two audio sources on the object and since it's a singleton, it can be accessed anywhere in the scene.
//Just use the methods in this script to play any audio clips.

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    [SerializeField] private List<SoundData> soundLibrary;
    private Dictionary<int, SoundData> soundDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<int, SoundData>();
        foreach (var entry in soundLibrary)
        {
            soundDictionary[entry.id] = entry;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        LoadVolumes();
    }

    public void PlayMusic(SoundData clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip.clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    //method for playing SFX
    public void PlaySFX(int idNum)
    {
        if (soundDictionary.TryGetValue(idNum, out SoundData data))
        {
            sfxSource.PlayOneShot(data.clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"Sound ID {idNum} not found in library!");
        }
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    public float getMusicVolume()
    {
        return musicVolume;
    }

    public float getSFXVolume()
    {
        return sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_KEY, volume);
    }

    public void PauseAudio()
    {
        sfxSource.Pause();
        musicSource.Pause();
    }

    public void ResumeAudio()
    {
        sfxSource.UnPause();
        musicSource.UnPause();
    }
}

