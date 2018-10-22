using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.Xml;
using System;
using System.IO;

public class Option : MonoBehaviour
{

    public const byte O_SCREENSIZEX = 0;
    public const byte O_SCREENSIZEY = 1;
    public const byte O_FULLSCREEN = 2;
    public const byte O_ALLVOLUME = 3;
    public const byte O_SFXVOLUME = 4;
    public const byte O_BGMVOLUME = 5;

    public static int nowScene;
    public int difficulty;

    public OptionSet option;
    //public float timer;

    private Vector3 touchedPos;
    
	void OnLevelWasLoaded(){
        nowScene = SceneManager.GetActiveScene().buildIndex;
        ApplySoundOption();
    }

	public void Start(){
		DontDestroyOnLoad (gameObject);
        difficulty = 0;
        option = new OptionSet();

        int error = LoadOption();
        if (-1 == error) {
            SaveOption();
        }

        ApplySoundOption();
        ApplyScreenOption();
    }

    public void ChangeOption(OptionSet o) {
        option = o;
        ApplySoundOption();
        ApplyScreenOption();
    }

    //load when started game and 
    public void ApplyScreenOption() {
        //screensize
        Screen.SetResolution(option.ScreenSizeX, option.ScreenSizeY, option.fullscreen);
    }

    //load per scene
    public void ApplySoundOption()
    {
        //cam sound
        //sound
    }

    public OptionSet getOption() {
        OptionSet OS = new OptionSet();
        OS.ScreenSizeX = option.ScreenSizeX;
        OS.ScreenSizeY = option.ScreenSizeY;
        OS.selectedSCRS = option.selectedSCRS;
        OS.fullscreen = option.fullscreen;
        OS.Allvolume = option.Allvolume;
        OS.SFXvolume = option.SFXvolume;
        OS.BGMvolume = option.BGMvolume;

        return OS;
    }

    public int LoadOption()
    {
        try
        {
            var path = Application.persistentDataPath + "/OptionData.xml";
            var serializer = new XmlSerializer(typeof(OptionSet));
            var stream = new FileStream(path, FileMode.Open);
            option = serializer.Deserialize(stream) as OptionSet;
            stream.Close();
            return 0;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return -1;
            //error;
        }
    }

    public void SaveOption()
    {
        try
        {
            var path = Application.persistentDataPath + "/OptionData.xml";
            var serializer = new XmlSerializer(typeof(OptionSet));
            var stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, option);
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            //error
        }
    }
}

[XmlRoot("Option")]
public class OptionSet
{
    public const int C_ScreenSizeOption = 2;
    public const int C_PerSoundVolume = 10;

    [XmlArrayItem("Display")]
    [XmlAttribute("x")]
    public int ScreenSizeX;
    [XmlAttribute("y")]
    public int ScreenSizeY;
    [XmlAttribute("fullscreen")]
    public bool fullscreen;
    [XmlArrayItem("Sound")]
    [XmlAttribute("vol")]
    public int Allvolume;
    [XmlAttribute("sfx")]
    public int SFXvolume;
    [XmlAttribute("bgm")]
    public int BGMvolume;

    public Vector2[] ScreenSizeSet;
    public int selectedSCRS;

    public OptionSet()
    {
        Allvolume = 50;
        SFXvolume = 50;
        BGMvolume = 50;
        ScreenSizeX = 2560;
        ScreenSizeY = 1440;
        fullscreen = false;

        selectedSCRS = 1;

        ScreenSizeSet = new Vector2[C_ScreenSizeOption];
        ScreenSizeSet[0] = new Vector2(1920, 1080);
        ScreenSizeSet[1] = new Vector2(2560, 1440);
    }
}