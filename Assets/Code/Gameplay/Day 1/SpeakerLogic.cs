using System.Threading.Tasks;
using UnityEngine;

public class SpeakerLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Sofa)
        {
            Debug.Log("NEED TO CONSUME MEDIA FIRST");
            return;
        }

        ProgressFlags._1_Laugh = true;
        Debug.Log("Hey Computer, make me laugh");
    }
}