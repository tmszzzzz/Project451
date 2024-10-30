using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "ProfileData", menuName = "Custom/ProfileData")]

public class ProfileData : ScriptableObject
{
    [System.Serializable]
    public class ProfileDataEntry
    {
        public string name;
        public Color color;
        public Sprite profileSprite;
    }

    public ProfileDataEntry[] profileDataEntry;

    public Color GetColorByName(string name)
    {
        foreach (ProfileDataEntry nameColor in profileDataEntry)
        {
            if (nameColor.name == name)
            {
                return nameColor.color;
            }
        }
        return Color.white;
    }

    public Sprite GetProfileSpriteByName(string name)
    {
        foreach (ProfileDataEntry nameColor in profileDataEntry)
        {
            if (nameColor.name == name)
            {
                return nameColor.profileSprite;
            }
        }
        return null;
    }
}
