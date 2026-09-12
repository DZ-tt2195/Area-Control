using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using TMPro;

public class CardMenu : MonoBehaviour
{
    public static CardMenu instance;
    [Foldout("UI", true)]
    [SerializeField] Button openCustomizer;
    [SerializeField] Transform customizerScreen;
    [SerializeField] Button confirmButton;
    [SerializeField] GridLayoutGroup storeVerticalButtons;
    [SerializeField] GridLayoutGroup storeHorizontalButtons;
    CardSelect mostRecentClick;
    List<(CardLayout, Button)> blankVerticalButtons = new();
    List<(CardLayout, Button)> blankHorizontalButtons = new();
    [SerializeField] List<CardSelect> cardSelectors = new();
    [Foldout("Text", true)]
    [SerializeField] TMP_Text chooseCards;
    [SerializeField] TMP_Text area1;
    [SerializeField] TMP_Text area2;
    [SerializeField] TMP_Text area3;
    [SerializeField] TMP_Text area4;
    [SerializeField] TMP_Text confirm;
    private void Start()
    {
        instance = this;
        storeVerticalButtons.gameObject.SetActive(false);
        storeHorizontalButtons.gameObject.SetActive(false);

        openCustomizer.onClick.AddListener(() => 
        {
            customizerScreen.gameObject.SetActive(true);
            AudioManager.instance.Menu();
        });
        confirmButton.onClick.AddListener(() => 
        {
            customizerScreen.gameObject.SetActive(false);
            AudioManager.instance.Menu();
            PlayerPrefs.Save();
        });

        for (int i = 0; i < storeHorizontalButtons.transform.childCount; i++)
        {
            Button nextButton = storeHorizontalButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
            blankHorizontalButtons.Add((nextButton.GetComponent<CardLayout>(), nextButton));
            nextButton.interactable = true;
            nextButton.onClick.RemoveAllListeners();
            int number = i;
            nextButton.onClick.AddListener(() => SendName(number));
        }
        for (int i = 0; i < storeVerticalButtons.transform.childCount; i++)
        {
            Button nextButton = storeVerticalButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
            blankVerticalButtons.Add((nextButton.GetComponent<CardLayout>(), nextButton));
            nextButton.interactable = true;
            nextButton.onClick.RemoveAllListeners();
            int number = i;
            nextButton.onClick.AddListener(() => SendName(number));
        }
        Translations();
    }
    public void ChooseFromList(CardSelect clicked, List<CardData> allData, bool vertical)
    {
        mostRecentClick = clicked;
        storeHorizontalButtons.gameObject.SetActive(!vertical);
        storeVerticalButtons.gameObject.SetActive(vertical);

        if (vertical)
        {
            for (int i = 0; i < blankVerticalButtons.Count; i++)
            {
                (CardLayout layout, Button button) = blankVerticalButtons[i];
                try
                {
                    layout.FillInCards(allData[i], 1, vertical);
                    button.gameObject.SetActive(true);
                }
                catch
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < blankHorizontalButtons.Count; i++)
            {
                (CardLayout layout, Button button) = blankHorizontalButtons[i];
                try
                {
                    layout.FillInCards(allData[i], 1, vertical);
                    button.gameObject.SetActive(true);
                }
                catch
                {
                    button.gameObject.SetActive(false);
                }
            }
            
        }
    }
    void SendName(int number)
    {
        CardData data = mostRecentClick.SetCardImage(number);
        foreach (CardSelect select in cardSelectors)
        {
            if (select != mostRecentClick && data != null && select.myData == data)
                select.SetCardImage(-1);
        }

        mostRecentClick = null;
        storeVerticalButtons.gameObject.SetActive(false);
        storeHorizontalButtons.gameObject.SetActive(false);        
    }
    void Translations()
    {
        openCustomizer.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Open_Customizer();
        chooseCards.text = AutoTranslate.Choose_Cards();
        area1.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Area_1());
        area2.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Area_2());
        area3.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Area_3());
        area4.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Area_4());
        confirm.text = AutoTranslate.Confirm();
    }
}
