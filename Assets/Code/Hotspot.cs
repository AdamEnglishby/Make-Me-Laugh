using System.Threading.Tasks;
using UnityEngine;

public abstract class Hotspot : MonoBehaviour
{
    public abstract Task Interact(Player player);
}