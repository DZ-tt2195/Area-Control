using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverVisible : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool clicked = false;
    [SerializeField] GameObject thing;
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => clicked = !clicked);
        thing.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thing.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thing.SetActive(false);
    }
}
