using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControl : MonoBehaviour {

    public GameObject[] cursor;
    public Vector2[] cursormovevector = new Vector2[4];
    public bool[] iscursorconnected = { false, false, false, false };
    public GameObject SelectedCharacterStatus;
    public bool[] clicked;
    public int[] btnnumber;

    float[] C_clampV;

    public SelectManager SM;

    float scx, scy;

    public CharacterCard[] selectedbutton = new CharacterCard[4];
    byte[] buttonid = new byte[4];

    public string[] nickname = new string[4];

    void OnLevelWasLoaded() {
        if (5 < SceneManager.GetActiveScene().buildIndex) {
            
            return;
        }
        cursor[0] = GameObject.Find("Cursor");
        cursor[1] = GameObject.Find("Cursor2");
        cursor[2] = GameObject.Find("Cursor3");
        cursor[3] = GameObject.Find("Cursor4");

        for (int i = 0; i < 4; ++i)
        {
            if(false == iscursorconnected[i])
                cursor[i].SetActive(false);
        }
    }

    // Use this for initialization
    void Start() {
        C_clampV = new float[4];
        clicked = new bool[4]{ false, false, false, false };
        btnnumber = new int[4];
        DontDestroyOnLoad(gameObject);
        scx = GameObject.Find("GameOptionPrefab").GetComponent<Option>().option.ScreenSizeX;
        scy = GameObject.Find("GameOptionPrefab").GetComponent<Option>().option.ScreenSizeX;
        cursor[0] = GameObject.Find("Cursor");
        cursor[1] = GameObject.Find("Cursor2");
        cursor[2] = GameObject.Find("Cursor3");
        cursor[3] = GameObject.Find("Cursor4");

        for (int i = 0; i < 4; ++i) {
            cursor[i].SetActive(false);
        }

        C_clampV[0] = (-1 * scx) / 2;
        C_clampV[1] = scx / 2;
        C_clampV[2] = (-1 * scy) / 2;
        C_clampV[3] = scy / 2;
    }
    public void Init()//오후 4시 추가
    {
        C_clampV = new float[4];
        clicked = new bool[4] { false, false, false, false };
        btnnumber = new int[4];
        DontDestroyOnLoad(gameObject);
        scx = GameObject.Find("GameOptionPrefab").GetComponent<Option>().option.ScreenSizeX;
        scy = GameObject.Find("GameOptionPrefab").GetComponent<Option>().option.ScreenSizeX;
        cursor[0] = GameObject.Find("Cursor");
        cursor[1] = GameObject.Find("Cursor2");
        cursor[2] = GameObject.Find("Cursor3");
        cursor[3] = GameObject.Find("Cursor4");

        for (int i = 0; i < 4; ++i)
        {
            cursor[i].SetActive(false);
        }

        C_clampV[0] = (-1 * scx) / 2;
        C_clampV[1] = scx / 2;
        C_clampV[2] = (-1 * scy) / 2;
        C_clampV[3] = scy / 2;

    }

    public void move(float x, float y, int playernumber){
        Vector2 v = new Vector2(80 * (scx / 2560) * x, 60 * (scy / 1440) * y);
        cursormovevector[playernumber] = v;
    }

   
    public void click(int playernumber, int buttonnumber)
    {
        clicked[playernumber] = true;
        btnnumber[playernumber] = buttonnumber;
    }

    public void disconnected(int id) {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }

    public bool characterallselected() {
        byte[] btid = new byte[4];

        for (int i = 0; i < 4; ++i) {
            if (true == iscursorconnected[i]) {
                if (null == selectedbutton[i]){
                    btid[i] = 125;
                    return false;
                }
                else if (selectedbutton[i].ch_type == 125){
                    btid[i] = 125;
                    return false;
                }
                btid[i] = selectedbutton[i].CCid;
            }
            else btid[i] = 125;
        }

        buttonid = btid;

        return true;
    }

    public byte[] selectedID()
    {
        return buttonid;
    }

    public void connected(int playernumber){
		iscursorconnected [playernumber] = true;
    }

    public void clicking(int i) {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(cursor[i].transform.position));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 5f);
        int roop = 0;
        int count = 0;

        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.tag == "BUTTON")
        {
            hit.collider.GetComponent<ButtonAction>().clicked();
        }

        //캐릭터 생성 삭제시 UI 어떻게할것인가===============================
        else if (hit.collider.tag == "CHARACTERCARD")
        {
            
            GameObject tmp;
            tmp = GameObject.Find("Select Manager(Clone)");
            if (null != tmp) SM = tmp.GetComponent<SelectManager>();
            switch (btnnumber[i])
            {
                case 0://5.31 홍승준 수정
                    if (SM == null || selectedbutton[i] == null)
                    {
                        break;
                    }
                    //5.15 홍승준 추가 -- 생성 씬 5.18 홍승준 수정
                    GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().SignalSend(i, NetworkController.SC_SELECT);
                    GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().Get_SelectManger();
                    //끝

                    SM.Before_Make(selectedbutton[i].CCid, (byte)i);
                    /*if (null == selectedbutton[i]) break;
                    SM.MakeCharacter(0, selectedbutton[i].CCid);*/
                    break;
                case 1:
                    if (null != selectedbutton[i])
                    {
                        selectedbutton[i].GetComponent<CharacterCard>().clickcancled();
                        selectedbutton[i] = null;
                    }
                    bool check = hit.collider.GetComponent<CharacterCard>().clicked(i);
                    if (true == check) selectedbutton[i] = hit.collider.GetComponent<CharacterCard>();
                    break;
                case 2:
                    if (null == selectedbutton[i]) break;
                    SM.DeleteCharacter(selectedbutton[i]);
                    break;
                case 3:
                    if (null == selectedbutton[i]) break;
                    selectedbutton[i].GetComponent<CharacterCard>().clickcancled();
                    selectedbutton[i] = null;
                    break;
                default:
                    break;
            }
        }
    }

	// Update is called once per frame
	public void Update () {
        if (5 < SceneManager.GetActiveScene().buildIndex)
        {

            return;
        }
        for (int i = 0; i < 4; ++i) {
			if (iscursorconnected [i]) {
                if (false == cursor[i].activeSelf) cursor[i].SetActive(true);
				cursor [i].transform.position += new Vector3 (cursormovevector [i].x, cursormovevector [i].y,0) * Time.fixedDeltaTime;
				Mathf.Clamp (cursor [i].transform.position.x, C_clampV[0], C_clampV[1]);
				Mathf.Clamp (cursor [i].transform.position.y, C_clampV[2], C_clampV[3]);

                if (clicked[i])
                {
                    clicked[i] = false;
                    clicking(i);
                }
			}
		}
    }
}
