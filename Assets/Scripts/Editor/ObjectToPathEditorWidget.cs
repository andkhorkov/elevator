using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetPathGetterAttribute))]
public class ObjectToPathEditorWidget : PropertyDrawer
{
    private const string resetBtnName = "Reset";

    public override void OnGUI(Rect initialRect, SerializedProperty property, GUIContent label)
    {
        var defaultLineHeight = base.GetPropertyHeight(property, label);
        label.text += ": ";

        EditorGUI.BeginProperty(initialRect, label, property);

        EditorGUI.PrefixLabel(initialRect, GUIUtility.GetControlID(FocusType.Passive), label);

        var path = property.stringValue;

        initialRect.height = defaultLineHeight;

        var pathLabelRect = new Rect(initialRect.x + GUI.skin.label.CalcSize(new GUIContent(label)).x, initialRect.y, initialRect.width, initialRect.height);
        var resetBtnRect = new Rect(initialRect.x, initialRect.y + defaultLineHeight, GUI.skin.label.CalcSize(new GUIContent(resetBtnName + 50)).x, initialRect.height);

        GameObject go = null;

        if (string.IsNullOrEmpty(path))
        {
            var objectInputFieldPos = new Rect(initialRect.x, initialRect.y + defaultLineHeight, initialRect.width, initialRect.height);
            go = (GameObject)EditorGUI.ObjectField(objectInputFieldPos, "Prefab:", go, typeof(GameObject), true);
            path = AssetDatabase.GetAssetPath(go);
            path = path.Replace(".prefab", string.Empty);
            path = path.Replace("Assets/Resources/", string.Empty);
        }
        else if (GUI.Button(resetBtnRect, "reset"))
        {
            path = string.Empty;
        }

        property.stringValue = path;

        EditorGUI.LabelField(pathLabelRect, path);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 3;
    }
}
