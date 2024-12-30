using UnityEngine;
using NaughtyAttributes;

public class MultipleCollectiable : MonoBehaviour
{
    [Button()]
    public void MultiplyMoneyAmountInScene()
    {
        var collectibles = FindObjectsOfType<Collectiable>();

        foreach (var collectible in collectibles) collectible.moneyAmount *= 2;
    }
}