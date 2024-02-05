using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    public Text kill;
    public Text score;

    public int shootNum = 0;
    public int Score = 0;

    public Toggle musicToggle;
    public AudioSource musicAudio;

    public Text MessageText;
    private void Awake()
    {
        _instance = this;
        //读档
        //判断指定键是否存在
        if (PlayerPrefs.HasKey("MusicOn"))
        {
            //判断存储中音乐的状态
            if (PlayerPrefs.GetInt("MusicOn") == 1)
            {   
                musicToggle.isOn = true;
                musicAudio.enabled = true;
            }
            else
            {
                musicToggle.isOn = false;
                musicAudio.enabled = false;
            }
        }
        else   //若不存在目标键即第一次打开游戏则默认为打开音乐
        {
            musicToggle.isOn = true;
            musicAudio.enabled= true;
        }
    }
    private void Update()
    {
        kill.text = "击杀数：" + shootNum.ToString();
        score.text = "得分：" + Score.ToString();
    }

    public void MusicSwitch()
    {
        if (musicToggle.isOn == false)
        {
            musicAudio.enabled = false;
            //存储   记录      保存音乐开关的开关状态，0关1开
            PlayerPrefs.SetInt("MusicOn",0);
        }
        else
        {
            musicAudio.enabled = true;
            //存储    记录
            PlayerPrefs.SetInt("MusicOn",1);
        }
        //存储   存档
        PlayerPrefs.Save();
    }
    public void AddKill()
    {
        shootNum++;
    }
    public void AddScore()
    {
        Score++;
    }

    public void ShowMessage(string str)
    {
        MessageText.text = str;
    }

}
