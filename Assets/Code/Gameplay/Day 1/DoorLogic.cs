using System.Threading.Tasks;
using UnityEngine;

public class DoorLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            Debug.Log("NEED TO SHOWER FIRST");
            return;
        }

        if (!ProgressFlags._1_Breakfast)
        {
            Debug.Log("NEED BREAKFAST FIRST");
            return;
        }

        if (!ProgressFlags._1_Work)
        {
            ProgressFlags._1_Work = true;

            player.MovementEnabled = false;
            await Awaitable.WaitForSecondsAsync(1f);
            // FADE OUT
            await Awaitable.WaitForSecondsAsync(1f);
            // FADE IN
            player.MovementEnabled = true;
            
            Debug.Log("GOING TO WORK");
            return;
        }
        
        Debug.Log("NOT GOING ANYWHERE THIS EVENING");
    }
}