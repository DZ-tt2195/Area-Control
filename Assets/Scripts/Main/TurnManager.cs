using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using System.Linq;

public class TurnManager : PhotonCompatible
{

#region Setup

    public static TurnManager inst;
    Dictionary<Player, ExitGames.Client.Photon.Hashtable> playerPropertyToChange;
    ExitGames.Client.Photon.Hashtable masterPropertyToChange;
    Dictionary<string, Turn> storedTurns = new();
    [SerializeField] Transform endScreen;
    [SerializeField] TMP_Text summaryText;
    [SerializeField] TMP_Text gameOver;
    [SerializeField] TMP_Text leave;

    protected override void Awake()
    {
        base.Awake();
        inst = this;
        this.bottomType = this.GetType();
        endScreen.gameObject.SetActive(false);
        playerPropertyToChange = new();
        masterPropertyToChange = new();
        leave.text = AutoTranslate.Leave();
        gameOver.text = AutoTranslate.Game_Over();
    }

    #endregion

#region Turns

    void UpdateWaitingText(List<Photon.Realtime.Player> toSend, int playersWaiting)
    {
        foreach (Photon.Realtime.Player player in toSend)
        {
            if (playersWaiting == 0)
            {
                MakeDecision.inst.DoFunction(() => MakeDecision.inst.PackagedInstructions(AutoTranslate.Blank()), player);                            
            }
            else
            {
                MakeDecision.inst.DoFunction(() => MakeDecision.inst.PackagedInstructions(OnlineTranslate.Online_Waiting_on_Players(playersWaiting.ToString())), player);            
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        bool HasPropertyAndValue(ExitGames.Client.Photon.Hashtable changedProps, string propertyName, object expected)
        {
            return (changedProps.ContainsKey(propertyName.ToString()) && changedProps[propertyName.ToString()].Equals(expected));
        }

        if (HasPropertyAndValue(changedProps, ConstantStrings.Waiting, true))
        {
            (List<Photon.Realtime.Player> players, List<Photon.Realtime.Player> spectators) = GetPlayers(false);
            int WaitingOnPlayers()
            {
                int playersWaiting = (int)GetRoomProperty(ConstantStrings.CanPlay);
                List<Photon.Realtime.Player> isWaiting = new();
                isWaiting.AddRange(spectators);

                foreach (Photon.Realtime.Player player in players)
                {
                    if ((bool)GetPlayerProperty(player, ConstantStrings.Waiting))
                    {
                        isWaiting.Add(player);
                        playersWaiting--;
                    }
                }
                UpdateWaitingText(isWaiting, playersWaiting);
                return playersWaiting;
            }
            //all players have finished their turn
            if (PhotonNetwork.IsMasterClient && WaitingOnPlayers() == 0 && !(bool)GetRoomProperty(ConstantStrings.GameOver))
            {
                foreach (Photon.Realtime.Player nextPlayer in players)
                    DoFunction(() => LockInPropertyChanges(), nextPlayer);

                UpdateWaitingText(spectators, players.Count);
                Invoke(nameof(EndPhase), 0.25f);
            }
        }
    }
    void EndPhase() //phase end
    {
        storedTurns[GetCurrentPhase()].MasterEnd();
        Invoke(nameof(NextPhase), 0.25f);
    }
    void NextPhase() //switch phases
    {
        string nextPhase = (string)GetRoomProperty(ConstantStrings.NextPhase);
        //Debug.Log($"move to {nextPhase}");

        (Player, int) highestScore = (null, 0);
        foreach (Player player in CreateGame.inst.GetPlayers())
        {
            int health = player.GetCoins();
            if (health > highestScore.Item2)
                highestScore = (player, health);
            else if (health == highestScore.Item2)
                highestScore = (null, health);
        }

        if (nextPhase.Equals(nameof(TakeTurn)) && highestScore.Item2 >= 20 && highestScore.Item1 != null)
        {
            TextForEnding(OnlineTranslate.Online_Player_Won(highestScore.Item1.name), -1);
            InstantChangeRoomProp(ConstantStrings.CurrentPhase, nameof(Ending));
        }
        else
        {
            InstantChangeRoomProp(ConstantStrings.NextPhase, nameof(TakeTurn));
            InstantChangeRoomProp(ConstantStrings.CurrentPhase, nextPhase);
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //players start a new turn
        if (propertiesThatChanged.ContainsKey(ConstantStrings.CurrentPhase))
        {
            //Debug.Log($"switched to {GetCurrentPhase()}");
            if (PhotonNetwork.IsMasterClient)
                storedTurns[GetCurrentPhase()].MasterStart();

            CreateGame.inst.RefreshUI(true);
            if (CreateGame.inst.mainPlayer != null)
                CreateGame.inst.mainPlayer.StartTurn();                
        }
    }

    #endregion

#region Property Helpers

    object FindThisProperty(string property, Player player)
    {
        if (player != null && !playerPropertyToChange.ContainsKey(player))
            playerPropertyToChange.Add(player, new());

        if (masterPropertyToChange.ContainsKey(property))
            return masterPropertyToChange[property];
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(property))
            return GetRoomProperty(property);

        if (playerPropertyToChange[player].ContainsKey(property))
            return playerPropertyToChange[player][property];
        else
            return GetPlayerProperty(player.photonView.Owner, property);
    }
    public int GetInt(string property, Player player) => (int)FindThisProperty(property, player);
    public int GetInt(string property) => (int)FindThisProperty(property, null);
    public int[] GetIntArray(string property, Player player) => (int[])FindThisProperty(property, player);
    public int[] GetIntArray(string property) => (int[])FindThisProperty(property, null);
    public List<string> GetStringList(string property, Player player)
    {
        string[] stringArray = (string[])FindThisProperty(property, player);
        return stringArray.ToList();
    }
    public List<Card> GetCardList(string property, Player player) => ConvertIntArray((int[])FindThisProperty(property, player));
    public List<Card> GetCardList(string property) => ConvertIntArray((int[])FindThisProperty(property, null));
    public string GetString(string property, Player player) => (string)FindThisProperty(property, player);
    #endregion

#region Change Properties
    string GetCurrentPhase()
    {
        try
        {
            string toReturn = (string)GetRoomProperty(ConstantStrings.CurrentPhase);
            if (!storedTurns.ContainsKey(toReturn))
                storedTurns.Add(toReturn, (Turn)Activator.CreateInstance(Type.GetType(toReturn)));
            return toReturn;
        }
        catch
        {
            return nameof(WaitForJoiners);
        }
    }
    public (string, Action) GetTurnAction(Player player)
    {
        string currentPhase = GetCurrentPhase();
        return (currentPhase, () => storedTurns[currentPhase].ForPlayer(player));
    }
    public void WillChangePlayerProperty(Player player, string playerProperty, object changeInto)
    {
        if (!playerPropertyToChange.ContainsKey(player))
            playerPropertyToChange.Add(player, new());

        if (playerPropertyToChange[player].ContainsKey(playerProperty.ToString()))
            playerPropertyToChange[player][playerProperty.ToString()] = changeInto;
        else
            playerPropertyToChange[player].Add(playerProperty.ToString(), changeInto);
    }
    public void WillChangeMasterProperty(string masterProperty, object changeInto)
    {
        if (masterPropertyToChange.ContainsKey(masterProperty))
            masterPropertyToChange[masterProperty] = changeInto;
        else
            masterPropertyToChange.Add(masterProperty, changeInto);
    }
    [PunRPC]
    void LockInPropertyChanges()
    {
        Log.inst.ShareTexts();
        foreach (var KVP in playerPropertyToChange)
        {
            KVP.Key.photonView.Owner.SetCustomProperties(KVP.Value);
            KVP.Value.Clear();
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(masterPropertyToChange);
        masterPropertyToChange.Clear();
        CreateGame.inst.mainPlayer.ClearCards();
    }

    #endregion

#region Ending

    public void TextForEnding(string packagedText, int resignPosition)
    {
        Log.inst.MasterText(true, packagedText);
        InstantChangeRoomProp(ConstantStrings.GameOver, true);
        DoFunction(() => ShowEnding(resignPosition), RpcTarget.All);
    }
    [PunRPC]
    void ShowEnding(int resignPosition)
    {
        AudioManager.instance.GameOver();
        endScreen.gameObject.SetActive(true);
        string text = "";
        Player resigned = null;

        List<Player> scoresInOrder = CreateGame.inst.GetPlayers().OrderByDescending(player => player.GetCoins()).ToList();
        foreach (Player player in scoresInOrder)
        {
            if (GetThisPlayerPosition(player.photonView.Owner) == resignPosition)
                resigned = player;
            else
                text += $"{player.name} - {AutoTranslate.Coin_Amount(player.GetCoins().ToString())}\n";   
        }
        if (resigned != null)
            text += $"{resigned.name} - {AutoTranslate.Coin_Amount(resigned.GetCoins().ToString())} {AutoTranslate.Resigned()}";
        summaryText.text = KeywordTooltip.instance.EditText(text);
    }

#endregion

}
