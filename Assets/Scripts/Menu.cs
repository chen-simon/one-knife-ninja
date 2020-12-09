using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public float fadeTime = 1;
    public Image fadeScreen;
    public GameObject beatGame;
    int level;

    public Button[] buttons = new Button[5];

    void Start()
    {
        StartCoroutine("FadeIn");
        Cursor.visible = true;
        int level = PlayerPrefs.GetInt("level", 1);
        for (int e = 0; e < level; e++)
        {
            if (e == 5)
            {
                beatGame.SetActive(true);
                break;
            }
            buttons[e].interactable = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.SetInt("level", 1);
            SceneManager.LoadScene("menu");
        }
    }

    public void LoadLevel(int level)
    {
        this.level = level;
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        fadeScreen.gameObject.SetActive(true);
        for (float time = 0; time < fadeTime; time += Time.deltaTime)
        {
            Color temp = fadeScreen.color;
            temp.a = Mathf.Sin(time / fadeTime * Mathf.PI / 2);
            fadeScreen.color = temp;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("lvl" + level);
    }

        IEnumerator FadeIn()
    {
        for (float time = 0; time < fadeTime; time += Time.deltaTime)
        {
            Color temp = fadeScreen.color;
            temp.a = Mathf.Cos(time / fadeTime * Mathf.PI / 2);
            fadeScreen.color = temp;
            yield return new WaitForEndOfFrame();
        }
        fadeScreen.gameObject.SetActive(false);
    }
}
