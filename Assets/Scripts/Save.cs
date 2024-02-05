using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//标志本类为序列化类
[System.Serializable]
public class Save
{
    public List<int> livingTargetPositions = new List<int>();
    public List<int> livingMonsterTypes = new List<int>();

    public int kill = 0;
    public int score = 0;
}
