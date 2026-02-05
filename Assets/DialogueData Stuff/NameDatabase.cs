using UnityEngine;

[CreateAssetMenu(fileName = "NameDatabase", menuName = "Scriptable Objects/NameDatabase")]
public class NameDatabase : ScriptableObject
{
    public string[] names;

    public string GetRandomName()
    {
        if (names.Length == 0) return "Unknown";
        return names[Random.Range(0, names.Length)];
    }
}
