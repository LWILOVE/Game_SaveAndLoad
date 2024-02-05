using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine("DestroySelf");
    }
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    }
}
