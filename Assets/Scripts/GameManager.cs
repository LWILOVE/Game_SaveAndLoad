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
        //设置鼠标是否可见
        Cursor.visible = true;
    }

    private void UnPause()
    {
        isPaused=false;
        menuGO.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
    }

    //创建Save对象并存储当前游戏状态信息
    private Save CreateSaveGo()
    {
        //新建Save对象
        Save save = new Save();
        //遍历所有的target
        //如果其中含有处于激活状态的怪物，则将target的位置信息和激活状态的怪物的类型添加到List中
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
        //把击杀数和得分保存在Save对象中
        save.kill = UIManager._instance.shootNum;
        save.score = UIManager._instance.Score;
        //返回Save对象 
        return save;
    }
    //通过读档信息重置游戏状态，分数，激活的怪物等
    private void SetGame(Save save)
    {
        //先将所有的Target里面的怪物清空并重置所有计时
        foreach (GameObject targetGo in targetGOs)
        {
            targetGo.GetComponent<TargetManager>().UpdateMonsters();
        }
        //通过反序列化得到的Save对象中存储的信息，激活指定的怪物
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            int position = save.livingTargetPositions[i];
            int type = save.livingMonsterTypes[i];
            targetGOs[position].GetComponent<TargetManager>().ActivateMonsterByType(type);
        }
        //更新UI显示
        UIManager._instance.shootNum = save.kill;
        UIManager._instance.Score = save.score;
        //调整为未暂停状态
        UnPause();
    }
    //二进制存储方式
    //需要引入IO流和Runtime.Serialization.Formatters.Binary（二进制格式器）;
    private void SaveByBin()
    {
        //序列化过程（将Save对象转换为字节流）
        //创建Save对象并保存当前游戏状态
        Save save = CreateSaveGo();
        //创建二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建文件流
        //Application.dataPath:获取本App所在的文件路径
        FileStream fileStream = File.Create(Application.dataPath + "/StreamingAssets" + "/byBin.txt");
        //用二进制格式化程序的序列化方法来序列化Save对象
        //参数：文件流  待序列化对象
        bf.Serialize(fileStream, save);
        //关闭文件流
        fileStream.Close();

        //若文件存在则给玩家提示保存成功
        if (File.Exists(Application.dataPath + "/StreamingAssets" + "/byBin.txt"))
        {
            UIManager._instance.ShowMessage("保存成功");
        }
    }
    //二进制加载方式
    private void LoadByBin()
    {
        if (File.Exists(Application.dataPath + "/StreamingAssets" + "/byBin.txt"))
        {
            //反序列化过程
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //打开一个文件流
            FileStream fileStream = File.Open(Application.dataPath + "/StreamingAssets" + "/byBin.txt", FileMode.Open);
            //调用格式化程序的反序列化方法  将文件流转化为Save对象
            Save save = (Save)bf.Deserialize(fileStream);
            //关闭文件流
            fileStream.Close();
            SetGame(save);
            UIManager._instance.ShowMessage("");
        }
        else
        {
            UIManager._instance.SendMessage("未检测到存档");
        }
    
    }
    //XML存储方式
    private void SaveByXml()
    {
        Save save = CreateSaveGo();
        //创建XML文件的存储路径
        string filePath = Application.dataPath + "/StreamingAssets" + "/byXML.txt";
        //创建XML文档
        XmlDocument xmlDoc = new XmlDocument();
        //创建根节点   即最上层节点
        XmlElement root = xmlDoc.CreateElement("save");
        //设置根节点的值
        root.SetAttribute("name","saveFile1");
        //创建XmlElement
        XmlElement target;
        XmlElement targetPosition;
        XmlElement monsterType;

        //遍历save中存储的数据，将数据转换为XML格式
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            target = xmlDoc.CreateElement("target");
            targetPosition = xmlDoc.CreateElement("targetposition");
            //设置InnerText值
            targetPosition.InnerText = save.livingTargetPositions[i].ToString();
            monsterType = xmlDoc.CreateElement("monsterType");
            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

            //设置节点间的层级关系  root---target---(targetPosition.monsterType)
            target.AppendChild(targetPosition);
            target.AppendChild(monsterType);
            root.AppendChild(target);
        }

        //设置射击数和分数节点并设置层级关系 xmlDoc---root---（target，skill，score）
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
            UIManager._instance.ShowMessage("保存成功");
        }
    }
    //XML加载方式
    private void LoadByXml()
    {
        string filePath = Application.dataPath + "/StreamingAssets" + "/byXML.txt";
        if (File.Exists(filePath))
        { 
            Save save = new Save();
            //加载XML文档
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            //通过节点名称来获取元素，结果为XmlNodeList类型
            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
            //遍历所有的target节点，并获得子节点和子节点的InnerText
            if (targets.Count != 0)
            {
                foreach (XmlNode target in targets)
                {
                    XmlNode targetPosition = target.ChildNodes[0];
                    int targetPosionIndex = int.Parse(targetPosition.InnerText);
                    //将得到的值存储到save中
                    save.livingTargetPositions.Add(targetPosionIndex);

                    XmlNode monsterType = target.ChildNodes[1];
                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
                    save.livingMonsterTypes.Add(monsterTypeIndex);
                }

                //获得存储的射击数和分数
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
            UIManager._instance.ShowMessage("未检测到存档");
        }
    }
    //Json存储方式
    private void SaveByJson()
    {
        Save save = CreateSaveGo();
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json";
        //利用JsonMapper将save对象转换为Json格式的字符串
        string saveJsonStr = JsonMapper.ToJson(save);
        //将这个字符串写入到文件中
        //创建一个StreamWriter
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        //关闭StreamWriter
        sw.Close();
        UIManager._instance.ShowMessage("保存成功");
    }
    //Json加载方式
    private void LoadByJson()
    { 
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json"; ;
        if (File.Exists(filePath))
        {
            //创建一个StreamReader，用于读取流
            StreamReader sr = new StreamReader(filePath);
            //将读取到的流赋值给JSONSTR
            string jsonStr = sr.ReadToEnd();
            //关闭
            sr.Close();
            //将字符串JSONSTR转换为Save对象
            Save save = JsonMapper.ToObject<Save>(jsonStr);
            SetGame(save);
            UIManager._instance.ShowMessage("");
        }
        else
        {
            UIManager._instance.ShowMessage("未检测到存档");
        }
    }

    public void ContinueGame()
    {
        UnPause();
        UIManager._instance.ShowMessage("");
    }

    //新游戏
    public void NewGame()
    {
        //怪物清0
        foreach (GameObject go in targetGOs)
        {
            go.GetComponent<TargetManager>().UpdateMonsters();
        }
        //分数清0
        UIManager._instance.shootNum = 0;
        UIManager._instance.Score = 0;
        UIManager._instance.ShowMessage("");
        UnPause();
    }
    //退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
    //游戏保存
    public void SaveGame()
    {
        //SaveByBin();
        //SaveByJson();
        SaveByXml();
    }

    //加载游戏
    public void LoadGame()
    {
        //LoadByBin();
        //LoadByJson();
        LoadByXml();
    }
}
