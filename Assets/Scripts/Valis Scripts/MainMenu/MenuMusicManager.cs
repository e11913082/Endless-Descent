using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuVolumeManager : MonoBehaviour
{   
    public static MenuVolumeManager Instance;
    
    public AudioSource backgroundMusicSource;
    public AudioSource effectAudioSource; // Reference to the Audio Source for background music
    public Slider musicSlider;
    public Slider effectSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
    }

    void Start()
    {
        // Ensure the AudioSource reference is assigned
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = GetComponent<AudioSource>();
            backgroundMusicSource.volume = 1f;
        }
        
        // Initialize the slider value with the AudioSource's current volume
        if (musicSlider != null)
        {
            musicSlider.value = backgroundMusicSource.volume;
            musicSlider.onValueChanged.AddListener(SetVolumeMusic);
        }

        if (effectSlider != null)
        {
            effectSlider.onValueChanged.AddListener(SetVolumeEffect);
        }
        
        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
        
    }

    public void SetVolumeMusic(float volume)
    {
        backgroundMusicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);  // Save volume setting for persistence
        
        Debug.Log("Music Volume set: " + backgroundMusicSource.volume);
    }

    public void SetVolumeEffect(float volume)
    {
        PlayerPrefs.SetFloat("EffectVolume", volume);
        
        Debug.Log("Effect Volume set: " + backgroundMusicSource.volume);
    }

    void OnEnable()
    {
        // Load volume from PlayerPrefs when the scene starts
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);  // Default value is 0.5 if not set
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = savedMusicVolume;
        }
        Debug.Log("Music Volume set: " + backgroundMusicSource.volume);
        if (musicSlider != null)
        {
            musicSlider.value = savedMusicVolume;
        }
        
        
        
    }
}