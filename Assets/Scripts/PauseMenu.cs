using EndlessDescent;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{   
    private Canvas settingsCanvas;
    public GameObject pauseMenu;
    private bool resume;
    private PlayerCharacter player;
    // Start is called before the first frame update
    void Start()
    {   
        player = GameObject.Find("/Main Character").GetComponent<PlayerCharacter>();
        player.SetPauseMenu(gameObject.transform.parent.gameObject.gameObject);
        MenuMusicManager musicManager = GameObject.Find("/BackgroundMusic").GetComponent<MenuMusicManager>();
        musicManager.SetSlider(GameObject.Find("/PauseMenu/PauseCanvas/PausePanel/SettingsCanvas/Slider"));
        musicManager.SetEffectSlider(GameObject.Find("/PauseMenu/PauseCanvas/PausePanel/SettingsCanvas/EffectSlider"));
        musicManager.InitializeSliders();
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
        pauseMenu.SetActive(false);
        resume = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) | (resume && pauseMenu.activeInHierarchy))
        {   
            player.UpdateControls();
            resume = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
    
    public void Resume()
    {
        resume = true;
    }

    public void OpenSettings()
    {
        if (settingsCanvas.enabled == false)
        {
            settingsCanvas.enabled = true;
        }
        else
        {
            settingsCanvas.enabled = false;
        }
    }

    public void ReturnToMainMenu()
    {
        //Time.timeScale = 1f;
        player.ResetStats();
        Destroy(GameObject.Find("/Hud V2"));
        Destroy(GameObject.Find("/Main Character"));
        Destroy(GameObject.Find("/BackgroundMusic"));
        Destroy(GameObject.Find("/Loop Entrance"));
        SceneManager.LoadScene("MainMenu");
    }
    
    
}
