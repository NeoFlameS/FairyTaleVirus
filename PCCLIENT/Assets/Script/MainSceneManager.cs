using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainSceneManager : MonoBehaviour {

    public PlayerIcon[] PI = new PlayerIcon[4];
    public GameObject CharacterInfoSet;
    Character[] ch;

    public GameObject OPTIONPOPUP;
    public GameObject BUTTON;

    public GameObject[] ScreenSize = new GameObject[OptionSet.C_ScreenSizeOption];
    public GameObject[] FullScreen = new GameObject[2];

    OptionSet option;
    
    bool changed_SS;
    bool changed_FS;
    public Text text_AllVolume;
    bool changed_AV;
    public Text text_SFXVolume;
    bool changed_SFXV;
    public Text text_BGMVolume;
    bool changed_BGMV;

    public void Start()
    {
        option = new OptionSet();
        option =  GameObject.Find("GameOptionPrefab").GetComponent<Option>().getOption();
        CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
        byte[] id = CC.selectedID();
        int count = 0;

        for (int i = 0; i < 4; ++i) {
            if (125 != id[i]) count++;
        }

        SelectManager SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
        ch = SM.LoadToScene(id, count);

        GameObject tmp = GameObject.Find("Selected Character Status(Clone)");
        if (null == tmp) tmp = Instantiate(CharacterInfoSet);

        CharacterSet CS = tmp.GetComponent<CharacterSet>();
        CS.init(ch);

        for (int i = 0; i < 4; ++i) {
            PI[i].init();

            if (125 != ch[i].ch_type)
            {
                PI[i].connected = true;
                PI[i].nickname = CC.nickname[i];
                CS.nickname[i] = CC.nickname[i];
                PI[i].level = ch[i].clearedround;
                PI[i].ch_type = ch[i].ch_type;
            }

            PI[i].show();
        }

        changed_SS = false;
        changed_SFXV = false;
        changed_FS = false;
        changed_BGMV = false;
        changed_AV = false;
    }

    public void OptionPopup() {

        OPTIONPOPUP.SetActive(true);
        BUTTON.SetActive(false);

        option = GameObject.Find("GameOptionPrefab").GetComponent<Option>().getOption();

        for (int i = 0; i < OptionSet.C_ScreenSizeOption; ++i)
        {
            ScreenSize[i].SetActive(false);
        }
        ScreenSize[option.selectedSCRS].SetActive(true);


        if (false == option.fullscreen)
        {
            FullScreen[0].SetActive(false);
            FullScreen[1].SetActive(true);
        }
        else
        {
            FullScreen[1].SetActive(false);
            FullScreen[0].SetActive(true);
        }
        

        text_SFXVolume.text = option.SFXvolume.ToString();
        text_BGMVolume.text = option.BGMvolume.ToString();
        text_AllVolume.text = option.Allvolume.ToString();
    }

    public void OptionClose(char ack)
    {
        if ('O' == ack)
        {
            GameObject.Find("GameOptionPrefab").GetComponent<Option>().ChangeOption(option);
            OPTIONPOPUP.SetActive(false);
            BUTTON.SetActive(true);
        }

        else
        {
            OPTIONPOPUP.SetActive(false);
            BUTTON.SetActive(true);
        }
    }

    public void OptionChange(byte optionN, bool Up) {
        switch (optionN) {
            case Option.O_SCREENSIZEX:
            case Option.O_SCREENSIZEY:
                if (Up)
                {
                    option.selectedSCRS = (option.selectedSCRS + 1) % OptionSet.C_ScreenSizeOption;
                    option.ScreenSizeX = (int)option.ScreenSizeSet[option.selectedSCRS].x;
                    option.ScreenSizeY = (int)option.ScreenSizeSet[option.selectedSCRS].y;
                }
                else
                {
                    if (option.selectedSCRS > 0) option.selectedSCRS = (option.selectedSCRS - 1);
                    else option.selectedSCRS = OptionSet.C_ScreenSizeOption-1;
                    option.ScreenSizeX = (int)option.ScreenSizeSet[option.selectedSCRS].x;
                    option.ScreenSizeY = (int)option.ScreenSizeSet[option.selectedSCRS].y;
                }
                changed_SS = true;
                break;
            case Option.O_FULLSCREEN:
                option.fullscreen = !option.fullscreen;
                changed_FS = true;
                break;
            case Option.O_ALLVOLUME:
                if (Up) option.Allvolume += OptionSet.C_PerSoundVolume;
                else option.Allvolume -= OptionSet.C_PerSoundVolume;
                option.Allvolume = Mathf.Clamp(option.Allvolume, 0, 100);
                changed_AV = true;
                break;
            case Option.O_SFXVOLUME:
                if (Up) option.SFXvolume += OptionSet.C_PerSoundVolume;
                else option.SFXvolume -= OptionSet.C_PerSoundVolume;
                option.SFXvolume = Mathf.Clamp(option.SFXvolume, 0, 100);
                changed_SFXV = true;
                break;
            case Option.O_BGMVOLUME:
                if (Up) option.BGMvolume += OptionSet.C_PerSoundVolume;
                else option.BGMvolume -= OptionSet.C_PerSoundVolume;
                option.BGMvolume = Mathf.Clamp(option.BGMvolume, 0, 100);
                changed_BGMV = true;
                break;
            default:
                //error
                break;
        }
    }

    public void LateUpdate()
    {
        // 텍스트 변경
        if (true == changed_SS)
        {
            for (int i = 0; i < OptionSet.C_ScreenSizeOption; ++i) {
                ScreenSize[i].SetActive(false);
            }
            ScreenSize[option.selectedSCRS].SetActive(true);
            changed_SS = false;
        }
        if (true == changed_SFXV)
        {
            text_SFXVolume.text = option.SFXvolume.ToString();
            changed_SFXV = false;
        }
        if (true == changed_FS) {
            if (false == option.fullscreen)
            {
                FullScreen[0].SetActive(false);
                FullScreen[1].SetActive(true);
            }
            else {
                FullScreen[1].SetActive(false);
                FullScreen[0].SetActive(true);
            }
            changed_FS = false;
        }
        if (true == changed_BGMV)
        {
            text_BGMVolume.text = option.BGMvolume.ToString();
            changed_BGMV = false;
        }
        if (true == changed_AV)
        {
            text_AllVolume.text = option.Allvolume.ToString();
            changed_AV = false;
        }

    }
}
