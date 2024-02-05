using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Animation anim;
    //������idle״̬��die״̬�Ķ���
    public AnimationClip idleClip;
    public AnimationClip dieClip;
    //ײ����Ч
    public AudioSource kickAudio;
    //��Ϸ������
    public GameObject GM;

    public int monsterType;
    private void Awake()
    {
        //��ö������
        anim = gameObject.GetComponent<Animation>();
        //��ȡ�������еĶ�Ӧ����
        anim.clip = idleClip;
        GM = GameObject.Find("GameManager");
        kickAudio = GM.GetComponents<AudioSource>()[1];
    }
    //�жϵ��ӵ�������ʱ
    private void OnCollisionEnter(Collision collision)
    {
        //���ӵ��򵽹���
        string[] test = collision.gameObject.name.Split("(");
        if (test[0] == "Bullet")
        {
            //����ײ����Ч
            kickAudio.Play();
            Destroy(collision.collider.gameObject);
            //������������
            anim.clip = dieClip;
            anim.Play();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine("Deactivate");
            UIManager._instance.AddScore();
            UIManager._instance.AddKill();
        }
    }
    //������ʧʱ���ָ�Ϊһ��״̬
    private void OnDisable()
    {
        anim.clip = idleClip;
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1);
        //ʹ��ǰ�������δ����״̬
        gameObject.GetComponentInParent<TargetManager>().UpdateMonsters();
    }
}
