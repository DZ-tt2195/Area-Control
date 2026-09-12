using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CardSelect : MonoBehaviour
{
    [SerializeField] CardLayout layout;
    [SerializeField] Button randomButton;
    [SerializeField] Button chooseButton;
    [SerializeField] TMP_Text randomText;
    [SerializeField] TMP_Text chooseText;
    List<CardData> allData;
    [SerializeField] TypesOfCards myType; 
    bool vertical;
    public CardData myData {get; private set;}

    private void Awake()
    {
        vertical = false;
        switch (myType)
        {
            case TypesOfCards.Area:
                allData = GameFiles.inst.areaFiles;
                vertical = false;
                break;
            case TypesOfCards.Tactic:
                allData = GameFiles.inst.tacticFiles;
                vertical = true;
                break;
        }
        randomText.text = AutoTranslate.Random();
        chooseText.text = AutoTranslate.Choose();
        randomButton.onClick.AddListener(() => SetCardImage(-1));
        chooseButton.onClick.AddListener(() => CardMenu.instance.ChooseFromList(this, allData, vertical));
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey(this.name) && PlayerPrefs.GetInt(this.name) >= 0)
            SetCardImage(PlayerPrefs.GetInt(this.name));
        else
            SetCardImage(-1);
    }
    public CardData SetCardImage(int number)
    {
        if (number < 0)
        {
            PlayerPrefs.SetInt(this.name, -1);
            layout.FillInCards(null, 0, vertical);
            myData = null;
        }
        else
        {
            PlayerPrefs.SetInt(this.name, number);
            layout.FillInCards(allData[number], 1, vertical);
            myData = allData[number];
        }
        PlayerPrefs.Save();
        return myData;
    }
}
