using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;
using customtools.customhierarchy.pcomponent;

namespace customtools.customhierarchy.phierarchy
{
	public class CustomHierarchySettingsWindow : EditorWindow 
	{	
        // STATIC
		[MenuItem ("GameBase/Custom Hierarchy &3")]
		public static void ShowWindow () 
		{ 
			EditorWindow window = EditorWindow.GetWindow(typeof(CustomHierarchySettingsWindow));           
            window.minSize = new Vector2(350, 50);

            #if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
                window.title = "CustomHierarchy";
            #else
                window.titleContent = new GUIContent("CustomHierarchy");
            #endif
		}

        // PRIVATE
        private bool inited = false;
        private Rect lastRect;
        private bool isProSkin;
        private int indentLevel;
        private Texture2D checkBoxChecked;
        private Texture2D checkBoxUnchecked;
        private Texture2D restoreButtonTexture;
        private Vector2 scrollPosition = new Vector2();
        private Color separatorColor;
        private Color yellowColor;
        private float totalWidth;
        private CustomComponentsOrderList componentsOrderList;

        // INIT
        private void init() 
        { 
            inited            = true;
            isProSkin         = EditorGUIUtility.isProSkin;
            separatorColor    = isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.59f, 0.59f, 0.59f);
            yellowColor       = isProSkin ? new Color(1.00f, 0.90f, 0.40f) : new Color(0.31f, 0.31f, 0.31f);
            checkBoxChecked   = CustomResources.getInstance().GetTexture(CustomTexture.CustomCheckBoxChecked);
            checkBoxUnchecked = CustomResources.getInstance().GetTexture(CustomTexture.CustomCheckBoxUnchecked);
            restoreButtonTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomRestoreButton);
            componentsOrderList = new CustomComponentsOrderList(this);
        } 
         
        // GUI
		void OnGUI()
		{
            if (!inited || isProSkin != EditorGUIUtility.isProSkin)  
                init();

            indentLevel = 8;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                Rect targetRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
                if (Event.current.type == EventType.Repaint) totalWidth = targetRect.width + 8;

                this.lastRect = new Rect(0, 1, 0, 0);

                // COMPONENTS
                drawSection("COMPONENTS SETTINGS");
                float sectionStartY = lastRect.y + lastRect.height;

                drawTreeMapComponentSettings();
                drawSeparator();
                drawMonoBehaviourIconComponentSettings();
                drawSeparator();
                drawSeparatorComponentSettings();
                drawSeparator();
                drawVisibilityComponentSettings();
                drawSeparator();
                drawLockComponentSettings();
                drawSeparator();
                drawStaticComponentSettings();
                drawSeparator();
                drawErrorComponentSettings();
                drawSeparator();
                drawRendererComponentSettings();
                drawSeparator();
                drawPrefabComponentSettings();
                drawSeparator();
                drawTagLayerComponentSettings();
                drawSeparator();
                drawColorComponentSettings();
                drawSeparator();
                drawGameObjectIconComponentSettings();
                drawSeparator();
                drawTagIconComponentSettings();
                drawSeparator();
                drawLayerIconComponentSettings();
                drawSeparator();
                drawChildrenCountComponentSettings();
                drawSeparator();
                drawVerticesAndTrianglesCountComponentSettings();
                drawSeparator();
                drawComponentsComponentSettings();
                drawLeftLine(sectionStartY, lastRect.y + lastRect.height, separatorColor);

                // ORDER
                drawSection("ORDER OF COMPONENTS");         
                sectionStartY = lastRect.y + lastRect.height;
                drawSpace(8);  
                drawOrderSettings();
                drawSpace(6);      
                drawLeftLine(sectionStartY, lastRect.y + lastRect.height, separatorColor);

                // ADDITIONAL
                drawSection("ADDITIONAL SETTINGS");             
                sectionStartY = lastRect.y + lastRect.height;
                drawSpace(3);  
                drawAdditionalSettings();
                drawLeftLine(sectionStartY, lastRect.y + lastRect.height + 4, separatorColor);

                indentLevel -= 1;
            }

            EditorGUILayout.EndScrollView();
        }

        // COMPONENTS
        private void drawTreeMapComponentSettings() 
        {
            if (drawComponentCheckBox("Hierarchy Tree", CustomSetting.TreeMapShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.TreeMapColor);
                    CustomSettings.getInstance().restore(CustomSetting.TreeMapEnhanced);
                    CustomSettings.getInstance().restore(CustomSetting.TreeMapTransparentBackground);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + 5);
                drawSpace(4);
                drawColorPicker("Tree color", CustomSetting.TreeMapColor);
                drawCheckBoxRight("Transparent background", CustomSetting.TreeMapTransparentBackground);
                drawCheckBoxRight("Enhanced (\"Transform Sort\" only)", CustomSetting.TreeMapEnhanced);
                drawSpace(1);
            }
        }
        
        private void drawMonoBehaviourIconComponentSettings() 
        {
            if (drawComponentCheckBox("MonoBehaviour Icon", CustomSetting.MonoBehaviourIconShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.MonoBehaviourIconShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.MonoBehaviourIconColor);
                    CustomSettings.getInstance().restore(CustomSetting.MonoBehaviourIconIgnoreUnityMonobehaviour);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.MonoBehaviourIconShowDuringPlayMode);
                drawColorPicker("Icon color", CustomSetting.MonoBehaviourIconColor);
                drawCheckBoxRight("Ignore Unity MonoBehaviours", CustomSetting.MonoBehaviourIconIgnoreUnityMonobehaviour);
                drawSpace(1);
            }
        }

        private void drawSeparatorComponentSettings() 
        {
            if (drawComponentCheckBox("Separator", CustomSetting.SeparatorShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.SeparatorColor);
                    CustomSettings.getInstance().restore(CustomSetting.SeparatorShowRowShading);
                    CustomSettings.getInstance().restore(CustomSetting.SeparatorOddRowShadingColor);
                    CustomSettings.getInstance().restore(CustomSetting.SeparatorEvenRowShadingColor);
                }
                bool rowShading = CustomSettings.getInstance().get<bool>(CustomSetting.SeparatorShowRowShading);

                drawBackground(rect.x, rect.y, rect.width, 18 * (rowShading ? 4 : 2) + 5);
                drawSpace(4);
                drawColorPicker("Separator Color", CustomSetting.SeparatorColor);
                drawCheckBoxRight("Row shading", CustomSetting.SeparatorShowRowShading);
                if (rowShading)                
                {
                    drawColorPicker("Even row shading color", CustomSetting.SeparatorEvenRowShadingColor);
                    drawColorPicker("Odd row shading color" , CustomSetting.SeparatorOddRowShadingColor);
                }
                drawSpace(1);
            }
        }

        private void drawVisibilityComponentSettings() 
        {
            if (drawComponentCheckBox("Visibility", CustomSetting.VisibilityShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.VisibilityShowDuringPlayMode);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.VisibilityShowDuringPlayMode);
                drawSpace(1);
            }
        }

        private void drawLockComponentSettings() 
        {
            if (drawComponentCheckBox("Lock", CustomSetting.LockShow))
            {   
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.LockShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.LockPreventSelectionOfLockedObjects);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 2 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.LockShowDuringPlayMode);
                drawCheckBoxRight("Prevent selection of locked objects", CustomSetting.LockPreventSelectionOfLockedObjects);
                drawSpace(1);
            }
        }

        private void drawStaticComponentSettings() 
        {
            if (drawComponentCheckBox("Static", CustomSetting.StaticShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.StaticShowDuringPlayMode);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.StaticShowDuringPlayMode);
                drawSpace(1);
            }        
        }

        private void drawErrorComponentSettings() 
        {
            if (drawComponentCheckBox("Error", CustomSetting.ErrorShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowIconOnParent);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowForDisabledComponents);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowForDisabledGameObjects);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowScriptIsMissing);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowReferenceIsMissing);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowReferenceIsNull);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowStringIsEmpty);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowMissingEventMethod);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorShowWhenTagOrLayerIsUndefined);
                    CustomSettings.getInstance().restore(CustomSetting.ErrorIgnoreString);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 12 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.ErrorShowDuringPlayMode);
                drawCheckBoxRight("Show error icon up of hierarchy (very slow)", CustomSetting.ErrorShowIconOnParent);
                drawCheckBoxRight("Show error icon for disabled components", CustomSetting.ErrorShowForDisabledComponents);
                drawCheckBoxRight("Show error icon for disabled GameObjects", CustomSetting.ErrorShowForDisabledGameObjects);
                drawLabel("Show error icon for the following:");
                indentLevel += 16;
                drawCheckBoxRight("- script is missing", CustomSetting.ErrorShowScriptIsMissing);
                drawCheckBoxRight("- reference is missing", CustomSetting.ErrorShowReferenceIsMissing);
                drawCheckBoxRight("- reference is null", CustomSetting.ErrorShowReferenceIsNull);
                drawCheckBoxRight("- string is empty", CustomSetting.ErrorShowStringIsEmpty);
                drawCheckBoxRight("- callback of event is missing (very slow)", CustomSetting.ErrorShowMissingEventMethod);
                drawCheckBoxRight("- tag or layer is undefined", CustomSetting.ErrorShowWhenTagOrLayerIsUndefined);
                indentLevel -= 16;
                drawTextField("Ignore packages/classes", CustomSetting.ErrorIgnoreString);
                drawSpace(1);
            }
        }

        private void drawRendererComponentSettings() 
        {
            if (drawComponentCheckBox("Renderer", CustomSetting.RendererShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.RendererShowDuringPlayMode);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.RendererShowDuringPlayMode);
                drawSpace(1);
            }
        }

        private void drawPrefabComponentSettings() 
        {
            if (drawComponentCheckBox("Prefab", CustomSetting.PrefabShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.PrefabShowBreakedPrefabsOnly);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show icon for broken prefabs only", CustomSetting.PrefabShowBreakedPrefabsOnly);
                drawSpace(1);
            }
        }

        private void drawTagLayerComponentSettings()
        {
            if (drawComponentCheckBox("Tag And Layer", CustomSetting.TagAndLayerShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerSizeShowType);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerType);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerSizeValueType);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerSizeValuePixel);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerSizeValuePercent);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerAligment);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerLabelSize);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerLabelAlpha);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerTagLabelColor);
                    CustomSettings.getInstance().restore(CustomSetting.TagAndLayerLayerLabelColor);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 10 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.TagAndLayerShowDuringPlayMode);  
                drawEnum("Show", CustomSetting.TagAndLayerSizeShowType, typeof(CustomHierarchyTagAndLayerShowType));
                drawEnum("Show tag and layer", CustomSetting.TagAndLayerType, typeof(CustomHierarchyTagAndLayerType));

                CustomHierarchyTagAndLayerSizeType newTagAndLayerSizeValueType = (CustomHierarchyTagAndLayerSizeType)drawEnum("Unit of width", CustomSetting.TagAndLayerSizeValueType, typeof(CustomHierarchyTagAndLayerSizeType));               

                if (newTagAndLayerSizeValueType == CustomHierarchyTagAndLayerSizeType.Pixel)                
                    drawIntSlider("Width in pixels", CustomSetting.TagAndLayerSizeValuePixel, 5, 250);                
                else                
                    drawFloatSlider("Percentage width", CustomSetting.TagAndLayerSizeValuePercent, 0, 0.5f);
                           
                drawEnum("Alignment", CustomSetting.TagAndLayerAligment, typeof(CustomHierarchyTagAndLayerAligment));
                drawEnum("Label size", CustomSetting.TagAndLayerLabelSize, typeof(CustomHierarchyTagAndLayerLabelSize));
                drawFloatSlider("Label alpha if default", CustomSetting.TagAndLayerLabelAlpha, 0, 1.0f);
                drawColorPicker("Tag label color", CustomSetting.TagAndLayerTagLabelColor);
                drawColorPicker("Layer label color", CustomSetting.TagAndLayerLayerLabelColor);
                drawSpace(1);
            }
        }

        private void drawColorComponentSettings() 
        {
            if (drawComponentCheckBox("Color", CustomSetting.ColorShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.ColorShowDuringPlayMode);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.ColorShowDuringPlayMode);
                drawSpace(1);
            }
        }

        private void drawGameObjectIconComponentSettings() 
        {
            if (drawComponentCheckBox("GameObject Icon", CustomSetting.GameObjectIconShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.GameObjectIconShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.GameObjectIconSize);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 2 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.GameObjectIconShowDuringPlayMode);
                drawEnum("Icon size", CustomSetting.GameObjectIconSize, typeof(CustomHierarchySizeAll));
                drawSpace(1);
            }
        }

        private void drawTagIconComponentSettings() 
        {
            if (drawComponentCheckBox("Tag Icon", CustomSetting.TagIconShow))
            {     
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

                bool showTagIconList = CustomSettings.getInstance().get<bool>(CustomSetting.TagIconListFoldout);

                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.TagIconShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.TagIconSize);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + (showTagIconList ? 18 * tags.Length : 0) + 4 + 5);    

                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.TagIconShowDuringPlayMode);
                drawEnum("Icon size", CustomSetting.TagIconSize, typeof(CustomHierarchySizeAll));
                if (drawFoldout("Tag icon list", CustomSetting.TagIconListFoldout))
                {
                    List<CustomTagTexture> tagTextureList = CustomTagTexture.LoadTagTextureList();
                
                    bool changed = false;
                    for (int i = 0; i < tags.Length; i++) 
                    {
                        string tag = tags[i];
                        CustomTagTexture tagTexture = tagTextureList.Find(t => t.tag == tag);
                        Texture2D newTexture = (Texture2D)EditorGUI.ObjectField(getControlRect(0, 16, 34 + 16, 6), 
                                                                                tag, tagTexture == null ? null : tagTexture.texture, typeof(Texture2D), false);
                        if (newTexture != null && tagTexture == null)
                        {
                            CustomTagTexture newTagTexture = new CustomTagTexture(tag, newTexture);
                            tagTextureList.Add(newTagTexture);
                            
                            changed = true;
                        }
                        else if (newTexture == null && tagTexture != null)
                        {
                            tagTextureList.Remove(tagTexture);                            
                            changed = true;
                        }
                        else if (tagTexture != null && tagTexture.texture != newTexture)
                        {
                            tagTexture.texture = newTexture;
                            changed = true;
                        }

                        drawSpace(i == tags.Length - 1 ? 2 : 2);
                    }                 

                    if (changed) 
                    {     
                        CustomTagTexture.SaveTagTextureList(CustomSetting.TagIconList, tagTextureList);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }

                drawSpace(1);
            }
        }

        private void drawLayerIconComponentSettings() 
        {
            if (drawComponentCheckBox("Layer Icon", CustomSetting.LayerIconShow))
            {     
                string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
                
                bool showLayerIconList = CustomSettings.getInstance().get<bool>(CustomSetting.LayerIconListFoldout);
                
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.LayerIconShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.LayerIconSize);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + (showLayerIconList ? 18 * layers.Length : 0) + 4 + 5);    
                
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.LayerIconShowDuringPlayMode);
                drawEnum("Icon size", CustomSetting.LayerIconSize, typeof(CustomHierarchySizeAll));
                if (drawFoldout("Layer icon list", CustomSetting.LayerIconListFoldout))
                {
                    List<CustomLayerTexture> layerTextureList = CustomLayerTexture.LoadLayerTextureList();
                    
                    bool changed = false;
                    for (int i = 0; i < layers.Length; i++) 
                    {
                        string layer = layers[i];
                        CustomLayerTexture layerTexture = layerTextureList.Find(t => t.layer == layer);
                        Texture2D newTexture = (Texture2D)EditorGUI.ObjectField(getControlRect(0, 16, 34 + 16, 6), 
                                                                                layer, layerTexture == null ? null : layerTexture.texture, typeof(Texture2D), false);
                        if (newTexture != null && layerTexture == null)
                        {
                            CustomLayerTexture newLayerTexture = new CustomLayerTexture(layer, newTexture);
                            layerTextureList.Add(newLayerTexture);
                            
                            changed = true;
                        }
                        else if (newTexture == null && layerTexture != null)
                        {
                            layerTextureList.Remove(layerTexture);                            
                            changed = true;
                        }
                        else if (layerTexture != null && layerTexture.texture != newTexture)
                        {
                            layerTexture.texture = newTexture;
                            changed = true;
                        }
                        
                        drawSpace(i == layers.Length - 1 ? 2 : 2);
                    }                 
                    
                    if (changed) 
                    {     
                        CustomLayerTexture.SaveLayerTextureList(CustomSetting.LayerIconList, layerTextureList);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }
                
                drawSpace(1);
            }
        }

        private void drawChildrenCountComponentSettings() 
        {
            if (drawComponentCheckBox("Children Count", CustomSetting.ChildrenCountShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.ChildrenCountShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.ChildrenCountLabelSize);
                    CustomSettings.getInstance().restore(CustomSetting.ChildrenCountLabelColor);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.ChildrenCountShowDuringPlayMode);
                drawEnum("Label size", CustomSetting.ChildrenCountLabelSize, typeof(CustomHierarchySize));
                drawColorPicker("Label color", CustomSetting.ChildrenCountLabelColor);
                drawSpace(1);
            }
        }
        
        private void drawVerticesAndTrianglesCountComponentSettings()
        {
            if (drawComponentCheckBox("Vertices And Triangles Count", CustomSetting.VerticesAndTrianglesShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesShowVertices);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesShowTriangles);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesCalculateTotalCount);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesLabelSize);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesVerticesLabelColor);
                    CustomSettings.getInstance().restore(CustomSetting.VerticesAndTrianglesTrianglesLabelColor);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 7 + 5);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.VerticesAndTrianglesShowDuringPlayMode);                   
                if (drawCheckBoxRight("Show vertices count", CustomSetting.VerticesAndTrianglesShowVertices))
                {
                    if (CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowVertices) == false &&
                        CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowTriangles) == false)                    
                        CustomSettings.getInstance().set(CustomSetting.VerticesAndTrianglesShowTriangles, true);
                }
                if (drawCheckBoxRight("Show triangles count (very slow)", CustomSetting.VerticesAndTrianglesShowTriangles))
                {
                    if (CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowVertices) == false &&
                        CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowTriangles) == false)                 
                        CustomSettings.getInstance().set(CustomSetting.VerticesAndTrianglesShowVertices, true);
                }
                drawCheckBoxRight("Calculate the count including children (very slow)", CustomSetting.VerticesAndTrianglesCalculateTotalCount);
                drawEnum("Label size", CustomSetting.VerticesAndTrianglesLabelSize, typeof(CustomHierarchySize));
                drawColorPicker("Vertices label color", CustomSetting.VerticesAndTrianglesVerticesLabelColor);
                drawColorPicker("Triangles label color", CustomSetting.VerticesAndTrianglesTrianglesLabelColor);
                drawSpace(1);
            }
        }

        private void drawComponentsComponentSettings() 
        {
            if (drawComponentCheckBox("Components", CustomSetting.ComponentsShow))
            {
                Rect rect = getControlRect(0, 0);
                if (drawRestore())
                {
                    CustomSettings.getInstance().restore(CustomSetting.ComponentsShowDuringPlayMode);
                    CustomSettings.getInstance().restore(CustomSetting.ComponentsIconSize);
                }
                drawBackground(rect.x, rect.y, rect.width, 18 * 3 + 6);
                drawSpace(4);
                drawCheckBoxRight("Show component during play mode", CustomSetting.ComponentsShowDuringPlayMode);
                drawEnum("Icon size", CustomSetting.ComponentsIconSize, typeof(CustomHierarchySizeAll));
                drawTextField("Ignore packages/classes", CustomSetting.ComponentsIgnore);
                drawSpace(2);
            }
        }

        // COMPONENTS ORDER
        private void drawOrderSettings()
        {
            if (drawRestore())
            {
                CustomSettings.getInstance().restore(CustomSetting.ComponentsOrder);
            }

            indentLevel += 4;
            
            string componentOrder = CustomSettings.getInstance().get<string>(CustomSetting.ComponentsOrder);
            string[] componentIds = componentOrder.Split(';');
            
            Rect rect = getControlRect(position.width, 17 * componentIds.Length + 10, 0, 0);
            if (componentsOrderList == null) 
                componentsOrderList = new CustomComponentsOrderList(this);
            componentsOrderList.draw(rect, componentIds);
            
            indentLevel -= 4;
        }  

        // ADDITIONAL SETTINGS
        private void drawAdditionalSettings()
        {
            if (drawRestore())
            {
                CustomSettings.getInstance().restore(CustomSetting.AdditionalShowHiddenCustomHierarchyObjectList);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalHideIconsIfNotFit);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalIdentation);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalShowModifierWarning);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalBackgroundColor);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalActiveColor);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalInactiveColor);
                CustomSettings.getInstance().restore(CustomSetting.AdditionalSpecialColor);
            }
            drawSpace(4);
            drawCheckBoxRight("Show CustomHierarchyObjectList GameObject", CustomSetting.AdditionalShowHiddenCustomHierarchyObjectList);
            drawCheckBoxRight("Hide icons if not fit", CustomSetting.AdditionalHideIconsIfNotFit);
            drawIntSlider("Right indent", CustomSetting.AdditionalIdentation, 0, 500);      
            drawCheckBoxRight("Show warning when using modifiers + click", CustomSetting.AdditionalShowModifierWarning);
            drawColorPicker("Background color", CustomSetting.AdditionalBackgroundColor);
            drawColorPicker("Active color", CustomSetting.AdditionalActiveColor);
            drawColorPicker("Inactive color", CustomSetting.AdditionalInactiveColor);
            drawColorPicker("Special color", CustomSetting.AdditionalSpecialColor);
            drawSpace(1);
        }

        // PRIVATE
        private void drawSection(string title)
        {
            Rect rect = getControlRect(0, 24, -3, 0);
            rect.width *= 2;
            rect.x = 0;
            GUI.Box(rect, "");             
            
            drawLeftLine(rect.y, rect.y + 24, yellowColor);
            
            rect.x = lastRect.x + 8;
            rect.y += 4;
            EditorGUI.LabelField(rect, title);
        }

        private void drawSeparator(int spaceBefore = 0, int spaceAfter = 0, int height = 1)
        {
            if (spaceBefore > 0) drawSpace(spaceBefore);
            Rect rect = getControlRect(0, height, 0, 0);
            rect.width += 8;
            EditorGUI.DrawRect(rect, separatorColor);
            if (spaceAfter > 0) drawSpace(spaceAfter);
        }

        private bool drawComponentCheckBox(string label, CustomSetting setting)
        {
            indentLevel += 8;

            Rect rect = getControlRect(0, 28, 0, 0);

            float rectWidth = rect.width;
            bool isChecked = CustomSettings.getInstance().get<bool>(setting);

            rect.x -= 1;
            rect.y += 7;
            rect.width  = 14;
            rect.height = 14;

            if (GUI.Button(rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none))
            {
                CustomSettings.getInstance().set(setting, !isChecked);
            }

            rect.x += 14 + 10;
            rect.width = rectWidth - 14 - 8;
            rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(rect, label);

            indentLevel -= 8;

            return isChecked;
        }

        private bool drawCheckBoxRight(string label, CustomSetting setting)
        {
            Rect rect = getControlRect(0, 18, 34, 6);
            bool result = false;
            bool isChecked = CustomSettings.getInstance().get<bool>(setting);

            float tempX = rect.x;
            rect.x += rect.width - 14;
            rect.y += 1;
            rect.width  = 14;
            rect.height = 14;
            
            if (GUI.Button(rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none))
            {
                CustomSettings.getInstance().set(setting, !isChecked);
                result = true;
            }

            rect.width = rect.x - tempX - 4;
            rect.x = tempX;
            rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.LabelField(rect, label);

            return result;
        }

        private void drawSpace(int value)
        {
            getControlRect(0, value, 0, 0);
        }

        private void drawBackground(float x, float y, float width, float height)
        {
            EditorGUI.DrawRect(new Rect(x, y, width, height), separatorColor);
        }
        
        private void drawLeftLine(float fromY, float toY, Color color, float width = 0)
        {
            EditorGUI.DrawRect(new Rect(0, fromY, width == 0 ? indentLevel : width, toY - fromY), color);
        }
        
        private Rect getControlRect(float width, float height, float addIndent = 0, float remWidth = 0)
        { 
            EditorGUILayout.GetControlRect(false, height, GUIStyle.none, GUILayout.ExpandWidth(true));
            Rect rect = new Rect(indentLevel + addIndent, lastRect.y + lastRect.height, (width == 0 ? totalWidth - indentLevel - addIndent - remWidth: width), height);
            lastRect = rect;
            return rect;
        }

        private bool drawRestore()
        {
            if (GUI.Button(new Rect(lastRect.x + lastRect.width - 16 - 5, lastRect.y - 20, 16, 16), restoreButtonTexture, GUIStyle.none))
            {
                if (EditorUtility.DisplayDialog("Restore", "Restore default settings?", "Ok", "Cancel"))
                {
                    return true;
                }
            }
            return false;
        }

        // GUI COMPONENTS
        private void drawLabel(string label)
        {
            Rect rect = getControlRect(0, 16, 34, 6);
            rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
            EditorGUI.LabelField(rect, label);
            drawSpace(2);
        }

        private void drawTextField(string label, CustomSetting setting)
        {
            string currentValue = CustomSettings.getInstance().get<string>(setting);
            string newValue     = EditorGUI.TextField(getControlRect(0, 16, 34, 6), label, currentValue);
            if (!currentValue.Equals(newValue)) CustomSettings.getInstance().set(setting, newValue);
            drawSpace(2);
        }

        private bool drawFoldout(string label, CustomSetting setting)
        {
            #if UNITY_2019_1_OR_NEWER
                Rect foldoutRect = getControlRect(0, 16, 19, 6);
            #else
                Rect foldoutRect = getControlRect(0, 16, 22, 6);
            #endif
            bool foldoutValue = CustomSettings.getInstance().get<bool>(setting);
            bool newFoldoutValue = EditorGUI.Foldout(foldoutRect, foldoutValue, label);
            if (foldoutValue != newFoldoutValue) CustomSettings.getInstance().set(setting, newFoldoutValue);
            drawSpace(2);
            return newFoldoutValue;
        }

        private void drawColorPicker(string label, CustomSetting setting)
        {
            Color currentColor = CustomSettings.getInstance().getColor(setting);
            Color newColor = EditorGUI.ColorField(getControlRect(0, 16, 34, 6), label, currentColor);
            if (!currentColor.Equals(newColor)) CustomSettings.getInstance().setColor(setting, newColor);
            drawSpace(2);
        }

        private Enum drawEnum(string label, CustomSetting setting, Type enumType)
        {
            Enum currentEnum = (Enum)Enum.ToObject(enumType, CustomSettings.getInstance().get<int>(setting));
            Enum newEnumValue;                      
            if (!(newEnumValue = EditorGUI.EnumPopup(getControlRect(0, 16, 34, 6), label, currentEnum)).Equals(currentEnum))                
                CustomSettings.getInstance().set(setting, (int)(object)newEnumValue);                  
            drawSpace(2);
            return newEnumValue;
        }

        private void drawIntSlider(string label, CustomSetting setting, int minValue, int maxValue)
        {
            Rect rect = getControlRect(0, 16, 34, 4);
            int currentValue = CustomSettings.getInstance().get<int>(setting);
            int newValue = EditorGUI.IntSlider(rect, label, currentValue, minValue, maxValue);
            if (currentValue != newValue) CustomSettings.getInstance().set(setting, newValue);
            drawSpace(2);
        }

        private void drawFloatSlider(string label, CustomSetting setting, float minValue, float maxValue)
        {
            Rect rect = getControlRect(0, 16, 34, 4);
            float currentValue = CustomSettings.getInstance().get<float>(setting);
            float newValue = EditorGUI.Slider(rect, label, currentValue, minValue, maxValue);
            if (currentValue != newValue) CustomSettings.getInstance().set(setting, newValue);
            drawSpace(2);
        }
	}
}