using System.Threading.Tasks;
using UnityEngine;

public class BedLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Work)
        {
            DialogueManager.AddText("...Should I even bother making my bed today?");
            return;
        }
        
        if (!ProgressFlags._1_Laugh)
        {
            DialogueManager.AddText("I'm not sleepy yet, maybe lazing in front of the TV for a while will do it.");
            return;
        }

        DialogueManager.AddText("Zzz...");
        await Awaitable.WaitForSecondsAsync(0.5f);
        await DialogueManager.FadeToBlack(5f);
    }
}