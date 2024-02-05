using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using System.Xml;
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public bool isPaused = true;
    public GameObject menuGO;
    public GameObject[] targetGOs;
    private void Awake()
    {
        _instance = this;
        Pause();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    private void Pause()
    {
        isPaused = true;
        menuGO.SetActive(true);
        Time.timeScale = 0;
        //��������Ƿ�ɼ�
        Cursor.visible = true;
    }

    private void UnPause()
    {
        isPaused=false;
        menuGO.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
    }

    //����Save���󲢴洢��ǰ��Ϸ״̬��Ϣ
    private Save CreateSaveGo()
    {
        //�½�Save����
        Save save = new Save();
        //�������е�target
        //������к��д��ڼ���״̬�Ĺ����target��λ����Ϣ�ͼ���״̬�Ĺ����������ӵ�List��
        foreach (GameObject targetGo in targetGOs)
        {
            TargetManager targetManager = targetGo.GetComponent<TargetManager>();
            if (targetManager.activeMonster != null)
            {
                save.livingTargetPositions.Add(targetManager.targetPosition);
                int type = targetManager.activeMonster.GetComponent<MonsterManager>().monsterType;
                save.livingMonsterTypes.Add(type);
            }
        }
        //�ѻ�ɱ���͵÷ֱ�����Save������
        save.kill = UIManager._instance.shootNum;
        save.score = UIManager._instance.Score;
        //����Save���� 
        return save;
    }
    //ͨ��������Ϣ������Ϸ״̬������������Ĺ����
    private void SetGame(Save save)
    {
        //�Ƚ����е�Target����Ĺ�����ղ��������м�ʱ
        foreach (GameObject targetGo in targetGOs)
        {
            targetGo.GetComponent<TargetManager>().UpdateMonsters();
        }
        //ͨ�������л��õ���Save�����д洢����Ϣ������ָ���Ĺ���
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            int position = save.livingTargetPositions[i];
            int type = save.livingMonsterTypes[i];
            targetGOs[position].GetComponent<TargetManager>().ActivateMonsterByType(type);
        }
        //����UI��ʾ
        UIManager._instance.shootNum = save.kill;
        UIManager._instance.Score = save.score;
        //����Ϊδ��ͣ״̬
        UnPause();
    }
    //�����ƴ洢��ʽ
    //��Ҫ����IO����Runtime.Serialization.Formatters.Binary�������Ƹ�ʽ����;
    private void SaveByBin()
    {
        //���л����̣���Save����ת��Ϊ�ֽ�����
        //����Save���󲢱��浱ǰ��Ϸ״̬
        Save save = CreateSaveGo();
        //���������Ƹ�ʽ������
        BinaryFormatter bf = new BinaryFormatter();
        //�����ļ���
        //Application.dataPath:��ȡ��App���ڵ��ļ�·��
        FileStream fileStream = File.Create(Application.dataPath + "/StreamingAssets" + "/byBin.txt");
        //�ö����Ƹ�ʽ����������л����������л�Save����
        //�������ļ���  �����л�����
        bf.Serialize(fileStream, save);
        //�ر��ļ���
        fileStream.Close();

        //���ļ�������������ʾ����ɹ�
        if (File.Exists(Application.dataPath + "/StreamingAssets" + "/byBin.txt"))
        {
            UIManager._instance.ShowMessage("����ɹ�");
        }
    }
    //�����Ƽ��ط�ʽ
    private void LoadByBin()
    {
        if (File.Exists(Application.dataPath + "/StreamingAssets" + "/byBin.txt"))
        {
            //�����л�����
            //����һ�������Ƹ�ʽ������
            BinaryFormatter bf = new BinaryFormatter();
            //��һ���ļ���
            FileStream fileStream = File.Open(Application.dataPath + "/StreamingAssets" + "/byBin.txt", FileMode.Open);
            //���ø�ʽ������ķ����л�����  ���ļ���ת��ΪSave����
            Save save = (Save)bf.Deserialize(fileStream);
            //�ر��ļ���
            fileStream.Close();
            SetGame(save);
            UIManager._instance.ShowMessage("");
        }
        else
        {
            UIManager._instance.SendMessage("δ��⵽�浵");
        }
    
    }
    //XML�洢��ʽ
    private void SaveByXml()
    {
        Save save = CreateSaveGo();
        //����XML�ļ��Ĵ洢·��
        string filePath = Application.dataPath + "/StreamingAssets" + "/byXML.txt";
        //����XML�ĵ�
        XmlDocument xmlDoc = new XmlDocument();
        //�������ڵ�   �����ϲ�ڵ�
        XmlElement root = xmlDoc.CreateElement("save");
        //���ø��ڵ��ֵ
        root.SetAttribute("name","saveFile1");
        //����XmlElement
        XmlElement target;
        XmlElement targetPosition;
        XmlElement monsterType;

        //����save�д洢�����ݣ�������ת��ΪXML��ʽ
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            target = xmlDoc.CreateElement("target");
            targetPosition = xmlDoc.CreateElement("targetposition");
            //����InnerTextֵ
            targetPosition.InnerText = save.livingTargetPositions[i].ToString();
            monsterType = xmlDoc.CreateElement("monsterType");
            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

            //���ýڵ��Ĳ㼶��ϵ  root---target---(targetPosition.monsterType)
            target.AppendChild(targetPosition);
            target.AppendChild(monsterType);
            root.AppendChild(target);
        }

        //����������ͷ����ڵ㲢���ò㼶��ϵ xmlDoc---root---��target��skill��score��
        XmlElement kill = xmlDoc.CreateElement("kill");
        kill.InnerText = save.kill.ToString();
        root.AppendChild(kill);
        XmlElement score = xmlDoc.CreateElement("score");
        score.InnerText = save.score.ToString();
        root.AppendChild (score);

        xmlDoc.AppendChild(root);
        xmlDoc.Save(filePath);

        if (File.Exists(Application.dataPath + "/StreamingAssets" + "/byXML.txt"))
        {
            UIManager._instance.ShowMessage("����ɹ�");
        }
    }
    //XML���ط�ʽ
    private void LoadByXml()
    {
        string filePath = Application.dataPath + "/StreamingAssets" + "/byXML.txt";
        if (File.Exists(filePath))
        { 
            Save save = new Save();
            //����XML�ĵ�
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            //ͨ���ڵ���������ȡԪ�أ����ΪXmlNodeList����
            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
            //�������е�target�ڵ㣬������ӽڵ���ӽڵ��InnerText
            if (targets.Count != 0)
            {
                foreach (XmlNode target in targets)
                {
                    XmlNode targetPosition = target.ChildNodes[0];
                    int targetPosionIndex = int.Parse(targetPosition.InnerText);
                    //���õ���ֵ�洢��save��
                    save.livingTargetPositions.Add(targetPosionIndex);

                    XmlNode monsterType = target.ChildNodes[1];
                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
                    save.livingMonsterTypes.Add(monsterTypeIndex);
                }

                //��ô洢��������ͷ���
                XmlNodeList kill = xmlDoc.GetElementsByTagName("kill");
                int killCount = int.Parse(kill[0].InnerText);
                save.kill = killCount;

                XmlNodeList score = xmlDoc.GetElementsByTagName("score");
                int scoreCount = int.Parse(score[0].InnerText);
                save.score = scoreCount;

                SetGame(save);
                UIManager._instance.ShowMessage("");
            }
        }
        else
        {
            UIManager._instance.ShowMessage("δ��⵽�浵");
        }
    }
    //Json�洢��ʽ
    private void SaveByJson()
    {
        Save save = CreateSaveGo();
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json";
        //����JsonMapper��save����ת��ΪJson��ʽ���ַ���
        string saveJsonStr = JsonMapper.ToJson(save);
        //������ַ���д�뵽�ļ���
        //����һ��StreamWriter
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        //�ر�StreamWriter
        sw.Close();
        UIManager._instance.ShowMessage("����ɹ�");
    }
    //Json���ط�ʽ
    private void LoadByJson()
    { 
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json"; ;
        if (File.Exists(filePath))
        {
            //����һ��StreamReader�����ڶ�ȡ��
            StreamReader sr = new StreamReader(filePath);
            //����ȡ��������ֵ��JSONSTR
            string jsonStr = sr.ReadToEnd();
            //�ر�
            sr.Close();
            //���ַ���JSONSTRת��ΪSave����
            Save save = JsonMapper.ToObject<Save>(jsonStr);
            SetGame(save);
            UIManager._instance.ShowMessage("");
        }
        else
        {
            UIManager._instance.ShowMessage("δ��⵽�浵");
        }
    }

    public void ContinueGame()
    {
        UnPause();
        UIManager._instance.ShowMessage("");
    }

    //����Ϸ
    public void NewGame()
    {
        //������0
        foreach (GameObject go in targetGOs)
        {
            go.GetComponent<TargetManager>().UpdateMonsters();
        }
        //������0
        UIManager._instance.shootNum = 0;
        UIManager._instance.Score = 0;
        UIManager._instance.ShowMessage("");
        UnPause();
    }
    //�˳���Ϸ
    public void QuitGame()
    {
        Application.Quit();
    }
    //��Ϸ����
    public void SaveGame()
    {
        //SaveByBin();
        //SaveByJson();
        SaveByXml();
    }

    //������Ϸ
    public void LoadGame()
    {
        //LoadByBin();
        //LoadByJson();
        LoadByXml();
    }
}
