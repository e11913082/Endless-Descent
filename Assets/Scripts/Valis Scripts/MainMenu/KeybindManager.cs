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
    private Dictionary<string, TextMeshProUGUI> keybindTexts = new Dictionary<string, TextMeshProUGUI>();
    
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
    public Button dashButton;
    public TextMeshProUGUI dashText;
    
    
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
        
        keybinds["up"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", KeyCode.W.ToString()));
        keybinds["down"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", KeyCode.S.ToString()));
        keybinds["right"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", KeyCode.D.ToString()));
        keybinds["left"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", KeyCode.A.ToString()));
        keybinds["attack"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("attack", KeyCode.Mouse0.ToString()));
        keybinds["interact"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", KeyCode.E.ToString()));
        keybinds["switch"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("switch", KeyCode.Tab.ToString()));
        keybinds["drop"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("drop", KeyCode.Q.ToString()));
        keybinds["dash"] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("dash", KeyCode.Space.ToString()));
        
        keybindTexts["up"] = upText;
        keybindTexts["down"] = downText;
        keybindTexts["right"] = rightText;
        keybindTexts["left"] = leftText;
        keybindTexts["attack"] = attackText;
        keybindTexts["interact"] = interactText;
        keybindTexts["switch"] = switchText;
        keybindTexts["drop"] = dropText;
        keybindTexts["dash"] = dashText;
    }

    // Start is called before the first frame update
    void Start()
    {
       
        UpdateKeybindUI();
        
        upButton.onClick.AddListener(() => StartRebinding("up", upText));
        downButton.onClick.AddListener(() => StartRebinding("down", downText));
        rightButton.onClick.AddListener(() => StartRebinding("right", rightText));
        leftButton.onClick.AddListener(() => StartRebinding("left", leftText));
        attackButton.onClick.AddListener(() => StartRebinding("attack", attackText));
        interactButton.onClick.AddListener(() => StartRebinding("interact", interactText));
        switchButton.onClick.AddListener(() => StartRebinding("switch", switchText));
        dropButton.onClick.AddListener(() => StartRebinding("drop", dropText));
        dashButton.onClick.AddListener(() => StartRebinding("dash", dashText));
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(keyToBind))
        {
            ListenForKeyPress();
        }  
    }

    public void StartRebinding(string key, TextMeshProUGUI text)
    {
        text.text = "<press a key>";
        keyToBind = key;
        Debug.Log("Press a key to rebind: "+key);
        
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        interactButton.gameObject.SetActive(false);
        switchButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
        dashButton.gameObject.SetActive(false);
    }
    
    
    public void ListenForKeyPress()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {   
                    CheckKeybindUsed(key);
                    
                    keybinds[keyToBind] = key;
                    
                    
                    PlayerPrefs.SetString(keyToBind, key.ToString());
                    PlayerPrefs.Save();

                    UpdateKeybindUI();
                    
                    keyToBind = "";
                    Debug.Log("Rebinding complete for " + keyToBind + " to " + key);
                    
                    upButton.gameObject.SetActive(true);
                    downButton.gameObject.SetActive(true);
                    rightButton.gameObject.SetActive(true);
                    leftButton.gameObject.SetActive(true);
                    attackButton.gameObject.SetActive(true);
                    interactButton.gameObject.SetActive(true);
                    switchButton.gameObject.SetActive(true);
                    dropButton.gameObject.SetActive(true);
                    dashButton.gameObject.SetActive(true);
                    
                    break;
                }
            }
        }
    }

    private void CheckKeybindUsed(KeyCode key)
    {
       if (keybinds.ContainsValue(key) && keybinds[keyToBind] != key)
       {
           String usedKeyString = "";
           foreach (KeyValuePair<string, KeyCode> pair in keybinds)
           {
               if (pair.Value == key)
               {
                   usedKeyString = pair.Key;
                   break;
               }
           }
           keybinds[usedKeyString] = KeyCode.None;
           PlayerPrefs.SetString(usedKeyString, KeyCode.None.ToString());
       }
    }
    
    void UpdateKeybindUI()
    {
        foreach (var keybind in keybinds)
        {
            string action = keybind.Key;
            KeyCode keyCode = keybind.Value;
            TextMeshProUGUI keyText = keybindTexts[action];
            
            keyText.text = keyCode.ToString();

            if (keyCode == KeyCode.None)
            {
                keyText.color = new Color(1f, 0.16f, 0.16f);
            }
            else
            {
                keyText.color = Color.black;
            }
        }
        
        
        upText.text = keybinds["up"].ToString();
        downText.text = keybinds["down"].ToString();
        rightText.text = keybinds["right"].ToString();
        leftText.text = keybinds["left"].ToString();
        attackText.text = keybinds["attack"].ToString();
        interactText.text = keybinds["interact"].ToString();
        switchText.text = keybinds["switch"].ToString();
        dropText.text = keybinds["drop"].ToString();
        dashText.text = keybinds["dash"].ToString();
    }
}
