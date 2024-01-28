using System.Threading.Tasks;
using UnityEngine;

public class StoveLogic : Hotspot
{
    public override Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            DialogueManager.AddText("I should shower first.");
            return Task.CompletedTask;
        }
        
        if(ProgressFlags._1_Showered && !ProgressFlags._1_Work)
        {
            ProgressFlags._1_Breakfast = true;
            DialogueManager.AddText("Not much of a breakfast, but it'll do.");
            return Task.CompletedTask;
        }

        if (!ProgressFlags._1_Work)
        {
            DialogueManager.AddText("I'll make dinner when I get home.");
            return Task.CompletedTask;
        }
        
        if (!ProgressFlags._1_Dinner)
        {
            ProgressFlags._1_Dinner = true;
            DialogueManager.AddText("I made dinner.");
            return Task.CompletedTask;
        }
        
        DialogueManager.AddText("I'm not hungry anymore.");
        return Task.CompletedTask;
    }
}