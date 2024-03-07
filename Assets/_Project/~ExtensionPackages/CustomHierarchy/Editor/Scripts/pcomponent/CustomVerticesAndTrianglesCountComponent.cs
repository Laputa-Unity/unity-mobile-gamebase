using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomVerticesAndTrianglesCountComponent: CustomBaseComponent
    {
        // PRIVATE
        private GUIStyle labelStyle;
        private Color verticesLabelColor;
        private Color trianglesLabelColor;
        private bool calculateTotalCount;
        private bool showTrianglesCount;
        private bool showVerticesCount;
        private CustomHierarchySize labelSize;

        // CONSTRUCTOR
        public CustomVerticesAndTrianglesCountComponent ()
        {
            labelStyle = new GUIStyle();            
            labelStyle.clipping = TextClipping.Clip;  
            labelStyle.alignment = TextAnchor.MiddleRight;

            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesShow                  , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesShowDuringPlayMode    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesCalculateTotalCount   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesShowTriangles         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesShowVertices          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesLabelSize             , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesVerticesLabelColor    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VerticesAndTrianglesTrianglesLabelColor   , settingsChanged);

            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowDuringPlayMode);
            calculateTotalCount         = CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesCalculateTotalCount);
            showTrianglesCount          = CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowTriangles);
            showVerticesCount           = CustomSettings.getInstance().get<bool>(CustomSetting.VerticesAndTrianglesShowVertices);
            verticesLabelColor          = CustomSettings.getInstance().getColor(CustomSetting.VerticesAndTrianglesVerticesLabelColor);
            trianglesLabelColor         = CustomSettings.getInstance().getColor(CustomSetting.VerticesAndTrianglesTrianglesLabelColor);
            labelSize                   = (CustomHierarchySize)CustomSettings.getInstance().get<int>(CustomSetting.VerticesAndTrianglesLabelSize);

            #if UNITY_2019_1_OR_NEWER
                labelStyle.fontSize = labelSize == CustomHierarchySize.Big ? 7 : 6;
                rect.width = labelSize == CustomHierarchySize.Big ? 24 : 22;
            #else
                labelStyle.fontSize = labelSize == CustomHierarchySize.Big ? 9 : 8;
                rect.width = labelSize == CustomHierarchySize.Big ? 33 : 25;
            #endif
        }   

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < rect.width)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= rect.width + 2;
                rect.x = curRect.x;
                rect.y = curRect.y;
                #if UNITY_2019_1_OR_NEWER                
                    rect.y += labelSize == CustomHierarchySize.Big ? 2 : 1;
                #endif
                return CustomLayoutStatus.Success;
            }
        }
        
        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {  
            int vertexCount = 0;
            int triangleCount = 0;

            MeshFilter[] meshFilterArray = calculateTotalCount ? gameObject.GetComponentsInChildren<MeshFilter>(true) : gameObject.GetComponents<MeshFilter>();
            for (int i = 0; i < meshFilterArray.Length; i++)
            {
                Mesh sharedMesh = meshFilterArray[i].sharedMesh;
                if (sharedMesh != null)
                {
                    if (showVerticesCount) vertexCount += sharedMesh.vertexCount;
                    if (showTrianglesCount) triangleCount += sharedMesh.triangles.Length;
                }
            }

            SkinnedMeshRenderer[] skinnedMeshRendererArray = calculateTotalCount ? gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) : gameObject.GetComponents<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshRendererArray.Length; i++)
            {
                Mesh sharedMesh = skinnedMeshRendererArray[i].sharedMesh;
                if (sharedMesh != null)
                {   
                    if (showVerticesCount) vertexCount += sharedMesh.vertexCount;
                    if (showTrianglesCount) triangleCount += sharedMesh.triangles.Length;
                }
            }

            triangleCount /= 3;

            if (vertexCount > 0 || triangleCount > 0)
            {
                if (showTrianglesCount && showVerticesCount) 
                {
                    rect.y -= 4;
                    labelStyle.normal.textColor = verticesLabelColor;
                    EditorGUI.LabelField(rect, getCountString(vertexCount), labelStyle);

                    rect.y += 8;
                    labelStyle.normal.textColor = trianglesLabelColor;
                    EditorGUI.LabelField(rect, getCountString(triangleCount), labelStyle);
                }
                else if (showVerticesCount)
                {
                    labelStyle.normal.textColor = verticesLabelColor;
                    EditorGUI.LabelField(rect, getCountString(vertexCount), labelStyle);
                }
                else
                {
                    labelStyle.normal.textColor = trianglesLabelColor;
                    EditorGUI.LabelField(rect, getCountString(triangleCount), labelStyle);
                }
            }
        }

        // PRIVATE
        private string getCountString(int count)
        {
            if (count < 1000) return count.ToString();
            else if (count < 1000000) 
            {
                if (count > 100000) return (count / 1000.0f).ToString("0") + "k";
                else return (count / 1000.0f).ToString("0.0") + "k";
            }
            else 
            {
                if (count > 10000000) return (count / 1000.0f).ToString("0") + "M";
                else return (count / 1000000.0f).ToString("0.0") + "M";
            }
        }
    }
}

