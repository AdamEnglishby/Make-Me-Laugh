using System.Threading.Tasks;
using UnityEngine;

public class SpeakerLogic : Hotspot
{
    public override Task Interact(Player player)
    {
        if (!ProgressFlags._1_Sofa)
        {
            DialogueManager.AddText("I have nothing to say to my smart speaker.");
            return Task.CompletedTask;
        }

        ProgressFlags._1_Laugh = true;
        DialogueManager.AddText("Hey computer, make me laugh.");
        return Task.CompletedTask;
    }
}