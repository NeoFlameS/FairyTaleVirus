using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageResult : MonoBehaviour {
    public Text mana, monster, corrupt;
    public Text text_result;
    public AudioClip sound_cleared;
    public AudioClip sound_fail;

    AudioSource AS;

    int mn, mon, cor;
    ResultDataSet rds;

	// Use this for initialization
	void Start () {
        rds = GameObject.Find("ResultData").GetComponent<ResultDataSet>();
        AS = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        mn = rds.mana;
        mon = rds.monster;
        cor = rds.corrupt;

        mana.text = ""+mn;
        monster.text = "" + mon;
        corrupt.text = "" + cor;

        if (true == rds.iscleard)
        {
            text_result.text = "정화 성공";
            AS.PlayOneShot(sound_cleared);
        }
        else
        {
            text_result.text = "정화 실패";
            AS.PlayOneShot(sound_fail);
        }



        if (cor < 20)
        {
            GameObject.Find("Selected Character Status").GetComponent<CharacterSet>().All_data_save();
        }

        Destroy(GameObject.Find("ResultData"));
    }

    public int cor_ret() {
        return cor;
    }
}
