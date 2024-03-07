using UnityEditor;

namespace CustomInspector.Editors
{
    public class CustomSettingsProvider : SettingsProvider
    {
        private class Styles
        {
        }

        public CustomSettingsProvider()
            : base("Project/Custom Inspector", SettingsScope.Project)
        {
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            base.OnGUI(searchContext);

            EditorGUI.EndDisabledGroup();
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomInspectorSettingsProvider()
        {
            var provider = new CustomSettingsProvider
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>(),
            };

            return provider;
        }
    }
}