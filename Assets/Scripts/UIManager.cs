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
        //����
        //�ж�ָ�����Ƿ����
        if (PlayerPrefs.HasKey("MusicOn"))
        {
            //�жϴ洢�����ֵ�״̬
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
        else   //��������Ŀ�������һ�δ���Ϸ��Ĭ��Ϊ������
        {
            musicToggle.isOn = true;
            musicAudio.enabled= true;
        }
    }
    private void Update()
    {
        kill.text = "��ɱ����" + shootNum.ToString();
        score.text = "�÷֣�" + Score.ToString();
    }

    public void MusicSwitch()
    {
        if (musicToggle.isOn == false)
        {
            musicAudio.enabled = false;
            //�洢   ��¼      �������ֿ��صĿ���״̬��0��1��
            PlayerPrefs.SetInt("MusicOn",0);
        }
        else
        {
            musicAudio.enabled = true;
            //�洢    ��¼
            PlayerPrefs.SetInt("MusicOn",1);
        }
        //�洢   �浵
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
