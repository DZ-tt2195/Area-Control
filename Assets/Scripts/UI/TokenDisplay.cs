using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using MyBox;

public enum TokenType { ArtIcon, HouseIcon, ToolIcon, BookIcon}
public class TokenDisplay : MonoBehaviour
{
    public ButtonSelect selectMe { get; private set; }
    [SerializeField] TMP_Text description;
    public (int level, TokenType type) info {get; private set;}

    private void Awake()
    {
        selectMe = GetComponent<ButtonSelect>();
    }

    public void ChangeInfo(int level, TokenType token, string text)
    {
        description.text = text;
        info = (level, token);
    }
}
