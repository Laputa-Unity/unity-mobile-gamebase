using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;

namespace customtools.customhierarchy.phelper
{
    [CustomEditor(typeof(CustomObjectList))]
    public class CustomObjectListInspector : Editor
    {
    	public override void OnInspectorGUI()
    	{
    		EditorGUILayout.HelpBox("\nThis is an auto created GameObject that managed by CustomHierarchy.\n\n" + 
                                    "It stores references to some GameObjects in the current scene. This object will not be included in the application build.\n\n" + 
                                    "You can safely remove it, but lock / unlock / visible / etc. states will be reset. Delete this object if you want to remove the CustomHierarchy.\n\n" +
                                    "This object can be hidden if you uncheck \"Show CustomHierarchy GameObject\" in the settings of the CustomHierarchy.\n"
                                    , MessageType.Info, true);

            if (CustomSettings.getInstance().get<bool>(CustomSetting.AdditionalShowObjectListContent))
            {
                if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(20)), "Hide content"))
                {
                    CustomSettings.getInstance().set(CustomSetting.AdditionalShowObjectListContent, false);
                }
                base.OnInspectorGUI();
            }
            else
            {
                if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(20)), "Show content"))
                {
                    CustomSettings.getInstance().set(CustomSetting.AdditionalShowObjectListContent, true);
                }
            }
    	}
    }
}