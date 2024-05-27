using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PROJECT_GUIDE))]
public class PROJECT_GUIDE_CUSTOM_EDITOR : Editor
{
    private static GUIStyle _titleStyle;
    private static GUIStyle _headerStyle;
    private static GUIStyle _bodyStyle;
    private static GUIStyle _rateStyle;

    public static void UpdateStyles()
    {
        if (_bodyStyle == null)
        {
            _bodyStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                fontSize = 14
            };

            _titleStyle = new GUIStyle(_bodyStyle)
            {
                fontSize = 26,
                alignment = TextAnchor.MiddleCenter
            };

            _headerStyle = new GUIStyle(_bodyStyle)
            {
                fontSize = 18
            };

            _rateStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 20
            };
        }
    }

    public override void OnInspectorGUI()
    {
        var guide = (PROJECT_GUIDE) target;

        UpdateStyles();

        EditorGUILayout.LabelField("Welcome to project " + guide.projectName + "!", _headerStyle);
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("This is a project of Laputa. This project will help you reduce time and significantly increase game development performance.", _bodyStyle);
        EditorGUILayout.Separator();
        if (GUILayout.Button(new GUIContent("Project Link", "Open In Web Browser"), GUILayout.Height(50)) == true)
        {
            Application.OpenURL("https://github.com/GuardianOfGods/unity-mobile-base");
        }
        
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Documentation", _headerStyle);
        EditorGUILayout.LabelField("Here is the documentations!", _bodyStyle);
        if (GUILayout.Button(new GUIContent("Wiki", "Github wiki")) == true)
        {
            Application.OpenURL("https://github.com/GuardianOfGods/unity-mobile-base/wiki");
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("More Information", _headerStyle);
        EditorGUILayout.LabelField("If you have any trouble with project, just send an issue or contact me directly!", _bodyStyle);
        if (GUILayout.Button(new GUIContent("Send An Issue", "Github issue")) == true)
        {
            Application.OpenURL("https://github.com/GuardianOfGods/unity-mobile-base/issues");
        }
        if (GUILayout.Button(new GUIContent("E-Mail Me", "Send an email")) == true)
        {
            Application.OpenURL("mailto:guardian.of.gods99@gmail.com");
        }
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.LabelField("Support", _headerStyle);
        EditorGUILayout.LabelField("If you like this project. A star will be a great motivation for the project to further develop!", _bodyStyle);
        
        var oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        
        if (GUILayout.Button(new GUIContent("☆ Star This Repository ☆", "Star repository")) == true)
        {
            Application.OpenURL("https://github.com/GuardianOfGods/unity-mobile-base");
        }
        
        GUI.backgroundColor = oldColor;
    }

    protected override void OnHeaderGUI()
    {
        var guide = (PROJECT_GUIDE) target;

        UpdateStyles();

        GUILayout.BeginHorizontal("In BigTitle");
        {
            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);
            var content = new GUIContent($"{guide.projectName}\nversion {guide.version}");
            var height = Mathf.Max(_titleStyle.CalcHeight(content, EditorGUIUtility.currentViewWidth - iconWidth),
                iconWidth);

            if (guide.Icon != null)
            {
                GUILayout.Label(guide.Icon, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(iconWidth),
                    GUILayout.Height(iconWidth));
            }

            GUILayout.Label(content, _titleStyle, GUILayout.Height(height));
        }
        GUILayout.EndHorizontal();
    }
}