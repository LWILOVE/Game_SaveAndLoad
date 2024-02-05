using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    //保存所有该目标下的怪物
    public GameObject[] monsters;
    //保存目前处于激活状态的怪物
    public GameObject activeMonster = null;
    //表示目标所在的位置（0-8）
    private float timer = 0;

    public int targetPosition;

    private void Start()
    {
        foreach (GameObject monster in monsters)
        {
            monster.GetComponent<BoxCollider>().enabled = false;
            monster.SetActive(false);
        }
        StartCoroutine("AliveTimer");
    }
    private void Update()
    {
    }
    //随机召唤怪物
    private void ActivateMonster()
    {
        //生成随机数，随机激活一只怪
        int index = Random.Range(0, monsters.Length);
        activeMonster = monsters[index];
        activeMonster.SetActive(true);
        activeMonster.GetComponent<BoxCollider>().enabled = true; ;
        StartCoroutine("DeathTimer");
    }
    //设置怪物的随机生成协程
    IEnumerator AliveTimer()
    {
        //等待1-4S召唤怪物
        yield return new WaitForSeconds(Random.Range(1,4));
        ActivateMonster();
    }
    //让激活的怪物隐形
    private void DeActivateMonster()
    {
        if (activeMonster != null)
        {
            activeMonster.GetComponent<BoxCollider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        StartCoroutine("AliveTimer");
    }
    //设置怪物死亡时间
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(Random.Range(2,3));
        DeActivateMonster();
    }
    //更新生命周期
    //在子弹命中敌人或游戏重新开始时启动
    public void UpdateMonsters()
    {
        StopAllCoroutines();
        if (activeMonster != null)
        {
            activeMonster.GetComponent<BoxCollider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        StartCoroutine("AliveTimer");
    }
    //按照给定的怪物类型激活怪物
    public void ActivateMonsterByType(int type)
    {
        StopAllCoroutines();
        if (activeMonster != null)
        {
            activeMonster.GetComponent<BoxCollider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        activeMonster = monsters[type];
        activeMonster.SetActive(true);
        activeMonster.GetComponent<BoxCollider>().enabled = true;
        StartCoroutine("DeathTimer");
    }
}
