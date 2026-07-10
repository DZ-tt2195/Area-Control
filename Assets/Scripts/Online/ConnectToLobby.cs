using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Photon.Realtime;
using UnityEngine.UI;
using MyBox;

public class ConnectToLobby : MonoBehaviourPunCallbacks
{

#region Setup

    [Foldout("General", true)]
    [SerializeField] TMP_Text error;

    [Foldout("Part 1", true)]
    [SerializeField] Transform part1;
    [SerializeField] TMP_InputField username;
    [SerializeField] Button reconnectButton;
    [SerializeField] TMP_Dropdown regionDropdown;
    [SerializeField] Button soundCreditsButton;
    [SerializeField] Transform soundCreditsObject;
    List<(string, string)> regionAndCode = new();

    [Foldout("Part 2", true)]
    [SerializeField] Transform part2;
    [SerializeField] Transform keepJoinButtons;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] Button joinManually;
    [SerializeField] Button disconnectButton;
    List<JoinRoomButton> listOfJoinButtons = new();
    [SerializeField] Slider playerSlider;
    [SerializeField] TMP_Text currentText;
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text enterUsename;
    [SerializeField] TMP_Text singlePlayer;
    [SerializeField] TMP_Text encyclopedia;
    [SerializeField] TMP_Text reconnect;
    [SerializeField] TMP_Text connect;
    [SerializeField] TMP_Text lastUpdate;
    [SerializeField] TMP_Text selectRegion;
    [SerializeField] TMP_Text author;
    [SerializeField] TMP_Text tutorial1;
    [SerializeField] TMP_Text tutorial2;
    [SerializeField] TMP_Text disconnect;
    [SerializeField] TMP_Text createRoomWithPlayers;
    [SerializeField] TMP_Text enterHostname;
    [SerializeField] TMP_Text join;
    [SerializeField] TMP_Text soundCredits;

    private void Start()
    {
        Translations();
        part2.gameObject.SetActive(true);
        joinManually.onClick.AddListener(() => JoinRoom(joinInput.text));
        disconnectButton.onClick.AddListener(() => PhotonNetwork.Disconnect());
        playerSlider.onValueChanged.AddListener(UpdateText);
        void UpdateText(float value)
        {
            currentText.text = KeywordTooltip.instance.EditText($"{(int)value}");
        }

        foreach (Transform child in keepJoinButtons)
            listOfJoinButtons.Add(child.GetComponent<JoinRoomButton>());
        foreach (JoinRoomButton button in listOfJoinButtons)
            button.ClearInfo();

        username.text = PlayerPrefs.GetString(ConstantStrings.MyUserName);
        error.gameObject.SetActive(false);
        part1.gameObject.SetActive(true);
        part2.gameObject.SetActive(false);
        reconnectButton.gameObject.SetActive(PlayerPrefs.HasKey(ConstantStrings.LastRoom));

        regionAndCode = new()
        {
            (AutoTranslate.US_West_Coast(), "usw"),
            (AutoTranslate.US_East_Coast(), "us"),
            (AutoTranslate.Europe(), "eu"),
            (AutoTranslate.Asia(), "asia")
        };
        foreach ((string, string) var in regionAndCode)
            regionDropdown.AddOptions(new List<string>() { var.Item1 });

        soundCreditsObject.gameObject.SetActive(false);
        soundCreditsButton.onClick.AddListener(SoundCreditsButton);
        void SoundCreditsButton()
        {
            AudioManager.instance.Menu();
            soundCreditsObject.gameObject.SetActive(!soundCreditsObject.gameObject.activeSelf);
        }
    }
    void Translations()
    {
        enterUsename.text = AutoTranslate.Enter_username();
        singlePlayer.text = AutoTranslate.Single_Player();
        encyclopedia.text = AutoTranslate.Encyclopedia();
        reconnect.text = AutoTranslate.Reconnect();
        connect.text = AutoTranslate.Connect();
        lastUpdate.text = AutoTranslate.Last_Update();
        selectRegion.text = AutoTranslate.Select_Region();
        author.text = AutoTranslate.Game_Designer();
        tutorial1.text = AutoTranslate.Tutorial_1();
        tutorial2.text = AutoTranslate.Tutorial_2();
        disconnect.text = AutoTranslate.Disconnect();
        createRoomWithPlayers.text = AutoTranslate.Create_Room_with_players();
        enterHostname.text = AutoTranslate.Enter_hostname();
        join.text = AutoTranslate.Join();
        soundCredits.text = AutoTranslate.Sound_Credits();
    }
    IEnumerator ErrorMessage(string text)
    {
        AudioManager.instance.Menu();
        error.text = text;
        float elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            error.gameObject.SetActive(true);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        error.gameObject.SetActive(false);
    }

    #endregion

#region Part 1

    bool CheckUsername()
    {
        string newName = username.text.Trim();
        if (newName == "")
        {
            StartCoroutine(ErrorMessage(AutoTranslate.Type_in_username()));
            return false;
        }
        else
        {
            PlayerPrefs.SetString(ConstantStrings.MyUserName, newName);
            PlayerPrefs.Save();
            PhotonNetwork.NickName = PlayerPrefs.GetString(ConstantStrings.MyUserName);
            return true;
        }
    }

    public void Join()
    {
        if (CheckUsername())
        {
            foreach ((string, string) var in regionAndCode)
            {
                if (var.Item1.Equals(regionDropdown.options[regionDropdown.value].text))
                    PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = var.Item2;
            }
            AudioManager.instance.Menu();
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OfflineRoom()
    {
        if (CheckUsername())
        { 
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(InitialPlayerProps());

            RoomOptions options = new()
            {
                MaxPlayers = 1,
                PlayerTtl = 0,
                EmptyRoomTtl = 0,
                CustomRoomProperties = InitialRoomProps(1),
            };
            PhotonNetwork.CreateRoom(PlayerPrefs.GetString(ConstantStrings.MyUserName), options);
            AudioManager.instance.Menu();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PlayerPrefs.DeleteKey(ConstantStrings.LastRoom);
        PhotonNetwork.OfflineMode = false;

        error.gameObject.SetActive(false);
        part1.gameObject.SetActive(false);
        part2.gameObject.SetActive(true);
    }

    public void Reconnect()
    {
        AudioManager.instance.Menu();
        StartCoroutine(ErrorMessage(AutoTranslate.Attempt_to_reconnect(PlayerPrefs.GetString(ConstantStrings.LastRoom))));
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(1.5f);
            bool tryReconnect = PhotonNetwork.ReconnectAndRejoin();

            if (!tryReconnect)
                StartCoroutine(ErrorMessage(AutoTranslate.Failed_to_reconnect(PlayerPrefs.GetString(ConstantStrings.LastRoom))));
        }
    }

    #endregion

#region Part 2

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (JoinRoomButton button in listOfJoinButtons)
            button.ClearInfo();

        int counter = 0;
        foreach (RoomInfo room in roomList)
        {
            if (room.CustomProperties.ContainsKey(ConstantStrings.GameName)
                && room.CustomProperties.ContainsKey(ConstantStrings.CanPlay)
                && room.CustomProperties.ContainsKey(ConstantStrings.JoinAsSpec)
                && room.CustomProperties.ContainsKey(ConstantStrings.GameOver))
            {
                if (room.CustomProperties[ConstantStrings.GameName].Equals(Application.productName)
                    && room.PlayerCount < room.MaxPlayers && room.MaxPlayers >= 2 && room.IsVisible
                    && counter < listOfJoinButtons.Count && !(bool)room.CustomProperties[ConstantStrings.GameOver])
                {
                    JoinRoomButton nextJoin = listOfJoinButtons[counter];
                    nextJoin.transform.SetParent(keepJoinButtons);
                    nextJoin.button.onClick.AddListener(() => JoinRoom(room.Name));
                    nextJoin.button.image.color = ((bool)room.CustomProperties[ConstantStrings.JoinAsSpec]) ? Color.yellow : Color.white;

                    nextJoin.thisName.text = room.Name;
                    nextJoin.playerCount.text = AutoTranslate.Player_Count(room.PlayerCount.ToString(), $"{(int)room.CustomProperties[ConstantStrings.CanPlay]}");
                    counter++;
                }
            }
        }
    }
    public void CreateRoom()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(InitialPlayerProps());
        AudioManager.instance.Menu();
        RoomOptions options = new()
        {
            MaxPlayers = 10,
            PlayerTtl = Application.isEditor ? 15000 : 120000,
            EmptyRoomTtl = 10000,
            CustomRoomProperties = InitialRoomProps((int)playerSlider.value),
            CustomRoomPropertiesForLobby = new string[] { ConstantStrings.GameName, ConstantStrings.CanPlay, ConstantStrings.JoinAsSpec, ConstantStrings.GameOver }
        };
        PhotonNetwork.CreateRoom(PlayerPrefs.GetString(ConstantStrings.MyUserName), options);
    }
    ExitGames.Client.Photon.Hashtable InitialRoomProps(int numPlayers)
    {
        Debug.Log("assigned room props");
        ExitGames.Client.Photon.Hashtable roomProps = new()
        {
            { ConstantStrings.GameName, Application.productName },
            { ConstantStrings.CurrentPhase, nameof(WaitForJoiners) },
            { ConstantStrings.NextPhase, nameof(DisplayTwists) },
            { ConstantStrings.CanPlay, numPlayers },
            { ConstantStrings.JoinAsSpec, false },
            { ConstantStrings.GameOver, false },
            { ConstantStrings.TwistList, new int[0]},
            { ConstantStrings.TokenCounter(TokenType.ArtIcon), 2},
            { ConstantStrings.TokenCounter(TokenType.HouseIcon), 2},
            { ConstantStrings.TokenCounter(TokenType.ToolIcon), 2},
            { ConstantStrings.TokenCounter(TokenType.BookIcon), 2},
            { ConstantStrings.TurnNumber, 1 },
            { ConstantStrings.MaxLevel, 6},
            { ConstantStrings.MasterDeck, new int[0]},
            { ConstantStrings.MasterDiscard, new int[0]}
        };
        return roomProps;
    }
    ExitGames.Client.Photon.Hashtable InitialPlayerProps()
    {
        Debug.Log("assigned player props");
        int numRanks = 6+1; //index 0 is ignored in the code

        ExitGames.Client.Photon.Hashtable playerProps = new()
        {
            [ConstantStrings.Playing] = true,
            [ConstantStrings.Waiting] = false,

            [ConstantStrings.MyCoins] = 0,
            [TokenType.ArtIcon.ToString()] = new int[numRanks],
            [TokenType.HouseIcon.ToString()] = new int[numRanks],
            [TokenType.ToolIcon.ToString()] = new int[numRanks],
            [TokenType.BookIcon.ToString()] = new int[numRanks],

            [ConstantStrings.MyHand] = new int[0],
            [ConstantStrings.MyDeck] = new int[0],
            [ConstantStrings.MyDiscard] = new int[0],
            [ConstantStrings.NeedDraw] = 0,
            [ConstantStrings.DrewThisTurn] = new int[0],
     
            [ConstantStrings.ChosenToken] = "",
            [ConstantStrings.CustomersSold] = 0,
        };
        return playerProps;
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(InitialPlayerProps());
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PlayerPrefs.DeleteKey(ConstantStrings.LastRoom);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("2. Game");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (part1.gameObject.activeSelf)
            StartCoroutine(ErrorMessage(AutoTranslate.Failed_to_connect_to_server()));
        else
            StartCoroutine(ErrorMessage(AutoTranslate.Disconnected_from_server()));

        part1.gameObject.SetActive(true);
        part2.gameObject.SetActive(false);
    }

#endregion

}
