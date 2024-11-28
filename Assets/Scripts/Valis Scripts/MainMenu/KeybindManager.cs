using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{   
    public static KeybindManager instance;
    
    public Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();
    
    // UI references
    public Button upButton;
    public TextMeshProUGUI upText;
    public Button downButton;
    public TextMeshProUGUI downText;
    public Button rightButton;
    public TextMeshProUGUI rightText;
    public Button leftButton;
    public TextMeshProUGUI leftText;
    public Button attackButton;
    public TextMeshProUGUI attackText;
    public Button interactButton;
    public TextMeshProUGUI interactText;
    public Button switchButton;
    public TextMeshProUGUI switchText;
    public Button dropButton;
    public TextMeshProUGUI dropText;
    
    
    private string keyToBind = "";


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        keybinds["up"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", KeyCode.W.ToString()));
        keybinds["down"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", KeyCode.S.ToString()));
        keybinds["right"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", KeyCode.D.ToString()));
        keybinds["left"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", KeyCode.A.ToString()));
        keybinds["attack"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("attack", KeyCode.Space.ToString()));
        keybinds["interact"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", KeyCode.E.ToString()));
        keybinds["switch"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("switch", KeyCode.Tab.ToString()));
        keybinds["drop"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("drop", KeyCode.Q.ToString()));
    }

    // Start is called before the first frame update
    void Start()
    {
       
        UpdateKeybindUI();
        
        upButton.onClick.AddListener(() => StartRebinding("up"));
        downButton.onClick.AddListener(() => StartRebinding("down"));
        rightButton.onClick.AddListener(() => StartRebinding("right"));
        leftButton.onClick.AddListener(() => StartRebinding("left"));
        attackButton.onClick.AddListener(() => StartRebinding("attack"));
        interactButton.onClick.AddListener(() => StartRebinding("interact"));
        switchButton.onClick.AddListener(() => StartRebinding("switch"));
        dropButton.onClick.AddListener(() => StartRebinding("drop"));
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(keyToBind))
        {
            ListenForKeyPress();
        }   
    }

    public void StartRebinding(string key)
    {
        keyToBind = key;
        Debug.Log("Press a key to rebind: "+key);
    }
    
    
    public void ListenForKeyPress()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    keybinds[keyToBind] = key;
                    
                    PlayerPrefs.SetString(keyToBind, key.ToString());
                    PlayerPrefs.Save();

                    UpdateKeybindUI();
                    
                    keyToBind = "";
                    Debug.Log("Rebinding complete for " + keyToBind + " to " + key);

                    break;
                }
            }
        }
    }

    void UpdateKeybindUI()
    {
        upText.text = keybinds["up"].ToString();
        downText.text = keybinds["down"].ToString();
        rightText.text = keybinds["right"].ToString();
        leftText.text = keybinds["left"].ToString();
        attackText.text = keybinds["attack"].ToString();
        interactText.text = keybinds["interact"].ToString();
        switchText.text = keybinds["switch"].ToString();
        dropText.text = keybinds["drop"].ToString();
    }
}
