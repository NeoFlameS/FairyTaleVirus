using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CursorControl : MonoBehaviour {

    public GameObject[] cursor;
    public Vector2[] cursormovevector = new Vector2[4];
    public bool[] iscursorconnected = { false, false, false, false };
    public GameObject SelectedCharacterStatus;

    public SelectManager SM;

    float scx, scy;

    CharacterCard[] selectedbutton = new CharacterCard[4];
    byte[] buttonid = new byte[4];

    public string[] nickname = new string[4];

    void OnLevelWasLoaded() {
        cursor[0] = GameObject.Find("Cursor");
        cursor[1] = GameObject.Find("Cursor2");
        cursor[2] = GameObject.Find("Cursor3");
        cursor[3] = GameObject.Find("Cursor4");
    }

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(gameObject);
        scx = Option.screensize.x;
        scx = Option.screensize.y;
        cursor[0] = GameObject.Find("Cursor");
        cursor[1] = GameObject.Find("Cursor2");
        cursor[2] = GameObject.Find("Cursor3");
        cursor[3] = GameObject.Find("Cursor4");
    }

    public void move(Vector2 force, int playernumber){
        Vector2 v = new Vector2(30 * (scx / 1920) * force.x, 30 * (scy / 1080) * force.y);
        cursormovevector[playernumber] = v;
    }

   
    public void click(int playernumber, int buttonnumber)
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(cursor[playernumber].transform.position));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 5f);
        

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
            tmp = GameObject.Find("Select Manager");
            if (null != tmp) SM = tmp.GetComponent<SelectManager>();
            switch (buttonnumber)
            {
                case 0:
                    if (null == selectedbutton[playernumber]) break;
                    SM.MakeCharacter(0, selectedbutton[playernumber].CCid);
                    break;
                case 1:
                    if (null != selectedbutton[playernumber]) {
                        selectedbutton[playernumber].GetComponent<CharacterCard>().clickcancled();
                        selectedbutton[playernumber] = null;
                    }
                    bool check = hit.collider.GetComponent<CharacterCard>().clicked(playernumber);
                    if (true == check) selectedbutton[playernumber] = hit.collider.GetComponent<CharacterCard>();
                    break;
                case 2:
                    if (null == selectedbutton[playernumber]) break;
                    SM.DeleteCharacter(selectedbutton[playernumber].CCid);
                    break;
                case 3:
                    if (null == selectedbutton[playernumber]) break;
                    selectedbutton[playernumber].GetComponent<CharacterCard>().clickcancled();
                    selectedbutton[playernumber] = null;
                    break;
                default:
                    break;
            }
        }
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

	// Update is called once per frame
	public void Update () {
		for (int i = 0; i < 4; ++i) {
			if (iscursorconnected [i]) {
				cursor [i].transform.position += new Vector3 (cursormovevector [i][0], cursormovevector [i][1]) * Time.fixedDeltaTime;
				Mathf.Clamp (cursor [i].transform.position.x, 0, scx);
				Mathf.Clamp (cursor [i].transform.position.y, 0, scy);
			}
		}
	}
}
