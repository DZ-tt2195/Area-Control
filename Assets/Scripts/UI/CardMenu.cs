using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using TMPro;

public class CardMenu : PhotonCompatible
{
    public static CardMenu instance;
    [Foldout("UI", true)]
    int step = 0;
    [SerializeField] Button confirmButton;
    [SerializeField] GridLayoutGroup storeVerticalButtons;
    [SerializeField] GridLayoutGroup storeHorizontalButtons;
    CardSelect mostRecentClick;
    List<(CardLayout, Button)> blankVerticalButtons = new();
    List<(CardLayout, Button)> blankHorizontalButtons = new();
    [SerializeField] List<CardSelect> cardSelectors = new();
    [Foldout("Text", true)]
    [SerializeField] TMP_Text chooseCards;
    [SerializeField] TMP_Text twistArt;
    [SerializeField] TMP_Text twistHouse;
    [SerializeField] TMP_Text twistTool;
    [SerializeField] TMP_Text twistBook;
    [SerializeField] TMP_Text confirm;

    protected override void Awake()
    {
        base.Awake();
        this.bottomType = this.GetType();
        instance = this;
    }
    private void Start()
    {
        string currentPhase = (string)GetRoomProperty(ConstantStrings.CurrentPhase);
        if (!(AmMaster() && currentPhase.Equals(nameof(WaitForJoiners))))
        {
            foreach (CardSelect select in cardSelectors)
                select.SetCardImage(-1);
            this.gameObject.SetActive(false);
        }
        else
        {
            Advance();
            confirmButton.onClick.AddListener(Advance);
        }
    }
    public void ChooseFromList(CardSelect clicked, List<CardData> allData, bool vertical)
    {
        mostRecentClick = clicked;
        storeHorizontalButtons.transform.parent.gameObject.SetActive(!vertical);
        storeVerticalButtons.transform.parent.gameObject.SetActive(vertical);

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
        mostRecentClick.SetCardImage(number);
        mostRecentClick = null;
        storeVerticalButtons.transform.parent.gameObject.SetActive(false);
        storeHorizontalButtons.transform.parent.gameObject.SetActive(false);
    }
    void Advance()
    {
        if (step == 0)
        {
            storeVerticalButtons.transform.parent.gameObject.SetActive(false);
            storeHorizontalButtons.transform.parent.gameObject.SetActive(false);
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
        else
        {
            PlayerPrefs.Save();
            this.gameObject.SetActive(false);
        }
        step++;
    }
    void Translations()
    {
        chooseCards.text = AutoTranslate.Choose_Twists();
        twistArt.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Art_Twist());
        twistHouse.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_House_Twist());
        twistTool.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Tool_Twist());
        twistBook.text = KeywordTooltip.instance.EditText(AutoTranslate.Custom_Book_Twist());
        confirm.text = AutoTranslate.Confirm();
    }
}
