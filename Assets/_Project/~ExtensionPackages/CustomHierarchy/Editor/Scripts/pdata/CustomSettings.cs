using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using customtools.customhierarchy.phierarchy;
using System.Text;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pdata
{
	public enum CustomSetting
	{
		TreeMapShow                                 = 0,
        TreeMapColor                                = 77,
        TreeMapEnhanced                             = 78,
        TreeMapTransparentBackground                = 60,

        MonoBehaviourIconShow                       = 4,
        MonoBehaviourIconShowDuringPlayMode         = 18,
        MonoBehaviourIconIgnoreUnityMonobehaviour   = 45,
        MonoBehaviourIconColor                      = 82,

        SeparatorShow                               = 8,
        SeparatorShowRowShading                     = 50,
        SeparatorColor                              = 80,
        SeparatorEvenRowShadingColor                = 79,       
        SeparatorOddRowShadingColor                 = 81,       

		VisibilityShow                              = 1,
        VisibilityShowDuringPlayMode                = 15,

		LockShow                                    = 2,
        LockShowDuringPlayMode                      = 16,
        LockPreventSelectionOfLockedObjects         = 41,

        StaticShow                                  = 12,
        StaticShowDuringPlayMode                    = 25,

        ErrorShow                                   = 6,
        ErrorShowDuringPlayMode                     = 20,
        ErrorShowIconOnParent                       = 27,
        ErrorShowScriptIsMissing                    = 28,
        ErrorShowReferenceIsNull                    = 29,
        ErrorShowReferenceIsMissing                 = 58,
        ErrorShowStringIsEmpty                      = 30,
        ErrorShowMissingEventMethod                 = 31,
        ErrorShowWhenTagOrLayerIsUndefined          = 32,
        ErrorIgnoreString                           = 33,
        ErrorShowForDisabledComponents              = 44,
        ErrorShowForDisabledGameObjects             = 59,

        RendererShow                                = 7,
        RendererShowDuringPlayMode                  = 21,

        PrefabShow                                  = 13,
        PrefabShowBreakedPrefabsOnly                = 51,

		TagAndLayerShow                             = 5,
        TagAndLayerShowDuringPlayMode               = 19,
        TagAndLayerSizeShowType                     = 68,
        TagAndLayerType                             = 34,
        TagAndLayerSizeType                         = 35,
        TagAndLayerSizeValuePixel                   = 36,
        TagAndLayerAligment                         = 37,
        TagAndLayerSizeValueType                    = 46,
        TagAndLayerSizeValuePercent                 = 47,
        TagAndLayerLabelSize                        = 48,
        TagAndLayerTagLabelColor                    = 66,
        TagAndLayerLayerLabelColor                  = 67,
        TagAndLayerLabelAlpha                       = 69,

        ColorShow                                   = 9,
        ColorShowDuringPlayMode                     = 22,

        GameObjectIconShow                          = 3,
        GameObjectIconShowDuringPlayMode            = 17,
        GameObjectIconSize                          = 63,

        TagIconShow                                 = 14,
        TagIconShowDuringPlayMode                   = 26,
        TagIconListFoldout                          = 84,
        TagIconList                                 = 40,
        TagIconSize                                 = 62,

        LayerIconShow                               = 85,
        LayerIconShowDuringPlayMode                 = 86,
        LayerIconListFoldout                        = 87,
        LayerIconList                               = 88,
        LayerIconSize                               = 89,

        ChildrenCountShow                           = 11,
        ChildrenCountShowDuringPlayMode             = 24,
        ChildrenCountLabelSize                      = 61,
        ChildrenCountLabelColor                     = 70,

        VerticesAndTrianglesShow                    = 53,
        VerticesAndTrianglesShowDuringPlayMode      = 54,
        VerticesAndTrianglesCalculateTotalCount     = 55,
        VerticesAndTrianglesShowTriangles           = 56, 
        VerticesAndTrianglesShowVertices            = 64, 
        VerticesAndTrianglesLabelSize               = 57,
        VerticesAndTrianglesVerticesLabelColor      = 71,
        VerticesAndTrianglesTrianglesLabelColor     = 72,

        ComponentsShow                              = 10,
        ComponentsShowDuringPlayMode                = 23,
        ComponentsIconSize                          = 65,
        ComponentsIgnore                            = 90,

		ComponentsOrder                             = 38,

        AdditionalIdentation                        = 39,
        AdditionalShowHiddenCustomHierarchyObjectList    = 42,
        AdditionalShowModifierWarning               = 43,
        AdditionalShowObjectListContent             = 49,
        AdditionalHideIconsIfNotFit                 = 52,  
        AdditionalBackgroundColor                   = 73,
        AdditionalActiveColor                       = 74,
        AdditionalInactiveColor                     = 75,
        AdditionalSpecialColor                      = 76,
	}
	
	public enum CustomHierarchyTagAndLayerType
	{
		Always           = 0,
		OnlyIfNotDefault = 1
	}

    public enum CustomHierarchyTagAndLayerShowType
    {
        TagAndLayer = 0,
        Tag         = 1,
        Layer       = 2
    }

    public enum CustomHierarchyTagAndLayerAligment
    {
        Left   = 0,
        Center = 1,
        Right  = 2
    }

    public enum CustomHierarchyTagAndLayerSizeType
    {
        Pixel   = 0,
        Percent = 1
    }

    public enum CustomHierarchyTagAndLayerLabelSize
    {
        Normal                          = 0,
        Big                             = 1,
        BigIfSpecifiedOnlyTagOrLayer    = 2
    }

    public enum CustomHierarchySize
    {
        Normal  = 0,
        Big     = 1
    }
        
    public enum CustomHierarchySizeAll
    {
        Small   = 0,
        Normal  = 1,
        Big     = 2
    }

	public enum CustomHierarchyComponentEnum
	{
        LockComponent               = 0,
        VisibilityComponent         = 1,
        StaticComponent             = 2,
        ColorComponent              = 3,
        ErrorComponent              = 4,
        RendererComponent           = 5,
        PrefabComponent             = 6,
        TagAndLayerComponent        = 7,
        GameObjectIconComponent     = 8,
        TagIconComponent            = 9,
        LayerIconComponent          = 10,
        ChildrenCountComponent      = 11,
        VerticesAndTrianglesCount   = 12,
        SeparatorComponent          = 1000,
        TreeMapComponent            = 1001,
        MonoBehaviourIconComponent  = 1002,
        ComponentsComponent         = 1003
	}

    public class CustomTagTexture
    {
        public string tag;
        public Texture2D texture;
        
        public CustomTagTexture(string tag, Texture2D texture)
        {
            this.tag = tag;
            this.texture = texture;
        }

        public static List<CustomTagTexture> LoadTagTextureList()
        {
            List<CustomTagTexture> tagTextureList = new List<CustomTagTexture>();
            string customTagIcon = CustomSettings.getInstance().get<string>(CustomSetting.TagIconList);
            string[] customTagIconArray = customTagIcon.Split(new char[]{';'});
            List<string> tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            for (int i = 0; i < customTagIconArray.Length - 1; i+=2)
            {
                string tag = customTagIconArray[i];
                if (!tags.Contains(tag)) continue;
                string texturePath = customTagIconArray[i+1];
                
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (texture != null) 
                { 
                    CustomTagTexture tagTexture = new CustomTagTexture(tag, texture);
                    tagTextureList.Add(tagTexture);
                }  
            }
            return tagTextureList;
        }

        public static void SaveTagTextureList(CustomSetting setting, List<CustomTagTexture> tagTextureList)
        { 
            string result = "";
            for (int i = 0; i < tagTextureList.Count; i++)            
                result += tagTextureList[i].tag + ";" + AssetDatabase.GetAssetPath(tagTextureList[i].texture.GetInstanceID()) + ";";
            CustomSettings.getInstance().set(setting, result);
        }
    }

    public class CustomLayerTexture
    {
        public string layer;
        public Texture2D texture;
        
        public CustomLayerTexture(string layer, Texture2D texture)
        {
            this.layer = layer;
            this.texture = texture;
        }
        
        public static List<CustomLayerTexture> LoadLayerTextureList()
        {
            List<CustomLayerTexture> layerTextureList = new List<CustomLayerTexture>();
            string customTagIcon = CustomSettings.getInstance().get<string>(CustomSetting.LayerIconList);
            string[] customLayerIconArray = customTagIcon.Split(new char[]{';'});
            List<string> layers = new List<string>(UnityEditorInternal.InternalEditorUtility.layers);
            for (int i = 0; i < customLayerIconArray.Length - 1; i+=2)
            {
                string layer = customLayerIconArray[i];
                if (!layers.Contains(layer)) continue;
                string texturePath = customLayerIconArray[i+1];
                
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (texture != null) 
                { 
                    CustomLayerTexture tagTexture = new CustomLayerTexture(layer, texture);
                    layerTextureList.Add(tagTexture);
                }  
            }
            return layerTextureList;
        }
        
        public static void SaveLayerTextureList(CustomSetting setting, List<CustomLayerTexture> layerTextureList)
        { 
            string result = "";
            for (int i = 0; i < layerTextureList.Count; i++)            
                result += layerTextureList[i].layer + ";" + AssetDatabase.GetAssetPath(layerTextureList[i].texture.GetInstanceID()) + ";";
            CustomSettings.getInstance().set(setting, result);
        }
    }

    public delegate void CustomSettingChangedHandler();

	public class CustomSettings 
	{
        // CONST
		private const string PREFS_PREFIX = "CustomTools.CustomHierarchy_";
        private const string PREFS_DARK = "Dark_";
        private const string PREFS_LIGHT = "Light_";
        public const string DEFAULT_ORDER = "0;1;2;3;4;5;6;7;8;9;10;11;12";
        public const int DEFAULT_ORDER_COUNT = 13;
        private const string SETTINGS_FILE_NAME = "CustomSettingsObjectAsset";

        // PRIVATE
        private CustomSettingsObject settingsObject;
        private Dictionary<int, object> defaultSettings = new Dictionary<int, object>();
        private HashSet<int> skinDependedSettings = new HashSet<int>();
        private Dictionary<int, CustomSettingChangedHandler> settingChangedHandlerList = new Dictionary<int, CustomSettingChangedHandler>();

        // SINGLETON
        private static CustomSettings instance;
        public static CustomSettings getInstance()
        {
            if (instance == null) instance = new CustomSettings();
            return instance;
        }

        // CONSTRUCTOR
		private CustomSettings()
		{ 
            string[] paths = AssetDatabase.FindAssets(SETTINGS_FILE_NAME); 
            for (int i = 0; i < paths.Length; i++)
            {
                settingsObject = (CustomSettingsObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(CustomSettingsObject));
                if (settingsObject != null) break;
            }
            if (settingsObject == null) 
            {
                settingsObject = ScriptableObject.CreateInstance<CustomSettingsObject>();
                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(settingsObject));
                path = path.Substring(0, path.LastIndexOf("/"));
                AssetDatabase.CreateAsset(settingsObject, path + "/" + SETTINGS_FILE_NAME + ".asset");
                AssetDatabase.SaveAssets();
            }  

            initSetting(CustomSetting.TreeMapShow                                , true);
            initSetting(CustomSetting.TreeMapColor                               , "39FFFFFF", "905D5D5D");
            initSetting(CustomSetting.TreeMapEnhanced                            , true);
            initSetting(CustomSetting.TreeMapTransparentBackground               , true);

            initSetting(CustomSetting.MonoBehaviourIconShow                      , true);
            initSetting(CustomSetting.MonoBehaviourIconShowDuringPlayMode        , true);
            initSetting(CustomSetting.MonoBehaviourIconIgnoreUnityMonobehaviour  , true);
            initSetting(CustomSetting.MonoBehaviourIconColor                     , "A01B6DBB");

            initSetting(CustomSetting.SeparatorShow                              , true);
            initSetting(CustomSetting.SeparatorShowRowShading                    , true);
            initSetting(CustomSetting.SeparatorColor                             , "FF303030", "48666666");
            initSetting(CustomSetting.SeparatorEvenRowShadingColor               , "13000000", "08000000");
            initSetting(CustomSetting.SeparatorOddRowShadingColor                , "00000000", "00FFFFFF");

            initSetting(CustomSetting.VisibilityShow                             , true);
            initSetting(CustomSetting.VisibilityShowDuringPlayMode               , true);

            initSetting(CustomSetting.LockShow                                   , true);
            initSetting(CustomSetting.LockShowDuringPlayMode                     , false);
            initSetting(CustomSetting.LockPreventSelectionOfLockedObjects        , false);

            initSetting(CustomSetting.StaticShow                                 , true); 
            initSetting(CustomSetting.StaticShowDuringPlayMode                   , false);

            initSetting(CustomSetting.ErrorShow                                  , true);
            initSetting(CustomSetting.ErrorShowDuringPlayMode                    , false);
            initSetting(CustomSetting.ErrorShowIconOnParent                      , false);
            initSetting(CustomSetting.ErrorShowScriptIsMissing                   , true);
            initSetting(CustomSetting.ErrorShowReferenceIsNull                   , false);
            initSetting(CustomSetting.ErrorShowReferenceIsMissing                , true);
            initSetting(CustomSetting.ErrorShowStringIsEmpty                     , false);
            initSetting(CustomSetting.ErrorShowMissingEventMethod                , true);
            initSetting(CustomSetting.ErrorShowWhenTagOrLayerIsUndefined         , true);
            initSetting(CustomSetting.ErrorIgnoreString                          , "");
            initSetting(CustomSetting.ErrorShowForDisabledComponents             , true);
            initSetting(CustomSetting.ErrorShowForDisabledGameObjects            , true);

            initSetting(CustomSetting.RendererShow                               , false);
            initSetting(CustomSetting.RendererShowDuringPlayMode                 , false);

            initSetting(CustomSetting.PrefabShow                                 , false);
            initSetting(CustomSetting.PrefabShowBreakedPrefabsOnly               , true);

            initSetting(CustomSetting.TagAndLayerShow                            , true);
            initSetting(CustomSetting.TagAndLayerShowDuringPlayMode              , true);
            initSetting(CustomSetting.TagAndLayerSizeShowType                    , (int)CustomHierarchyTagAndLayerShowType.TagAndLayer);
            initSetting(CustomSetting.TagAndLayerType                            , (int)CustomHierarchyTagAndLayerType.OnlyIfNotDefault);
            initSetting(CustomSetting.TagAndLayerAligment                        , (int)CustomHierarchyTagAndLayerAligment.Left);
            initSetting(CustomSetting.TagAndLayerSizeValueType                   , (int)CustomHierarchyTagAndLayerSizeType.Pixel);
            initSetting(CustomSetting.TagAndLayerSizeValuePercent                , 0.25f);
            initSetting(CustomSetting.TagAndLayerSizeValuePixel                  , 75);
            initSetting(CustomSetting.TagAndLayerLabelSize                       , (int)CustomHierarchyTagAndLayerLabelSize.Normal);
            initSetting(CustomSetting.TagAndLayerTagLabelColor                   , "FFCCCCCC", "FF333333");
            initSetting(CustomSetting.TagAndLayerLayerLabelColor                 , "FFCCCCCC", "FF333333");
            initSetting(CustomSetting.TagAndLayerLabelAlpha                      , 0.35f);

            initSetting(CustomSetting.ColorShow                                  , true);
            initSetting(CustomSetting.ColorShowDuringPlayMode                    , true);

            initSetting(CustomSetting.GameObjectIconShow                         , false);
            initSetting(CustomSetting.GameObjectIconShowDuringPlayMode           , true);
            initSetting(CustomSetting.GameObjectIconSize                         , (int)CustomHierarchySizeAll.Small);

            initSetting(CustomSetting.TagIconShow                                , false);
            initSetting(CustomSetting.TagIconShowDuringPlayMode                  , true);
            initSetting(CustomSetting.TagIconListFoldout                         , false);
            initSetting(CustomSetting.TagIconList                                , "");
            initSetting(CustomSetting.TagIconSize                                , (int)CustomHierarchySizeAll.Small);

            initSetting(CustomSetting.LayerIconShow                              , false);
            initSetting(CustomSetting.LayerIconShowDuringPlayMode                , true);
            initSetting(CustomSetting.LayerIconListFoldout                       , false);
            initSetting(CustomSetting.LayerIconList                              , "");
            initSetting(CustomSetting.LayerIconSize                              , (int)CustomHierarchySizeAll.Small);

            initSetting(CustomSetting.ChildrenCountShow                          , false);     
            initSetting(CustomSetting.ChildrenCountShowDuringPlayMode            , true);
            initSetting(CustomSetting.ChildrenCountLabelSize                     , (int)CustomHierarchySize.Normal);
            initSetting(CustomSetting.ChildrenCountLabelColor                    , "FFCCCCCC", "FF333333");

            initSetting(CustomSetting.VerticesAndTrianglesShow                   , false);
            initSetting(CustomSetting.VerticesAndTrianglesShowDuringPlayMode     , false);
            initSetting(CustomSetting.VerticesAndTrianglesCalculateTotalCount    , false);
            initSetting(CustomSetting.VerticesAndTrianglesShowTriangles          , false);
            initSetting(CustomSetting.VerticesAndTrianglesShowVertices           , true);
            initSetting(CustomSetting.VerticesAndTrianglesLabelSize              , (int)CustomHierarchySize.Normal);
            initSetting(CustomSetting.VerticesAndTrianglesVerticesLabelColor     , "FFCCCCCC", "FF333333");
            initSetting(CustomSetting.VerticesAndTrianglesTrianglesLabelColor    , "FFCCCCCC", "FF333333");

            initSetting(CustomSetting.ComponentsShow                             , false);
            initSetting(CustomSetting.ComponentsShowDuringPlayMode               , false);
            initSetting(CustomSetting.ComponentsIconSize                         , (int)CustomHierarchySizeAll.Small);
            initSetting(CustomSetting.ComponentsIgnore                           , "");

            initSetting(CustomSetting.ComponentsOrder                            , DEFAULT_ORDER);

            initSetting(CustomSetting.AdditionalShowObjectListContent            , false);
            initSetting(CustomSetting.AdditionalShowHiddenCustomHierarchyObjectList   , true);
            initSetting(CustomSetting.AdditionalHideIconsIfNotFit                , true);
            initSetting(CustomSetting.AdditionalIdentation                       , 0);
            initSetting(CustomSetting.AdditionalShowModifierWarning              , true);

            #if UNITY_2019_1_OR_NEWER
            initSetting(CustomSetting.AdditionalBackgroundColor                  , "00383838", "00CFCFCF");
            #else
            initSetting(CustomSetting.AdditionalBackgroundColor                  , "00383838", "00C2C2C2");
            #endif
            initSetting(CustomSetting.AdditionalActiveColor                      , "FFFFFF80", "CF363636");
            initSetting(CustomSetting.AdditionalInactiveColor                    , "FF4F4F4F", "1E000000");
            initSetting(CustomSetting.AdditionalSpecialColor                     , "FF2CA8CA", "FF1D78D5");
		} 

        // DESTRUCTOR
        public void OnDestroy()
        {
            skinDependedSettings = null;
            defaultSettings = null;
            settingsObject = null;
            settingChangedHandlerList = null;
            instance = null;
        }

        // PUBLIC
        public T get<T>(CustomSetting setting)
        {
            return (T)settingsObject.GET<T>(getSettingName(setting));
        }

        public Color getColor(CustomSetting setting)
        {
            string stringColor = (string)settingsObject.GET<string>(getSettingName(setting));
            return CustomColorUtils.fromString(stringColor);
        }

        public void setColor(CustomSetting setting, Color color)
        {
            string stringColor = CustomColorUtils.toString(color);
            set(setting, stringColor);
        }

        public void set<T>(CustomSetting setting, T value, bool invokeChanger = true)
        {
            int settingId = (int)setting;
            settingsObject.Set(getSettingName(setting), value);

            if (invokeChanger && settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)            
                settingChangedHandlerList[settingId].Invoke();
            
            EditorApplication.RepaintHierarchyWindow();
        }

        public void addEventListener(CustomSetting setting, CustomSettingChangedHandler handler)
        {
            int settingId = (int)setting;
            
            if (!settingChangedHandlerList.ContainsKey(settingId))          
                settingChangedHandlerList.Add(settingId, null);
            
            if (settingChangedHandlerList[settingId] == null)           
                settingChangedHandlerList[settingId] = handler;
            else            
                settingChangedHandlerList[settingId] += handler;
        }
        
        public void removeEventListener(CustomSetting setting, CustomSettingChangedHandler handler)
        {
            int settingId = (int)setting;
            
            if (settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)       
                settingChangedHandlerList[settingId] -= handler;
        }
        
        public void restore(CustomSetting setting)
        {
            set(setting, defaultSettings[(int)setting]);
        }

        // PRIVATE
        private void initSetting(CustomSetting setting, object defaultValueDark, object defaultValueLight)
        {
            skinDependedSettings.Add((int)setting);
            initSetting(setting, EditorGUIUtility.isProSkin ? defaultValueDark : defaultValueLight);
        }
        
        private void initSetting(CustomSetting setting, object defaultValue)
        {
            string settingName = getSettingName(setting);
            defaultSettings.Add((int)setting, defaultValue);
            object value = settingsObject.Get(settingName, defaultValue);
            if (value == null || value.GetType() != defaultValue.GetType())
            {
                settingsObject.Set(settingName, defaultValue);
            }        
        }

        private string getSettingName(CustomSetting setting)
        {
            int settingId = (int)setting;
            string settingName = PREFS_PREFIX;
            if (skinDependedSettings.Contains(settingId))            
                settingName += EditorGUIUtility.isProSkin ? PREFS_DARK : PREFS_LIGHT;            
            settingName += setting.ToString("G");
            return settingName.ToString();
        }
	}
}