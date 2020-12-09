using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public int currentLevel;
    public float fadeTime;

    bool gameOver;

    public bool paused;
    bool fading;

    public Image fadeScreen;
    public GameObject pauseText;

    private void Awake()
    {
        //Singleton
        if (!manager) { manager = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        StartCoroutine("FadeIn");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine("FadeOut");
        }
    }

    public void GameOver()
    {
        gameOver = true;
        StartCoroutine("FadeOut");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !fading)
        {
            Pause();
        }
        else if (paused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("menu");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("lvl" + currentLevel, LoadSceneMode.Single);
            }
        }
    }

    void Pause()
    {
        if (paused)
        {
            paused = false;
            pauseText.SetActive(false);
            Cursor.visible = false;
            fadeScreen.color = new Color(0, 0, 0, 0);
            Time.timeScale = 1;
        }
        else
        {
            paused = true;
            Cursor.visible = true;
            pauseText.SetActive(true);
            fadeScreen.color = new Color(0, 0, 0, 0.6f);
            Time.timeScale = 0;
        }
    }

    IEnumerator FadeOut()
    {
        fading = true;
        for (float time = 0; time < fadeTime; time += Time.deltaTime)
        {
            Color temp = fadeScreen.color;
            temp.a = Mathf.Sin(time / fadeTime * Mathf.PI/2);
            fadeScreen.color = temp;
            yield return new WaitForEndOfFrame();
        }
        fadeScreen.color = Color.black;
        //Sees if fade out was via Game Over
        if (gameOver)
        {
            SceneManager.LoadScene("lvl" + currentLevel);
        }
        else
        {
            if (PlayerPrefs.GetInt("level", 1) < currentLevel + 1)
            {
                PlayerPrefs.SetInt("level", currentLevel + 1);
            }
            SceneManager.LoadScene("menu");
        }
    }

    IEnumerator FadeIn()
    {
        fading = true;
        for (float time = 0; time < fadeTime; time += Time.deltaTime)
        {
            Color temp = fadeScreen.color;
            temp.a = Mathf.Cos(time / fadeTime * Mathf.PI / 2);
            fadeScreen.color = temp;
            yield return new WaitForEndOfFrame();
        }
        fadeScreen.color = new Color(0, 0, 0, 0);
        fading = false;
    }
}
