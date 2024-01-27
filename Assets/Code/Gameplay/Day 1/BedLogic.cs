using System.Threading.Tasks;
using UnityEngine;

public class BedLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Laugh)
        {
            player.MovementEnabled = false;
            await DialogueManager.AddText("Not bedtime yet!");
            player.MovementEnabled = true;
            player.InteractionEnabled = true;
            return;
        }
        
        Debug.Log("GOING TO BED");
    }
}