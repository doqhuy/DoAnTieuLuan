using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterDefaultData", menuName = "ScriptableObjects/CharacterDefaultData")]
public class CharacterDefaultData : ScriptableObject
{
    public List<Character> Characters;

    bool CheckCharacterExist(Character character)
    {
        if(Characters.Contains(character)) return true;
        else
        return false;
    }    

    public void SaveThisData()
    {
        string saveFolderPath = "";

        #if UNITY_ANDROID
        string rootPath = Application.persistentDataPath;
        saveFolderPath = Path.Combine(rootPath, "Save0");
        #else
        saveFolderPath = "Assets/Resources/Save0";
        #endif

        string SaveDefaultCharacter = JsonUtility.ToJson(this);
        File.WriteAllText(Path.Combine(saveFolderPath, "CharacterDefaultData.json"), SaveDefaultCharacter);
    }

    public void SetDefaultData(Character character)
    {

        string saveFolderPath = "";
#if UNITY_ANDROID
        string rootPath = Application.persistentDataPath;
        saveFolderPath = Path.Combine(rootPath, "Save0" );
#else
        saveFolderPath = "Assets/Resources/Save0";
#endif
        if (!CheckCharacterExist(character))
        {

            Characters.Add(character);
            string saveCharacter = JsonUtility.ToJson(character);
            File.WriteAllText(Path.Combine(saveFolderPath, "CharacterDefaultData/" + character.Name + ".json"), saveCharacter);
        }
        SaveThisData();
    }
    public void GetDefaultData()
    {
        string saveFolderPath = "";
        string rootPath = Application.persistentDataPath;
#if UNITY_ANDROID

        saveFolderPath = Path.Combine(rootPath, "Save0");
#else
        saveFolderPath = "Assets/Resources/Save0";
#endif

        for (int i = 0; i < Characters.Count; i++)
        {
            string saveQuest = File.ReadAllText(Path.Combine(saveFolderPath, "CharacterDefaultData/" + Characters[i].Name + ".json"));
            JsonUtility.FromJsonOverwrite(saveQuest, Characters[i]);
        }
    }


}
