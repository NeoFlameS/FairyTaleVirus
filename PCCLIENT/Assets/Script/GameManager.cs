using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Text;

public class GameManager : MonoBehaviour {
    public const int MAXSTAGECOUNT = 1;

    StageInfo[] stageinfoset = new StageInfo[MAXSTAGECOUNT];
    StageInfo stageinfo = new StageInfo();
    byte Round;
    byte Stage;
    byte difficulty;
    byte monsterwave;
    int infect;

    public void Start() {
        try {
            XmlDocument doc = new XmlDocument();
            doc.Load("../Data/StageData.xml");
            XmlElement root = doc.DocumentElement;

            XmlNodeList nodes = root.ChildNodes;

            int count = 0;

            foreach (XmlNode node in nodes) {
                stageinfoset[count].stagenumber = byte.Parse(node["stagenumber"].InnerText);
                stageinfoset[count].stagename = node["stagename"].InnerText;
                stageinfoset[count].round = byte.Parse(node["round"].InnerText);
                stageinfoset[count].monsterperwave = new byte[stageinfoset[count].round];
                for (int i = 0; i < stageinfoset[count].round; ++i) {
                    stageinfoset[count].monsterperwave[i] = byte.Parse(node[i.ToString()].InnerText);
                }
            }

            stageinfo = stageinfoset[Stage-1];
        }
        catch (Exception e) {
            Debug.Log(e);
            //error;
        }
    }

    //몬스터 관리
    public void StartStage() {
        //몬스터 생성
        //이동명령
        //스테이터스 실행중
        //스테이지 +1
    }
    //충돌시
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Monster") return;
            //오염도 증가~
            //체킹
            //게임오버?
    }

    //캐릭터 관리 - 자동공격 범위
    

    //스테이지


}

[XmlRoot("Info")]
struct StageInfo {
    [XmlAttribute("stagename")]
    public string stagename;
    [XmlAttribute("stagenumber")]
    public byte stagenumber;
    [XmlAttribute("round")]
    public byte round;
    [XmlAttribute("stagename")]
    public byte[] monsterperwave;
}
