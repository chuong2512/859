using UnityEngine;
using NaughtyAttributes;

public class MultipleWallet : MonoBehaviour
{
    [Button()]
    public void MultiplyMoneyAmountInScene()
    {
        var scripts = FindObjectsOfType<Wallet>();

        foreach (var script in scripts)
            if (!script.player)
                script.moeyAmount *= 2;
    }
}