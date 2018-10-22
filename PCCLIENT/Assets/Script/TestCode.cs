using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TestCode : MonoBehaviour {

    public GameObject cursor;
    public SelectManager SM;
    public CursorControl CC;
    public byte btnnumber;
    public Camera cam;

    public CharacterCard selectedbutton;

    // Use this for initialization
    void Start () {
        cursor = GameObject.Find("Cursor");
	}

    void OnLevelWasLoaded()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        cursor = GameObject.Find("Cursor");
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            clicking(0);
        }

        if (Input.GetMouseButtonDown(1)) {
            clicking(1);
        }

        if (Input.GetMouseButtonDown(2))
        {
            clicking(2);
        }

        if (Input.GetKey(KeyCode.S))
        {
            clicking(3);
        }
        if (Input.GetKey(KeyCode.X))
        {
            clicking(4);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            clicking(5);
        }
        if (Input.GetKey(KeyCode.C))
        {
            clicking(6);
        }
        if (Input.GetKey(KeyCode.D))
        {
            clicking(7);
        }
        if (Input.GetKey(KeyCode.A))
        {
            clicking(8);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            btnnumber = 0;
            clicking(0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            btnnumber = 1;
            clicking(0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            btnnumber = 2;
            clicking(0);
        }

        if (Input.GetKey(KeyCode.R))
        {
            btnnumber = 3;
            clicking(0);
        }

        if (Input.GetKey(KeyCode.UpArrow)) cursor.transform.localPosition += new Vector3(0, 10, 0);
        if (Input.GetKey(KeyCode.DownArrow)) cursor.transform.localPosition += new Vector3(0, -10, 0);
        if (Input.GetKey(KeyCode.LeftArrow)) cursor.transform.localPosition += new Vector3(-10, 0, 0);
        if (Input.GetKey(KeyCode.RightArrow)) cursor.transform.localPosition += new Vector3(10, 0, 0);
        
    }

    public void clicking(int i)
    {
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 0) {
            MainGameSystem MGS = GameObject.Find("GAME SYSTEM").GetComponent<MainGameSystem>();
            MGS.started = true;
            MGS.time = MGS.u_timer + MainGameSystem.REGENTIME;
            return;
        }

        if (7 == SceneManager.GetActiveScene().buildIndex && i == 2)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.targetChange(3);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 1)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.targetChange(3);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 3)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Move(0, 1);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 4)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Move(0, -1);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 5)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Move(-1, 0);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 6)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Move(1, 0);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 7)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Rotate(true);
            return;
        }
        if (7 == SceneManager.GetActiveScene().buildIndex && i == 8)
        {
            CameraSystem CS = GameObject.Find("Camera Arm").GetComponent<CameraSystem>();
            CS.Rotate(false);
            return;
        }


        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(cursor.transform.position));
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
            if (null == CC) CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
            switch (btnnumber)
            {
                case 0://5.31 홍승준 수정
                    if (SM == null)
                    {
                        break;
                    }

                    for (roop = 0; roop < 9; roop++)
                    {
                        if (SM.CC[roop].select_id == i)
                        {
                            count++;
                        }
                    }

                    if (count != 1)
                    {
                        return;
                    }
                    //5.15 홍승준 추가 -- 생성 씬 5.18 홍승준 수정
                    GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().SignalSend(i, NetworkController.SC_SELECT);
                    GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().Get_SelectManger();
                    //끝

                    SM.Before_Make(CC.selectedbutton[0].CCid, (byte)i);
                    /*if (null == selectedbutton[i]) break;
                    SM.MakeCharacter(0, selectedbutton[i].CCid);*/
                    break;
                case 1:
                    if (null != CC.selectedbutton[0])
                    {
                        CC.selectedbutton[0].GetComponent<CharacterCard>().clickcancled();
                        CC.selectedbutton[0] = null;
                    }
                    bool check = hit.collider.GetComponent<CharacterCard>().clicked(i);
                    if (true == check) CC.selectedbutton[0] = hit.collider.GetComponent<CharacterCard>();
                    break;
                case 2:
                    if (null == CC.selectedbutton[0]) break;
                    SM.DeleteCharacter(CC.selectedbutton[0]);
                    break;
                case 3:
                    if (null == CC.selectedbutton[0]) break;
                    CC.selectedbutton[0].GetComponent<CharacterCard>().clickcancled();
                    CC.selectedbutton[0] = null;
                    break;
                default:
                    break;
            }
        }
    }
}
