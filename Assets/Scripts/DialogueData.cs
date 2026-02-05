using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    //public string[] c_name;
    
    [TextArea(3, 10)] public string[] c_words;

    /*
    public string GetRandomName()
    {
        return c_name[Random.Range(0, c_name.Length)];
    }
    */

    public string GetRandomWords()
    {
        return c_words[Random.Range(0, c_words.Length)];
    }
}
