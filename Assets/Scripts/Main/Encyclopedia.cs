using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using MyBox;
using Photon.Pun;

public class Encyclopedia : MonoBehaviour
{
    public static Encyclopedia inst;
    [Foldout("UI", true)]
    [SerializeField] Card cardPrefab;
    [SerializeField] Card areaPrefab;
    [SerializeField] RectTransform cardView;
    [SerializeField] GridLayoutGroup cardGrid;
    [SerializeField] RectTransform areaView;
    [SerializeField] GridLayoutGroup areaGrid;
    [SerializeField] Slider viewSlider;
    List<Card> allCards = new();
    List<Card> allAreas = new();
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text card;
    [SerializeField] TMP_Text area;
    [SerializeField] TMP_Text close;

    private void Awake()
    {
        inst = this;
        viewSlider.onValueChanged.AddListener(Change);
        Change(0);

        void Change(float value)
        {
            cardView.gameObject.SetActive((int)value == 0);
            areaView.gameObject.SetActive((int)value == 1);
        }
    }
    private void Start()
    {
        Translations();
        for (int i = 0; i < GameFiles.inst.cardFiles.Count; i++)
        {
            GameObject nextCard = Instantiate(cardPrefab.gameObject);
            Card cardPV = nextCard.GetComponent<Card>();
            cardPV.AssignCard(GameFiles.inst.cardFiles[i], 1f, true, Vector3.one);
            allCards.Add(cardPV);
            cardPV.transform.SetParent(cardGrid.transform);
        }
        for (int i = 0; i < GameFiles.inst.areaFiles.Count; i++)
        {
            GameObject nextCard = Instantiate(areaPrefab.gameObject);
            Card cardPV = nextCard.GetComponent<Card>();
            cardPV.AssignCard(GameFiles.inst.areaFiles[i], 1f, false, Vector3.one);
            allAreas.Add(cardPV);
            cardPV.transform.SetParent(areaGrid.transform);
        }
    }
    void Translations()
    {
        area.text = AutoTranslate.Area();
        card.text = AutoTranslate.Card();
        close.text = AutoTranslate.Close();
    }
}
