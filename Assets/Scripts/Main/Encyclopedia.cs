using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using MyBox;

public class Encyclopedia : MonoBehaviour
{
    public static Encyclopedia inst;
    [Foldout("UI", true)]
    [SerializeField] Card tacticPrefab;
    [SerializeField] Card areaPrefab;
    [SerializeField] RectTransform tacticView;
    [SerializeField] GridLayoutGroup tacticGrid;
    [SerializeField] RectTransform areaView;
    [SerializeField] GridLayoutGroup areaGrid;
    [SerializeField] Slider viewSlider;
    List<Card> allCards = new();
    List<Card> allAreas = new();
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text tactic;
    [SerializeField] TMP_Text area;
    [SerializeField] TMP_Text close;

    private void Awake()
    {
        inst = this;
        viewSlider.onValueChanged.AddListener(Change);
        Change(0);

        void Change(float value)
        {
            tacticView.gameObject.SetActive((int)value == 0);
            areaView.gameObject.SetActive((int)value == 1);
        }
    }
    private void Start()
    {
        Translations();
        for (int i = 0; i < GameFiles.inst.tacticFiles.Count; i++)
        {
            GameObject nextCard = Instantiate(tacticPrefab.gameObject);
            Card cardPV = nextCard.GetComponent<Card>();
            cardPV.AssignCard(GameFiles.inst.tacticFiles[i], 1f, true, Vector3.one);
            allCards.Add(cardPV);
            cardPV.transform.SetParent(tacticGrid.transform);
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
        tactic.text = AutoTranslate.Tactic();
        close.text = AutoTranslate.Close();
    }
}
