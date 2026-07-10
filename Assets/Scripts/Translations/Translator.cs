using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;
using System;
using System.Reflection;
using Photon.Pun;
using TMPro;

public class Translator : PhotonCompatible
{

#region Setup

    public static Translator inst;
    Dictionary<string, Dictionary<string, string>> keyTranslate = new();
    [Scene][SerializeField] string toLoad;
    [SerializeField] List<TextAsset> allLanguageFiles = new();
    [SerializeField] TMP_Text volume;
    [SerializeField] TMP_Text pauseSetting;
    [SerializeField] TMP_Text undoSetting;
    [SerializeField] TMP_Text clickSetting;

    protected override void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        foreach (TextAsset language in allLanguageFiles)
        {
            string fileName = ConvertName(language);

            string ConvertName(TextAsset asset)
            {
                //pattern: "0. English"
                string pattern = @"^\d+\.\s*(.+)$";
                Match match = Regex.Match(asset.name, pattern);
                if (match.Success)
                    return match.Groups[1].Value;
                else
                    return asset.name;
            }

            Dictionary<string, string> newDictionary = ReadLanguageFile(language.text);
            keyTranslate.Add(fileName, newDictionary);
        }

        if (!PlayerPrefs.HasKey("English") || !keyTranslate.ContainsKey(PlayerPrefs.GetString("Language")))
            PlayerPrefs.SetString("Language", "English");
        TranslateScreen();
    }

    public static Dictionary<string, string> ReadLanguageFile(string textToConvert)
    {
        string[] splitUp = textToConvert.Split('\n');
        Dictionary<string, string> toReturn = new();

        foreach (string line in splitUp)
        {
            int index = line.IndexOf('\t');
            string partOne = line[..index].Trim();
            string partTwo = /*partOne.Equals("Blank") ? "" :*/ line[(index + 1)..].Trim();
            toReturn.Add(partOne, partTwo);
        }
        return toReturn;
    }

    #endregion

#region Helpers

    public bool TranslationExists(string key) => keyTranslate["English"].ContainsKey(key);
    public string Translate(string key, List<(string, string)> toReplace = null)
    {
        string answer;
        try
        {
            answer = keyTranslate[PlayerPrefs.GetString("Language")][key];
        }
        catch
        {
            try
            {
                //Debug.Log($"{key} failed to translate in {PlayerPrefs.GetString("Language")}");
                answer = keyTranslate[("English")][key];
            }
            catch
            {
                //Debug.Log($"{key} failed to translate at all");
                return key;
            }
        }

        if (toReplace != null)
        {
            foreach ((string one, string two) in toReplace)
                answer = answer.Replace($"${one}$", Translate(two));
        }
        return answer;
    }
    public Dictionary<string, Dictionary<string, string>> GetTranslations()
    {
        return keyTranslate;
    }
    public void ChangeLanguage(string newLanguage, Dictionary<string, string> addedTranslation)
    {
        if (addedTranslation != null)
        {
            keyTranslate.Add(newLanguage, addedTranslation);
        }
        if (!PlayerPrefs.GetString("Language").Equals(newLanguage))
        {
            PlayerPrefs.SetString("Language", newLanguage);
            TranslateScreen();
        }
    }
    void TranslateScreen()
    {
        KeywordTooltip.instance.SwitchLanguage();
        volume.text = AutoTranslate.Volume();
        pauseSetting.text = AutoTranslate.Pause_Setting();
        undoSetting.text = AutoTranslate.Undo_Setting();
        clickSetting.text = AutoTranslate.Click_Setting();
        SceneManager.LoadScene(toLoad);        
    }
    public string UnPackage(string toSplit, int owner = -1)
    {
        string targetText;
        string[] splitUp = toSplit.Split('\t');

        int myPosition = GetThisPlayerPosition(PhotonNetwork.LocalPlayer);
        if (TranslationExists($"{splitUp[0]}_Others") && myPosition >= 0 && myPosition != owner)
            targetText = $"{splitUp[0]}_Others";
        else
            targetText = splitUp[0];

        List<(string, string)> toReplace = new();
        for (int i = 1; i<splitUp.Length; i+=2)
            toReplace.Add((splitUp[i], splitUp[i+1]));

        string translated = Translate(targetText, toReplace);
        return KeywordTooltip.instance.EditText(translated);
    }

#endregion

}
