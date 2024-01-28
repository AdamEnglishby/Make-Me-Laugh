using System.Threading.Tasks;
using UnityEngine;

public class SofaLogic : Hotspot
{
    public override Task Interact(Player player)
    {
        if (!ProgressFlags._1_Dinner)
        {
            DialogueManager.AddText("I need to get food before I sit down for the night.");
            return Task.CompletedTask;
        }

        if (!ProgressFlags._1_Sofa)
        {
            ProgressFlags._1_Sofa = true;
            DialogueManager.AddText("[Consuming media]...");
            return Task.CompletedTask;
        }
        
        DialogueManager.AddText("I need to get some sleep...");
        return Task.CompletedTask;
    }
}