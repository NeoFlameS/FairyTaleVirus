using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Infected : MonoBehaviour {

    public Slider u_slider_infected;
    public Text u_text_maxinfected;
    public Text u_text_nowinfected;

    public int u_maxinfected;
    public int u_nowinfected;

    public void init()
    {
        u_maxinfected = 20;
        u_nowinfected = 0;
        u_slider_infected.maxValue = 20;
    }

    public void init(byte[] v) {
        u_maxinfected = v[0];
        u_nowinfected = v[1];
        u_slider_infected.maxValue = u_maxinfected;
    }

    public bool damaged(int x) {
        u_nowinfected += x;
        GameObject.Find("ResultData").GetComponent<ResultDataSet>().corrupt = u_nowinfected;//0624 홍승준 추가
        if (u_nowinfected >= u_maxinfected) return true;
        return false;
    }

    public void show() {
        u_text_maxinfected.text = u_maxinfected.ToString();
        u_text_nowinfected.text = u_nowinfected.ToString();
        u_slider_infected.value = u_nowinfected;
    }
}