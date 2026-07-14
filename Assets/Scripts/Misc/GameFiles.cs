using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public enum TypesOfCards {Tactic, Area}
[Serializable]
public class CardData
{
    public string cardName;
    public int coinCost = 0;
    public int troopAdvance = 0;
    public string artCredit;
    public Sprite sprite;
}

public class GameFiles : MonoBehaviour
{
    public static GameFiles inst;
    [SerializeField] TextAsset tactics;
    [SerializeField] TextAsset areas;
    [SerializeField] List<Sprite> cardArt;
    Dictionary<string, Sprite> cardArtDictionary = new();
    public List<CardData> tacticFiles { get; private set; }
    public List<CardData> areaFiles { get; private set; }

    void Awake()
    {
        inst = this;
        foreach (Sprite sprite in cardArt)
            cardArtDictionary.Add(sprite.name, sprite);

        tacticFiles = ReadCardFile<CardData>(tactics.text);
        areaFiles = ReadCardFile<CardData>(areas.text);
    }
    List<T> ReadCardFile<T>(string textToConvert) where T : new()
    {
        string[] splitUp = textToConvert.Split('\n');
        Dictionary<string, int> columnIndex = new();

        string[] headers = splitUp[0].Split('\t');
        for (int i = 0; i<headers.Length; i++)
            columnIndex[headers[i].Trim()] = i;

        List<T> toReturn = new();
        for (int i = 1; i<splitUp.Length; i++)
        {
            T nextData = new();
            toReturn.Add(nextData);
            string[] thisRow = splitUp[i].Split('\t');

            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (columnIndex.TryGetValue(field.Name, out int index))
                {
                    string sheetValue = thisRow[index].Trim();
                    if (field.FieldType == typeof(int))
                        field.SetValue(nextData, StringToInt(sheetValue));
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(nextData, StringToBool(sheetValue));
                    else if (field.FieldType == typeof(string))
                        field.SetValue(nextData, sheetValue);

                    int StringToInt(string line)
                    {
                        try
                        {
                            return (line.Equals("")) ? -1 : int.Parse(line);
                        }
                        catch (FormatException)
                        {
                            return 0;
                        }
                    }

                    bool StringToBool(string line)
                    {
                        return line.Equals("TRUE");
                    }
                }
                else
                {
                    if (field.FieldType == typeof(Sprite))
                    {
                        try
                        {
                            field.SetValue(nextData, cardArtDictionary[thisRow[columnIndex["cardName"]]]);
                        }
                        catch
                        {
                            Debug.LogError($"no art for {thisRow[columnIndex["cardName"]]}");
                        }
                    }
                }
            }

        }
        return toReturn;
    }
}
