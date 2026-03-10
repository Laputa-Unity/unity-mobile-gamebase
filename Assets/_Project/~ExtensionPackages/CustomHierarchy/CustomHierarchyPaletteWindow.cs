#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Type = System.Type;
using static CustomHierarchy.CustomHierarchy;
using static CustomHierarchy.CustomHierarchyData;
using static CustomHierarchy.CustomHierarchyPalette;
using static CustomHierarchy.Libs.VUtils;
using static CustomHierarchy.Libs.VGUI;
// using static VTools.VDebug;



namespace CustomHierarchy
{
    public class CustomHierarchyPaletteWindow : EditorWindow
    {

        void OnGUI()
        {
            if (!palette) { Close(); return; }

            int hoveredColorIndex = -1;
            string hoveredIconNameOrGuid = null;

            void background()
            {
                position.SetPos(0, 0).Draw(windowBackground);
            }
            void outline()
            {
                if (Application.platform == RuntimePlatform.OSXEditor) return;

                position.SetPos(0, 0).DrawOutline(Greyscale(.1f));

            }
            void colors()
            {
                if (!palette.colorsEnabled) return;

                var rowRect = this.position.SetPos(paddingX, paddingY).SetHeight(cellSize);


                void color(int i)
                {
                    var cellRect = rowRect.MoveX(i * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize);

                    void backgroundSelected()
                    {
                        if (!colorIndexes_initial.Contains(i)) return;

                        cellRect.Resize(1).DrawRounded(selectedBackground, 2);

                    }
                    void backgroundHovered()
                    {
                        if (!cellRect.IsHovered()) return;

                        cellRect.Resize(1).DrawRounded(this.hoveredBackground, 2);

                    }
                    void crossIcon()
                    {
                        if (i != 0) return;

                        GUI.DrawTexture(cellRect.SetSizeFromMid(iconSize), EditorIcons.GetIcon("CrossIcon"));

                    }
                    void colorOutline()
                    {
                        if (i == 0) return;

                        var outlineColor = i <= CustomHierarchyPalette.greyColorsCount ? Greyscale(.0f, .4f) : Greyscale(.15f, .2f);

                        cellRect.Resize(3).DrawRounded(outlineColor, 4);

                    }
                    void color()
                    {
                        if (i == 0) return;

                        var brightness = palette.colorBrightness;
                        var saturation = palette.colorSaturation;
                        var drawGradients = palette.colorGradientsEnabled;

                        if (!palette.colorGradientsEnabled)
                            brightness *= isDarkTheme ? .75f : .97f;

                        if (i <= CustomHierarchyPalette.greyColorsCount)
                        {
                            saturation = brightness = 1;
                            drawGradients = false;
                        }


                        var colorRaw = palette ? palette.colors[i - 1] : CustomHierarchyPalette.GetDefaultColor(i - 1);

                        var color = MathUtil.Lerp(Greyscale(.2f), colorRaw, brightness);

                        Color.RGBToHSV(color, out float h, out float s, out float v);
                        color = Color.HSVToRGB(h, s * saturation, v);

                        color = MathUtil.Lerp(color, colorRaw, .5f).SetAlpha(1);

                        if (i > CustomHierarchyPalette.greyColorsCount && isDarkTheme)
                            color *= 1.41f;




                        cellRect.Resize(4).DrawRounded(color, 3);

                        if (drawGradients)
                            cellRect.Resize(4).AddWidthFromRight(-2).DrawCurtainLeft(GUIColors.windowBackground.SetAlpha(.45f));

                    }
                    void recursiveIndicator()
                    {
                        if (!curEvent.isRepaint) return;


                        var isRecursive = goDatas.First().colorIndex == i && goDatas.First().isColorRecursive;

                        if (!isRecursive) return;



                        var iconRect = cellRect.SetSizeFromMid(16).Move(-6, -7);
                        var shadowRect = iconRect.Resize(3).Move(2, 1).AddWidthFromRight(3);
                        var shadowRadius = 4;

                        shadowRect.DrawBlurred(GUIColors.windowBackground, shadowRadius);


                        SetGUIColor(Color.white * 2);

                        GUI.DrawTexture(iconRect, EditorIcons.GetIcon("UnityEditor.SceneHierarchyWindow@2x"));

                        ResetGUIColor();


                    }

                    void setHovered()
                    {
                        if (!cellRect.IsHovered()) return;

                        hoveredColorIndex = i;

                    }
                    void closeOnClick()
                    {
                        if (!cellRect.IsHovered()) return;
                        if (!curEvent.isMouseUp) return;

                        curEvent.Use();

                        Close();

                    }



                    cellRect.MarkInteractive();

                    backgroundSelected();
                    backgroundHovered();
                    crossIcon();
                    colorOutline();
                    color();
                    recursiveIndicator();

                    setHovered();
                    closeOnClick();

                }


                for (int i = 0; i < palette.colors.Count + 1; i++)
                    color(i);

            }
            void icons()
            {
                void row(int i, IconRow iconRow)
                {
                    var rowRect = this.position.SetPos(paddingX, paddingY).SetHeight(cellSize).MoveY(palette.colorsEnabled ? cellSize + spaceAfterColors : 0).MoveY(i * (cellSize + rowSpacing));

                    var isFirstEnabledRow = palette.iconRows.First(r => r.enabled) == iconRow;


                    void icon(int i)
                    {
                        var cellRect = rowRect.MoveX(i * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize);

                        var isCrossIcon = isFirstEnabledRow && i == 0;
                        var actualIconIndex = isFirstEnabledRow ? i - 1 : i;
                        var isCustomIcon = !isCrossIcon && actualIconIndex >= iconRow.builtinIcons.Count;
                        var iconNameOrGuid = isCrossIcon ? "" : isCustomIcon ? iconRow.customIcons[actualIconIndex - iconRow.builtinIcons.Count] : iconRow.builtinIcons[actualIconIndex];


                        void backgroundSelected()
                        {
                            if (!iconNamesOrGuids_initial.Contains(iconNameOrGuid)) return;

                            cellRect.Resize(1).DrawRounded(selectedBackground, 2);

                        }
                        void backgroundHovered()
                        {
                            if (!cellRect.IsHovered()) return;

                            cellRect.Resize(1).DrawRounded(this.hoveredBackground, 2);

                        }
                        void crossIcon()
                        {
                            if (!isCrossIcon) return;

                            GUI.DrawTexture(cellRect.SetSizeFromMid(iconSize), EditorIcons.GetIcon("CrossIcon"));

                        }
                        void normalIcon()
                        {
                            if (isCrossIcon) return;

                            var iconNameOrPath = iconNameOrGuid?.Length == 32 ? iconNameOrGuid.ToPath() : iconNameOrGuid;
                            var icon = EditorIcons.GetIcon(iconNameOrPath) ?? Texture2D.blackTexture;

                            var iconRect = cellRect.SetSizeFromMid(iconSize);

                            if (icon.width < icon.height) iconRect = iconRect.SetWidthFromMid(iconRect.height * icon.width / icon.height);
                            if (icon.height < icon.width) iconRect = iconRect.SetHeightFromMid(iconRect.width * icon.height / icon.width);


                            GUI.DrawTexture(iconRect, icon);

                        }
                        void recursiveIndicator()
                        {
                            if (!curEvent.isRepaint) return;


                            var isRecursive = goDatas.First().iconNameOrGuid == iconNameOrGuid && goDatas.First().isIconRecursive;

                            if (!isRecursive) return;



                            var iconRect = cellRect.SetSizeFromMid(16).Move(-6, -7);
                            var shadowRect = iconRect.Resize(3).Move(2, 1).AddWidthFromRight(3);
                            var shadowRadius = 4;

                            shadowRect.DrawBlurred(GUIColors.windowBackground, shadowRadius);



                            SetGUIColor(Color.white * 2);

                            GUI.DrawTexture(iconRect, EditorIcons.GetIcon("UnityEditor.SceneHierarchyWindow@2x"));

                            ResetGUIColor();


                        }

                        void setHovered()
                        {
                            if (!cellRect.IsHovered()) return;

                            hoveredIconNameOrGuid = iconNameOrGuid;

                        }
                        void closeOnClick()
                        {
                            if (!cellRect.IsHovered()) return;
                            if (!curEvent.isMouseUp) return;

                            curEvent.Use();

                            Close();

                        }



                        cellRect.MarkInteractive();

                        backgroundSelected();
                        backgroundHovered();
                        crossIcon();
                        normalIcon();
                        recursiveIndicator();

                        setHovered();
                        closeOnClick();

                    }


                    for (int j = 0; j < iconRow.iconCount + (isFirstEnabledRow ? 1 : 0); j++)
                        icon(j);

                }


                var i = 0;

                foreach (var iconRow in palette.iconRows)
                {
                    if (!iconRow.enabled) continue;
                    if (iconRow.isEmpty) continue;

                    row(i, iconRow);

                    i++;
                }

            }
            void editPaletteButton()
            {
                var buttonRect = position.SetPos(0, 0).SetWidthFromRight(16).SetHeightFromBottom(16).Move(-14, -14);
                var buttonColor = isDarkTheme ? Greyscale(.6f) : Greyscale(1, .6f);

                if (!IconButton(buttonRect, "Toolbar Plus", 16, buttonColor)) return;


                palette.SelectInInspector(frameInProject: false);

                EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"))?.Show();

                this.Close();

            }

            void setColorsAndIcons()
            {
                if (!curEvent.isLayout) return;


                if (palette.iconRows.Any(r => r.enabled))
                    if (hoveredIconNameOrGuid != null)
                        SetIcon(hoveredIconNameOrGuid, isRecursive: curEvent.holdingAlt);
                    else
                        SetInitialIcons();


                if (palette.colorsEnabled)
                    if (hoveredColorIndex != -1)
                        SetColor(hoveredColorIndex, isRecursive: curEvent.holdingAlt);
                    else
                        SetInitialColors();

            }
            void updatePosition()
            {
                if (!curEvent.isLayout) return;

                void calcDeltaTime()
                {
                    deltaTime = (float)(EditorApplication.timeSinceStartup - lastLayoutTime);

                    if (deltaTime > .05f)
                        deltaTime = .0166f;

                    lastLayoutTime = EditorApplication.timeSinceStartup;

                }
                void resetCurPos()
                {
                    if (currentPosition != default) return;

                    currentPosition = position.position; // position.position is always int, which can't be used for lerping

                }
                void lerpCurPos()
                {
                    var speed = 9;

                    MathUtil.SmoothDamp(ref currentPosition, targetPosition, speed, ref positionDeriv, deltaTime);
                    // MathfUtils.Lerp(ref currentPosition, targetPosition, speed, deltaTime);

                }
                void setCurPos()
                {
                    position = position.SetPos(currentPosition);
                }

                calcDeltaTime();
                resetCurPos();
                lerpCurPos();
                setCurPos();

                if (!currentPosition.magnitude.Approx(targetPosition.magnitude))
                    Repaint();

            }
            void closeOnEscape()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                SetInitialColors();
                SetInitialIcons();

                Close();

            }


            RecordUndoOnDatas();

            background();
            outline();
            colors();
            icons();
            editPaletteButton();

            setColorsAndIcons();
            updatePosition();
            closeOnEscape();


            CustomHierarchy.goInfoCache.Clear();

            EditorApplication.RepaintHierarchyWindow();

        }

        static float iconSize => 14;
        static float iconSpacing => 6;
        static float cellSize => iconSize + iconSpacing;
        static float spaceAfterColors => 13;
        public float rowSpacing = 1;
        static float paddingX => 12;
        static float paddingY => 12;

        Color windowBackground => isDarkTheme ? Greyscale(.23f) : Greyscale(.75f);
        Color selectedBackground => isDarkTheme ? new Color(.3f, .5f, .7f, .8f) : new Color(.3f, .5f, .7f, .6f) * 1.25f;
        Color hoveredBackground => isDarkTheme ? Greyscale(1, .3f) : Greyscale(0, .1f);

        public Vector2 targetPosition;
        public Vector2 currentPosition;
        Vector2 positionDeriv;
        float deltaTime;
        double lastLayoutTime;






        void SetIcon(string iconNameOrGuid, bool isRecursive)
        {
            foreach (var r in goDatas)
            {
                r.isIconRecursive = isRecursive; // setting it firstbecause iconNameOrGuid setter relies on isIconRecursive
                r.iconNameOrGuid = iconNameOrGuid;
            }
        }
        void SetColor(int colorIndex, bool isRecursive)
        {
            foreach (var r in goDatas)
            {
                r.isColorRecursive = isRecursive;
                r.colorIndex = colorIndex;
            }
        }

        void SetInitialIcons()
        {
            for (int i = 0; i < goDatas.Count; i++)
            {
                goDatas[i].isIconRecursive = isIconRecursives_initial[i];
                goDatas[i].iconNameOrGuid = iconNamesOrGuids_initial[i];
            }
        }
        void SetInitialColors()
        {
            for (int i = 0; i < goDatas.Count; i++)
            {
                goDatas[i].isColorRecursive = isColorRecursives_initial[i];
                goDatas[i].colorIndex = colorIndexes_initial[i];
            }
        }

        void RemoveEmptyGoDatas()
        {
            var toRemove = goDatas.Where(r => r.iconNameOrGuid == "" && r.colorIndex == 0);

            foreach (var goData in toRemove)
                goData.sceneData.goDatas_byGlobalId.RemoveValue(goData);

            if (toRemove.Any())
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup() - 1);

        }

        void RecordUndoOnDatas()
        {
            if (usingDataSO)
                if (data)
                    data.RecordUndo();

            foreach (var r in usedDataComponents)
                r.RecordUndo();

        }
        void MarkDatasDirty()
        {
            if (usingDataSO)
                if (data)
                    data.Dirty();

            foreach (var r in usedDataComponents)
                r.Dirty();
        }
        void SaveData()
        {
            // if (usingDataSO)
            // data.Save();
        }

        bool usingDataSO => !gameObjects.Select(r => r.scene).All(r => CustomHierarchy.dataComponents_byScene.GetValueOrDefault(r) != null);
        IEnumerable<CustomHierarchyDataComponent> usedDataComponents => CustomHierarchy.dataComponents_byScene.Where(kvp => kvp.Value && gameObjects.Select(r => r.scene).Contains(kvp.Key)).Select(kvp => kvp.Value);







        void OnLostFocus()
        {
            if (curEvent.holdingAlt && EditorWindow.focusedWindow?.GetType().Name == "SceneHierarchyWindow")
                CloseNextFrameIfNotRefocused();
            else
                Close();

        }

        void CloseNextFrameIfNotRefocused()
        {
            EditorApplication.delayCall += () => { if (EditorWindow.focusedWindow != this) Close(); };
        }




        static void RepaintOnAlt() // Update 
        {
            if (curEvent.holdingAlt != wasHoldingAlt)
                if (EditorWindow.mouseOverWindow is CustomHierarchyPaletteWindow paletteWindow)
                    paletteWindow.Repaint();

            wasHoldingAlt = curEvent.holdingAlt;

        }

        static bool wasHoldingAlt;









        public void Init(List<GameObject> gameObjects)
        {
            void createData()
            {
                if (CustomHierarchy.data) return;

                CustomHierarchy.data = ScriptableObject.CreateInstance<CustomHierarchyData>();

                AssetDatabase.CreateAsset(CustomHierarchy.data, GetScriptPath("CustomHierarchy").GetParentPath().CombinePath("CustomHierarchy Data.asset"));

            }
            void createPalette()
            {
                if (CustomHierarchy.palette) return;

                CustomHierarchy.palette = ScriptableObject.CreateInstance<CustomHierarchyPalette>();

                AssetDatabase.CreateAsset(CustomHierarchy.palette, GetScriptPath("CustomHierarchy").GetParentPath().CombinePath("CustomHierarchy Palette.asset"));

            }
            void setSize()
            {
                if (!palette.colorsEnabled && !palette.iconRows.Any(r => r.enabled && !r.isEmpty)) // somehow happened on first palette window opening in 2022.3.50
                    palette.InvokeMethod("Reset");



                var rowCellCounts = new List<int>();

                if (palette.colorsEnabled)
                    rowCellCounts.Add(palette.colors.Count + 1);

                foreach (var r in palette.iconRows.Where(r => r.enabled && !r.isEmpty))
                    rowCellCounts.Add(r.iconCount + (r == palette.iconRows.First(r => r.enabled) ? 1 : 0));

                var width = paddingX
                          + rowCellCounts.Max() * cellSize
                          + (rowCellCounts.Last() == rowCellCounts.Max() ? cellSize : 0)
                          + paddingX;



                var iconRowCount = palette.iconRows.Count(r => r.enabled && !r.isEmpty);

                var height = paddingY
                           + (palette.colorsEnabled ? cellSize : 0)
                           + (palette.colorsEnabled && palette.iconRows.Any(r => r.enabled && !r.isEmpty) ? spaceAfterColors : 0)
                           + cellSize * iconRowCount
                           + rowSpacing * (iconRowCount - 1)
                           + paddingY;


                position = position.SetSize(width, height).SetPos(targetPosition);

            }
            void getInfos()
            {
                goDatas.Clear();

                foreach (var r in gameObjects)
                    goDatas.Add(CustomHierarchy.GetGameObjectData(r, createDataIfDoesntExist: true));

            }
            void getInitialState()
            {
                iconNamesOrGuids_initial = goDatas.Select(r => r.iconNameOrGuid).ToList();
                colorIndexes_initial = goDatas.Select(r => r.colorIndex).ToList();

                isIconRecursives_initial = goDatas.Select(r => r.isIconRecursive).ToList();
                isColorRecursives_initial = goDatas.Select(r => r.isColorRecursive).ToList();


            }


            this.gameObjects = gameObjects;

            RecordUndoOnDatas();

            createData();
            createPalette();
            setSize();
            getInfos();
            getInitialState();

            EditorApplication.update -= RepaintOnAlt;
            EditorApplication.update += RepaintOnAlt;

        }

        void OnDestroy()
        {
            RemoveEmptyGoDatas();
            MarkDatasDirty();
            SaveData();

            EditorApplication.update -= RepaintOnAlt;

        }

        public List<GameObject> gameObjects = new();
        public List<GameObjectData> goDatas = new();

        public List<string> iconNamesOrGuids_initial = new();
        public List<int> colorIndexes_initial = new();

        public List<bool> isIconRecursives_initial = new();
        public List<bool> isColorRecursives_initial = new();

        static CustomHierarchyPalette palette => CustomHierarchy.palette;
        static CustomHierarchyData data => CustomHierarchy.data;







        public static void CreateInstance(Vector2 position)
        {
            instance = ScriptableObject.CreateInstance<CustomHierarchyPaletteWindow>();

            instance.ShowPopup();

            instance.position = instance.position.SetPos(position).SetSize(200, 300);
            instance.targetPosition = position;

        }

        public static CustomHierarchyPaletteWindow instance;

    }
}
#endif