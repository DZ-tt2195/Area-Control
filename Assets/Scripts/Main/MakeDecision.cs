using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using MyBox;
using Photon.Pun;

public class CardButtonInfo
{
    public Card card {get; private set;}
    public Action<Card> action{get; private set;}
    public float alpha{get; private set;}
    public bool clickable{get; private set;}

    public CardButtonInfo(Card card, Action<Card> action = null, float alpha = 1f, bool clickable = true)
    {
        this.card = card;
        this.action = action;
        this.alpha = alpha;
        this.clickable = clickable;
    }
}

public class TextButtonInfo
{
    public string myText{get; private set;}
    public Color buttonColor{get; private set;}
    public Color textColor{get; private set;}
    public Action action{get; private set;}

    public TextButtonInfo(string myText, Action action = null)
    {
        this.myText = myText;
        this.buttonColor = Color.white;
        this.action = action;
        this.textColor = Color.black;
    }

    public TextButtonInfo(string myText, Color buttonColor, Color textColor, Action action = null)
    {
        this.myText = myText;
        this.action = action;
        this.buttonColor = buttonColor;
        this.textColor = textColor;
    }
}

public class MakeDecision : PhotonCompatible
{

#region Setup

    public static MakeDecision inst;
    [SerializeField] TMP_Text instructionsText;
    [SerializeField] Transform findTextButtons;
    [SerializeField] Transform findCardButtons;
    List<(ButtonSelect, TMP_Text)> textButtons = new();
    List<(ButtonSelect, CardLayout)> cardButtons = new();
    HashSet<ButtonSelect> availableUI = new();
    [SerializeField] ButtonSelect sliderConfirm;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text minimumText;
    [SerializeField] TMP_Text maximumText;
    [SerializeField] TMP_Text currentText;
    [SerializeField] TMP_Text confirmText;
    List<int> sliderNumbers = new();

    protected override void Awake()
    {
        base.Awake();
        inst = this;
        this.bottomType = this.GetType();
        instructionsText.text = "";
        confirmText.text = AutoTranslate.Confirm();
        slider.onValueChanged.AddListener(UpdateSliderText);

        slider.gameObject.SetActive(false);
        foreach (Transform child in findTextButtons)
        {
            textButtons.Add((child.GetComponent<ButtonSelect>(), child.transform.GetComponentInChildren<TMP_Text>()));
            child.gameObject.SetActive(false);
        }
        /*
        foreach (Transform child in findCardButtons)
        {
            cardButtons.Add((child.GetComponent<ButtonSelect>(), child.GetComponent<CardLayout>()));
            child.gameObject.SetActive(false);
        }*/

    }
    void UpdateSliderText(float value)
    {
        currentText.text = sliderNumbers[(int)value].ToString();
    }

    #endregion

#region Decisions

    public void ChooseTextButton(List<TextButtonInfo> possibleChoices, string instructions, bool autoResolve = true)
    {
        if (possibleChoices.Count == 1 && autoResolve && !PermaUI.inst.NeedClick())
        {
            Log.inst.inReaction.Add(() => possibleChoices[0].action?.Invoke());
        }
        else if (possibleChoices.Count >= 1)
        {
            Log.inst.SetUndoPoint(true);
            instructionsText.text = KeywordTooltip.instance.EditText(instructions);

            for (int i = 0; i<textButtons.Count; i++)
            {
                (ButtonSelect, TMP_Text) nextButton = textButtons[i];
                if (i < possibleChoices.Count)
                {
                    TextButtonInfo info = possibleChoices[i];
                    availableUI.Add(nextButton.Item1);
                    nextButton.Item1.gameObject.SetActive(true);
                    
                    nextButton.Item1.button.interactable = true;
                    nextButton.Item1.name = info.myText;
                    nextButton.Item1.button.onClick.AddListener(Resolve);
                    nextButton.Item1.button.image.color = info.buttonColor;

                    nextButton.Item2.text = KeywordTooltip.instance.EditText(info.myText);
                    nextButton.Item2.color = info.textColor;

                    void Resolve()
                    {
                        AudioManager.instance.Menu();
                        Log.inst.inReaction.Add(() => info.action?.Invoke());
                        Log.inst.PopStack();
                    }
                }
                else
                {
                    nextButton.Item1.gameObject.SetActive(false);
                }
            }
        }
    }
    public void ChooseCardOnScreen(List<Card> listOfCards, string instructions, Action<Card> action = null, bool autoResolve = true)
    {
        if (listOfCards.Count == 1 && autoResolve && !PermaUI.inst.NeedClick())
        {
            Log.inst.inReaction.Add(() => action?.Invoke(listOfCards[0]));
        }
        else if (listOfCards.Count >= 1)
        {
            Log.inst.SetUndoPoint(true);
            instructionsText.text = KeywordTooltip.instance.EditText(instructions);

            for (int j = 0; j < listOfCards.Count; j++)
            {
                Card nextCard = listOfCards[j];
                availableUI.Add(nextCard.selectMe);
                Button cardButton = nextCard.selectMe.button;

                cardButton.interactable = true;
                nextCard.selectMe.SetBorder(true);
                cardButton.onClick.AddListener(ClickedThis);

                void ClickedThis()
                {
                    AudioManager.instance.Menu();
                    Log.inst.inReaction.Add(() => action?.Invoke(nextCard));
                    Log.inst.PopStack();
                }
            }
        }
    }
    public void ChooseDisplayOnScreen(List<TokenDisplay> listOfDisplays, string instructions, Action<(int level, TokenType type)> action = null, bool autoResolve = true)
    {
        if (listOfDisplays.Count == 1 && autoResolve && !PermaUI.inst.NeedClick())
        {
            Log.inst.inReaction.Add(() => action?.Invoke(listOfDisplays[0].info));
        }
        else if (listOfDisplays.Count >= 1)
        {
            Log.inst.SetUndoPoint(true);
            instructionsText.text = KeywordTooltip.instance.EditText(instructions);

            for (int j = 0; j < listOfDisplays.Count; j++)
            {
                TokenDisplay nextCard = listOfDisplays[j];
                availableUI.Add(nextCard.selectMe);
                Button cardButton = nextCard.selectMe.button;

                cardButton.interactable = true;
                nextCard.selectMe.SetBorder(true);
                cardButton.onClick.AddListener(ClickedThis);

                void ClickedThis()
                {
                    AudioManager.instance.Menu();
                    Log.inst.inReaction.Add(() => action?.Invoke(nextCard.info));
                    Log.inst.PopStack();
                }
            }
        }
    }
    public void ChooseFromSlider(List<int> numbersInOrder, string instructions, Action<int> action = null, bool autoResolve = true)
    {
        if (numbersInOrder.Count == 1 && autoResolve)
        {
            Log.inst.inReaction.Add(() => action?.Invoke(numbersInOrder[0]));
        }
        else if (numbersInOrder.Count >= 1)
        {
            Log.inst.SetUndoPoint(true);
            instructionsText.text = KeywordTooltip.instance.EditText(instructions);
            sliderNumbers = numbersInOrder;

            slider.gameObject.SetActive(true);
            availableUI.Add(sliderConfirm);
            sliderConfirm.button.onClick.AddListener(DecisionMade);

            slider.minValue = 0;
            slider.maxValue = sliderNumbers.Count-1;
            slider.value = 0;
            UpdateSliderText(0);
        
            minimumText.text = sliderNumbers[0].ToString();
            maximumText.text = sliderNumbers[^1].ToString();

            void DecisionMade()
            {
                AudioManager.instance.Menu();
                Log.inst.inReaction.Add(() => action?.Invoke((int)slider.value));
                Log.inst.PopStack();
            }
        }
    }
    public void ChooseCardInPopup(List<CardButtonInfo> possibleCards, string instructions, bool autoResolve = true)
    {
        if (possibleCards.Count == 1 && autoResolve && !PermaUI.inst.NeedClick())
        {
            CardButtonInfo onlyOne = possibleCards[0];
            Log.inst.inReaction.Add(() => onlyOne.action?.Invoke(onlyOne.card));
        }
        else if (possibleCards.Count >= 1)
        {
            Log.inst.SetUndoPoint(true);
            instructionsText.text = KeywordTooltip.instance.EditText(instructions);
            
            for (int i = 0; i < cardButtons.Count; i++)
            {
                (ButtonSelect, CardLayout) nextButton = cardButtons[i];
                if (i < possibleCards.Count)
                {
                    CardButtonInfo info = possibleCards[i];
                    availableUI.Add(nextButton.Item1);
                    nextButton.Item1.gameObject.SetActive(true);
                    nextButton.Item1.SetBorder(true);

                    nextButton.Item1.name = possibleCards[i].card.name;
                    nextButton.Item1.button.onClick.AddListener(Resolve);
                    nextButton.Item1.button.interactable = true;
                    nextButton.Item2.FillInCards(info.card.dataFile, info.alpha, info.card.vertical);

                    void Resolve()
                    {
                        AudioManager.instance.Menu();
                        Log.inst.inReaction.Add(() => info.action?.Invoke(info.card));
                        Log.inst.PopStack();
                    }
                }
                else
                {
                    nextButton.Item1.gameObject.SetActive(false);
                }
            }
        }
    }

    #endregion

#region Misc

    public void ClearDecisions()
    {
        instructionsText.text = "";
        foreach (ButtonSelect select in availableUI)
        {
            select.button.onClick.RemoveAllListeners();
            select.button.interactable = false;
            select.SetBorder(false);
        }
        availableUI.Clear();

        slider.gameObject.SetActive(false);

        foreach (var next in cardButtons) next.Item1.gameObject.SetActive(false);
        foreach (var next in textButtons) next.Item1.gameObject.SetActive(false);
    }
    public static List<int> NumbersInOrder(int minimum, int maximum)
    {
        List<int> toReturn = new();
        for (int i = minimum; i<=maximum; i++)
            toReturn.Add(i);
        return toReturn;        
    }
    [PunRPC]
    public string PackagedInstructions(string packagedText)
    {
        string answer = Translator.inst.UnPackage(packagedText);
        instructionsText.text = answer;
        return answer;
    }

#endregion

}
