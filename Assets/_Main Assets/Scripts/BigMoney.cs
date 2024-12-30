using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using UnityEngine;

public class BigMoney : Poolable
{
    public void GetThis(Vector3 pos, Transform parent, float scaleTime, bool firstTime)
    {
        gameObject.SetActive(true);
        transform.parent = parent;
        if (firstTime)
            transform.localPosition = Vector3.zero;
        else
            transform.position = pos;


        transform.rotation = Quaternion.Euler(0, Random.Range(-30, 30), 0);

        var scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, scaleTime);

        transform.parent = LevelManager.Instance.transform;
    }

    public void Depossit()
    {
        ReturnToPool();
    }
}