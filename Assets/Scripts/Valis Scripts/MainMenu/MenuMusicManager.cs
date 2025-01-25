using UnityEngine;
using UnityEngine.UI;

public class MenuMusicManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;  // Reference to the Audio Source for background music
    public Slider volumeSlider;                // Reference to the Slider
    public Slider effectVolumeSlider;                // Reference to the Slider
    
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

    public void SetEffectSlider(GameObject effectSlider)
    {
        effectVolumeSlider = effectSlider.GetComponent<Slider>();
    }
    
    void Start()
    {   
        
        // Ensure the AudioSource reference is assigned
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = GetComponent<AudioSource>();
            backgroundMusicSource.volume = 1f;
        }

        InitializeSliders();
    }

    public void InitializeSliders()
    {
        // Initialize the slider value with the AudioSource's current volume
        if (volumeSlider != null)
        {
            volumeSlider.value = backgroundMusicSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        if (effectVolumeSlider != null)
        {
            effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.5f);
            effectVolumeSlider.onValueChanged.AddListener(SetEffectVolume);
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
        
        Debug.Log("Music Volume set: " + backgroundMusicSource.volume);
    }

    public void SetEffectVolume(float effectVolume)
    {
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);  // Save volume setting for persistence
        
        Debug.Log("Effect Volume set: " + effectVolume);
    }

    void OnEnable()
    {
        // Load volume from PlayerPrefs when the scene starts
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);  // Default value is 0.5 if not set
        float savedEffectVolume = PlayerPrefs.GetFloat("EffectVolume", 0.5f);  // Default value is 0.5 if not set
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = savedVolume;
        }
        Debug.Log("Music Volume set: " + backgroundMusicSource.volume);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        if (effectVolumeSlider != null)
        {
            effectVolumeSlider.value = savedEffectVolume;
            Debug.Log("Effect Volume set: " + savedEffectVolume);
        }
    }
}