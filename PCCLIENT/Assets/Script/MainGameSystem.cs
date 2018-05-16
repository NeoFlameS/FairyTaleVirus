using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;


public class MainGameSystem : MonoBehaviour {
    public UI_Infected ui_infected;
    public PlayerIcon[] ui_PI;
    public CharacterSet u_CS;
    public PlayerCharacter[] PC = new PlayerCharacter[4];

    public GameObject[] CharacterSet = new GameObject[1];

    public GameObject[] Cursor = new GameObject[4];
    public Vector2[] cursormovevector = new Vector2[4];
    public bool[] iscconnected = { false, false, false, false };

    public const int MAXSTAGECOUNT = 1;
    public const float REGENTIME = 1;
    public const float RESTTIME = 5000;
    public const int MONSTERTYPECOUNT = 3;

    public bool[] moveorder;

    StageInfo[] stageinfoset;
    StageInfo stageinfo = new StageInfo();
    byte Round;
    byte Stage;
    byte difficulty;
    byte monsterwave;
    byte monstercount = 0;
    bool started = false;
    byte usercount = 0;

    float u_timer;
    float time = RESTTIME;

    public GameObject[] monsterpref = new GameObject[MONSTERTYPECOUNT];
    public GameObject[] monstergenpoint = new GameObject[3];

    public List<Monster> M;



    // Use this for initialization
    void Start () {
        //캐릭터 생성~
        //커서 생성~

        //스테이지 불러오기~
        Stage = 1;
        started = false;

        monsterpref[0] = Resources.Load<GameObject>("3D/Monster1R") as GameObject;
        monsterpref[1] = Resources.Load<GameObject>("3D/Monster1Y") as GameObject;
        monsterpref[2] = Resources.Load<GameObject>("3D/Monster1B") as GameObject;

        Character[] l_ch = new Character[4];
        moveorder = new bool[4] { false, false, false, false };

        GameObject tmp;
        tmp = GameObject.Find("Selected Character Status(Clone)");
        if(null != tmp) u_CS = tmp.GetComponent<CharacterSet>();

        l_ch = u_CS.Ch;
        
        for (int i = 0; i < 4; ++i) {
            //ui_PI[i] = GameObject.Find("PLAYERICON" + i).GetComponent<PlayerIcon>();

            ui_PI[i].init();

            if (125 == l_ch[i].ch_type) continue;
            ui_PI[i].connected = true;
            iscconnected[i] = true;
            usercount += 1;
            ui_PI[i].ch_type = l_ch[i].ch_type;
            ui_PI[i].nickname = u_CS.nickname[i];
            ui_PI[i].level = l_ch[i].clearedround;

            ui_PI[i].show();

            Cursor[i].active = true;
            PC[i] = Instantiate(CharacterSet[l_ch[i].ch_type], new Vector3(-32, 7355, -28), Quaternion.identity).GetComponent<PlayerCharacter>();
            PC[i].init(l_ch[i]);
        }

        ui_infected.init();
        ui_infected.show();

        //Stage data load
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.persistentDataPath + "/SavedData.xml");
            XmlElement root = doc.DocumentElement;

            XmlNodeList nodes = root.ChildNodes;

            int count = 0;

            stageinfoset = new StageInfo[1];            

            foreach (XmlNode node in nodes)
            {
                stageinfoset[count].stagenumber = byte.Parse(node["stagenumber"].InnerText);
                stageinfoset[count].stagename = node["stagename"].InnerText;
                stageinfoset[count].round = byte.Parse(node["round"].InnerText);
                stageinfoset[count].monsterperwave = new byte[stageinfoset[count].round];
                for (int i = 0; i < stageinfoset[count].round; ++i)
                {
                    stageinfoset[count].monsterperwave[i] = byte.Parse(node["r" + (i+1).ToString()].InnerText);
                }
                count++;
            }

            stageinfo = stageinfoset[Stage - 1];
            Round = 0;
            monsterwave = stageinfo.monsterperwave[Round];
        }
        catch (Exception e)
        {
            Debug.Log(e);
            //error;
        }

        time = u_timer + RESTTIME;

        PC[0].init();
    }
	
    public void reconnected(int id)
    {
        //id check to ...?
    }

    public void disconnected(int id)
    {
        //disconnected ! socket...?
    }

    public void move(float x, float y, int id) {
        //cursor move
        cursormovevector[id] = new Vector2(x * 20, y*20);
        if (x == 0 && y == 0) moveorder[id] = true;
    }

    public void click(int id, int btnnumber) {
        //switch skill~
        return;
    }

    //CURSORCONTROLL
    public void TestMonster() {
        monsterpref = Resources.Load<GameObject>("3D/Monster") as GameObject;
        GameObject l_temp = new GameObject();

        l_temp = Instantiate(monsterpref,
        monstergenpoint[0].transform.position,
        Quaternion.identity);

        l_temp.GetComponent<Monster>().Move();
    }

    //monster arrived endpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Monster") return;
        bool gameover = ui_infected.damaged(other.gameObject.GetComponent<Monster>().mon_infect);
        ui_infected.show();
        if (true == gameover) SceneManager.LoadScene("Result");
        //gameover scene
    }
    
    //스테이지
    public void Update()
    {

        //MONSTER ORDER
        u_timer += Time.deltaTime;

        if (false == started && time <= u_timer)
        {
            started = true;
            time = u_timer + REGENTIME;
        }

        if (true == started && time <= u_timer && monstercount < monsterwave)
        {
            int j = UnityEngine.Random.Range(0, 3);
            int mon_type = UnityEngine.Random.Range(0, 3);
            GameObject l_temp = new GameObject();
            //TETSLETSETTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTES
            l_temp = Instantiate(monsterpref[mon_type], monstergenpoint[j].transform.position, Quaternion.identity);

            Monster mm = l_temp.GetComponent<Monster>();
            mm.init(Round, Stage, usercount, mon_type);
            mm.Move();
            M.Add(mm);

            monstercount += 1;
            if (monsterwave <= monstercount)
            {
                time = u_timer + RESTTIME;
                started = false;
                Round += 1;
                //monsterwave = stageinfo.monsterperwave[Round];
            }
            time = u_timer + REGENTIME;
        }

        //CHARACTER ORDER & cursormove
        for (int i = 0; i < 4; ++i) {
            if (true == iscconnected[i])
            {
                PC[i].LocalUpdate(M, u_timer);
                Cursor[i].transform.position = new Vector3(
                    Mathf.Clamp(Cursor[i].transform.position.x + (cursormovevector[i].x * Time.deltaTime), PC[i].transform.position.x - 50, PC[i].transform.position.x + 50),
                    7351,
                   Mathf.Clamp(Cursor[i].transform.position.z + (cursormovevector[i].y * Time.deltaTime), PC[i].transform.position.z - 50, PC[i].transform.position.z + 50));
                if (moveorder[i])
                {
                    moveorder[i] = false;
                    PC[i].move(Cursor[i].transform.position);
                }
            }
        }
        
        //TESTTTTTTTTTTTTTTTTTTTTTTTTTTTT
        if (Input.GetMouseButton(0)) // TESET
        {
            started = true;
            time = u_timer + REGENTIME;
        }
        if (Input.GetMouseButton(1)) // TESET
        {
            move(1, 1, 0);
        }
        if (Input.GetMouseButton(2)) // TESET
        {
            move(0, 0, 0);
        }

    }
}

[XmlRoot("Info")]
struct StageInfo
{
    [XmlAttribute("stagename")]
    public string stagename;
    [XmlAttribute("stagenumber")]
    public byte stagenumber;
    public byte[] monsterperwave;
    [XmlAttribute("round")]
    public byte round;
    [XmlAttribute("r1")]
    public byte r1;
    [XmlAttribute("r2")]
    public byte r2;
    [XmlAttribute("r3")]
    public byte r3;
    [XmlAttribute("r4")]
    public byte r4;
    [XmlAttribute("r5")]
    public byte r5;
    [XmlAttribute("r6")]
    public byte r6;
    [XmlAttribute("r7")]
    public byte r7;
}
