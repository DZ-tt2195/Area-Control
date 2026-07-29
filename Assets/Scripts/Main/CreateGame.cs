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
public class PlayerUI
{
    public GameObject parentObject;
    public TMP_Text playerText;
    public List<TroopScoutDisplay> listOfDisplays = new();
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
    [SerializeField] List<Card> listOfAreas = new();
    Dictionary<int, Card> areaDict = new();
    [SerializeField] List<PlayerUI> listOfPlayerUI = new();
    List<Player> whoControls = new();
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

        for (int i = 0; i<listOfAreas.Count; i++)
        {
            whoControls.Add(null);
            if (listOfAreas[i] != null)
                areaDict.Add(i, listOfAreas[i]);
        }
        foreach (PlayerUI ui in listOfPlayerUI)
            ui.parentObject.SetActive(false);
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
        playerDropdown.gameObject.SetActive((int)GetRoomProperty(ConstantStrings.CanPlay) >= 2);
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
        VisualCards((int[])GetRoomProperty(ConstantStrings.AreaList));
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
        if (forced)
            CalculateControllers();
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
        UpdatePlayerUI(player, player.name);

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

#region Displays
    public List<TroopScoutDisplay> GetAllDisplays(Player player)
    {
        int num = listOfPlayers.IndexOf(player);
        List<TroopScoutDisplay> allDisplays = new(listOfPlayerUI[num].listOfDisplays);
        allDisplays.RemoveAt(0);
        return allDisplays;
    }
    public void UpdatePlayerUI(Player player, string playerText)
    {
        int num = listOfPlayers.IndexOf(player);
        if (num == -1) return;

        PlayerUI ui = listOfPlayerUI[num];
        ui.parentObject.SetActive(true);
        ui.playerText.text = KeywordTooltip.instance.EditText(playerText);

        int[] troops = player.GetTroops();
        int[] scouts = player.GetScouts();

        for (int i = 1; i<=4; i++)
            ui.listOfDisplays[i].ChangeInfo(i, troops[i], scouts[i], $"{troops[i]} {AutoTranslate.TroopIcon()} {scouts[i]} {AutoTranslate.ScoutIcon()}");
    }
    public List<Player> CalculateControllers()
    {
        for (int i = 1; i<=4; i++)
        {
            (int highestNum, Player controller) best = (0, null);
            foreach (Player player in listOfPlayers)
            {
                int myNum = player.GetTroops()[i] + player.GetScouts()[i];
                if (myNum > best.highestNum)
                    best = (myNum, player);
                else if (myNum == best.highestNum)
                    best = (myNum, null);
            }
            whoControls[i] = best.controller;
            for (int j = 0; j<listOfPlayers.Count; j++)
            {
                if (listOfPlayers[j] == best.controller)
                    listOfPlayerUI[j].listOfDisplays[i].selectMe.button.image.color = Color.orange;
                else
                    listOfPlayerUI[j].listOfDisplays[i].selectMe.button.image.color = Color.gray;
            }
        }
        return whoControls;
    }
    public bool IsControlling(Player player, int area)
    {
        return whoControls[area] == player;        
    }
    public List<TroopScoutDisplay> AllControl(Player player, bool doControl)
    {
        return GetAllDisplays(player).Where(d => IsControlling(player, d.info.area) == doControl).ToList();
    }
#endregion 

#region Areas
    public void CreateAreas()
    {
        List<int> areaIDs = new();
        for (int i = 0; i<GameFiles.inst.areaFiles.Count; i++)
            areaIDs.Add(i);
        areaIDs = areaIDs.Shuffle();

        for (int i = 1; i<=4; i++)
        {
            int chosenNumber = PlayerPrefs.GetInt($"Area {i}");
            if (chosenNumber >= 0 && areaIDs.Remove(chosenNumber))
                areaIDs.Insert(i, chosenNumber);
        }

        int[] chosenAreas = new int[5];
        for (int i = 1; i<=4; i++)
        {
            chosenAreas[i] = areaIDs[i];
            //Debug.Log(TwistIDs[i]);
        }
        InstantChangeRoomProp(ConstantStrings.AreaList, chosenAreas.ToArray());
    }
    void VisualCards(int[] cardIDs)
    {
        if (cardIDs.Length == 0) return;
        for (int i = 0; i<cardIDs.Length; i++)
        {
            if (listOfAreas[i] != null)
            {
                listOfAreas[i].gameObject.SetActive(true);
                CardData data = GameFiles.inst.areaFiles[cardIDs[i]];
                listOfAreas[i].AssignCard(data, 1, false, Vector3.one);
            }
        }
        for (int i = cardIDs.Length; i<listOfAreas.Count; i++)
        {
            listOfAreas[i].gameObject.SetActive(false);
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(ConstantStrings.AreaList))
        {
            VisualCards((int[])propertiesThatChanged[ConstantStrings.AreaList]);
        }
    }
    public Card GetArea(int num) => areaDict[num];

    #endregion

}