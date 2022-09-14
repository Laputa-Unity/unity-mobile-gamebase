#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ActivePrefabCategoryView : EntityView
    {
        #region Private Constant Variables
        private const float _actionButtonScale = 0.8f;
        private const float _categoryActionBtnSize = 155.0f;
        #endregion

        #region Private Variables
        [SerializeField]
        private ActivePrefabCategoryViewData _viewData;

        [SerializeField]
        private PrefabCategoryPrefabScrollView _prefabScrollView = new PrefabCategoryPrefabScrollView();

        [NonSerialized]
        private List<string> _categoryNames = new List<string>();
        [NonSerialized]
        private int _selectedCategoryIndex = -1;
        #endregion

        #region Private Properties
        private ActivePrefabCategoryViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ActivePrefabCategoryViewData>();
                return _viewData;
            }
        }
        #endregion

        public PrefabCategoryPrefabScrollView PrefabScrollView { get { return _prefabScrollView; } }

        #region Constructors
        public ActivePrefabCategoryView()
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Active Prefab Category View";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            var content = new GUIContent();

            _prefabScrollView.PrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            EditorGUILayout.BeginHorizontal();
            RenderShowPrefabCategoryFolderNamesToggle();
            RenderShowHintsToggle();
            EditorGUILayout.EndHorizontal();

            if (ViewData.ShowHints)
            {
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("Note: When a config is loaded, the tool will use the prefab path to load the prefab back. If the prefab's path has changed (e.g. moved to " +
                                    "a different folder, or changed the name of a folder inside the folder hierarchy, or changed the name of the prefab asset etc), the prefab will not be loaded.");
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("When a config is saved, all prefab categories (together with their prefabs) and prefab tags are saved. When a config is loaded, " +
                                                          "all prefab categories and tags are removed and replaced with the ones which are loaded from the config file. Also, any missing object groups " +
                                                          "referenced by prefab categories, will automatically be created on load.");
                EditorGUILayout.HelpBox("You can right click inside the prefab management window to bring up a context menu that will allow you to manage prefabs and prefab categories (e.g. remove, clear etc).", UnityEditor.MessageType.Warning);
            }

            if (ViewData.ShowPrefabCategoryFolderNames) RenderMaxNumberOfCategoryFolderNamesToggle();
            RenderActiveCategorySelectionPopup();
            RenderActiveCategoryNameChangeTextField();
            RenderObjectGroupSelectionPopup();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            RenderPrefabsToActiveCategoryDropOperationSettingsButton();
            RenderCreateNewCategoryControls();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderSavePrefabConfigButton();
            RenderLoadPrefabConfigButton();

            content.text = "Look and feel...";
            content.tooltip = "Opens up a new window which allows you to control the look and feel of the prefab view area.";
            if(GUILayout.Button(content, GUILayout.Width(_categoryActionBtnSize)))
            {
                _prefabScrollView.LookAndFeelWindow.ShowOctave3DWindow();
            }
            EditorGUILayout.EndHorizontal();

            _prefabScrollView.Render();
            RenderActionsView();
        }
        #endregion

        #region Private Methods
        private void RenderShowPrefabCategoryFolderNamesToggle()
        {
            var content = new GUIContent();
            content.text = "Show folder names (where applicable)";
            content.tooltip = "If this is checked, prefab categories which were created from folder drops will have their name adjusted " + 
                              "to include the names of the folders which appear in the source/dropped folder path. Folders appear from " + 
                              "left to right starting from the bottom most folder in the hierarchy.";

            bool newBool = EditorGUILayout.ToggleLeft(content, ViewData.ShowPrefabCategoryFolderNames, GUILayout.Width(250.0f));
            if(newBool != ViewData.ShowPrefabCategoryFolderNames)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.ShowPrefabCategoryFolderNames = newBool;
            }
        }

        private void RenderShowHintsToggle()
        {
            var content = new GUIContent();
            content.text = "Show hints";
            content.tooltip = "Show/hide hints.";

            EditorGUI.BeginChangeCheck();
            bool val = EditorGUILayout.ToggleLeft(content, ViewData.ShowHints);
            if (EditorGUI.EndChangeCheck())
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.ShowHints = val;
            }
        }

        private void RenderMaxNumberOfCategoryFolderNamesToggle()
        {
            var content = new GUIContent();
            content.text = "Max number of folders";
            content.tooltip = "If \'Show folder names\' is checked, this controls the maximum number of folder names that will be shown.";

            int newInt = EditorGUILayout.IntField(content, ViewData.MaxNumberOfCategoryFolderNames);
            if (newInt != ViewData.MaxNumberOfCategoryFolderNames)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.MaxNumberOfCategoryFolderNames = newInt;
            }
        }

        private void RenderActiveCategorySelectionPopup()
        {
            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            if (prefabCategoryDatabase.NumberOfCategories != _categoryNames.Count ||
                _selectedCategoryIndex < 0) RefreshCategoryNames();

            if (prefabCategoryDatabase.ActivePrefabCategory != null &&
                prefabCategoryDatabase.ActivePrefabCategory.Name != _categoryNames[_selectedCategoryIndex])
                RefreshCategoryNames();

            int selectedIndex = EditorGUILayout.Popup(GetContentForActivePrefabCategorySelectionPopup(),
                                    _selectedCategoryIndex, _categoryNames.ToArray());

            if (ViewData.ShowPrefabCategoryFolderNames)
            {
                string newActiveCategoryName = _categoryNames[selectedIndex];
                if ((prefabCategoryDatabase.ActivePrefabCategory == null && !string.IsNullOrEmpty(newActiveCategoryName)) ||
                     newActiveCategoryName != prefabCategoryDatabase.ActivePrefabCategory.GetNameWithConcatFolderNames(ViewData.MaxNumberOfCategoryFolderNames))
                {
                    UndoEx.RecordForToolAction(prefabCategoryDatabase);
                    prefabCategoryDatabase.SetActivePrefabCategory(prefabCategoryDatabase.GetPrefabCategoryByNameWithFolders(newActiveCategoryName, ViewData.MaxNumberOfCategoryFolderNames));
                    CalcSelectedCategoryItemIndex();
                }
            }
            else
            {
                string newActiveCategoryName = _categoryNames[selectedIndex];
                if ((prefabCategoryDatabase.ActivePrefabCategory == null && !string.IsNullOrEmpty(newActiveCategoryName)) ||
                     newActiveCategoryName != prefabCategoryDatabase.ActivePrefabCategory.Name)
                {
                    UndoEx.RecordForToolAction(prefabCategoryDatabase);
                    prefabCategoryDatabase.SetActivePrefabCategory(prefabCategoryDatabase.GetPrefabCategoryByName(newActiveCategoryName));
                    CalcSelectedCategoryItemIndex();
                }
            }
        }

        private void RefreshCategoryNames()
        {
            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            if (ViewData.ShowPrefabCategoryFolderNames)
                prefabCategoryDatabase.GetAllPrefabCategoryNamesWithFolders(ViewData.MaxNumberOfCategoryFolderNames, _categoryNames);
            else prefabCategoryDatabase.GetAllPrefabCategoryNames(_categoryNames);

            _categoryNames.Sort(delegate (string s0, string s1)
            { return s0.CompareTo(s1); });

            CalcSelectedCategoryItemIndex();
        }

        private void CalcSelectedCategoryItemIndex()
        {
            var activeCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            if (activeCategory != null)
                _selectedCategoryIndex = _categoryNames.IndexOf(activeCategory.Name);
        }

        private GUIContent GetContentForActivePrefabCategorySelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active category";
            content.tooltip = "Allows you to change the active prefab category.";

            return content;
        }

        private void RenderActiveCategoryNameChangeTextField()
        {
            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            PrefabCategory activeCategory = prefabCategoryDatabase.ActivePrefabCategory;

            if (prefabCategoryDatabase.CanPrefabCategoryBeRenamed(activeCategory))
            {
                string newString = EditorGUILayoutEx.DelayedTextField(GetContentForActiveCategoryNameChangeField(), activeCategory.Name);
                if (newString != activeCategory.Name)
                {
                    UndoEx.RecordForToolAction(activeCategory);
                    prefabCategoryDatabase.RenamePrefabCategory(activeCategory, newString);
                }
            }
            else EditorGUILayout.HelpBox("The default category can not be renamed.", UnityEditor.MessageType.None);
        }

        private GUIContent GetContentForActiveCategoryNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name:";
            content.tooltip = "Allows you to change the name of the active prefab category. Note: The default category can not be renamed.";

            return content;
        }

        private void RenderObjectGroupSelectionPopup()
        {
            ObjectGroupDatabase objectGroupDatabase = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase;
            if(objectGroupDatabase.NumberOfGroups == 0)
            {
                EditorGUILayout.HelpBox("No object groups are currently available.", UnityEditor.MessageType.None);
                return;
            }

            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            PrefabCategory activeCategory = prefabCategoryDatabase.ActivePrefabCategory;

            List<ObjectGroup> allObjectGroups = objectGroupDatabase.GetAllObjectGroups();
            if (activeCategory.ObjectGroup == null) activeCategory.SetObjectGroup(allObjectGroups[0]);

            int currentGroupIndex =  allObjectGroups.FindIndex(0, item => item == activeCategory.ObjectGroup);
            if(currentGroupIndex < 0) return;

            int newGroupIndex = EditorGUILayoutEx.Popup(GetContentForObjectGroupSelectionPopup(), currentGroupIndex, objectGroupDatabase.GetAllObjectGroupNames());    
            if(newGroupIndex != currentGroupIndex)
            {
                UndoEx.RecordForToolAction(activeCategory);
                activeCategory.SetObjectGroup(allObjectGroups[newGroupIndex]);
            }
        }

        private GUIContent GetContentForObjectGroupSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Object group";
            content.tooltip = "The object group which must be associated with all prefabs in this category.";

            return content;
        }

        private void RenderPrefabsToActiveCategoryDropOperationSettingsButton()
        {
            if (GUILayout.Button(GetContentForPrefabsToActiveCategoryDropOperationSettingsButton(), GUILayout.Width(_categoryActionBtnSize)))
            {
                PrefabsToCategoryDropSettingsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForPrefabsToActiveCategoryDropOperationSettingsButton()
        {
            var content = new GUIContent();
            content.text = "Prefab drop settings...";
            content.tooltip = "Opens a new window which allows you to specify different settings related to prefab and prefab folder drop actions.";
           
            return content;
        }

        private void RenderSavePrefabConfigButton()
        {
            if (GUILayout.Button(GetContentForSavePrefabConfigButton(), GUILayout.Width(_categoryActionBtnSize)))
            {
                string fileName = EditorUtility.SaveFilePanel("Save Prefab Config", ViewData.PrefabConfigSaveDir, "", "pcfg");
                if(!string.IsNullOrEmpty(fileName))
                {
                    PrefabConfigSave.SaveConfig(fileName);
                    ViewData.PrefabConfigSaveDir = FileSystem.GetLastFolderNameInPath(fileName);
                    EditorUtility.DisplayDialog("Prefab Config Save", "The configuration was saved successfully!", "OK");
                }
            }
        }

        private GUIContent GetContentForSavePrefabConfigButton()
        {
            var content = new GUIContent();
            content.text = "Save prefab config...";
            content.tooltip = "Saves all prefab categories and prefab tags inside a config file which can be loaded when needed.";

            return content;
        }

        private void RenderLoadPrefabConfigButton()
        {
            if (GUILayout.Button(GetContentForLoadPrefabConfigButton(), GUILayout.Width(_categoryActionBtnSize)))
            {
                string fileName = EditorUtility.OpenFilePanel("Load Prefab Config", ViewData.PrefabConfigLoadDir, "pcfg");
                if (!string.IsNullOrEmpty(fileName))
                {
                    PrefabConfigLoad.LoadConfig(fileName);
                    ViewData.PrefabConfigLoadDir = FileSystem.GetLastFolderNameInPath(fileName);
                    EditorUtility.DisplayDialog("Prefab Config Load", "The configuration was loaded successfully!", "OK");
                }
            }
        }

        private GUIContent GetContentForLoadPrefabConfigButton()
        {
            var content = new GUIContent();
            content.text = "Load prefab config...";
            content.tooltip = "Loads a prefab configuration from a specified file. Note: Loading a prefab config will overwrite all prefab, prefab category and prefab tag data.";

            return content;
        }

        private void RenderActionsView()
        {
            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent();
            content.text = "Move";
            content.tooltip = "Allows you to move prefabs to a destination category. The popup controls to the right allow you to choose what prefabs will be moved and to which category.";
            if(GUILayout.Button(content, GUILayout.Width(100.0f)))
            {
                if(ViewData.PrefabMoveType == PrefabMoveType.AllPrefabs)
                {
                    PrefabCategory destinationCategory = ViewData.DestinationCategoryForPrefabMove;
                    if (destinationCategory != null)
                    {
                        UndoEx.RecordForToolAction(destinationCategory);
                        UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                        PrefabCategoryDatabase.Get().ActivePrefabCategory.TransferAllPrefabsToCategory(destinationCategory);
                    }
                }
                else
                if(ViewData.PrefabMoveType == PrefabMoveType.FilteredPrefabs)
                {
                    PrefabCategory destinationCategory = ViewData.DestinationCategoryForPrefabMove;
                    if (destinationCategory != null)
                    {
                        PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                        UndoEx.RecordForToolAction(destinationCategory);
                        UndoEx.RecordForToolAction(activePrefabCategory);

                        activePrefabCategory.TransferPrefabCollectionToCategory(activePrefabCategory.GetFilteredPrefabs(), destinationCategory);
                    }
                }
                else
                if(ViewData.PrefabMoveType == PrefabMoveType.ActivePrefab)
                {
                    Prefab activePrefab = PrefabCategoryDatabase.Get().ActivePrefabCategory.ActivePrefab;
                    if(activePrefab != null)
                    {
                        PrefabCategory destinationCategory = ViewData.DestinationCategoryForPrefabMove;
                        if (destinationCategory != null)
                        {
                            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                            UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                            UndoEx.RecordForToolAction(destinationCategory);
                            UndoEx.RecordForToolAction(activePrefabCategory);

                            activePrefabCategory.TransferPrefabToCategory(activePrefab, destinationCategory);
                            PrefabCategoryDatabase.Get().SetActivePrefabCategory(destinationCategory);
                            destinationCategory.SetActivePrefab(activePrefab);
                        }
                    }
                }
            }

            PrefabMoveType newPrefabMoveType = (PrefabMoveType)EditorGUILayout.EnumPopup(ViewData.PrefabMoveType);
            if(newPrefabMoveType != ViewData.PrefabMoveType)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabMoveType = newPrefabMoveType;
            }

            List<string> allPrefabCategoryNames = PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames();
            string newString = EditorGUILayoutEx.Popup(new GUIContent(), ViewData.DestinationCategoryForPrefabMove.Name, allPrefabCategoryNames);
            if (newString != ViewData.DestinationCategoryForPrefabMove.Name)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.DestinationCategoryForPrefabMove = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            RenderSetPrefabOffsetFromGridSurfaceInActiveCategoryButton();
            RenderPrefabOffsetFromGridSurfaceField();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton();
            RenderPrefabOffsetFromObjectSurfaceField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewCategoryButton();
            RenderCreateNewCategoryNameChangeTextField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewCategoryButton()
        {
            if (GUILayout.Button(GetContentForCreateNewCategoryButton(), GUILayout.Width(_categoryActionBtnSize)))
            {
                PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
                UndoEx.RecordForToolAction(prefabCategoryDatabase);

                PrefabCategory newCategory = prefabCategoryDatabase.CreatePrefabCategory(ViewData.NameForNewPrefabCategory);
                prefabCategoryDatabase.SetActivePrefabCategory(newCategory);
            }
        }

        private GUIContent GetContentForCreateNewCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Create category";
            content.tooltip = "Creates a new prefab category using the name specified in the adjacent text field. " +
                              "Note: Names are automatically adjusted such that each category name is unique.";

            return content;
        }

        private void RenderCreateNewCategoryNameChangeTextField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewPrefabCategory);
            if (newString != ViewData.NameForNewPrefabCategory)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewPrefabCategory = newString;
            }
        }

        private void RenderSetPrefabOffsetFromGridSurfaceInActiveCategoryButton()
        {
            if(GUILayout.Button(GetContentForSetPrefabOffsetFromGridSurfaceInActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth + 3.0f)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                UndoEx.RecordForToolAction(activePrefabCategory);
                PrefabCategoryActions.SetPrefabOffsetFromGridSurface(activePrefabCategory, ViewData.PrefabOffsetFromGridSurface);
            }
        }

        private GUIContent GetContentForSetPrefabOffsetFromGridSurfaceInActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Set prefab offset from grid surface";
            content.tooltip = "Pressing this button will change the offset from the grid surface for all prefabs which reside in the active category.";

            return content;
        }

        private void RenderPrefabOffsetFromGridSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(ViewData.PrefabOffsetFromGridSurface);
            if (newFloat != ViewData.PrefabOffsetFromGridSurface)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabOffsetFromGridSurface = newFloat;
            }
        }

        private void RenderSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton()
        {
            if (GUILayout.Button(GetContentForSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth + 3.0f)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                UndoEx.RecordForToolAction(activePrefabCategory);
                PrefabCategoryActions.SetPrefabOffsetFromObjectSurface(activePrefabCategory, ViewData.PrefabOffsetFromObjectSurface);
            }
        }

        private GUIContent GetContentForSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Set prefab offset from object surface";
            content.tooltip = "Pressing this button will change the offset from object surfaces for all prefabs which reside in the active category.";

            return content;
        }

        private void RenderPrefabOffsetFromObjectSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(ViewData.PrefabOffsetFromObjectSurface);
            if (newFloat != ViewData.PrefabOffsetFromObjectSurface)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabOffsetFromObjectSurface = newFloat;
            }
        }
        #endregion
    }
}
#endif