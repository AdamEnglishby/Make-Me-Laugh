using System.Threading.Tasks;
using UnityEngine;

public class ShowerLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            ProgressFlags._1_Showered = true;
            Debug.Log("SHOWERING");
            return;
        }
        
        Debug.Log("ALREADY SHOWERED");
    }
}