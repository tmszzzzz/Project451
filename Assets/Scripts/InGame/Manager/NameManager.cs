using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    public static NameManager instance;
    public string filePath;
    public List<string> names;
    [System.Serializable]
    public class Names
    {
        public List<string> names;
    }

    private void Awake()
    {
        instance = this;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Names deserializedData = JsonUtility.FromJson<Names>(json);
            names = deserializedData.names;
        }
    }

    public string ConvertIdToName(int id)
    {
        if (names.Count > id)
        {
            return names[id];
        }

        return "佚名";
    }
    
    public string ConvertNodeNameToName(string name)
    {
        int id = int.Parse(name.Substring(5));
        if (names.Count > id)
        {
            return names[id];
        }

        return "佚名";
    }
}
