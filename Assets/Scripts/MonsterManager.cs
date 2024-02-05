using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Animation anim;
    //定义了idle状态和die状态的动画
    public AnimationClip idleClip;
    public AnimationClip dieClip;
    //撞击音效
    public AudioSource kickAudio;
    //游戏管理器
    public GameObject GM;

    public int monsterType;
    private void Awake()
    {
        //获得动画组件
        anim = gameObject.GetComponent<Animation>();
        //获取动画堆中的对应动画
        anim.clip = idleClip;
        GM = GameObject.Find("GameManager");
        kickAudio = GM.GetComponents<AudioSource>()[1];
    }
    //判断当子弹碰到怪时
    private void OnCollisionEnter(Collision collision)
    {
        //若子弹打到怪物
        string[] test = collision.gameObject.name.Split("(");
        if (test[0] == "Bullet")
        {
            //播放撞击音效
            kickAudio.Play();
            Destroy(collision.collider.gameObject);
            //播放死亡动画
            anim.clip = dieClip;
            anim.Play();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine("Deactivate");
            UIManager._instance.AddScore();
            UIManager._instance.AddKill();
        }
    }
    //怪物消失时，恢复为一般状态
    private void OnDisable()
    {
        anim.clip = idleClip;
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1);
        //使当前怪物进入未激活状态
        gameObject.GetComponentInParent<TargetManager>().UpdateMonsters();
    }
}
