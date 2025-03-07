using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DontDestroyStuff : MonoBehaviour
{
    public AudioSource audioSource;
    public Slider VolumeSlider;

    static DontDestroyStuff Instance = null;

    private void Awake()
    {
        // See if there's another object with this component already
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            // I'm not sure if this is needed, because we are destroying the object
            // in the Awake function but let's keep it for safety
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        VolumeSlider.SetValueWithoutNotify(audioSource.volume);

        GlobalManager.Instance.ShowPauseMenu(true);
    }

    public void SetVolume()
    {
        audioSource.volume = VolumeSlider.value;
    }
}
