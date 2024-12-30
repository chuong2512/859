using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PropCollectiable : MonoBehaviour
{
    public enum PropType
    {
        Kasa,
        Bank
    }

    [SerializeField] private PropType propType;
    private MoneyParticle moneyParticle;
    private Animator animator;
    private static readonly int KapakPlay = Animator.StringToHash("KapakPlay");
    private float dotFive = .5f, one = 1f;
    private static readonly int BankDown = Animator.StringToHash("BankDown");

    private void Start()
    {
        animator = GetComponent<Animator>();
        moneyParticle = GetComponent<MoneyParticle>();
    }

    private IEnumerator RotateObject()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * (360 * Time.deltaTime * 2));
            yield return null;
        }
    }

    private Coroutine rotateObjectCo;

    public void Action(Transform player)
    {
        moneyParticle.PlayerTransform = player;
        switch (propType)
        {
            case PropType.Kasa:
                rotateObjectCo = StartCoroutine(RotateObject());

                transform.DOMoveZ(transform.position.z + 20, dotFive)
                    .OnComplete(() =>
                    {
                        StopCoroutine(rotateObjectCo);
                        StopCoroutine(RotateObject());
                        animator.SetTrigger(KapakPlay);
                        transform.DOPunchScale(Vector3.one * 1.1f, dotFive, 1);
                    });

                break;

            case PropType.Bank:
                transform.DOPunchScale(Vector3.one * 1.5f, dotFive, 1)
                    .OnComplete(() => { animator.SetTrigger(BankDown); });
                break;
        }
    }


    private void ParticlePlay()
    {
        moneyParticle.Action();
    }
}