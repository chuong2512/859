using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class LevelEndEditor : MonoBehaviour
{
    public GameObject prefab; // Your prefab with TextMeshPro component
    [SerializeField] private List<GameObject> myList = new(); // The list of your objects
    private int clickCount = 1; // A counter for the button click

    [Button()]
    private void AddTextMesh()
    {
        foreach (var obj in myList)
        {
            // Instantiate the prefab and parent it to the object
            var newPrefab = Instantiate(prefab, obj.transform, false);

            // Get the TextMeshPro component and change its text
            var textMesh = newPrefab.GetComponent<TextMeshPro>();
            if (textMesh != null) textMesh.text = clickCount + "X";
            clickCount++;
        }

        // Increase the click count
        clickCount++;
    }
}