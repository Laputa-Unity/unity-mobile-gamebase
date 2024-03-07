using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using customtools.customhierarchy.phelper;
using customtools.customhierarchy.phierarchy;
using customtools.customhierarchy.pdata;
using UnityEditor.Callbacks;

namespace customtools.customhierarchy
{
    [InitializeOnLoad]
    public class CustomHierarchyInitializer
    {
        private static CustomHierarchy hierarchy;

        static CustomHierarchyInitializer()
        {
            EditorApplication.update -= update;
            EditorApplication.update += update;

            EditorApplication.hierarchyWindowItemOnGUI -= hierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUIHandler;
            
            EditorApplication.hierarchyChanged -= hierarchyWindowChanged;
            EditorApplication.hierarchyChanged += hierarchyWindowChanged;

            Undo.undoRedoPerformed -= undoRedoPerformed;
            Undo.undoRedoPerformed += undoRedoPerformed;
        }

        static void undoRedoPerformed()
        {
            EditorApplication.RepaintHierarchyWindow();          
        }

        static void init()
        {       
            hierarchy = new CustomHierarchy();
        } 

        static void update()
        {
            if (hierarchy == null) init();
            CustomObjectListManager.getInstance().Update();
        }

        static void hierarchyWindowItemOnGUIHandler(int instanceId, Rect selectionRect)
        {
            if (hierarchy == null) init();
             hierarchy.hierarchyWindowItemOnGUIHandler(instanceId, selectionRect);
        }

        static void hierarchyWindowChanged()
        {
            if (hierarchy == null) init();
            CustomObjectListManager.getInstance().validate();
        }
    }
}

