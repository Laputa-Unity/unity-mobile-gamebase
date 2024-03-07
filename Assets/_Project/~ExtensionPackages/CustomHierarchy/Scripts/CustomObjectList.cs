using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace customtools.customhierarchy
{	
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class CustomObjectList: MonoBehaviour, ISerializationCallbackReceiver
	{
		public static List<CustomObjectList> instances = new List<CustomObjectList>();

		public List<GameObject> lockedObjects = new List<GameObject>();
		public List<GameObject> editModeVisibleObjects = new List<GameObject>();
		public List<GameObject> editModeInvisibleObjects = new List<GameObject>();
		public List<GameObject> wireframeHiddenObjects = new List<GameObject>();		
		public Dictionary<GameObject, Color> gameObjectColor = new Dictionary<GameObject, Color>();
		public List<GameObject> gameObjectColorKeys   = new List<GameObject>();		
		public List<Color> 		gameObjectColorValues = new List<Color>();

		public void Awake() 
		{
			CheckIntegrity(); 
			
			foreach (GameObject editModeGameObject in editModeVisibleObjects)               
				editModeGameObject.SetActive(!Application.isPlaying);                
			
			foreach (GameObject editModeGameObject in editModeInvisibleObjects)                
				editModeGameObject.SetActive(Application.isPlaying);

			if (!Application.isEditor && Application.isPlaying)		
			{
				instances.Remove(this);
				DestroyImmediate(gameObject);
				return;
			}

			instances.RemoveAll(item => item == null);
			if (!instances.Contains(this)) instances.Add(this);
		}

		public void OnEnable() 
		{  
			if (!instances.Contains(this)) instances.Add(this);

			#if UNITY_EDITOR
			foreach (GameObject wireframeGameObject in wireframeHiddenObjects)
			{
				Renderer renderer = wireframeGameObject.GetComponent<Renderer>();
				if (renderer != null) 
                {
                    #if UNITY_5_5_OR_NEWER
                    EditorUtility.SetSelectedRenderState(renderer, EditorSelectedRenderState.Hidden);
                    #else
                    EditorUtility.SetSelectedWireframeHidden(renderer, true);
                    #endif
                }
			}
			#endif
		}

		public void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				CheckIntegrity();
				
				foreach (GameObject gameObject in editModeVisibleObjects)               
					gameObject.SetActive(false);                
				
				foreach (GameObject gameObject in editModeInvisibleObjects)                
					gameObject.SetActive(true);
				
				foreach (GameObject gameObject in lockedObjects)   			
					gameObject.hideFlags &= ~HideFlags.NotEditable;

				instances.Remove(this);
			}
		}

		public void merge(CustomObjectList anotherInstance)
		{ 
			for (int i = anotherInstance.lockedObjects.Count - 1; i >= 0; i--)
			{
				if (!lockedObjects.Contains(anotherInstance.lockedObjects[i]))
					lockedObjects.Add(anotherInstance.lockedObjects[i]);
			}

			for (int i = anotherInstance.editModeVisibleObjects.Count - 1; i >= 0; i--)
			{
				if (!editModeVisibleObjects.Contains(anotherInstance.editModeVisibleObjects[i]))
					editModeVisibleObjects.Add(anotherInstance.editModeVisibleObjects[i]);
			}

			for (int i = anotherInstance.editModeInvisibleObjects.Count - 1; i >= 0; i--)
			{
				if (!editModeInvisibleObjects.Contains(anotherInstance.editModeInvisibleObjects[i]))
					editModeInvisibleObjects.Add(anotherInstance.editModeInvisibleObjects[i]);
			}

			for (int i = anotherInstance.wireframeHiddenObjects.Count - 1; i >= 0; i--)
			{
				if (!wireframeHiddenObjects.Contains(anotherInstance.wireframeHiddenObjects[i]))
					wireframeHiddenObjects.Add(anotherInstance.wireframeHiddenObjects[i]);
			}

			for (int i = anotherInstance.gameObjectColorKeys.Count - 1; i >= 0; i--)
			{
				if (!gameObjectColorKeys.Contains(anotherInstance.gameObjectColorKeys[i]))
				{
					gameObjectColorKeys.Add(anotherInstance.gameObjectColorKeys[i]);
					gameObjectColorValues.Add(anotherInstance.gameObjectColorValues[i]);
					gameObjectColor.Add(anotherInstance.gameObjectColorKeys[i], anotherInstance.gameObjectColorValues[i]);
				}
			}
        } 
        
		public void CheckIntegrity()
		{
			lockedObjects.RemoveAll(item => item == null);
			editModeVisibleObjects.RemoveAll(item => item == null);
			editModeInvisibleObjects.RemoveAll(item => item == null);
			wireframeHiddenObjects.RemoveAll(item => item == null);

			for (int i = gameObjectColorKeys.Count - 1; i >= 0; i--)
			{
				if (gameObjectColorKeys[i] == null)
				{
					gameObjectColorKeys.RemoveAt(i);
					gameObjectColorValues.RemoveAt(i);
				}
			}
			OnAfterDeserialize();
		}          

		public void OnBeforeSerialize()
		{  
			gameObjectColorKeys.Clear();
			gameObjectColorValues.Clear();
			foreach(KeyValuePair<GameObject, Color> pair in gameObjectColor)
			{
				gameObjectColorKeys.Add(pair.Key);
				gameObjectColorValues.Add(pair.Value);
			}
		}
		
		public void OnAfterDeserialize()
		{
			gameObjectColor.Clear();			
			for(int i = 0; i < gameObjectColorKeys.Count; i++)
				gameObjectColor.Add(gameObjectColorKeys[i], gameObjectColorValues[i]);
		}
	}
}