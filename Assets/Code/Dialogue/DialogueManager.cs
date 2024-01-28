using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private SpriteRenderer blackOverlay;
    
    private bool _receivedButton;

    private static DialogueManager _instance;

    private void OnEnable()
    {
        _instance = this;
    }

    public static void ClearAll()
    {
        _instance.bodyText.text = "";
    }
    
    public static void AddText(string text)
    {
        _instance.bodyText.text += $"\n\n{text}";
    }

    public static async Task FadeToBlack(float time = 1f)
    {
        await _instance.blackOverlay.DOColor(Color.black, 1f).SetEase(Ease.OutCubic).AsyncWaitForCompletion();
    }

    public static async Task FadeBackIn(float time = 1f)
    {
        await _instance.blackOverlay.DOColor(new Color(0,0,0,0), 1f).SetEase(Ease.InCubic).AsyncWaitForCompletion();
    }

    public static void SnapToBlack()
    {
        _instance.blackOverlay.color = Color.black;
    }
    
}