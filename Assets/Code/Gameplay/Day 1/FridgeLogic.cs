using System.Threading.Tasks;

namespace Code.Gameplay.Day_1
{
    public class FridgeLogic : Hotspot
    {
        public override Task Interact(Player player)
        {
            return Task.CompletedTask;
        }
    }
}