using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDataSet : MonoBehaviour {
    public int mana;
    public int corrupt;
    public int monster;

    public bool iscleard;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);

        mana = 0;
        corrupt = 0;
        monster = 0;
        iscleard = false;
    }
}
