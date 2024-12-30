using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Variables/Float Variable", fileName = "Float Variable")]
public class FloatVariable : ScriptableObject
{
    public float Value;
    public GameEvent OnChangeEvent;
}