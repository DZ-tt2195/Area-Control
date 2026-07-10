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
    [SerializeField] Card customerPrefab;
    [SerializeField] Card twistPrefab;
    [SerializeField] RectTransform customerView;
    [SerializeField] GridLayoutGroup customerGrid;
    [SerializeField] RectTransform twistView;
    [SerializeField] GridLayoutGroup twistGrid;
    [SerializeField] Slider viewSlider;
    List<Card> allCustomers = new();
    List<Card> allTwists = new();
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text customer;
    [SerializeField] TMP_Text twist;
    [SerializeField] TMP_Text close;

    private void Awake()
    {
        inst = this;
        viewSlider.onValueChanged.AddListener(Change);
        Change(0);

        void Change(float value)
        {
            customerView.gameObject.SetActive((int)value == 0);
            twistView.gameObject.SetActive((int)value == 1);
        }
    }
    private void Start()
    {
        Translations();
        for (int i = 0; i < GameFiles.inst.customerFiles.Count; i++)
        {
            GameObject nextCard = Instantiate(customerPrefab.gameObject);
            Card cardPV = nextCard.GetComponent<Card>();
            cardPV.AssignCard(GameFiles.inst.customerFiles[i], 1f, true, Vector3.one);
            allCustomers.Add(cardPV);
            cardPV.transform.SetParent(customerGrid.transform);
        }
        for (int i = 0; i < GameFiles.inst.twistFiles.Count; i++)
        {
            GameObject nextCard = Instantiate(twistPrefab.gameObject);
            Card cardPV = nextCard.GetComponent<Card>();
            cardPV.AssignCard(GameFiles.inst.twistFiles[i], 1f, false, Vector3.one);
            allTwists.Add(cardPV);
            cardPV.transform.SetParent(twistGrid.transform);
        }
    }
    void Translations()
    {
        customer.text = AutoTranslate.Customer();
        twist.text = AutoTranslate.Twist();
        close.text = AutoTranslate.Close();
    }
}
