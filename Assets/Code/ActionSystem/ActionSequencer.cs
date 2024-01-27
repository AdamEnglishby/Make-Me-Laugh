using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
#endif

public class ActionSequencer : MonoBehaviour
{
    [SerializeField] private bool runOnStart;
    [SerializeField] private List<Action> actions;

    private async void Start()
    {
        if (runOnStart) await Run();
    }

    public async Task Run()
    {
        foreach (var action in actions)
        {
            await action.Run();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ActionSequencer))]
public class ActionSequencerEditor : Editor
{
    private SerializedProperty Actions => serializedObject.FindProperty("actions");
    
    private static Dictionary<Action.ActionCategory, Dictionary<Type, string>> _allActions;

    private static Vector2 _moveUpSize, _moveDownSize;
    private static int _toDelete = -1;
    private static GUIStyle _actionContainer;

    private void OnEnable()
    {
        _actionContainer = new GUIStyle
        {
            padding = new RectOffset(0, 0, 2, 2),
            margin = new RectOffset(4, 4, 6, 6)
        };

        PopulateActions();
        DeepCopyActions();
    }

    private static void PopulateActions()
    {
        _allActions = new Dictionary<Action.ActionCategory, Dictionary<Type, string>>();

        foreach (Action.ActionCategory category in Enum.GetValues(typeof(Action.ActionCategory)))
        {
            _allActions.Add(category, new Dictionary<Type, string>());
        }

        var types = Assembly.GetAssembly(typeof(Action)).GetTypes();
        foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Action))))
        {
            var i = (Action) CreateInstance(type);
            _allActions[i.Category].Add(type, i.Title);
        }
    }

    private void DeepCopyActions()
    {
        var newArray = new List<Action>(Actions.arraySize);
        for (var i = 0; i < Actions.arraySize; i++)
        {
            var action = Actions.GetArrayElementAtIndex(i);
            var cast = (object) action.objectReferenceValue as Action;

            if (!cast) continue;

            var copy = Instantiate(cast);
            newArray.Add(copy);
        }

        Actions.SetUnderlyingValue(newArray);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        if (_allActions == null)
        {
            PopulateActions();
        }

        DrawRunNowButton();
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("runOnStart"), new GUIContent("Run on Start()?"));

        _moveUpSize = GUI.skin.verticalScrollbarUpButton.CalcSize(new GUIContent());
        _moveDownSize = GUI.skin.verticalScrollbarDownButton.CalcSize(new GUIContent());

        // iterate through the list of Actions, and draw them inside their own verticals
        for (var i = 0; i < Actions.arraySize; i++)
        {
            var action = Actions.GetArrayElementAtIndex(i);
            var cast = (object) action.objectReferenceValue as Action;

            EditorGUILayout.BeginVertical(_actionContainer);

            DrawHeader(i, cast, Actions);

            if (cast)
            {
                // erase name to avoid "(Clone)(Clone)(Clone)..." hell
                cast.name = "";
                DrawActionEditorFields(cast);
            }

            EditorGUILayout.EndVertical();
        }

        if (_toDelete != -1)
        {
            Actions.DeleteArrayElementAtIndex(_toDelete);
            _toDelete = -1;
        }

        DrawAdvancedMenu();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRunNowButton()
    {
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Run ActionSequencer now"))
        {
            (target as ActionSequencer)?.Run();
        }

        EditorGUI.EndDisabledGroup();
    }

    private static void DrawHeader(int i, Action cast, SerializedProperty array)
    {
        var style = new GUIStyle(GUI.skin.window);
        style.padding.top = style.padding.bottom *= 2;

        var headerContainer = EditorGUILayout.BeginHorizontal(style);

        DrawHeaderText(i, cast);
        DrawHeaderDeleteButton(i);
        DrawMoveActionButtons(headerContainer, i, array);

        EditorGUILayout.EndHorizontal();
    }

    private static void DrawHeaderText(int i, Action cast)
    {
        var text = $"#{i + 1}. ";
        if (cast)
        {
            text += cast.Title;
        }
        else
        {
            text += "<none>";
        }

        GUILayout.Label(new GUIContent(text), new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            margin = new RectOffset(8, 0, 0, 0)
        });
    }

    private static void DrawHeaderDeleteButton(int i)
    {
        var delButtonWidth =
            GUI.skin.button.CalcSize(new GUIContent(EditorGUIUtility.IconContent("d_TreeEditor.Trash"))).x;

        if (GUILayout.Button(
            EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Delete this Action"),
            new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, Mathf.RoundToInt(_moveUpSize.x) + 16, 0, 0),
                fixedWidth = delButtonWidth
            })
        )
        {
            _toDelete = i;
        }
    }

    private static void DrawMoveActionButtons(Rect rect, int i, SerializedProperty array)
    {
        if (i > 0)
        {
            var moveUpRect = new Rect(rect.width - _actionContainer.padding.left,
                rect.y - _actionContainer.padding.top + 4, _moveUpSize.x, _moveUpSize.y);
            if (GUI.Button(moveUpRect, "", GUI.skin.verticalScrollbarUpButton))
            {
                array.MoveArrayElement(i, i - 1);
            }
        }

        if (i >= array.arraySize - 1) return;

        var moveDownRect = new Rect(rect.width - _actionContainer.padding.left,
            rect.y + _actionContainer.padding.bottom + _moveUpSize.y, _moveDownSize.x, _moveDownSize.y);
        if (GUI.Button(moveDownRect, "", GUI.skin.verticalScrollbarDownButton))
        {
            array.MoveArrayElement(i, i + 1);
        }
    }

    private void DrawActionEditorFields(Action action)
    {
        EditorGUI.indentLevel++;

        var a = new SerializedObject(action);
        action.ShowGUI(a); // TODO: proper undo support

        // marks as dirty so we don't forget to save changes
        if (GUI.changed && !Application.isPlaying)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(target.GameObject().scene);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawAdvancedMenu()
    {
        const float buttonWidth = 230f;

        var style = new GUIStyle(EditorStyles.miniButton)
        {
            fixedWidth = buttonWidth,
            fixedHeight = 24
        };

        var rect = GUILayoutUtility.GetRect(new GUIContent("Add Action"), EditorStyles.toolbarButton);
        rect.width += rect.position.x + 5;
        rect.position = new Vector2(0, rect.position.y);

        rect.position += new Vector2(Mathf.Max((rect.width - buttonWidth) / 2, 0), 0);
        rect.width = buttonWidth;

        GUILayout.BeginHorizontal();
        var press = GUI.Button(rect, "Add Action", style);
        GUILayout.EndHorizontal();

        if (!press) return;

        rect.position += new Vector2(-1, 4);

        var dropdown = new ActionsDropdown(new AdvancedDropdownState());
        dropdown.Show(rect);
        var window = EditorWindow.focusedWindow;
        window.minSize = new Vector2(230, 315);
        window.maxSize = new Vector2(230, 315);
        window.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), window.position.size);

        dropdown.OnItemSelect += AddActionByTitle;
    }

    private void AddActionByTitle(string title)
    {
        Type type = null;
        foreach (var (_, dictionary) in _allActions)
        {
            foreach (var (actionType, actionTitle) in dictionary)
            {
                if (actionTitle == title)
                {
                    type = actionType;
                }
            }
        }

        if (type == null) return;

        Actions.arraySize++;
        var newAction = Actions.GetArrayElementAtIndex(Actions.arraySize - 1);
        newAction.objectReferenceValue = CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
    }
}

internal class ActionsDropdown : AdvancedDropdown
{
    public ActionsDropdown(AdvancedDropdownState state) : base(state)
    {
        minimumSize = new Vector2(230, 315);
    }

    public event Action<string> OnItemSelect;

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Search");
        var categories = new List<AdvancedDropdownItem>();

        var categoryNames = Enum.GetNames(typeof(Action.ActionCategory));
        Array.Sort(categoryNames);

        foreach (var categoryName in categoryNames)
        {
            if (categoryName == Action.ActionCategory.Uncategorised.ToString()) continue;

            // could do icons in here?
            var dropdownItem = new AdvancedDropdownItem(categoryName);
            categories.Add(dropdownItem);
        }

        var allTypes = Assembly.GetAssembly(typeof(Action)).GetTypes();
        foreach (var type in allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Action))))
        {
            var action = ScriptableObject.CreateInstance(type) as Action;
            if (!action) break;

            if (action.Category == Action.ActionCategory.Uncategorised) continue;

            var c = categories.Find(category => category.name == action.Category.ToString());
            c.AddChild(new AdvancedDropdownItem(action.Title));
        }

        foreach (var item in categories.Where(item => item.children.Any()))
        {
            root.AddChild(item);
        }

        // add uncategorised types last
        foreach (var type in allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Action))))
        {
            var action = ScriptableObject.CreateInstance(type) as Action;
            if (!action) break;
            if (action.Category != Action.ActionCategory.Uncategorised) continue;
            root.AddChild(new AdvancedDropdownItem(action.Title));
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);
        OnItemSelect?.Invoke(item.name);
    }
}
#endif