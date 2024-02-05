using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    private AudioSource gunAudio;
    //设置枪的最大/最小旋转角度
    private float maxYRotation = 120;
    private float minYRotation = 0;
    private float maxXRotation = 60;
    private float minXRotation = 0;
    //射击间隔时长
    private float shootTime = 0.5f;
    private float shootTimer = 0;

    public GameObject bulletGD;
    public Transform firePosition;

    private void Awake()
    {
        gunAudio = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager._instance.isPaused == false)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootTime)
            {
                //可以射击
                if (Input.GetMouseButtonDown(0))
                {
                    //播放开火音效
                    gunAudio.Play();
                    //实例化子弹
                    GameObject bulletCurrent = GameObject.Instantiate(bulletGD, firePosition.position, Quaternion.identity);
                    shootTimer = 0;
                    //控制子弹移动
                    bulletCurrent.GetComponent<Rigidbody>().AddForce(transform.forward * 8000);
                    gameObject.GetComponent<Animation>().Play();
                }
            }
            //控制枪的旋转
            float yPosPrecent = Input.mousePosition.x / Screen.width;
            float xPosPrecent = Input.mousePosition.y / Screen.height;

            float xAngle = -Mathf.Clamp(xPosPrecent * maxXRotation, minXRotation, maxXRotation) + 15;
            float yAngle = Mathf.Clamp(yPosPrecent * maxYRotation, minYRotation, maxYRotation) - 60;

            transform.eulerAngles = new Vector3(xAngle, yAngle, 0);
        }
    }

}
