#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static CustomHierarchy.Libs.VUtils;
using static CustomHierarchy.Libs.VGUI;
// using static VTools.VDebug;


namespace CustomHierarchy
{
    class CustomHierarchyMenu
    {

        public static bool navigationBarEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-navigationBarEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-navigationBarEnabled", value); }
        public static bool sceneSelectorEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-sceneSelectorEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-sceneSelectorEnabled", value); }
        public static bool componentMinimapEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-componentMinimapEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-componentMinimapEnabled", value); }
        public static bool activationToggleEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-acctivationToggleEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-acctivationToggleEnabled", value); }
        public static bool hierarchyLinesEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-hierarchyLinesEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-hierarchyLinesEnabled", value); }
        public static bool minimalModeEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-minimalModeEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-minimalModeEnabled", value); }
        public static bool zebraStripingEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-zebraStripingEnabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-zebraStripingEnabled", value); }

        public static bool toggleActiveEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-toggleActiveEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-toggleActiveEnabled", value); }
        public static bool focusEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-focusEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-focusEnabled", value); }
        public static bool deleteEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-deleteEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-deleteEnabled", value); }
        public static bool toggleExpandedEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-toggleExpandedEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-toggleExpandedEnabled", value); }
        public static bool isolateEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-collapseEverythingElseEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-collapseEverythingElseEnabled", value); }
        public static bool collapseEverythingEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-collapseEverythingEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-collapseEverythingEnabled", value); }
        public static bool setDefaultParentEnabled { get => EditorPrefsCached.GetBool("CustomHierarchy-setDefaultParentEnabled", true); set => EditorPrefsCached.SetBool("CustomHierarchy-setDefaultParentEnabled", value); }

        public static bool pluginDisabled { get => EditorPrefsCached.GetBool("CustomHierarchy-pluginDisabled", false); set => EditorPrefsCached.SetBool("CustomHierarchy-pluginDisabled", value); }




        const string dir = "GameBase/Custom Hierarchy/";

        const string navigationBar = dir + "Navigation bar";
        const string sceneSelector = dir + "Scene selector";
        const string componentMinimap = dir + "Component minimap";
        const string activationToggle = dir + "Activation toggle";
        const string hierarchyLines = dir + "Hierarchy lines";
        const string zebraStriping = dir + "Zebra striping";
        const string minimalMode = dir + "Minimal mode";

        const string toggleActive = dir + "A to toggle active";
        const string focus = dir + "F to focus";
        const string delete = dir + "X to delete";
        const string toggleExpanded = dir + "E to expand or collapse";
        const string isolate = dir + "Shift-E to isolate";
        const string collapseEverything = dir + "Ctrl-Shift-E to collapse all";
        const string setDefaultParent = dir + "D to set default parent";

        const string disablePlugin = dir + "Disable CustomHierarchy";

        

        [MenuItem(navigationBar, false, 2)] static void dadsaadsdsadasdsadadsas() { navigationBarEnabled = !navigationBarEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(navigationBar, true, 2)] static bool dadsaddasdsasadadsdasadsas() { Menu.SetChecked(navigationBar, navigationBarEnabled); return !pluginDisabled; }

        [MenuItem(sceneSelector, false, 3)] static void dadsaadsdsadassddsadadsas() { sceneSelectorEnabled = !sceneSelectorEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(sceneSelector, true, 3)] static bool dadsaddasdsasadsdadsdasadsas() { Menu.SetChecked(sceneSelector, sceneSelectorEnabled); return !pluginDisabled; }

        [MenuItem(componentMinimap, false, 4)] static void daadsdsadasdadsas() { componentMinimapEnabled = !componentMinimapEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(componentMinimap, true, 4)] static bool dadsadasddasadsas() { Menu.SetChecked(componentMinimap, componentMinimapEnabled); return !pluginDisabled; }

        [MenuItem(activationToggle, false, 5)] static void daadsdsadadsasdadsas() { activationToggleEnabled = !activationToggleEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(activationToggle, true, 5)] static bool dadsadasdsaddasadsas() { Menu.SetChecked(activationToggle, activationToggleEnabled); return !pluginDisabled; }

        [MenuItem(hierarchyLines, false, 6)] static void dadsadadsadadasss() { hierarchyLinesEnabled = !hierarchyLinesEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(hierarchyLines, true, 6)] static bool dadsaddasdasaasddsas() { Menu.SetChecked(hierarchyLines, hierarchyLinesEnabled); return !pluginDisabled; }

        [MenuItem(zebraStriping, false, 7)] static void dadsadadadssadsadass() { zebraStripingEnabled = !zebraStripingEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(zebraStriping, true, 7)] static bool dadsaddadaadsssadsaasddsas() { Menu.SetChecked(zebraStriping, zebraStripingEnabled); return !pluginDisabled; }

        [MenuItem(minimalMode, false, 8)] static void dadsadadasdsdasadadasss() { minimalModeEnabled = !minimalModeEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(minimalMode, true, 8)] static bool dadsaddadsasdadsasaasddsas() { Menu.SetChecked(minimalMode, minimalModeEnabled); return !pluginDisabled; }






        [MenuItem(dir + "Shortcuts", false, 101)] static void dadsas() { }
        [MenuItem(dir + "Shortcuts", true, 101)] static bool dadsas123() => false;



        [MenuItem(setDefaultParent, false, 102)] static void dadsadasdsdasadasdsadadsas() => setDefaultParentEnabled = !setDefaultParentEnabled;
        [MenuItem(setDefaultParent, true, 102)] static bool dadsadsdadssdaasadadsdasadsas() { Menu.SetChecked(setDefaultParent, setDefaultParentEnabled); return !pluginDisabled; }


        [MenuItem(toggleActive, false, 103)] static void dadsadadsas() => toggleActiveEnabled = !toggleActiveEnabled;
        [MenuItem(toggleActive, true, 103)] static bool dadsaddasadsas() { Menu.SetChecked(toggleActive, toggleActiveEnabled); return !pluginDisabled; }

        [MenuItem(focus, false, 104)] static void dadsadasdadsas() => focusEnabled = !focusEnabled;
        [MenuItem(focus, true, 104)] static bool dadsadsaddasadsas() { Menu.SetChecked(focus, focusEnabled); return !pluginDisabled; }

        [MenuItem(delete, false, 105)] static void dadsadsadasdadsas() => deleteEnabled = !deleteEnabled;
        [MenuItem(delete, true, 105)] static bool dadsaddsasaddasadsas() { Menu.SetChecked(delete, deleteEnabled); return !pluginDisabled; }

        [MenuItem(toggleExpanded, false, 106)] static void dadsadsadasdsadadsas() => toggleExpandedEnabled = !toggleExpandedEnabled;
        [MenuItem(toggleExpanded, true, 106)] static bool dadsaddsasadadsdasadsas() { Menu.SetChecked(toggleExpanded, toggleExpandedEnabled); return !pluginDisabled; }

        [MenuItem(isolate, false, 107)] static void dadsadsasdadasdsadadsas() => isolateEnabled = !isolateEnabled;
        [MenuItem(isolate, true, 107)] static bool dadsaddsdasasadadsdasadsas() { Menu.SetChecked(isolate, isolateEnabled); return !pluginDisabled; }

        [MenuItem(collapseEverything, false, 108)] static void dadsadsdasadasdsadadsas() => collapseEverythingEnabled = !collapseEverythingEnabled;
        [MenuItem(collapseEverything, true, 108)] static bool dadsaddssdaasadadsdasadsas() { Menu.SetChecked(collapseEverything, collapseEverythingEnabled); return !pluginDisabled; }

        [MenuItem(disablePlugin, false, 10001)] static void dadsadsdasadasdasdsadadsas() { pluginDisabled = !pluginDisabled; UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); }
        [MenuItem(disablePlugin, true, 10001)] static bool dadsaddssdaasadsadadsdasadsas() { Menu.SetChecked(disablePlugin, pluginDisabled); return true; }





    }
}
#endif