using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class TroopScoutDisplay : MonoBehaviour
{
    public ButtonSelect selectMe { get; private set; }
    [SerializeField] TMP_Text description;
    public (int troops, int scouts) info {get; private set;}

    private void Awake()
    {
        selectMe = GetComponent<ButtonSelect>();
    }

    public void ChangeInfo(int troops, int scouts, string text)
    {
        description.text = text;
        info = (troops, scouts);
    }
}
