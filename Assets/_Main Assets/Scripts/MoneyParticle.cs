using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoneyParticle : MonoBehaviour
{
    public List<GameObject> Monies;

    public Transform PlayerTransform;
    [SerializeField] private float CurveHeight = 2f;
    [SerializeField] private float SpeedMultiplier = 2f;
    [SerializeField] private float DescentSpeedMultiplier = 1f;
    [SerializeField] private float Spread = 0.5f;
    [SerializeField] private float MaxRotationSpeed = 100f;
    [SerializeField] private float WaitAtPeak = 0.5f;


    public void Action()
    {
        foreach (var money in Monies) money.transform.GetChild(0).gameObject.SetActive(true);
        foreach (var money in Monies)
        {
            money.SetActive(true);
            money.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360),
                Random.Range(0, 360));
            StartCoroutine(MoveMoneyToPlayer(money, transform));
        }
    }

    public void Action(Transform startTransform)
    {
        foreach (var money in Monies) money.transform.GetChild(0).gameObject.SetActive(true);
        foreach (var money in Monies)
        {
            money.SetActive(true);
            money.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360),
                Random.Range(0, 360));
            StartCoroutine(MoveMoneyToPlayer(money, startTransform));
        }
    }

    private IEnumerator MoveMoneyToPlayer(GameObject money, Transform startPos)
    {
        var startPoint = startPos.position + new Vector3(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread),
            Random.Range(-Spread, Spread));
        var controlPoint = startPoint + Vector3.up * CurveHeight +
                           new Vector3(Random.Range(-Spread, Spread), 0,
                               Random.Range(-Spread, Spread));
        float time = 0;

        // Initial delay before starting movement
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        while (time < 1)
        {
            var speedMultiplier =
                time < 0.5f
                    ? SpeedMultiplier
                    : DescentSpeedMultiplier;
            time += Time.deltaTime * speedMultiplier;

            var endPoint = PlayerTransform.position;


            var position = (1 - time) * (1 - time) * startPoint
                           + 2 * (1 - time) * time * controlPoint
                           + time * time * endPoint;

            position.z = PlayerTransform.position.z;

            money.transform.position = position;


            var rotationSpeed = Random.Range(-MaxRotationSpeed, MaxRotationSpeed) * Time.deltaTime;
            money.transform.Rotate(Vector3.up, rotationSpeed);
            money.transform.Rotate(Vector3.right, rotationSpeed);
            money.transform.Rotate(Vector3.forward, rotationSpeed);

            if (time >= 0.5f &&
                time - Time.deltaTime * speedMultiplier <
                0.5f)
            {
                var waitTime = WaitAtPeak;
                while (waitTime > 0)
                {
                    waitTime -= Time.deltaTime;
                    endPoint = PlayerTransform.position;
                    position = (1 - time) * (1 - time) * startPoint
                               + 2 * (1 - time) * time * controlPoint
                               + time * time * endPoint;

                    position.z = PlayerTransform.position.z;

                    money.transform.position = position;
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }


        var randomOffset = new Vector3(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread),
            Random.Range(-Spread, Spread));
        money.transform.position = PlayerTransform.position + randomOffset;

        money.SetActive(false);
        PlayerTransform.GetComponent<Player>().PuncBag();
    }
}