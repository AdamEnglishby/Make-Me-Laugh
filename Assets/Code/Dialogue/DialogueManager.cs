using System;
using System.Threading.Tasks;
using Febucci.UI;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Text bodyText;

    private bool _receivedButton;

    private static DialogueManager _instance;

    private void OnEnable()
    {
        _instance = this;
    }

    public static async Task AddText(string text)
    {
        
    }

}