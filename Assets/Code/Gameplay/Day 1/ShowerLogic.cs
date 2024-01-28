using System.Threading.Tasks;

public class ShowerLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            ProgressFlags._1_Showered = true;
            player.InteractionEnabled = false;
            player.MovementEnabled = false;
            await DialogueManager.FadeToBlack();
            DialogueManager.AddText("Showering...");
            await DialogueManager.FadeBackIn();
            player.MovementEnabled = true;
            player.InteractionEnabled = true;
            return;
        }

        if (!ProgressFlags._1_Work)
        {
            DialogueManager.AddText("I've already showered, need to make breakfast...");
            return;
        }
        
        DialogueManager.AddText("I'll shower in the morning...");
    }
}