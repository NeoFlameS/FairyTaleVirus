using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;


public class MainGameSystem : MonoBehaviour
{
    public const int         MAXSTAGECOUNT = 1;
    public const float       REGENTIME = 1;
    public const float       RESTTIME = 35;
    public const int         MONSTERTYPECOUNT = 3;
    public const int         MAXCHARACTER = 2;
    
    public const int         DIF_EASY = 0;
    public const int         DIF_NORMAL = 1;
    public const int         DIF_HARD = 2;

    public UI_Infected       ui_infected;
    public PlayerIcon[]      ui_PI;
    public CharacterSet      u_CS;
    public PlayerCharacter[] PC                = new PlayerCharacter[4];
    public SkillSystem       SS;
    public Camera            C;

    public GameObject[]     CharacterSet       = new GameObject[MAXCHARACTER];

    public GameObject[]     Cursor             = new GameObject[4];
    public Vector2[]        cursormovevector   = new Vector2[4];
    public Vector2[]        cursormovemultiply = new Vector2[4];
    public bool[]           iscconnected       = { false, false, false, false };

    public bool[]           moveorder;
    public ResultDataSet    rds;


    StageInfo[]             stageinfoset;
    StageInfo stageinfo = new StageInfo();
    byte Round;
    byte Stage;
    byte difficulty;
    byte monsterwave;
    byte monstercount = 0;
    public bool started = false;
    public byte usercount = 0;

    public float g_yrotation_m;
    float g_yrotation;

    public float u_timer;
    public float time = RESTTIME;

    public GameObject[] monsterpref = new GameObject[MONSTERTYPECOUNT];
    public GameObject[] monstergenpoint = new GameObject[3];

    public List<Monster> M;

    // Use this for initialization
    void Start () {
        cursormovevector = new Vector2[4];
        cursormovemultiply = new Vector2[4];
        //캐릭터 생성~
        //커서 생성~
        SS = GetComponent<SkillSystem>();//5.31 홍승준 추가
        GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>().not_show = true;//5.31 홍승준 추가
        //스테이지 불러오기~
        Stage = 1;
        started = false;

        Character[] l_ch = new Character[4];
        moveorder = new bool[4] { false, false, false, false };

        GameObject tmp;
        tmp = GameObject.Find("Selected Character Status(Clone)");
        if(null != tmp) u_CS = tmp.GetComponent<CharacterSet>();

        l_ch = u_CS.Ch;

        int[][] skills = new int[4][];

        for (int i = 0; i < 4; ++i) {
            //ui_PI[i] = GameObject.Find("PLAYERICON" + i).GetComponent<PlayerIcon>();
            cursormovemultiply[i] = new Vector2(0, 0);
            cursormovevector[i] = new Vector2(0, 0);

            ui_PI[i].init();

            if (125 == l_ch[i].ch_type) {
                ui_PI[i].show();
                continue;
            }
            ui_PI[i].connected = true;
            iscconnected[i] = true;
            usercount += 1;
            ui_PI[i].ch_type = l_ch[i].ch_type;
            ui_PI[i].nickname = u_CS.nickname[i];
            ui_PI[i].level = l_ch[i].clearedround;

            ui_PI[i].show();

            Cursor[i].SetActive(true);
            
            PC[i] = Instantiate(CharacterSet[l_ch[i].ch_type], Cursor[i].transform.position, Quaternion.identity).GetComponent<PlayerCharacter>();
            PC[i].init(l_ch[i]);
            skills[i] = new int[4];
            skills[i] = l_ch[i].skill;
        }

        ui_infected.init();
        ui_infected.show();
        SS.init(skills, PC);

        //Stage data load
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.persistentDataPath + "/SavedData.xml");
            XmlElement root = doc.DocumentElement;

            XmlNodeList nodes = root.ChildNodes;

            int count = 0;

            stageinfoset = new StageInfo[3];            

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
        //if (true == moveorder[id]) return;
        if (x == 0 && y == 0)
        {
            cursormovemultiply[id] = new Vector2(0, 0);
            moveorder[id] = true;
        }
        else {
            
            cursormovevector[id] = new Vector2(x, y);
            cursormovevector[id] *= 20;
            cursormovemultiply[id].x = cursormovevector[id].x * Mathf.Cos(g_yrotation) - cursormovevector[id].y * Mathf.Sin(g_yrotation);
            cursormovemultiply[id].y = cursormovevector[id].x * Mathf.Sin(g_yrotation) + cursormovevector[id].y * Mathf.Cos(g_yrotation);
        }
    }

    public void click(int id, byte btnnumber) {
        PC[id].m_UseSkill(btnnumber);
        //switch skill~
        return;
    }


    //monster arrived endpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Monster") return;
        bool gameover = ui_infected.damaged(other.gameObject.GetComponent<Monster>().mon_infect);
        ui_infected.show();
        if (true == gameover) {
            rds.iscleard = false;
            SceneManager.LoadScene("Scene_Result");
        }
        //gameover scene
    }
    
    //스테이지
    public void Update()
    {
        g_yrotation = -1 * C.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;


        //MONSTER ORDER
        u_timer += Time.deltaTime;

        if (false == started && time <= u_timer)
        {
            started = true;
            time = u_timer + REGENTIME;
        }
        else if (true == started && time <= u_timer && monstercount < monsterwave)
        {
            int j = UnityEngine.Random.Range(0, monstergenpoint.Length);
            int mon_type = UnityEngine.Random.Range(0, MONSTERTYPECOUNT);
            GameObject l_temp = new GameObject();
            //TETSLETSETTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTESTES
            l_temp = Instantiate(monsterpref[mon_type], monstergenpoint[j].transform.position, Quaternion.identity);

            Monster mm = l_temp.GetComponent<Monster>();
            mm.init(Round, Stage, usercount, mon_type, difficulty);
            mm.Move();
            M.Add(mm);

            monstercount += 1;
            if (monsterwave <= monstercount)
            {
                time = u_timer + RESTTIME;
                started = false;
                if (Round < stageinfo.round-1)
                {
                    Round += 1;
                    monsterwave = stageinfo.monsterperwave[Round];
                }
                monstercount = 0;
            }
            time = u_timer + REGENTIME;
        }
        if (true == started && Round >= stageinfo.round && M.Count == 0)
        {
            rds.iscleard = true;
            GameObject.Find("FadeInOut").GetComponent<FadeInOut>().winGame();
        }

        //CHARACTER ORDER & cursormove
        for (int i = 0; i < 4; ++i) {
            if (true == iscconnected[i])
            {
                PC[i].LocalUpdate(M, u_timer);
                Cursor[i].transform.position = new Vector3(
                    Mathf.Clamp(Cursor[i].transform.position.x + (cursormovemultiply[i].x * Time.deltaTime), PC[i].transform.position.x - 50, PC[i].transform.position.x + 50),
                    Cursor[i].transform.position.y,
                   Mathf.Clamp(Cursor[i].transform.position.z + (cursormovemultiply[i].y * Time.deltaTime), PC[i].transform.position.z - 50, PC[i].transform.position.z + 50));
                if (moveorder[i])
                {
                    PC[i].move(Cursor[i].transform.position);
                    moveorder[i] = false;
                }
            }
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
