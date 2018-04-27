using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectChecker : MonoBehaviour {

    public GameObject GameOptionPrefab;
    public GameObject Networkmanager;
    public GameObject CursorManager;
    public GameObject SelectManager;

    void Start()
    {
        if (null == GameObject.Find("Select Manager(Clone)") && null != SelectManager) Instantiate(SelectManager);
        if (null == GameObject.Find("GameOptionPrefab(Clone)") && null != GameOptionPrefab) Instantiate(GameOptionPrefab);
        if (null == GameObject.Find("Cursor Manager(Clone)") && null != CursorManager) Instantiate(CursorManager);
        if (null == GameObject.Find("Network Manager(Clone)") && null != Networkmanager) Instantiate(Networkmanager);
    }
}
