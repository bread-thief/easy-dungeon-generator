using BreadThief.EasyDungeonGenerator;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EasyDungeonGenerator))]
public class EasyDungeonGeneratorEditor : Editor
{
    private bool _isTitleHovered = false;
    private Color _primaryColor = new Color(0.902f, 0.698f, 0.290f);

    public override void OnInspectorGUI()
    {
        DrawBanner();
        EditorGUILayout.Space(20);
        DrawPropertiesExcluding(serializedObject, "m_Script");
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawBanner()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            Rect topRect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            EditorGUI.DrawRect(topRect, _primaryColor * 0.8f);

            EditorGUILayout.Space(15);

            GUIStyle iconStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                normal = { textColor = _primaryColor },
                hover = { textColor = _primaryColor },
                active = { textColor = _primaryColor },
                focused = { textColor = _primaryColor },
                onNormal = { textColor = _primaryColor },
                onHover = { textColor = _primaryColor },
                onActive = { textColor = _primaryColor },
                onFocused = { textColor = _primaryColor }
            };

            Rect iconRect = EditorGUILayout.GetControlRect(GUILayout.Height(45));
            string iconPath = "Packages/com.bread-thief.easy-dungeon-generator/Resources/icon.png";
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            float iconSize = 45f;
            Rect drawRect = new Rect(iconRect.center.x - iconSize / 2, iconRect.y, iconSize, iconSize);
            GUI.DrawTexture(drawRect, icon, ScaleMode.ScaleToFit);

            EditorGUILayout.Space(8);

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                normal = {
                    textColor = _isTitleHovered ? Color.Lerp(_primaryColor, Color.white, 0.2f) : _primaryColor
                }
            };

            Rect titleRect = EditorGUILayout.GetControlRect(GUILayout.Height(22));
            _isTitleHovered = titleRect.Contains(Event.current.mousePosition);

            if (_isTitleHovered)
            {
                EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);
            }

            if (Event.current.type == EventType.MouseDown && _isTitleHovered)
            {
                Application.OpenURL("https://github.com/bread-thief/easy-dungeon-generator");
                Event.current.Use();
            }

            EditorGUI.LabelField(titleRect, "Easy Dungeon Generator", titleStyle);

            EditorGUILayout.Space(2);

            GUIStyle authorStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Italic,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };

            Rect authorRect = EditorGUILayout.GetControlRect(GUILayout.Height(14));
            EditorGUI.LabelField(authorRect, "by Bread Thief", authorStyle);

            EditorGUILayout.Space(15);

            Rect bottomRect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            EditorGUI.DrawRect(bottomRect, _primaryColor * 0.8f);
        }
        EditorGUILayout.EndVertical();
    }
}