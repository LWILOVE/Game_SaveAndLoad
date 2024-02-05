using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    //�������и�Ŀ���µĹ���
    public GameObject[] monsters;
    //����Ŀǰ���ڼ���״̬�Ĺ���
    public GameObject activeMonster = null;
    //��ʾĿ�����ڵ�λ�ã�0-8��
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
    //����ٻ�����
    private void ActivateMonster()
    {
        //������������������һֻ��
        int index = Random.Range(0, monsters.Length);
        activeMonster = monsters[index];
        activeMonster.SetActive(true);
        activeMonster.GetComponent<BoxCollider>().enabled = true; ;
        StartCoroutine("DeathTimer");
    }
    //���ù�����������Э��
    IEnumerator AliveTimer()
    {
        //�ȴ�1-4S�ٻ�����
        yield return new WaitForSeconds(Random.Range(1,4));
        ActivateMonster();
    }
    //�ü���Ĺ�������
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
    //���ù�������ʱ��
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(Random.Range(2,3));
        DeActivateMonster();
    }
    //������������
    //���ӵ����е��˻���Ϸ���¿�ʼʱ����
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
    //���ո����Ĺ������ͼ������
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
