using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourBest : MonoBehaviour
{
    public Transform player;
    private Vector3 startPos = Vector3.zero;
    private float bestScore;

    public void StartMeth()
    {
        startPos = transform.position;
        bestScore = PlayerPrefs.GetFloat("BestScore", transform.position.y);
        var targetPosition = transform.position = new Vector3(transform.position.x, bestScore, transform.position.z);
        ;
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (bestScore < player.position.y + startPos.y)
        {
            bestScore = player.position.y + startPos.y;
            PlayerPrefs.SetFloat("BestScore", bestScore);

            transform.position =
                new Vector3(transform.position.x, player.position.y + startPos.y, transform.position.z);
        }
    }
}