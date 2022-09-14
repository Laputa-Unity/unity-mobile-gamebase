// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A welcome screen for <see cref="Animancer"/>.</summary>
    // [CreateAssetMenu(menuName = Strings.MenuPrefix + "Read Me", order = Strings.AssetMenuOrder)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "." + nameof(Animancer.Editor) + "/" + nameof(ReadMe))]
    public class ReadMe : ScriptableObject
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        /// <summary>The release ID of the current version.</summary>
        /// <example><list type="bullet">
        /// <item>[ 1] = v1.0: 2018-05-02.</item>
        /// <item>[ 2] = v1.1: 2018-05-29.</item>
        /// <item>[ 3] = v1.2: 2018-08-14.</item>
        /// <item>[ 4] = v1.3: 2018-09-12.</item>
        /// <item>[ 5] = v2.0: 2018-10-08.</item>
        /// <item>[ 6] = v3.0: 2019-05-27.</item>
        /// <item>[ 7] = v3.1: 2019-08-12.</item>
        /// <item>[ 8] = v4.0: 2020-01-28.</item>
        /// <item>[ 9] = v4.1: 2020-02-21.</item>
        /// <item>[10] = v4.2: 2020-03-02.</item>
        /// <item>[11] = v4.3: 2020-03-13.</item>
        /// <item>[12] = v4.4: 2020-03-27.</item>
        /// <item>[13] = v5.0: 2020-07-17.</item>
        /// <item>[14] = v5.1: 2020-07-27.</item>
        /// <item>[15] = v5.2: 2020-09-16.</item>
        /// <item>[16] = v5.3: 2020-10-06.</item>
        /// <item>[17] = v6.0: 2020-12-04.</item>
        /// <item>[18] = v6.1: 2021-04-13.</item>
        /// <item>[19] = v7.0: 2021-07-29.</item>
        /// </list></example>
        protected virtual int ReleaseNumber => 19;

        /// <summary>The key used to save the release number.</summary>
        protected virtual string ReleaseNumberPrefKey => nameof(Animancer) + "." + nameof(ReleaseNumber);

        /// <summary>The name of this product.</summary>
        protected virtual string ProductName => Strings.ProductName + " Pro";

        /// <summary>The display name of this product version.</summary>
        protected virtual string VersionName => "v7.0";

        /// <summary>The URL for the documentation.</summary>
        protected virtual string DocumentationURL => Strings.DocsURLs.Documentation;

        /// <summary>The URL for the change log of this Animancer version.</summary>
        protected virtual string ChangeLogURL => Strings.DocsURLs.ChangeLogPrefix + "v7-0";

        /// <summary>The URL for the example documentation.</summary>
        protected virtual string ExampleURL => Strings.DocsURLs.Examples;

        /// <summary>The URL for the Unity Forum thread.</summary>
        protected virtual string ForumURL => Strings.DocsURLs.Forum;

        /// <summary>The URL for the Github Issues page.</summary>
        protected virtual string IssuesURL => Strings.DocsURLs.Issues;

        /// <summary>The developer email address.</summary>
        protected virtual string DeveloperEmail => Strings.DocsURLs.DeveloperEmail;

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="ReadMe"/> file name ends with the <see cref="VersionName"/> to detect if the user imported
        /// this version without deleting a previous version.
        /// <para></para>
        /// When Unity's package importer sees an existing file with the same GUID as one in the package, it will
        /// overwrite that file but not move or rename it if the name has changed. So it will leave the file there with
        /// the old version name.
        /// </summary>
        private bool HasCorrectName => name.EndsWith(VersionName);

        /************************************************************************************************************************/

        [SerializeField]
        private DefaultAsset _ExamplesFolder;

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Show On Startup
        /************************************************************************************************************************/

        [SerializeField]
        private bool _DontShowOnStartup;

        /// <summary>Should the system be prevented from automatically selecting this asset on startup?</summary>
        public bool DontShowOnStartup => _DontShowOnStartup && HasCorrectName;

        /************************************************************************************************************************/

        /// <summary>Automatically selects a <see cref="ReadMe"/> on startup.</summary>
        [InitializeOnLoadMethod]
        private static void ShowReadMe()
        {
            EditorApplication.delayCall += () =>
            {
                var asset = FindReadMe();
                if (asset != null)// Delay the call again to ensure that the Project window actually shows the selection.
                    EditorApplication.delayCall += () =>
                        Selection.activeObject = asset;
            };
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Finds the most recently modified <see cref="ReadMe"/> asset with <see cref="DontShowOnStartup"/> disabled.
        /// </summary>
        private static ReadMe FindReadMe()
        {
            DateTime latestWriteTime = default;
            ReadMe latestReadMe = null;
            string latestGUID = null;

            var guids = AssetDatabase.FindAssets($"t:{nameof(ReadMe)}");
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                if (SessionState.GetBool(guid, false))
                    continue;

                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ReadMe>(assetPath);
                if (asset != null && !asset.DontShowOnStartup)
                {
                    var writeTime = File.GetLastWriteTimeUtc(assetPath);
                    if (latestWriteTime < writeTime)
                    {
                        latestWriteTime = writeTime;
                        latestReadMe = asset;
                        latestGUID = guid;
                    }
                }
            }

            if (latestGUID != null)
                SessionState.SetBool(latestGUID, true);

            return latestReadMe;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Custom Editor
        /************************************************************************************************************************/

        [CustomEditor(typeof(ReadMe), editorForChildClasses: true)]
        protected class Editor : UnityEditor.Editor
        {
            /************************************************************************************************************************/

            [NonSerialized] private ReadMe _Target;
            [NonSerialized] private Texture2D _Icon;
            [NonSerialized] private int _PreviousVersion;
            [NonSerialized] private string _ExamplesDirectory;
            [NonSerialized] private List<ExampleGroup> _Examples;
            [NonSerialized] private string _Title;
            [NonSerialized] private string _EmailLink;
            [NonSerialized] private SerializedProperty _DontShowOnStartupProperty;

            /************************************************************************************************************************/

            /// <summary>Don't use any margins.</summary>
            public override bool UseDefaultMargins() => false;

            /************************************************************************************************************************/

            private void OnEnable()
            {
                _Target = (ReadMe)target;
                _Icon = AssetPreview.GetMiniThumbnail(target);

                var key = _Target.ReleaseNumberPrefKey;
                _PreviousVersion = PlayerPrefs.GetInt(key, -1);
                if (_PreviousVersion < 0)
                    _PreviousVersion = EditorPrefs.GetInt(key, -1);// Animancer v2.0 used EditorPrefs.

                _Examples = ExampleGroup.Gather(_Target._ExamplesFolder, out _ExamplesDirectory);

                _Title = $"{_Target.ProductName}\n{_Target.VersionName}";
                _EmailLink = $"mailto:{_Target.DeveloperEmail}?subject={_Target.ProductName.Replace(" ", "%20")}";
                _DontShowOnStartupProperty = serializedObject.FindProperty(nameof(_DontShowOnStartup));
            }

            /************************************************************************************************************************/

            protected override void OnHeaderGUI()
            {
                GUILayout.BeginHorizontal(Styles.TitleArea);
                {
                    using (ObjectPool.Disposable.AcquireContent(out var label, _Title, null, false))
                    {
                        var iconWidth = Styles.Title.CalcHeight(label, EditorGUIUtility.currentViewWidth);
                        GUILayout.Label(_Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                        GUILayout.Label(label, Styles.Title);
                    }
                }
                GUILayout.EndHorizontal();
            }

            /************************************************************************************************************************/

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DoSpace();

                DoWarnings();

                DoShowOnStartup();

                DoSpace();

                DoIntroductionBlock();

                DoSpace();

                DoExampleBlock();

                DoSpace();

                DoSupportBlock();

                DoSpace();

                DoShowOnStartup();

                serializedObject.ApplyModifiedProperties();
            }

            /************************************************************************************************************************/

            protected static void DoSpace() => GUILayout.Space(AnimancerGUI.LineHeight * 0.2f);

            /************************************************************************************************************************/

            private void DoShowOnStartup()
            {
                var area = AnimancerGUI.LayoutSingleLineRect();
                area.xMin += AnimancerGUI.LineHeight * 0.2f;

                using (ObjectPool.Disposable.AcquireContent(out var content, _DontShowOnStartupProperty, false))
                {
                    var label = EditorGUI.BeginProperty(area, content, _DontShowOnStartupProperty);
                    EditorGUI.BeginChangeCheck();
                    var value = _DontShowOnStartupProperty.boolValue;
                    value = GUI.Toggle(area, value, label);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _DontShowOnStartupProperty.boolValue = value;
                        if (value)
                            PlayerPrefs.SetInt(_Target.ReleaseNumberPrefKey, _Target.ReleaseNumber);
                    }
                    EditorGUI.EndProperty();
                }
            }

            /************************************************************************************************************************/

            private void DoWarnings()
            {
                MessageType messageType;

                if (!_Target.HasCorrectName)
                {
                    messageType = MessageType.Error;
                }
                else if (_PreviousVersion >= 0 && _PreviousVersion < _Target.ReleaseNumber)
                {
                    messageType = MessageType.Warning;
                }
                else return;

                // Upgraded from any older version.

                DoSpace();

                var directory = AssetDatabase.GetAssetPath(_Target);
                directory = Path.GetDirectoryName(directory);

                var productName = _Target.ProductName;

                string versionWarning;
                if (messageType == MessageType.Error)
                {
                    versionWarning = $"You must fully delete any old version of {productName} before importing a new version." +
                        $"\n1. Check the Upgrade Guide in the Change Log." +
                        $"\n2. Click here to delete '{directory}'." +
                        $"\n3. Import {productName} again.";
                }
                else
                {
                    versionWarning = $"You must fully delete any old version of {productName} before importing a new version." +
                        $"\n1. Ignore this message if you have already deleted the old version." +
                        $"\n2. Check the Upgrade Guide in the Change Log." +
                        $"\n3. Click here to delete '{directory}'." +
                        $"\n4. Import {productName} again.";
                }

                EditorGUILayout.HelpBox(versionWarning, messageType);
                CheckDeleteDirectory(directory);

                DoSpace();
            }

            /************************************************************************************************************************/

            /// <summary>Asks if the user wants to delete the `directory` and does so if they confirm.</summary>
            private void CheckDeleteDirectory(string directory)
            {
                if (!AnimancerGUI.TryUseClickEventInLastRect())
                    return;

                var name = _Target.ProductName;

                if (!AssetDatabase.IsValidFolder(directory))
                {
                    Debug.Log($"{directory} doesn't exist." +
                        $" You must have moved {name} somewhere else so you will need to delete it manually.", this);
                    return;
                }

                if (!EditorUtility.DisplayDialog($"Delete {name}? ",
                    $"Would you like to delete {directory}?\n\nYou will then need to reimport {name} manually.",
                    "Delete", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(directory);
            }

            /************************************************************************************************************************/

            protected virtual void DoIntroductionBlock()
            {
                GUILayout.BeginVertical(Styles.Block);

                DoHeadingLink("Documentation", null, _Target.DocumentationURL);

                DoSpace();

                DoHeadingLink("Change Log", null, _Target.ChangeLogURL);

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            protected virtual void DoExampleBlock()
            {
                GUILayout.BeginVertical(Styles.Block);

                DoHeadingLink("Examples", null, _Target.ExampleURL);
                if (_Target._ExamplesFolder != null)
                {
                    EditorGUILayout.ObjectField(_ExamplesDirectory, _Target._ExamplesFolder, typeof(SceneAsset), false);

                    ExampleGroup.DoExampleGUI(_Examples);
                }

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            protected virtual void DoSupportBlock()
            {
                GUILayout.BeginVertical(Styles.Block);

                DoHeadingLink("Forum",
                    "for general discussions, feedback, and news",
                    _Target.ForumURL);

                DoSpace();

                DoHeadingLink("Issues",
                    "for questions, suggestions, and bug reports",
                    _Target.IssuesURL);

                DoSpace();

                DoHeadingLink("Email",
                    "for anything private",
                    _EmailLink, _Target.DeveloperEmail);

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            protected void DoHeadingLink(string heading, string description, string url, string displayURL = null)
            {
                using (ObjectPool.Disposable.AcquireContent(out var label, heading, null, false))
                {
                    var size = Styles.HeaderLink.CalcSize(label);
                    var area = GUILayoutUtility.GetRect(0, size.y);

                    // Heading.

                    var headingArea = AnimancerGUI.StealFromLeft(ref area, size.x);
                    DoHeadingButton(headingArea, label, url);

                    // Description.

                    area.y += AnimancerGUI.StandardSpacing;

                    var urlHeight = Styles.URL.fontSize + Styles.URL.margin.vertical;
                    area.height -= urlHeight;

                    if (description != null)
                        GUI.Label(area, description, Styles.Description);

                    // URL.

                    area.y += area.height;
                    area.height = urlHeight;

                    if (displayURL == null)
                        displayURL = url;

                    if (displayURL != null)
                    {
                        label.text = displayURL;
                        label.tooltip = "Click to copy this link to the clipboard";
                        if (GUI.Button(area, label, Styles.URL))
                        {
                            GUIUtility.systemCopyBuffer = displayURL;
                            Debug.Log($"Copied '{displayURL}' to the clipboard.", this);
                        }

                        EditorGUIUtility.AddCursorRect(area, MouseCursor.Text);
                    }
                }
            }

            /************************************************************************************************************************/

            protected static void DoHeadingButton(Rect area, GUIContent label, string url)
            {
                if (url == null)
                {
                    GUI.Label(area, label, Styles.HeaderLabel);
                    return;
                }

                if (GUI.Button(area, label, Styles.HeaderLink))
                    Application.OpenURL(url);

                EditorGUIUtility.AddCursorRect(area, MouseCursor.Link);

                DrawLine(
                    new Vector2(area.xMin, area.yMax),
                    new Vector2(area.xMax, area.yMax),
                    Styles.HeaderLink.normal.textColor);
            }

            /************************************************************************************************************************/

            /// <summary>Draws a line between the `start` and `end` using the `color`.</summary>
            public static void DrawLine(Vector2 start, Vector2 end, Color color)
            {
                var previousColor = Handles.color;
                Handles.BeginGUI();
                Handles.color = color;
                Handles.DrawLine(start, end);
                Handles.color = previousColor;
                Handles.EndGUI();
            }

            /************************************************************************************************************************/

            /// <summary>Various <see cref="GUIStyle"/>s used by the <see cref="Editor"/>.</summary>
            protected static class Styles
            {
                /************************************************************************************************************************/

                public static readonly GUIStyle TitleArea = "In BigTitle";

                public static readonly GUIStyle Title = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 26,
                };

                public static readonly GUIStyle Block = GUI.skin.box;

                public static readonly GUIStyle HeaderLabel = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 22,
                    stretchWidth = false,
                };

                public static readonly GUIStyle HeaderLink = new GUIStyle(HeaderLabel);

                public static readonly GUIStyle Description = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerLeft,
                };

                public static readonly GUIStyle URL = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 9,
                    alignment = TextAnchor.LowerLeft,
                };

                /************************************************************************************************************************/

                static Styles()
                {
                    HeaderLink.normal.textColor = HeaderLink.hover.textColor = new Color32(0x00, 0x78, 0xDA, 0xFF);

                    URL.normal.textColor = Color.Lerp(URL.normal.textColor, Color.grey, 0.8f);
                }

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/

            /// <summary>A group of example scenes.</summary>
            private sealed class ExampleGroup
            {
                /************************************************************************************************************************/

                /// <summary>The name of this group.</summary>
                public readonly string Name;

                /// <summary>The scenes in this group.</summary>
                public readonly List<SceneAsset> Scenes = new List<SceneAsset>();

                /// <summary>The folder paths of each of the <see cref="Scenes"/>.</summary>
                public readonly List<string> Directories = new List<string>();

                /// <summary>Indicates whether this group should show its contents in the GUI.</summary>
                private bool _IsExpanded;

                /************************************************************************************************************************/

                public static List<ExampleGroup> Gather(DefaultAsset rootDirectoryAsset, out string examplesDirectory)
                {
                    if (rootDirectoryAsset == null)
                    {
                        examplesDirectory = null;
                        return null;
                    }

                    examplesDirectory = AssetDatabase.GetAssetPath(rootDirectoryAsset);
                    if (string.IsNullOrEmpty(examplesDirectory))
                        return null;

                    var directories = Directory.GetDirectories(examplesDirectory);
                    var examples = new List<ExampleGroup>();

                    for (int i = 0; i < directories.Length; i++)
                    {
                        var group = Gather(examplesDirectory, directories[i]);

                        if (group != null)
                            examples.Add(group);
                    }

                    if (examples.Count == 0)
                    {
                        var group = Gather(examplesDirectory, examplesDirectory);
                        if (group != null)
                            examples.Add(group);
                    }

                    examplesDirectory = Path.GetDirectoryName(examplesDirectory);

                    return examples;
                }

                /************************************************************************************************************************/

                public static ExampleGroup Gather(string rootDirectory, string directory)
                {
                    var files = Directory.GetFiles(directory, "*.unity", SearchOption.AllDirectories);

                    List<SceneAsset> scenes = null;

                    for (int j = 0; j < files.Length; j++)
                    {
                        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(files[j]);
                        if (scene != null)
                        {
                            if (scenes == null)
                                scenes = new List<SceneAsset>();
                            scenes.Add(scene);
                        }
                    }

                    if (scenes == null)
                        return null;

                    return new ExampleGroup(rootDirectory, directory, scenes);
                }

                /************************************************************************************************************************/

                public ExampleGroup(string rootDirectory, string directory, List<SceneAsset> scenes)
                {
                    var start = rootDirectory.Length + 1;
                    Name = start < directory.Length ?
                        directory.Substring(start, directory.Length - start) :
                        Path.GetFileName(directory);
                    Scenes = scenes;

                    start = directory.Length + 1;

                    for (int i = 0; i < scenes.Count; i++)
                    {
                        directory = AssetDatabase.GetAssetPath(scenes[i]);

                        directory = directory.Substring(start, directory.Length - start);
                        directory = Path.GetDirectoryName(directory);
                        Directories.Add(directory);
                    }
                }

                /************************************************************************************************************************/

                public static void DoExampleGUI(List<ExampleGroup> examples)
                {
                    if (examples == null)
                        return;

                    for (int i = 0; i < examples.Count; i++)
                        examples[i].DoExampleGUI();
                }

                public void DoExampleGUI()
                {
                    EditorGUI.indentLevel++;

                    using (ObjectPool.Disposable.AcquireContent(out var label, Name, null, false))
                        _IsExpanded = EditorGUILayout.Foldout(_IsExpanded, label, true);

                    if (_IsExpanded)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < Scenes.Count; i++)
                            EditorGUILayout.ObjectField(Directories[i], Scenes[i], typeof(SceneAsset), false);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

