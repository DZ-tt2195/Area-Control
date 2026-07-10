using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class MainDeck : PhotonCompatible
{
    public static MainDeck inst;
    [SerializeField] Card cardPrefab;
    protected override void Awake()
    {
        base.Awake();
        inst = this;
        this.bottomType = this.GetType();
        if (((int[])GetRoomProperty(ConstantStrings.MasterDeck)).Length == 0) CreateDeck();
        InvokeRepeating(nameof(HandOutDraws), 0, 0.3f);
    }
    void CreateDeck()
    {
        List<int> startingcustomerDeck = new();
        List<int> customerIDs = new();
        for (int i = 0; i<GameFiles.inst.customerFiles.Count; i++)
        {
            for (int j = 0; j<2; j++)
            {
                GameObject nextCard = MakeObject(cardPrefab.gameObject);
                PhotonView cardPV = nextCard.GetComponent<PhotonView>();
                startingcustomerDeck.Add(cardPV.ViewID);
                customerIDs.Add(i);
            }
        }
        customerIDs = customerIDs.Shuffle();
        InstantChangeRoomProp(ConstantStrings.MasterDeck, startingcustomerDeck.ToArray());
        DoFunction(() => CreateCards("Customer", startingcustomerDeck.ToArray(), customerIDs.ToArray()));
    }
    [PunRPC]
    void CreateCards(string typeToFind, int[] arrayOfPVs, int[] cardNames)
    {
        List<CardData> toFind = new();
        bool vertical = false;
        if (typeToFind.Equals("Twist"))
        {
            toFind = GameFiles.inst.twistFiles;
            vertical = false;
        }
        else if (typeToFind.Equals("Customer"))
        {
            toFind = GameFiles.inst.customerFiles;
            vertical = true;
        }

        for (int i = 0; i<arrayOfPVs.Length; i++)
        {
            GameObject obj = PhotonView.Find(arrayOfPVs[i]).gameObject;
            obj.GetComponent<Card>().AssignCard(toFind[cardNames[i]], 0f, vertical, Vector3.one);
        }
    }
    void HandOutDraws()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<Card> masterDeck = TurnManager.inst.GetCardList(ConstantStrings.MasterDeck);
            foreach (Player player in CreateGame.inst.GetPlayers())
            {
                int numToDraw = (int)GetPlayerProperty(player, ConstantStrings.NeedDraw);
                if (numToDraw > 0)
                {
                    List<Card> toGiveOut = new();
                    if (numToDraw+5 > masterDeck.Count) 
                        masterDeck.AddRange(ShuffleDiscard());

                    for (int i = 0; i<numToDraw+5; i++)
                    {
                        Card nextCard = masterDeck[0];
                        masterDeck.RemoveAt(0);
                        toGiveOut.Add(nextCard);
                    }
                     
                    InstantChangeRoomProp(ConstantStrings.MasterDeck, TurnManager.ConvertCardList(masterDeck));
                    player.ReceiveCardsRPC(toGiveOut);
                    break;
                }
            }
        }
    }
    public void ReceiveDiscardRPC(List<Card> discarded)
    {
        if (discarded.Count > 1)
            DoFunction(() => ReceiveDiscard(TurnManager.ConvertCardList(discarded)), RpcTarget.MasterClient);
    }
    [PunRPC]
    void ReceiveDiscard(int[] discarded)
    {
        List<Card> masterDiscard = TurnManager.inst.GetCardList(ConstantStrings.MasterDiscard);
        masterDiscard.AddRange(TurnManager.ConvertIntArray(discarded));
        InstantChangeRoomProp(ConstantStrings.MasterDiscard, TurnManager.ConvertCardList(masterDiscard));
    }
    List<Card> ShuffleDiscard()
    {
        List<Card> masterDiscard = TurnManager.inst.GetCardList(ConstantStrings.MasterDiscard);
        masterDiscard = masterDiscard.Shuffle();
        InstantChangeRoomProp(ConstantStrings.MasterDiscard, new int[0]);
        return masterDiscard;
    }
}