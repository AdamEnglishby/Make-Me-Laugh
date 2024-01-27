using System.Threading.Tasks;
using UnityEngine;

public class SofaLogic : Hotspot
{
    public override async Task Interact(Player player)
    {
        if (!ProgressFlags._1_Dinner)
        {
            Debug.Log("NEED TO MAKE DINNER FIRST");
            return;
        }

        if (!ProgressFlags._1_Sofa)
        {
            ProgressFlags._1_Sofa = true;
            Debug.Log("CONSUMING MEDIA ON THE SOFA");
            return;
        }
        
        Debug.Log("TIME FOR BED");
    }
}