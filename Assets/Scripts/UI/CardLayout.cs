using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardLayout : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CanvasGroup cg;
    [SerializeField] Image cardArt;
    [SerializeField] Image cardBackground;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text textBox;
    CardData storedData;
    bool vertical;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            PermaUI.inst.RightClickDisplay(storedData, GetAlpha() == 1f, vertical);
    }
    public float GetAlpha() => cg.alpha;
    public void FillInCards(CardData dataFile, float alpha, bool vertical)
    {
        bool newCard = storedData != dataFile;
        storedData = dataFile;
        cg.alpha = alpha;
        this.vertical = vertical;

        if (dataFile != null && newCard)
        {
            if (dataFile.coinAmount >= 1)
                cardName.text = KeywordTooltip.instance.EditText($"{Translator.inst.Translate(dataFile.cardName)}: {AutoTranslate.Coin_Amount(dataFile.coinAmount.ToString())}");
            else
                cardName.text = KeywordTooltip.instance.EditText($"{Translator.inst.Translate(dataFile.cardName)}");
            
            cardArt.sprite = dataFile.sprite;
            textBox.text = KeywordTooltip.instance.EditText(Translator.inst.Translate($"{dataFile.cardName}_Text"));
        }
    }
}
