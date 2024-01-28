using System.Threading.Tasks;
using UnityEngine;

public class DoorLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            DialogueManager.AddText("I should shower & eat first...");
            return;
        }

        if (!ProgressFlags._1_Breakfast)
        {
            DialogueManager.AddText("I need to make breakfast...");
            return;
        }

        if (!ProgressFlags._1_Work)
        {
            ProgressFlags._1_Work = true;

            player.MovementEnabled = false;
            await DialogueManager.FadeToBlack();
            await Awaitable.WaitForSecondsAsync(1.5f);
            DialogueManager.AddText("Another long day... I'll make some food & zone out for a while.");
            await DialogueManager.FadeBackIn();
            player.MovementEnabled = true;
            
            return;
        }
        
        DialogueManager.AddText("I have nowhere to go this evening.");
    }
}