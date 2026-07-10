using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;
using TMPro;

public class PermaUI : MonoBehaviour
{

#region Setup

    public static PermaUI inst;
    [SerializeField] Transform permanentCanvas;

    [Foldout("Right click", true)]
    [SerializeField] Transform rightClickBackground;
    [SerializeField] CardLayout rightClickVertical;
    [SerializeField] CardLayout rightClickHorizontal;
    [SerializeField] TMP_Text artistCredit;
    
    [Foldout("Settings", true)]
    [SerializeField] Button settingsButton;
    [SerializeField] Transform settingsBackground;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle pauseToggle; public bool PauseToRead() => pauseToggle.isOn;
    [SerializeField] Toggle undoToggle; public bool PauseToUndo() => undoToggle.isOn;
    [SerializeField] Toggle clickToggle; public bool NeedClick() => clickToggle.isOn;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(this.gameObject);

            rightClickBackground.gameObject.SetActive(false);
            settingsBackground.gameObject.SetActive(false);

            settingsButton.onClick.AddListener(ToggleSettings);
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            volumeSlider.onValueChanged.AddListener(SetLevel);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        undoToggle.onValueChanged.AddListener(SetUndo);
        SetUndo(!PlayerPrefs.HasKey("Undo") || PlayerPrefs.GetInt("Undo") == 1);
          
        pauseToggle.onValueChanged.AddListener(SetPause);
        SetPause(!PlayerPrefs.HasKey("Pause") || PlayerPrefs.GetInt("Pause") == 1);
           
        clickToggle.onValueChanged.AddListener(SetClick);
        SetClick(!PlayerPrefs.HasKey("Click") || PlayerPrefs.GetInt("Click") == 1);  
        SetLevel(PlayerPrefs.GetFloat("Volume"));
    }

    public string PrintIntList(List<int> listOfInts)
    {
        string answer = "";
        foreach (int next in listOfInts)
            answer += $"{next}, ";
        return answer;
    }

    #endregion

#region Right click

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            rightClickBackground.gameObject.SetActive(false);
    }

    public void RightClickDisplay(CardData dataFile, bool visible, bool vertical)
    {
        AudioManager.instance.Menu();
        rightClickBackground.gameObject.SetActive(true);
        rightClickVertical.gameObject.SetActive(vertical);
        rightClickHorizontal.gameObject.SetActive(!vertical);

        if (vertical)
            rightClickVertical.FillInCards(dataFile, visible ? 1f : 0f, vertical);
        else
            rightClickHorizontal.FillInCards(dataFile, visible ? 1f : 0f, vertical);

        if (visible)
            artistCredit.text = dataFile.artCredit;
        else
            artistCredit.text = "";
    }

    #endregion

#region Settings

    void ToggleSettings()
    {
        AudioManager.instance.Menu();
        settingsBackground.gameObject.SetActive(!settingsBackground.gameObject.activeSelf);
    }
    void SetLevel(float value)
    {
        AudioManager.instance.mixer.SetFloat("Volume", (Mathf.Log10(volumeSlider.value) * 20));
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }
    void SetUndo(bool value)
    {
        AudioManager.instance.Menu();
        undoToggle.isOn = value;
        PlayerPrefs.SetInt("Undo", value ? 1 : 0);
        PlayerPrefs.Save();        
    }
    void SetClick(bool value)
    {
        AudioManager.instance.Menu();
        clickToggle.isOn = value;
        PlayerPrefs.SetInt("Click", value ? 1 : 0);
        PlayerPrefs.Save();        
    }
    void SetPause(bool value)
    {
        AudioManager.instance.Menu();
        pauseToggle.isOn = value;
        PlayerPrefs.SetInt("Pause", value ? 1 : 0);
        PlayerPrefs.Save();        
    }

#endregion

}
