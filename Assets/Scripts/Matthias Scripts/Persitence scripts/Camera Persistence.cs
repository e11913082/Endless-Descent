
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraPersistence : MonoBehaviour
{
   private static CameraPersistence instance;
   
   public void Awake()
    {   
            if(instance == null)
            {   
                if (SceneManager.GetActiveScene().name != "MainMenu")
                {
                    instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                
            }
            else
            {   
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    Destroy(instance.gameObject);
                    instance = null;
                }
                else
                {
                    Destroy(gameObject);    
                }
            }
    }
}
