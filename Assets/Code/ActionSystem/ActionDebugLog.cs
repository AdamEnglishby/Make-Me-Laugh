using System.Threading.Tasks;
using UnityEngine;

public class ActionDebugLog : Action
{
    public override string Title => "Debug.Log";
    public override ActionCategory Category => ActionCategory.Engine;

    public string message;

    public override Task Run()
    {
        Debug.Log(message);
        return Task.CompletedTask;
    }

#if UNITY_EDITOR
    public override void ShowGUI(UnityEditor.SerializedObject serializedObject)
    {
        message = UnityEditor.EditorGUILayout.TextField("Message:", message);
    }
#endif
    
}