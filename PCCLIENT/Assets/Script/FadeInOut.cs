using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour {

    public UnityEngine.UI.Image fade;
    float fades = 1.0f;
    float time = 0;
    float timecut = 0.025f;
    float fadevalue = 0.025f;

    bool started = true;
    public bool scenechange = false;
    public string scenename;
    // Use this for initialization

    public void changeScene(string name) {
        scenename = name;
        scenechange = true;
    }

    public void winGame()
    {
        scenename = "Scene_Result";
        fadevalue = 0.01f;
        scenechange = true;
    }

    private void Start()
    {
        fade = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update () {
        if (true == started)
        {
            time += Time.deltaTime;
            if (fades > 0.0f && time > timecut)
            {
                fades -= fadevalue;
                fade.color = new Color(0, 0, 0, fades);
                time = 0;
            }
            else if (fades <= 0.0f) {
                started = false;
            }
        }
        else if (true == scenechange)
        {
            time += Time.deltaTime;
            if (fades < 1.0f && time > timecut) {
                fades += fadevalue;
                if (fades >= 1.0f) fades = 1.0f;
                fade.color = new Color(0, 0, 0, fades);
                time = 0;
            }
            else if (fades >= 1.0f)
            {
                //scene move
                SceneManager.LoadScene(scenename);
                time = 0;
            }
        }
	}
}
