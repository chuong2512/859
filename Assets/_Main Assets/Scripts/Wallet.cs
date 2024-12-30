using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Game;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wallet : MonoBehaviour
{
    public int moeyAmount;
    private float one = 1;
    public int MoeyAmount => moeyAmount;


    [SerializeField] private TextMeshPro textMeshPro;
    [ShowIf("player")] [SerializeField] private TextMeshPro moneyTextForAnim;
    [ShowIf("player")] [SerializeField] private ParticleSystem moneyEarn;
    private int moeyAmount1;
    public bool player;
    [ShowIf("player")] [SerializeField] private SkinnedMeshRenderer bagBlendShape;
    [ShowIf("player")] [SerializeField] private Color green, red;
    [SerializeField] private MoneyParticle moneyParticle;
    [SerializeField] private bool mysterious;

    private void Start()
    {
        if (moneyParticle)
        {
            moeyAmount = 0;
            foreach (var collectiable in moneyParticle.Monies)
                moeyAmount += collectiable.GetComponent<Collectiable>().moneyAmount;
        }

        RefreshText();
    }

    private void MoneyAnim(int amount)
    {
        if (player)
        {
            if (amount > 0)
            {
                moneyTextForAnim.color = green;
                moneyTextForAnim.text = "$" + amount.ToString();
            }
            else
            {
                moneyTextForAnim.text = " - $" + Mathf.Abs(amount);
                moneyTextForAnim.color = red;
            }


            moneyTextForAnim.gameObject.SetActive(true);
            moneyTextForAnim.transform.DOKill();
            moneyTextForAnim.transform.position =
                new Vector3(textMeshPro.transform.position.x, textMeshPro.transform.position.y + .5f,
                    textMeshPro.transform.position.z);
            moneyTextForAnim.transform.DOMoveY(moneyTextForAnim.transform.position.y + 1.5f, dotFive).OnComplete(() =>
            {
                moneyTextForAnim.gameObject.SetActive(false);
            });
        }
    }

    private void LoseMoneyChangeColor()
    {
        if (textMeshPro)
            textMeshPro.color = red;
        Invoke(nameof(LoseMoneyReturnColor), .5f);
    }

    private void LoseMoneyReturnColor()
    {
        if (textMeshPro)
            textMeshPro.color = green;
    }

    public void AddMoney(int amount, bool collectiable)
    {
        MoneyAnim(amount);
        moeyAmount += amount;

        RefreshText();
        moneyEarn.Play();
        ScaleUpDown();
    }

    public void AddMoney(int amount)
    {
        MoneyAnim(amount);
        if (moeyAmount + amount > 0)
            moeyAmount += amount;
        else
            moeyAmount = 0;

        RefreshText();
    }

    public void RemoveMoney(int amount)
    {
        MoneyAnim(-amount);
        if (moeyAmount - amount >= 0)
            moeyAmount -= amount;
        else
            moeyAmount = 0;
        RefreshText();
        ScaleUpDown();
        if (player) LoseMoneyChangeColor();
    }

    public void RemoveAllMoney()
    {
        moeyAmount = 0;
        RefreshText();
    }


    public void GiveAllMoney(Wallet wallet)
    {
        wallet.MoneyAnim(moeyAmount);
        wallet.AddMoney(moeyAmount);
        wallet.ScaleUpDown();
        RemoveAllMoney();
    }


    private void RefreshText()
    {
        var endValue = 40f / 500f * moeyAmount;

        if (bagBlendShape)
        {
            bagBlendShape.DOKill();
            DOTween.To(() => bagBlendShape.GetBlendShapeWeight(0), x => bagBlendShape.SetBlendShapeWeight(0, x),
                endValue,
                one);
        }

        if (textMeshPro && !mysterious)
        {
            if (moeyAmount < 0)
                textMeshPro.text = " - $" + Mathf.Abs(moeyAmount);
            else
                textMeshPro.text = "$" + moeyAmount.ToString();
        }

        if (mysterious)
        {
            textMeshPro.color = Color.black;
            textMeshPro.text = "?";
        }
    }


    private void ScaleUpDown()
    {
        if (textMeshPro && textMeshPro.gameObject.activeSelf && player)
        {
            textMeshPro.transform.DOComplete();
            textMeshPro.transform.DOPunchScale(Vector3.one * .15f, dotFive, 1);
        }
    }

    private float dotFive = .5f;

    public void EarnRealMoney(int amount)
    {
        if (moeyAmount - amount >= 0)
            moeyAmount -= amount;
        else
            moeyAmount = 0;
        RefreshText();

        PlayerEconomy.Instance.AddMoney(amount);
    }

    public void TextHide()
    {
        if (textMeshPro)
            textMeshPro.gameObject.SetActive(false);
    }

    public void TextShow()
    {
        if (textMeshPro)
            textMeshPro.gameObject.SetActive(true);
    }
}