using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vocab : MonoBehaviour
{
    public string BTN_StartName, Quit_StartName;
    public Text StartBTNText, QuitBTNText;
    void Awake()
    {
        (StartBTNText.text, QuitBTNText.text) = (BTN_StartName, Quit_StartName);
    }
}
