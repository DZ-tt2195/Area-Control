using System.Text.RegularExpressions;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyBox;
[Serializable]
public class KeywordHover
{
    public string original;
    [ReadOnly] public string translated;
    [ReadOnly] public string description;
    public Color color = Color.white;
}
public class FoundMatch
{
    public int start;
    public int length;
    public string replacement;
}
public class KeywordTooltip : MonoBehaviour
{
    public static KeywordTooltip instance;
    float XCap;
    float Ydisplace;
    [SerializeField] List<KeywordHover> linkedKeywords = new();
    [SerializeField] List<KeywordHover> spriteKeywords = new();
    [SerializeField] TMP_Text tooltipText;
    Dictionary<string, (CardData, bool)> listOfCardRC = new();

    private void Awake()
    {
        instance = this;
        XCap = tooltipText.rectTransform.sizeDelta.x / 2f;
        Ydisplace = tooltipText.rectTransform.sizeDelta.y * 1.25f;
    }
    public void SwitchLanguage()
    {
        foreach (KeywordHover hover in linkedKeywords)
        {
            hover.translated = Translator.inst.Translate(hover.original);
            if (Translator.inst.TranslationExists($"{hover.original}_Text"))
                hover.description = Translator.inst.Translate($"{hover.original}_Text");
        }
        foreach (KeywordHover hover in spriteKeywords)
        {
            hover.translated = Translator.inst.Translate(hover.original);
            if (Translator.inst.TranslationExists($"{hover.original}_Text"))
                hover.description = Translator.inst.Translate($"{hover.original}_Text");
        }
        foreach (KeywordHover hover in linkedKeywords)
            hover.description = EditText(hover.description);
        foreach (KeywordHover hover in spriteKeywords)
            hover.description = EditText(hover.description);

        listOfCardRC.Clear();
        foreach (CardData data in GameFiles.inst.tacticFiles)
            listOfCardRC[Translator.inst.Translate(data.cardName)] = (data, true);
        foreach (CardData data in GameFiles.inst.areaFiles)
            listOfCardRC[Translator.inst.Translate(data.cardName)] = (data, false);
    }
    public string EditText(string textToEdit)
    {
        if (textToEdit.Length == 0)
            return "";

        List<FoundMatch> matches = new();
        foreach (KeywordHover link in linkedKeywords)
            FindMatch(link.translated, $"<link=\"{link.original}\"><u><color=#{ColorUtility.ToHtmlStringRGB(link.color)}>{link.translated}<color=#FFFFFF></u></link>");
        foreach (KeywordHover link in spriteKeywords)
            FindMatch(link.translated, $"<link=\"{link.original}\"><sprite=\"{link.original}\" name=\"{link.original}\"></link>");
        foreach (var next in listOfCardRC)
            FindMatch(next.Key, $"<link=\"{next.Key}\"><i>{next.Key}</i></link>");

        void FindMatch(string search, string replacement)
        {
            string pattern = $@"(?<![\p{{L}}\p{{N}}]){Regex.Escape(search)}(?![\p{{L}}\p{{N}}])";

            foreach (Match match in Regex.Matches(textToEdit, pattern))
            {
                matches.Add(new FoundMatch
                {
                    start = match.Index, length = match.Length,
                    replacement = replacement
                });
            }            
        }

        matches = matches.OrderByDescending(x => x.length).ToList();

        List<FoundMatch> accepted = new();
        foreach (FoundMatch match in matches)
        {
            bool overlaps = accepted.Any(other =>
                match.start < other.start + other.length &&
                match.start + match.length > other.start
            );
            if (!overlaps)accepted.Add(match);
        }
        accepted = accepted.OrderBy(x => x.start).ToList();

        StringBuilder result = new();
        int position = 0;

        foreach (FoundMatch match in accepted)
        {
            result.Append(textToEdit.Substring(position,match.start - position));
            result.Append(match.replacement);
            position = match.start + match.length;
        }
        result.Append(textToEdit.Substring(position));
        return result.ToString();
    }
    public KeywordHover SearchForKeyword(string target)
    {
        foreach (KeywordHover link in linkedKeywords)
        {
            if (link.translated.Equals(target))
                return link;
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            if (link.translated.Equals(target))
                return link;
        }
        Debug.LogError($"{target} couldn't be found");
        return null;
    }
    private void Update()
    {
        tooltipText.transform.parent.gameObject.SetActive(false);
    }
    Vector3 CalculatePosition(Vector3 mousePosition)
    {
        return new Vector3
            (Mathf.Clamp(mousePosition.x, XCap, Screen.width - XCap),
            mousePosition.y + (mousePosition.y > Ydisplace ? -0.5f : 0.5f) * Ydisplace,
            0);
    }
    public void ActivateTextBox(string target, Vector3 mousePosition)
    {
        this.transform.SetAsLastSibling();

        foreach (KeywordHover entry in linkedKeywords)
        {
            if (entry.original.Equals(target) && !entry.description.Equals(""))
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = CalculatePosition(mousePosition);
                tooltipText.transform.parent.gameObject.SetActive(true);
                return;
            }
        }
        foreach (KeywordHover entry in spriteKeywords)
        {
            if (entry.original.Equals(target) && !entry.description.Equals(""))
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = CalculatePosition(mousePosition);
                tooltipText.transform.parent.gameObject.SetActive(true);
                return;
            }
        }
    }
    public (CardData, bool) FindCardRC(string cardName)
    {
        return listOfCardRC[cardName];
    }
}