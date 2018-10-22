using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour {
    public string SceneorPopup;
	public int ButtonType;

    public const int B_MOVESCENE = 0;
    public const int B_QUIT = 1;
    public const int B_CHARSELECTNEXT = 2;
    public const int B_CHARBACK = 3;
    public const int B_OPTIONPOPUP = 4;
    public const int B_OPTIONCLOSE = 5;
    public const int B_DIFFICULTY = 6;
    public const int B_DIFFICULTYBACK = 7;
    public const int B_SKILL = 8;
    public const int B_SKILLBACK = 9;
    public const int B_OPTIONSELECT = 10;
    public const int B_GAMESTART = 11;
    public const int B_SKILL_MANULO = 12;//6.23 홍승준 추가
    public const int B_SKILL_MANULC = 13;
    public const int B_DIFFICULTYSELECT = 14;
    public const int B_RESULT_OUT = 15;//0624 추가 
    
    public void clicked(){
        switch (ButtonType)
        {
            case B_MOVESCENE:
                //NEWTWORK DATA SAVE
                //OPTION DATA SAVE
                //CURSOR DATA SAVE{
                {
                    GetComponent<AudioSource>().Play();
                    GameObject FIOO = GameObject.Find("FadeInOut");
                    if (FIOO == null) SceneManager.LoadScene(SceneorPopup);
                    else 
                    {
                        FadeInOut FIO = FIOO.GetComponent<FadeInOut>();
                        FIO.changeScene(SceneorPopup);
                    }
                    break;
                }
            case B_QUIT:
                GetComponent<AudioSource>().Play();
                Application.Quit();
                break;
            case B_CHARSELECTNEXT://Character select next button
                {
                    GetComponent<AudioSource>().Play();
                    CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
                    if (false == CC.characterallselected()) break;

                    SelectManager p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                    p_SM.SelfSave();

                    GameObject FIOO = GameObject.Find("FadeInOut");
                    if (FIOO == null) SceneManager.LoadScene(SceneorPopup);
                    else
                    {
                        FadeInOut FIO = FIOO.GetComponent<FadeInOut>();
                        FIO.changeScene(SceneorPopup);
                    }
                    break;
                }
            case B_CHARBACK://character select back
                {
                    GetComponent<AudioSource>().Play();
                    SelectManager p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                    p_SM.SelfSave();

                    GameObject FIOO = GameObject.Find("FadeInOut");
                    if (FIOO == null) SceneManager.LoadScene(SceneorPopup);
                    else
                    {
                        FadeInOut FIO = FIOO.GetComponent<FadeInOut>();
                        FIO.changeScene(SceneorPopup);
                    }

                    break;
                }
            case B_OPTIONPOPUP://option popup
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>().OptionPopup();
                    break;
                }
            case B_OPTIONCLOSE://option close
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>().OptionClose(SceneorPopup[0]);
                    break;
                }
            case B_DIFFICULTY://Difficulty
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().selectBook((byte)(SceneorPopup[0] - '0'));
                    break;
                }
            case B_DIFFICULTYBACK://difficulty back
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().cancleBook();
                break;
                }
            case B_SKILL://skill
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().selectSkill();
                    break;
                }
            case B_SKILLBACK://skill back
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().cancleSkill();
                    break;
                }
            case B_OPTIONSELECT://option select
                {
                    GetComponent<AudioSource>().Play();
                    if ('U' == SceneorPopup[1])
                        GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>().OptionChange((byte)(SceneorPopup[0] - '0'), true);
                    else
                        GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>().OptionChange((byte)(SceneorPopup[0] - '0'), false);
                    break;
                }
            case B_GAMESTART:
                {
                    GetComponent<AudioSource>().Play();
                    int p1 = GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().player;
                    int p2 = GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>().player_count;
                    SceneorPopup = "Scene_InGame" + GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().choosebook;

                    if (0 != p2) {
                        GameObject FIOO = GameObject.Find("FadeInOut");
                        if (FIOO == null) SceneManager.LoadScene(SceneorPopup);
                        else
                        {
                            FadeInOut FIO = FIOO.GetComponent<FadeInOut>();
                            FIO.changeScene(SceneorPopup);
                        }
                    }
                    break;
                }
            case B_SKILL_MANULO:
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().openManual();
                    break;
                }
            case B_SKILL_MANULC:
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().cancleManual();
                    break;
                }
            case B_DIFFICULTYSELECT:
                {
                    GetComponent<AudioSource>().Play();
                    GameObject.Find("GameOptionPrefab").GetComponent<Option>().difficulty = (byte)(SceneorPopup[0] - '0');
                    GameObject.Find("checker").transform.localPosition = transform.localPosition;
                    break;
                }
            case B_RESULT_OUT://0624 추가 
                {
                    GetComponent<AudioSource>().Play();
                    SceneManager.LoadScene("Main");
                    break;
                }
            default:
                break;
        }
    }
}
