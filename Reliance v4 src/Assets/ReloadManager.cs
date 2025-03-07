using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReloadManager : MonoBehaviour
{
    public float DeathAnimSpeed = 5;
    public RectTransform UiGradient;
    public RectTransform UiBlack;
    public RectTransform UiBlack2Transparent;

    public GameObject GameOverPanel;
    public Text GameOverScoreText;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(PlayStartupAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        //TEST
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ReloadGame();
        }

        if (GameOverPanel.activeSelf)
        {
            if (Input.GetButtonDown("Submit"))
            {
                ReloadGame();
            }
        }
    }

    public void ReloadGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Died()
    {
        StartCoroutine(DiedCoroutine());
    }

    protected IEnumerator DiedCoroutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        yield return StartCoroutine(PlayDeathAnimation());
        GameOverScoreText.text = string.Format("Reached Altitude: {0}", GlobalManager.Instance.Altitude);
        GameOverPanel.SetActive(true);
    }

    public IEnumerator PlayDeathAnimation()
    {
        UiGradient.sizeDelta = new Vector2(0, Screen.height);

        // double the screen height so that we have plenty of black above and we don't
        // accidentally scroll too far down
        UiBlack.sizeDelta = new Vector2(0, Screen.height * 2); 
        UiGradient.anchoredPosition = new Vector2(0, 0);
        var gradPos = UiGradient.anchoredPosition;
        float fullBlackPos = -UiGradient.sizeDelta.y * 2;
        float speedFactor = Screen.height;
        Debug.LogFormat("grad pos {0}, fullbkack pos {1}", gradPos.y, fullBlackPos);
        while (gradPos.y > fullBlackPos)
        {
            // Multiply by screen height so that the fade TIME is the same no matter the 
            // resolution
            gradPos.y -= Time.unscaledDeltaTime * DeathAnimSpeed * Screen.height;
            UiGradient.anchoredPosition = gradPos;
            yield return null;
        }
        yield break;
    }

    public IEnumerator PlayStartupAnimation()
    {
        UiGradient.sizeDelta = new Vector2(0, Screen.height);
        UiBlack.sizeDelta = new Vector2(0, Screen.height);
        UiBlack2Transparent.sizeDelta = new Vector2(0, Screen.height);
        var gradPos = UiGradient.anchoredPosition;
        float fullBlackPos = -UiGradient.sizeDelta.y * 2;
        float fullTransparentPos = -UiGradient.sizeDelta.y * 4;
        gradPos.y = fullBlackPos;
        UiGradient.anchoredPosition = gradPos;
        while (gradPos.y > fullTransparentPos)
        {
            // Multiply by screen height so that the fade TIME is the same no matter the 
            // resolution
            gradPos.y -= Time.unscaledDeltaTime * DeathAnimSpeed * Screen.height;
            UiGradient.anchoredPosition = gradPos;
            yield return null;
        }
        yield break;
    }
}
