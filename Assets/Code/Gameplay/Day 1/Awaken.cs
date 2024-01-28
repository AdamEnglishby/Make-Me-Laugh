using UnityEngine;

public class Awaken : MonoBehaviour
{
    private async void Start()
    {
        Player.Instance.MovementEnabled = false;
        DialogueManager.SnapToBlack();
        DialogueManager.ClearAll();
        DialogueManager.AddText("Ugh... time to get up. I have to shower and get breakfast, then off to work once more.");
        await Awaitable.WaitForSecondsAsync(2f);
        await DialogueManager.FadeBackIn(5f);
        Player.Instance.MovementEnabled = true;
    }
}