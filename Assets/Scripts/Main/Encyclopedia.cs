using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using MyBox;

public class Encyclopedia : MonoBehaviour
{
    public static Encyclopedia inst;
    [Foldout("UI", true)]
    [SerializeField] ListUI tacticList;
    [SerializeField] ListUI areaList;
    [SerializeField] Slider viewSlider;
    List<Card> allTactics = new();
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
            tacticList.mainThing.gameObject.SetActive((int)value == 0);
            areaList.mainThing.gameObject.SetActive((int)value == 1);
        }
    }
    private void Start()
    {
        area.text = AutoTranslate.Area();
        tactic.text = AutoTranslate.Tactic();
        close.text = AutoTranslate.Close();

        for (int i = 0; i < GameFiles.inst.tacticFiles.Count; i++)
        {
            Card nextCard = Instantiate(tacticList.prefab).GetComponent<Card>();
            nextCard.AssignCard(GameFiles.inst.tacticFiles[i], 1f, true, Vector3.one);
            allTactics.Add(nextCard);
            nextCard.transform.SetParent(tacticList.storePrefabs.transform);
        }
        for (int i = 0; i < GameFiles.inst.areaFiles.Count; i++)
        {
            Card nextCard = Instantiate(areaList.prefab).GetComponent<Card>();
            nextCard.AssignCard(GameFiles.inst.areaFiles[i], 1f, false, Vector3.one);
            allAreas.Add(nextCard);
            nextCard.transform.SetParent(areaList.storePrefabs.transform);
        }
    }
}