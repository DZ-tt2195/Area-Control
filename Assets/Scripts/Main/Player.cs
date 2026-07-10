using Photon.Pun;
using UnityEngine;
using TMPro;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class Player : PhotonCompatible
{

#region Setup

    bool initialized = false;
    [ReadOnly] public bool endPause = true;
    [SerializeField] Transform keepHand;
    public Dictionary<string, bool> uiDictionary = new();
    List<Card> myDeck;
    List<Card> myDiscard;
    List<Card> myHand;
    int myCoins;
    int myActions;
    int[] myTroops;
    int[] myScouts;

    protected override void Awake()
    {
        base.Awake();
        this.bottomType = this.GetType();

        List<string> toAdd = new() { ConstantStrings.MyHand, ConstantStrings.MyDeck, ConstantStrings.MyDiscard, ConstantStrings.MyCoins, ConstantStrings.MyActions, ConstantStrings.MyScouts, ConstantStrings.MyTroops };
        foreach (string next in toAdd)
            uiDictionary.Add(next, true);

        Invoke(nameof(Beginning), 1f);
    }
    void Beginning()
    {
        if (photonView.AmOwner && !initialized)
            DoFunction(() => SendName(PlayerPrefs.GetString(ConstantStrings.MyUserName)), RpcTarget.AllBuffered);
    }
    [PunRPC]
    void SendName(string username)
    {
        initialized = true;
        this.name = username;
        SetToPlayerProps();

        if (photonView.AmOwner)
        {
            CreateGame.inst.mainPlayer = this;
            Button resignButton = GameObject.Find("Resign").GetComponent<Button>();
            resignButton.onClick.AddListener(() => TurnManager.inst.TextForEnding(OnlineTranslate.Online_Player_Resigned(this.name), GetThisPlayerPosition(PhotonNetwork.LocalPlayer)));
            StartTurn();
        }
        UpdateUI(true);
    }
    void SetToPlayerProps()
    {
        myCoins = TurnManager.inst.GetInt(ConstantStrings.MyCoins, this);
        myDeck = TurnManager.inst.GetCardList(ConstantStrings.MyDeck, this);
        myDiscard = TurnManager.inst.GetCardList(ConstantStrings.MyDiscard, this);
        myHand = TurnManager.inst.GetCardList(ConstantStrings.MyHand, this);
        myScouts = TurnManager.inst.GetIntArray(ConstantStrings.MyScouts);
        myTroops = TurnManager.inst.GetIntArray(ConstantStrings.MyTroops);
    }

    #endregion

#region Cards
    public List<Card> GetHand() => myHand;
    public void DrawCustomerRPC(int amount, int logged = 0)
    {
        if (amount <= 0) return;
        Log.inst.groupToWait.StartCoroutine(WaitForCards());

        IEnumerator WaitForCards()
        {
            InstantChangePlayerProp(this, ConstantStrings.NeedDraw, amount - myDeck.Count);
            while (myDeck.Count < amount)
            {
                yield return null;
            }

            List<Card> toDraw = new();
            for (int i = 0; i < amount; i++)
            {
                Card card = myDeck[i];
                Log.inst.AddMyText(false, OnlineTranslate.Online_Draw_Card(this.name, card.name), logged);
                toDraw.Add(card);
            }
            Log.inst.NewRollback(() => DrawCustomer(toDraw));            
        }
    }
    void DrawCustomer(List<Card> cardsToAdd)
    {
        if (!Log.inst.forward)
        {
            for (int i = cardsToAdd.Count-1; i>= 0; i--)
            {
                Card card = cardsToAdd[i];
                card.transform.SetParent(null);
                myHand.Remove(card);
                myDeck.Insert(0, card);
            }
        }
        else
        {
            for (int i = 0; i < cardsToAdd.Count; i++)
            {
                Card card = cardsToAdd[i];
                myHand.Add(card);
                myDeck.Remove(card);
            }
        }
        myHand = myHand.OrderBy(card => card.dataFile.coinCost).ThenBy(card => card.dataFile.cardName).ToList();
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyHand, ConvertCardList(myHand)); uiDictionary[ConstantStrings.MyHand] = true;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyDeck, ConvertCardList(myDeck)); uiDictionary[ConstantStrings.MyDeck] = true;
    }
    public void DiscardCustomerRPC(Card card, int logged = 0)
    {
        Log.inst.NewRollback(() => DiscardCustomer(card));
        Log.inst.AddMyText(false, OnlineTranslate.Online_Discard_Card(this.name, card.name), logged);
    }
    void DiscardCustomer(Card card)
    {
        if (!Log.inst.forward)
        {
            myHand.Add(card);
            myDiscard.Remove(card);
        }
        else
        {
            myHand.Remove(card);
            myDiscard.Add(card);
            card.transform.SetParent(null);
        }
        myHand = myHand.OrderBy(card => card.dataFile.coinCost).ThenBy(card => card.dataFile.cardName).ToList();
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyHand, ConvertCardList(myHand)); uiDictionary[ConstantStrings.MyHand] = true;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyDiscard, ConvertCardList(myDiscard)); uiDictionary[ConstantStrings.MyDiscard] = true;
    }
    public void ReceiveCardsRPC(List<Card> newCards)
    {
        DoFunction(() => ReceiveCards(ConvertCardList(newCards)), this.photonView.Owner);
    }
    [PunRPC]
    void ReceiveCards(int[] newCards)
    {
        List<Card> newCardList = TurnManager.ConvertIntArray(newCards);
        myDeck.AddRange(newCardList);
        InstantChangePlayerProp(this, ConstantStrings.NeedDraw, 0);

        int[] array = (int[])GetPlayerProperty(this, ConstantStrings.DrewThisTurn);
        List<Card> drewThisTurn = TurnManager.ConvertIntArray(array);
        drewThisTurn.AddRange(newCardList);
        InstantChangePlayerProp(this, ConstantStrings.DrewThisTurn, TurnManager.ConvertCardList(drewThisTurn));
    }
   
    #endregion

#region Resources
    public int GetCoins() => myCoins;
    public void CoinRPC(int num, int logged = 0, bool important = false)
    {
        if (num == 0)
            return;

        int actualAmount = (myCoins + num < 0) ? -1*myCoins : num;

        if (actualAmount > 0)
            Log.inst.AddMyText(important, OnlineTranslate.Online_Add_Coin(this.name, actualAmount.ToString()), logged);
        else
            Log.inst.AddMyText(important, OnlineTranslate.Online_Lose_Coin(this.name, Mathf.Abs(actualAmount).ToString()), logged);
        Log.inst.NewRollback(() => ChangeCoin(actualAmount));
    }
    void ChangeCoin(int num)
    {
        myCoins += (!Log.inst.forward) ? -num : num;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyCoins, myCoins); uiDictionary[ConstantStrings.MyCoins] = true;
    }
    public int GetActions() => myActions;
    public void ActionRPC(int num, int logged = 0, bool important = false)
    {
        if (num == 0)
            return;

        int actualAmount = (myActions + num < 0) ? -1*myActions : num;

        if (actualAmount > 0)
            Log.inst.AddMyText(important, OnlineTranslate.Online_Add_Action(this.name, actualAmount.ToString()), logged);
        else
            Log.inst.AddMyText(important, OnlineTranslate.Online_Lose_Action(this.name, Mathf.Abs(actualAmount).ToString()), logged);
        Log.inst.NewRollback(() => ChangeAction(actualAmount));
    }
    void ChangeAction(int num)
    {
        myActions += (!Log.inst.forward) ? -num : num;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyActions, myActions); uiDictionary[ConstantStrings.MyActions] = true;
    }

    #endregion

#region Troops/Scouts
    public int[] GetScouts() => myScouts;
    public void ScoutRPC(int num, int area, int logged = 0, bool important = false)
    {
        if (num == 0)
            return;

        int actualAmount = (myScouts[area] + num < 0) ? -1*myScouts[area] : num;

        if (actualAmount > 0)
            Log.inst.AddMyText(important, OnlineTranslate.Online_Add_Scout(this.name, actualAmount.ToString(), area.ToString()), logged);
        else
            Log.inst.AddMyText(important, OnlineTranslate.Online_Remove_Scout(this.name, Mathf.Abs(actualAmount).ToString(), area.ToString()), logged);
        Log.inst.NewRollback(() => ChangeScout(area, actualAmount));
    }
    void ChangeScout(int area, int num)
    {
        myScouts[area] += (!Log.inst.forward) ? -num : num;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyScouts, myScouts); uiDictionary[ConstantStrings.MyScouts] = true;        
    }
    public int[] GetTroops() => myTroops;
    public void TroopRPC(int num, int oldArea, int newArea, int logged = 0, bool important = false)
    {
        if (num == 0 || oldArea == newArea)
            return;

        int actualAmount = (myTroops[oldArea] + num < 0) ? -1*myScouts[oldArea] : num;
        if (actualAmount > 0)
            Log.inst.AddMyText(important, OnlineTranslate.Online_Advance_Troop(this.name, actualAmount.ToString(), oldArea.ToString(), newArea.ToString()), logged);
        else
            Log.inst.AddMyText(important, OnlineTranslate.Online_Retreat_Troop(this.name, Mathf.Abs(actualAmount).ToString(), oldArea.ToString(), newArea.ToString()), logged);
        
        Log.inst.NewRollback(() => ChangeTroop(oldArea, -actualAmount));
        Log.inst.NewRollback(() => ChangeTroop(newArea, actualAmount));
    }
    void ChangeTroop(int area, int num)
    {
        myTroops[area] += (!Log.inst.forward) ? -num : num;
        TurnManager.inst.WillChangePlayerProperty(this, ConstantStrings.MyTroops, myTroops); uiDictionary[ConstantStrings.MyTroops] = true;        
    }

#endregion

#region Turns

    void Update()
    {
        if (photonView.AmOwner && Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                GetPlayers(true);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                PhotonNetwork.Disconnect();
        }
    }
    public void StartTurn()
    {
        CreateGame.inst.SwitchToPlayer(this);
        InstantChangePlayerProp(this, ConstantStrings.Waiting, false);
        endPause = true;

        int[] array = (int[])GetPlayerProperty(this, ConstantStrings.DrewThisTurn);
        List<Card> drewThisTurn = TurnManager.ConvertIntArray(array);
        myDeck.AddRange(drewThisTurn);

        (string phase, Action action) = TurnManager.inst.GetTurnAction(this);
        if (phase != nameof(WaitForJoiners) && phase != nameof(DisplayStart))
            Log.inst.AddMyText(true, AutoTranslate.Blank());

        Log.inst.NewDecisionContainer(() => action(), 0);
        Log.inst.NewDecisionContainer(() => EndTurn(), -1);
        Log.inst.PopStack();
    }
    void EndTurn()
    {
        Log.inst.inReaction.Add(Done);
        if (endPause)
        {
            if (Log.inst.undosInLog.Count >= 1)
            {
                if (PermaUI.inst.PauseToUndo())
                    MakeDecision.inst.ChooseTextButton(new() { new(AutoTranslate.Done()) }, AutoTranslate.Pause_to_Undo(),false);
            }
            else
            {
                if (PermaUI.inst.PauseToRead())
                    MakeDecision.inst.ChooseTextButton(new() { new(AutoTranslate.Done()) }, AutoTranslate.Pause_to_Read(),false);
            }
        }

        void Done()
        {
            StartCoroutine(SmallDelay());
            IEnumerator SmallDelay()
            {
                yield return new WaitForSeconds(0.5f);
                Log.inst.DoneWithTurn();
                InstantChangePlayerProp(this, ConstantStrings.Waiting, true);
            }
        }    
    }
    public void ClearCards()
    {
        InstantChangePlayerProp(this, ConstantStrings.DrewThisTurn, new int[0]);

        int[] discardedArray = (int[])GetPlayerProperty(this, ConstantStrings.MyDiscard);
        if (discardedArray.Length > 0)
        {
            DoFunction(() => MakeCardsNull(discardedArray), RpcTarget.All);
            MainDeck.inst.ReceiveDiscardRPC(TurnManager.ConvertIntArray(discardedArray));
            InstantChangePlayerProp(this, ConstantStrings.MyDiscard, new int[0]);
        }        
    }
    [PunRPC]
    void MakeCardsNull(int[] removed)
    {
        foreach (int next in removed)
            PhotonView.Find(next).transform.SetParent(null);
    }
    #endregion

#region UI
    public void UpdateUI(bool forcedUpdate)
    {
        List<string> uiKeys = uiDictionary.Keys.ToList();
        int myPosition = GetThisPlayerPosition(PhotonNetwork.LocalPlayer);
        int thisPlayerPosition = GetThisPlayerPosition(this.photonView.Owner);

        if (forcedUpdate)
        {
            SetToPlayerProps();
            foreach (var key in uiKeys)
                uiDictionary[key] = true;
        }

        if (uiDictionary[ConstantStrings.MyHand])
        {
            if (this.transform.parent != null) AudioManager.instance.Card();
            List<Vector2> handPositions = ObjectPositions(myHand.Count, -1125, 475, 225, -550, true);
            for (int i = 0; i < myHand.Count; i++)
            {
                Card nextCard = myHand[i];
                if (nextCard.transform.parent != keepHand)
                {
                    nextCard.transform.SetParent(keepHand);
                    nextCard.transform.localPosition = new(0, -1000);
                }
                nextCard.transform.SetSiblingIndex(i);
                nextCard.selectMe.SetBorder(false);
                nextCard.MoveCardRPC(handPositions[i], 0.25f, Vector3.one);

                if (myPosition == -1 || thisPlayerPosition == myPosition)
                    nextCard.FlipCardRPC(1, 0.25f);
            }
        }

        if (uiDictionary[ConstantStrings.MyDeck])
        {
            foreach (Card card in myDeck)
                card.transform.SetParent(null);
        }

        if (uiDictionary[ConstantStrings.MyDiscard])
        {
            foreach (Card card in myDiscard)
                card.transform.SetParent(null);
        }

        if (uiDictionary[ConstantStrings.MyHand] || uiDictionary[ConstantStrings.MyActions] || uiDictionary[ConstantStrings.MyCoins] || uiDictionary[ConstantStrings.MyScouts] || uiDictionary[ConstantStrings.MyTroops])
            CreateGame.inst.UpdatePlayerUI(this, $"{this.name}: {myHand.Count} {AutoTranslate.CardIcon()}, {myActions} {AutoTranslate.ActionIcon()}, {myCoins} {AutoTranslate.CoinIcon()}");

        foreach (var key in uiKeys)
            uiDictionary[key] = false;
    }
    List<Vector2> ObjectPositions(int objectAmount, float start, float end, float gap, float fixedPosition, bool useX)
    {
        float midPoint = (start + end) / 2f;
        int maxFit = (int)((Mathf.Abs(start) + Mathf.Abs(end)) / gap);
        float offByOne = objectAmount - 1;

        List<Vector2> toReturn = new();
        for (int i = 0; i<objectAmount; i++)
        {
            float starting = (objectAmount <= maxFit) ? midPoint - (gap * (offByOne / 2f)) : start;
            float difference = (objectAmount <= maxFit) ? gap : gap * (maxFit / offByOne);

            if (useX)
                toReturn.Add(new(starting + difference * i, fixedPosition));
            else
                toReturn.Add(new(fixedPosition, starting + difference * i));
        }
        return toReturn;
    } 

#endregion

}