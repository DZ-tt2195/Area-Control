using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class TroopScoutDisplay : MonoBehaviour
{
    public ButtonSelect selectMe { get; private set; }
    [SerializeField] TMP_Text description;
    public (int area, int troops, int scouts) info {get; private set;}

    private void Awake()
    {
        selectMe = GetComponent<ButtonSelect>();
    }

    public void ChangeInfo(int area, int troops, int scouts, string text)
    {
        description.text = KeywordTooltip.instance.EditText(text);
        info = (area, troops, scouts);
    }
}
