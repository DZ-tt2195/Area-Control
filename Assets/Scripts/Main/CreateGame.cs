using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Photon.Realtime;
using System.Collections;
using MyBox;
using UnityEngine.UI;
using TMPro;
using System.Linq;
[Serializable]
public class TwistVisual
{
    public Card card;
    public TMP_Text countText;
    public TokenType token;
}
public class CreateGame : PhotonCompatible
{

#region Setup

    public static CreateGame inst;
    [Foldout("Players", true)]
    List<Player> listOfPlayers = new();
    [ReadOnly] public Player mainPlayer;
    [SerializeField] Player playerPrefab;
    [SerializeField] Card cardPrefab;
    [SerializeField] TMP_Dropdown playerDropdown;

    [Foldout("UI and Animation", true)]
    public Camera mainCamera;
    public float opacity { get; private set; }
    bool decrease = true;
    public Canvas canvas { get; private set; }
    [SerializeField] List<TwistVisual> twistInfo = new();
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text switchPlayer;
    [SerializeField] TMP_Text rules;
    [SerializeField] TMP_Text rulesSummary;
    [SerializeField] TMP_Text resignText;
    protected override void Awake()
    {
        base.Awake();
        this.bottomType = this.GetType();
        inst = this;
        Translations();
        PhotonNetwork.AutomaticallySyncScene = true;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    void Translations()
    {
        resignText.text = AutoTranslate.Resign();
        switchPlayer.text = AutoTranslate.Switch_Player();
        rules.text = AutoTranslate.Rules();
        rulesSummary.text = KeywordTooltip.instance.EditText(AutoTranslate.Rules_Summary());
    }
    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            string playerName = PlayerPrefs.GetString(ConstantStrings.MyUserName);

            if (PlayerPrefs.GetString(ConstantStrings.LastRoom).Equals(PhotonNetwork.CurrentRoom.Name))
            {
                CommHub.inst.ShareMessageRPC(OnlineTranslate.Online_Player_Reconnected(playerName), true);
            }
            else if ((bool)GetRoomProperty(ConstantStrings.JoinAsSpec))
            {
                CommHub.inst.ShareMessageRPC(OnlineTranslate.Online_Player_Spectating(playerName), true);
                ExitGames.Client.Photon.Hashtable playerProps = new()
                {
                    [ConstantStrings.Waiting] = true,
                    [ConstantStrings.Playing] = false,
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
                StartCoroutine(Wait());
            }
            else
            {
                CommHub.inst.ShareMessageRPC(OnlineTranslate.Online_Player_Playing(playerName), true);
                PlayerPrefs.SetString(ConstantStrings.LastRoom, PhotonNetwork.CurrentRoom.Name);
                PlayerPrefs.Save();
                StartCoroutine(MakePlayer());
                
                if (GetPlayers(false).Item1.Count == (int)GetRoomProperty(ConstantStrings.CanPlay))
                    InstantChangeRoomProp(ConstantStrings.JoinAsSpec, true, false);
            }
        }
        else
        {
            PlayerPrefs.DeleteKey(ConstantStrings.LastRoom);
            InstantChangeRoomProp(ConstantStrings.CanPlay, 1);
            StartCoroutine(MakePlayer());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(1.5f);
            RefreshUI(true);
        }

        IEnumerator MakePlayer()
        {
            yield return new WaitForSeconds(1f);
            while (CardMenu.instance.gameObject.activeSelf)
            {
                yield return null;
            }
            MakeObject(playerPrefab.gameObject);
        }
        VisualCards((int[])GetRoomProperty(ConstantStrings.TwistList));
        playerDropdown.onValueChanged.AddListener(SwitchToPlayer);        
    }
    #endregion

#region Online

    public void Leave()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.LeaveRoom(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        OnLeftRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("0. Loading");
    }

    #endregion

#region UI

    private void FixedUpdate()
    {
        if (decrease)
            opacity -= 0.05f;
        else
            opacity += 0.05f;
        if (opacity < 0 || opacity > 1)
            decrease = !decrease;
    }
    public void RefreshUI(bool forced)
    {
        Log.inst.ChangeScrolling();
        foreach (Player player in listOfPlayers)
            player.UpdateUI(forced);
    }
    public void SwitchToPlayer(Player player) => playerDropdown.value = listOfPlayers.IndexOf(player);
    public void SwitchToPlayer(int value)
    {
        for (int i = 0; i<listOfPlayers.Count; i++)
        {
            Player player = listOfPlayers[i];
            if (i == value)
            {
                player.transform.SetParent(canvas.transform);
                player.transform.SetAsFirstSibling();
                player.transform.localPosition = Vector3.zero;
                AudioManager.instance.Menu();
            }
            else
            {
                player.transform.SetParent(null);
            }
        }
    }
    public List<Player> GetPlayers() => listOfPlayers;
    public void AddPlayerRPC(Player player)
    {
        DoFunction(() => AddPlayer(player.photonView.ViewID), RpcTarget.AllBuffered);
    }
    [PunRPC]
    void AddPlayer(int playerID)
    {
        Player player = PhotonView.Find(playerID).GetComponent<Player>();
        if (listOfPlayers.Contains(player)) return;
        listOfPlayers.Add(player);

        playerDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new(player.name) });
        if (listOfPlayers.Count == (int)GetRoomProperty(ConstantStrings.CanPlay))
        {
            playerDropdown.gameObject.SetActive(playerDropdown.options.Count >= 2);
            int myPosition = GetThisPlayerPosition(PhotonNetwork.LocalPlayer);
            int index = listOfPlayers.IndexOf(mainPlayer);

            if (myPosition == -1 || index == 0)
                SwitchToPlayer(0);
            else
                playerDropdown.value = index;
        }
    }

#endregion 

#region  Twists
    public void CreateTwists()
    {
        List<int> TwistIDs = new();
        for (int i = 0; i<GameFiles.inst.twistFiles.Count; i++)
            TwistIDs.Add(i);
        TwistIDs = TwistIDs.Shuffle();

        int forcedTwists = 4;
        for (int i = 0; i<forcedTwists; i++)
        {
            int chosenNumber = PlayerPrefs.GetInt($"Twist {i}");
            if (chosenNumber >= 0 && TwistIDs.Remove(chosenNumber))
                TwistIDs.Insert(i, chosenNumber);
        }

        int[] chosenTwists = new int[forcedTwists];
        for (int i = 0; i<forcedTwists; i++)
        {
            chosenTwists[i] = TwistIDs[i];
            //Debug.Log(TwistIDs[i]);
        }
        InstantChangeRoomProp(ConstantStrings.TwistList, chosenTwists.ToArray());
    }
    void VisualCards(int[] cardIDs)
    {
        for (int i = 0; i<cardIDs.Length; i++)
        {
            twistInfo[i].card.gameObject.SetActive(true);
            CardData data = GameFiles.inst.twistFiles[cardIDs[i]];
            twistInfo[i].card.AssignCard(data, 1, false, new(0.5f, 0.5f, 0.5f));
        }
        for (int i = cardIDs.Length; i<twistInfo.Count; i++)
        {
            twistInfo[i].card.gameObject.SetActive(false);
        }
        UpdateTexts();
    }
    public TwistVisual GetTwist(TokenType type)
    {
        foreach (TwistVisual tv in twistInfo)
        {
            if (tv.token == type)
                return tv;
        }
        return null;
    }
    void UpdateTexts()
    {
        foreach (TwistVisual visual in twistInfo)
        {
            string tokenText = ConstantStrings.TokenCounter(visual.token);
            visual.countText.text = KeywordTooltip.instance.EditText($"{TurnManager.inst.GetInt(tokenText)}{Translator.inst.Translate(visual.token.ToString())}");
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(ConstantStrings.TwistList))
        {
            VisualCards((int[])propertiesThatChanged[ConstantStrings.TwistList]);
        }
        else
        {
            foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
            {
                string changedTokenText = ConstantStrings.TokenCounter(token);
                if (propertiesThatChanged.ContainsKey(changedTokenText))
                {
                    UpdateTexts();
                    return;
                }
            }   
        }
    }

    #endregion
}
