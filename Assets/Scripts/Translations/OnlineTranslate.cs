public static class OnlineTranslate 
{
public static string Online_Player_Playing (string Player) => $"Online_Player_Playing\tPlayer\t{Player}";
public static string Online_Player_Spectating (string Player) => $"Online_Player_Spectating\tPlayer\t{Player}";
public static string Online_Player_Reconnected (string Player) => $"Online_Player_Reconnected\tPlayer\t{Player}";
public static string Online_Player_Disconnected (string Player) => $"Online_Player_Disconnected\tPlayer\t{Player}";
public static string Online_Player_Quit (string Player) => $"Online_Player_Quit\tPlayer\t{Player}";
public static string Online_Waiting_on_Players (string Num) => $"Online_Waiting_on_Players\tNum\t{Num}";
public static string Online_Next_Turn (string Card) => $"Online_Next_Turn\tCard\t{Card}";
public static string Online_Resolve_Card (string Player,string Card) => $"Online_Resolve_Card\tPlayer\t{Player}\tCard\t{Card}";
public static string Online_Add_Resource (string Player,string Num,string Resource) => $"Online_Add_Resource\tPlayer\t{Player}\tNum\t{Num}\tResource\t{Resource}";
public static string Online_Lose_Resource (string Player,string Num,string Resource) => $"Online_Lose_Resource\tPlayer\t{Player}\tNum\t{Num}\tResource\t{Resource}";
public static string Online_Advance_Troop (string Player,string Num,string Num1,string Num2) => $"Online_Advance_Troop\tPlayer\t{Player}\tNum\t{Num}\tNum1\t{Num1}\tNum2\t{Num2}";
public static string Online_Fail_Advance (string Player) => $"Online_Fail_Advance\tPlayer\t{Player}";
public static string Online_Retreat_Troop (string Player,string Num,string Num1,string Num2) => $"Online_Retreat_Troop\tPlayer\t{Player}\tNum\t{Num}\tNum1\t{Num1}\tNum2\t{Num2}";
public static string Online_Add_Scout (string Player,string Num,string AreaNum) => $"Online_Add_Scout\tPlayer\t{Player}\tNum\t{Num}\tAreaNum\t{AreaNum}";
public static string Online_Remove_Scout (string Player,string Num,string AreaNum) => $"Online_Remove_Scout\tPlayer\t{Player}\tNum\t{Num}\tAreaNum\t{AreaNum}";
public static string Online_Draw_Card (string Player,string Card) => $"Online_Draw_Card\tPlayer\t{Player}\tCard\t{Card}";
public static string Online_Draw_Card_Others (string Player) => $"Online_Draw_Card_Others\tPlayer\t{Player}";
public static string Online_Discard_Card (string Player,string Card) => $"Online_Discard_Card\tPlayer\t{Player}\tCard\t{Card}";
public static string Online_Discard_Card_Others (string Player) => $"Online_Discard_Card_Others\tPlayer\t{Player}";
public static string Online_Play_Card (string Player,string Card) => $"Online_Play_Card\tPlayer\t{Player}\tCard\t{Card}";
public static string Online_Choose_Zero (string Player) => $"Online_Choose_Zero\tPlayer\t{Player}";
public static string Online_Miss_Ability (string Player,string Card) => $"Online_Miss_Ability\tPlayer\t{Player}\tCard\t{Card}";
public static string Online_No_Play (string Player) => $"Online_No_Play\tPlayer\t{Player}";
public static string Online_Who_Controls (string Player,string Num) => $"Online_Who_Controls\tPlayer\t{Player}\tNum\t{Num}";
public static string Online_Player_Won (string Player) => $"Online_Player_Won\tPlayer\t{Player}";
public static string Online_Tie_Game () => $"Online_Tie_Game";
public static string Online_Player_Resigned (string Player) => $"Online_Player_Resigned\tPlayer\t{Player}";
}
