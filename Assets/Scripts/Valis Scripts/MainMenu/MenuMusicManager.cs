using UnityEngine;
using UnityEngine.UI;

public class MenuMusicManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;  // Reference to the Audio Source for background music
    public Slider volumeSlider;                // Reference to the Slider
    
    private static MenuMusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSlider(GameObject slider)
    {
        volumeSlider = slider.GetComponent<Slider>();
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
        if (volumeSlider != null)
        {
            volumeSlider.value = backgroundMusicSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
        
    }

    public void SetVolume(float volume)
    {
        backgroundMusicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);  // Save volume setting for persistence
        
        Debug.Log("Volume set: " + backgroundMusicSource.volume);
    }

    void OnEnable()
    {
        // Load volume from PlayerPrefs when the scene starts
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);  // Default value is 0.5 if not set
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = savedVolume;
        }
        Debug.Log("Volume set: " + backgroundMusicSource.volume);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }
}