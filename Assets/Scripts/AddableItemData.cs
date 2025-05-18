using UnityEngine;

[CreateAssetMenu(fileName = "AddableItemData", menuName = "ARMagicBar/AddableItemData")]
public class AddableItemData : ScriptableObject
{
    public string objectName;
    public GameObject prefab;
    public Texture2D uiSprite;
}
