using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideLine : MonoBehaviour {

    public Text up,down,left,right,title;
    public GameObject gup, gdown, gleft, gright, gtitle;
    Cursor cs;
    // Use this for initialization
    void Start () {
        cs = GameObject.Find("Cursor").GetComponent<Cursor>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void GuideLine_Change(int type) {
        switch (type)
        {
            case 0:
                title.text = "확대 축소";

                gup.SetActive(true);
                gdown.SetActive(true);
                gleft.SetActive(false);
                gright.SetActive(false);

                up.text = "확대";
                down.text = "축소";
                break;
            case 1:
                title.text = "화면 회전";

                gup.SetActive(false);
                gdown.SetActive(false);
                gleft.SetActive(true);
                gright.SetActive(true);

                left.text = "왼쪽 회전";
                right.text = "오른쪽 회전";
                break;
            case 2:
                title.text = "화면 이동";

                gup.SetActive(false);
                gdown.SetActive(false);
                gleft.SetActive(false);
                gright.SetActive(false);

                break;
            case 3:
                title.text = "화면 시점";

                gup.SetActive(true);
                gdown.SetActive(true);
                gleft.SetActive(true);
                gright.SetActive(true);

                up.text = "자유 시점";
                down.text = "쿼터뷰 시점";
                left.text = "캐릭터 시점";
                right.text = "몬스터 시점";

                break;
            default:
                break;
        }
    }
}
