using Photon.Pun;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string NextTurn(Player affectMe, Card cardEffect)
    {
        //format: player id \t card id
        return $"{affectMe.photonView.ViewID}\t{cardEffect.photonView.ViewID}";
        //delete list of future card effects at end of round, then add them
    }
    public void ReadCard(string readThis)
    {
        string[] splitUp = readThis.Split('\t');
        Player player = PhotonView.Find(int.Parse(splitUp[0])).GetComponent<Player>();
        Card card = PhotonView.Find(int.Parse(splitUp[1])).GetComponent<Card>();
    }
}
