using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Cursor : MonoBehaviour
{
    public GameObject view_manual;
   

    public bool in_game = false;
    //이미지 설정
    public Image pad_img;

    //네트워크 통신용 
    DateTime frame;
    NetWorkManager nm;

    //화면 처리 변수
    Camera c;

    //UI 처리 변수
    //public Button stat;//stat 탭 버튼
    GameObject cur;//본 오브젝트
    Vector3 startpos;//커서 시작점
    Vector3 startpos_screen;//시작점 스크린 좌표
    Vector3 now_pos;//커서 현재점
    
    Vector3 v;
    Touch now;//현재 터치 
    int pad_index = 10;//디폴트 터치 카운트
    int pad_up = 0;
    //터치 관련 변수
    int touch_delay = 33;
    bool up_delay = false;
    bool check = false;

    int mode = 0;//컨트롤 모드 : 0 / 카메라 제어 모드 : 1 
    public byte camera_type = 0;//0: 줌 1:회전 2:이동 3: 타겟

    float pad_boundx;
    float pad_boundy;
                       // Use this for initialization
    void Start()
    {
        //카메라 및 패드 객체 변수 초기화
        c = Camera.main;
        cur = GameObject.Find("Cursor");

        //이미지 변수 초기화
        pad_boundx = (Screen.width / 2) - 100;
        pad_boundy = 3 * Screen.height / 4;

        //좌표 변수들
        startpos = cur.transform.position;
        startpos_screen=c.WorldToScreenPoint(startpos);
        now_pos = cur.transform.position;
        v = new Vector3();//패드 계산용

        //네트워크 통신용 변수 초기화
        
        nm = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();//네트워크 매니저 오브젝트에 접근 클래스 불러오기
        nm.Get_CS();
        
        
        frame = DateTime.Now;

        string img_path = "Img/ControllUI/pad_"+nm.My_Info.color;
        //Debug.Log("Pad Color path : "+img_path);
        pad_img.sprite = Resources.Load<Sprite>(img_path) as Sprite;
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        in_game = nm.in_game;
        int i;
        int touch_count = Input.touchCount;
        if (nm.skill_select & check==false) {
            nm.skill_select = false;
            SceneManager.LoadScene("SelectSkill");
            PadUp();
        }
        if (touch_count > 0)
        {

            for (i = 0; i < touch_count; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.position.x < pad_boundx && touch.position.y < pad_boundy)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:

                            PadTouch(touch.position.x, touch.position.y, startpos.z);
                            up_delay = true;
                            break;
                        default:
                            PadUp();
                            break;
                    }
                }
            }
        }
        else if(up_delay){
            PadUp();
            up_delay = false;
        }
    }

    void PadTouch(float x, float y, float z)
    {//패드를 터치했을때 
        float x_a, y_a;


        if (mode == 1)
        {
            v.Set(x, y, z);
            pad_up = 0;
            cur.transform.position = c.ScreenToWorldPoint(v);

            TimeSpan t = DateTime.Now - frame;
            
            if (((t.Minutes*60+t.Seconds)*1000)+t.Milliseconds >= touch_delay )
            {
                frame = DateTime.Now;
                CS_CAMERA_PACKET cs = new CS_CAMERA_PACKET();

                x_a = (x - startpos_screen.x) / (Screen.width / 4);
                y_a = (y - startpos_screen.y) / (Screen.height / 3);

                cs.type = camera_type;
                cs.x = x_a;
                cs.y = y_a;

                nm.GameDataSend(cs, NetworkController.CS_CAMERA_CHANGE);
                if (touch_delay != 2000 && camera_type==3) { touch_delay = 2000; }
            }
        }
        else
        {
            v.Set(x, y, z);
            pad_up = 0;
            cur.transform.position = c.ScreenToWorldPoint(v);


            

            //패드 터치 정보 송신
            if (DateTime.Now.Millisecond - frame.Millisecond >= touch_delay || DateTime.Now.Millisecond - frame.Millisecond < -touch_delay)
            {

                frame = DateTime.Now;
                CS_MOVE_PACKET mov;
                mov.id = nm.My_Info.id;


                mov.x = (x - startpos_screen.x) / (Screen.width / 4);
                mov.y = (y - startpos_screen.y) / (Screen.height / 3);


                nm.GameDataSend(mov, NetworkController.CS_MOVE);

                if (touch_delay != 33) { touch_delay = 33; }
            }
        }
    }

    void PadUp()
    {
        if (mode == 1)
        {
            cur.transform.position = startpos;
        }
        else {
            cur.transform.position = startpos;

            frame = DateTime.Now;
            CS_MOVE_PACKET mov;
            mov.id = nm.My_Info.id;


            mov.x = 0;
            mov.y = 0;
            pad_up++;
            touch_delay = 100;
            nm.GameDataSend(mov, NetworkController.CS_MOVE);
            
        }
        

    }

    public void Btn0Touch()
    {//1번 버튼을 터치했을때 
        if (DateTime.Now.Millisecond - frame.Millisecond >= 33 || DateTime.Now.Millisecond - frame.Millisecond < -33) {
            if (mode == 0)
            {
                frame = DateTime.Now;
                CS_BUTTON_PACKET bt;
                bt.id = nm.My_Info.id;
                bt.btn_number = (char)0;

                nm.GameDataSend(bt, NetworkController.CS_BTN);
            }
            else {
                camera_type = 0;
                view_manual.GetComponent<GuideLine>().GuideLine_Change(camera_type);
            }     
        }

        return;
    }
    public void Btn1Touch()
    {//2번 버튼을 터치했을때 
       
        if (DateTime.Now.Millisecond - frame.Millisecond >= 33 || DateTime.Now.Millisecond - frame.Millisecond < -33)
        {
            if (mode == 0)
            {
                frame = DateTime.Now;
                CS_BUTTON_PACKET bt;
                bt.id = nm.My_Info.id;
                bt.btn_number = (char)1;

                nm.GameDataSend(bt, NetworkController.CS_BTN);
            }
            else {
                camera_type = 1;
                view_manual.GetComponent<GuideLine>().GuideLine_Change(camera_type);
            }
        }
        return;
    }
    public void Btn2Touch()
    {//3번 버튼을 터치했을때 
        
        if (DateTime.Now.Millisecond - frame.Millisecond >= 33 || DateTime.Now.Millisecond - frame.Millisecond < -33)
        {
            if (mode == 0)
            {
                frame = DateTime.Now;
                CS_BUTTON_PACKET bt;
                bt.id = nm.My_Info.id;
                bt.btn_number = (char)2;

                nm.GameDataSend(bt, NetworkController.CS_BTN);
            }
            else {
                camera_type = 2;
                view_manual.GetComponent<GuideLine>().GuideLine_Change(camera_type);
            }
        }
        return;
    }
    public void Btn3Touch()
    {//4번 버튼을 터치했을때 
        
        if (DateTime.Now.Millisecond - frame.Millisecond >= 33 || DateTime.Now.Millisecond - frame.Millisecond < -33)
        {
            if (mode == 0)
            {
                frame = DateTime.Now;
                CS_BUTTON_PACKET bt;
                bt.id = nm.My_Info.id;
                bt.btn_number = (char)3;

                nm.GameDataSend(bt, NetworkController.CS_BTN);
            }
            else {
                camera_type = 3;
                view_manual.GetComponent<GuideLine>().GuideLine_Change(camera_type);
            }
            
        }
        return;
    }

    public void BtnStat() {
        PadUp();
        if (!in_game) { return; }
        else { SceneManager.LoadScene("Status"); }
        
    }

    public void BtnUpgrade() {
        PadUp();
        if (!in_game) { return; }
        else { SceneManager.LoadScene("Upgrade"); }
        
    }

    public void BtnCamera() {
        if (!in_game) {
            mode = 0;
            touch_delay = 33;
            view_manual.SetActive(false);
            return;
        }
        else if (mode == 1)
        {
            mode = 0;
            touch_delay = 33;
            view_manual.SetActive(false);
        }
        else {
            mode = 1;
            view_manual.SetActive(true);
            view_manual.GetComponent<GuideLine>().GuideLine_Change(camera_type);
        }
    }
}
