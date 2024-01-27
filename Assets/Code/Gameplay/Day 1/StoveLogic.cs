using System.Threading.Tasks;
using UnityEngine;

public class StoveLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Showered)
        {
            Debug.Log("CAN'T MAKE BREAKFAST, NEED TO SHOWER");
            return;
        }
        
        if(ProgressFlags._1_Showered && !ProgressFlags._1_Work)
        {
            ProgressFlags._1_Breakfast = true;
            Debug.Log("MADE BREAKFAST");
            return;
        }

        if (!ProgressFlags._1_Work)
        {
            Debug.Log("CAN'T MAKE FOOD YET");
            return;
        }
        
        if (!ProgressFlags._1_Dinner)
        {
            ProgressFlags._1_Dinner = true;
            Debug.Log("MAKING DINNER");
            return;
        }
        
        Debug.Log("ALREADY MADE DINNER");
    }
}