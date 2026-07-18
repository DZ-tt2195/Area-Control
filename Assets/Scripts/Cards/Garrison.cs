using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Garrison : CardType
{
    public Garrison(CardData dataFile) : base(dataFile)
    {
    }
    public override void DoInstructions(Player player, int thisArea, int logged)
    {
        if (player.GetScouts()[thisArea] >= 2)
        {
            List<TextButtonInfo> textButtonInfos = new() {new(AutoTranslate.Confirm(), DidIt), new(AutoTranslate.Decline(), DidNot)};
            MakeDecision.inst.ChooseTextButton(textButtonInfos, AutoTranslate.Ask_Remove());

            void DidIt()
            {
                foreach (TroopScoutDisplay display in CreateGame.inst.GetAllDisplays(player))
                {
                    if (display.info.area == thisArea)
                        player.ScoutRPC(-2, display.info.area);
                    else
                        player.ScoutRPC(1, display.info.area);
                }
            }
            void DidNot()
            {
                Log.inst.AddMyText(false, OnlineTranslate.Online_Decline_Ability(player.name, nameof(Garrison)), logged);
            }
        }
    }
}
