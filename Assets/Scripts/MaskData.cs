using UnityEngine;

[CreateAssetMenu(menuName = "Mask Data")]
public class MaskData : ScriptableObject
{
    public Sprite sprite;

    [Header("Properties")]
    public bool isCracked;
    public bool isBloody;
    public bool isNormal;
    public bool isSad;
}
