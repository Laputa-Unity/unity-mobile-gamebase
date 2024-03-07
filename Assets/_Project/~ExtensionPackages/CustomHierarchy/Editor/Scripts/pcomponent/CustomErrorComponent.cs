using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using customtools.customhierarchy.phierarchy;
using System.Reflection;
using System.Collections;
using UnityEditorInternal;
using System.Text;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomErrorComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color activeColor;
        private Color inactiveColor;
        private Texture2D errorIconTexture;
        private bool showErrorOfChildren;
        private bool showErrorTypeReferenceIsNull;
        private bool showErrorTypeReferenceIsMissing;
        private bool showErrorTypeStringIsEmpty;
        private bool showErrorIconScriptIsMissing;
        private bool showErrorIconWhenTagIsUndefined;
        private bool showErrorForDisabledComponents;
        private bool showErrorIconMissingEventMethod;
        private bool showErrorForDisabledGameObjects;
        private List<string> ignoreErrorOfMonoBehaviours;
        private StringBuilder errorStringBuilder;
        private int errorCount;

        // CONSTRUCTOR
        public CustomErrorComponent ()
        {
            rect.width = 7; 

            errorIconTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomErrorIcon);

            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowIconOnParent             , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowReferenceIsNull          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowReferenceIsMissing       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowStringIsEmpty            , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowScriptIsMissing          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowForDisabledComponents    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowForDisabledGameObjects   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowMissingEventMethod       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowWhenTagOrLayerIsUndefined, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShow                         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorShowDuringPlayMode           , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ErrorIgnoreString                 , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalActiveColor             , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor           , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showErrorOfChildren             = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowIconOnParent);
            showErrorTypeReferenceIsNull    = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowReferenceIsNull);
            showErrorTypeReferenceIsMissing = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowReferenceIsMissing);
            showErrorTypeStringIsEmpty      = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowStringIsEmpty);
            showErrorIconScriptIsMissing    = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowScriptIsMissing);
            showErrorForDisabledComponents  = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowForDisabledComponents);
            showErrorForDisabledGameObjects = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowForDisabledGameObjects);
            showErrorIconMissingEventMethod = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowMissingEventMethod);
            showErrorIconWhenTagIsUndefined = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowWhenTagOrLayerIsUndefined);
            activeColor                     = CustomSettings.getInstance().getColor(CustomSetting.AdditionalActiveColor);
            inactiveColor                   = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
            enabled                         = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShow);
            showComponentDuringPlayMode     = CustomSettings.getInstance().get<bool>(CustomSetting.ErrorShowDuringPlayMode);

            string ignoreErrorOfMonoBehavioursString = CustomSettings.getInstance().get<string>(CustomSetting.ErrorIgnoreString);
            if (ignoreErrorOfMonoBehavioursString != "") 
            {
                ignoreErrorOfMonoBehaviours = new List<string>(ignoreErrorOfMonoBehavioursString.Split(new char[] { ',', ';', '.', ' ' }));
                ignoreErrorOfMonoBehaviours.RemoveAll(item => item == "");
            }
            else ignoreErrorOfMonoBehaviours = null;
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 7) 
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 7;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            bool errorFound = findError(gameObject, gameObject.GetComponents<MonoBehaviour>());

            if (errorFound)
            {           
                CustomColorUtils.setColor(activeColor);
                GUI.DrawTexture(rect, errorIconTexture);
                CustomColorUtils.clearColor();
            }
            else if (showErrorOfChildren) 
            {
                errorFound = findError(gameObject, gameObject.GetComponentsInChildren<MonoBehaviour>(true));
                if (errorFound) 
                {
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, errorIconTexture);
                    CustomColorUtils.clearColor();
                }
            }            
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                errorCount = 0;
                errorStringBuilder = new StringBuilder();
                findError(gameObject, gameObject.GetComponents<MonoBehaviour>(), true);

                if (errorCount > 0)
                {
                    EditorUtility.DisplayDialog(errorCount + (errorCount == 1 ? " error was found" : " errors were found"), errorStringBuilder.ToString(), "OK");
                }
            }
        }

        // PRIVATE
        private bool findError(GameObject gameObject, MonoBehaviour[] components, bool printError = false)
        {
            if (showErrorIconWhenTagIsUndefined)
            {
                try
                { 
                    gameObject.tag.CompareTo(null); 
                }
                catch 
                {
                    if (printError)
                    {
                        appendErrorLine("Tag is undefined");
                    }
                    else
                    {
                        return true;
                    }
                }

                if (LayerMask.LayerToName(gameObject.layer).Equals("")) 
                {
                    if (printError)
                    {
                        appendErrorLine("Layer is undefined");
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < components.Length; i++)
            {
                MonoBehaviour monoBehaviour = components[i];
                if (monoBehaviour == null)
                {
                    if (showErrorIconScriptIsMissing)
                    {
                        if (printError)
                        {
                            appendErrorLine("Component #" + i + " is missing");
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (ignoreErrorOfMonoBehaviours != null)
                    {
                        for (int j = ignoreErrorOfMonoBehaviours.Count - 1; j >= 0; j--)
                        {
                            if (monoBehaviour.GetType().FullName.Contains(ignoreErrorOfMonoBehaviours[j]))
                            {
                                return false;
                            } 
                        }
                    }

                    if (showErrorIconMissingEventMethod)
                    {
                        if (monoBehaviour.gameObject.activeSelf || showErrorForDisabledComponents)
                        {
                            try
                            {
                                if (isUnityEventsNullOrMissing(monoBehaviour, printError))
                                {
                                    if (!printError)
                                    {
                                        return true;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (showErrorTypeReferenceIsNull || showErrorTypeStringIsEmpty || showErrorTypeReferenceIsMissing)
                    {                       
                        if (!monoBehaviour.enabled && !showErrorForDisabledComponents) continue;
                        if (!monoBehaviour.gameObject.activeSelf && !showErrorForDisabledGameObjects) continue;

						Type type = monoBehaviour.GetType();

                        while (type != null) {
							
							BindingFlags bf = BindingFlags.Instance | BindingFlags.Public;
							if (!type.FullName.Contains("UnityEngine")) 
								bf |= BindingFlags.NonPublic;
							FieldInfo[] fieldArray = type.GetFields(bf);

							for (int j = 0; j < fieldArray.Length; j++)
							{
								FieldInfo field = fieldArray[j];

                                if (System.Attribute.IsDefined(field, typeof(HideInInspector)) || 
                                    System.Attribute.IsDefined(field, typeof(CustomHierarchyNullableAttribute)) ||									
                                    System.Attribute.IsDefined(field, typeof(NonSerializedAttribute)) ||
									field.IsStatic) continue;     

								if (field.IsPrivate || !field.IsPublic) 
								{
									if (!Attribute.IsDefined(field, typeof(SerializeField)))
									{
										continue;
									}
								}

								object value = field.GetValue(monoBehaviour);

								try
								{
									if (showErrorTypeStringIsEmpty && field.FieldType == typeof(string) && value != null && ((string)value).Equals(""))
									{                                
										if (printError)
										{
											appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": String value is empty");
											continue;
										}
										else
										{
											return true;                                 
										}
									}
								}
								catch
								{
								}

								try
								{
									if (showErrorTypeReferenceIsMissing && value != null && value is Component && (Component)value == null)
									{
										if (printError)
										{
											appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": Reference is missing");
											continue;
										}
										else
										{
											return true;
										}
									}
								}
								catch
								{
								}

								try
								{
									if (showErrorTypeReferenceIsNull && (value == null || value.Equals(null)))
									{           
										if (printError)
										{
											appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": Reference is null");
											continue;
										}
										else
										{
											return true;
										}
									}
								}
								catch
								{
								}
										  
								try
								{
									if (showErrorTypeReferenceIsNull && value != null && (value is IEnumerable))
									{
										foreach (var item in (IEnumerable)value)
										{
											if (item == null || item.Equals(null))
											{
												if (printError)
												{
													appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": IEnumerable has value with null reference");
													continue;
												}
												else
												{
													return true;
												}
											}
										}
									}
								}
								catch
								{
								}                            
							}
                            type = type.BaseType;
						}
                    }
                }
            }
            return false;
        }

        private List<string> targetPropertiesNames = new List<string>(10);
        
        private bool isUnityEventsNullOrMissing(MonoBehaviour monoBehaviour, bool printError) 
        {
            targetPropertiesNames.Clear();
            FieldInfo[] fieldArray = monoBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); 
   
            for (int i = fieldArray.Length - 1; i >= 0; i--) 
            {
                FieldInfo field = fieldArray[i];                    
                if (field.FieldType == typeof(UnityEventBase) || field.FieldType.IsSubclassOf(typeof(UnityEventBase))) 
                {
                    targetPropertiesNames.Add(field.Name);
                }
            }
            
            if (targetPropertiesNames.Count > 0) 
            {
                SerializedObject serializedMonoBehaviour = new SerializedObject(monoBehaviour); 
                for (int i = targetPropertiesNames.Count - 1; i >= 0; i--) 
                {
                    string targetProperty = targetPropertiesNames[i];

                    SerializedProperty property = serializedMonoBehaviour.FindProperty(targetProperty);
                    SerializedProperty propertyRelativeArrray = property.FindPropertyRelative("m_PersistentCalls.m_Calls");
                    
                    for (int j = propertyRelativeArrray.arraySize - 1; j >= 0; j--)
                    {
                        SerializedProperty arrayElementAtIndex = propertyRelativeArrray.GetArrayElementAtIndex(j);

                        SerializedProperty propertyTarget       = arrayElementAtIndex.FindPropertyRelative("m_Target");
                        if (propertyTarget.objectReferenceValue == null)
                        {
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event object reference is null");
                            }
                            else
                            {
                                return true;
                            }
                        }

                        SerializedProperty propertyMethodName   = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
                        if (string.IsNullOrEmpty(propertyMethodName.stringValue)) 
                        {
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event handler function is not selected");
                                continue;
                            }
                            else
                            {
                                return true;
                            }
                        }
                         
                        string argumentAssemblyTypeName = arrayElementAtIndex.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
                        System.Type argumentAssemblyType;
                        if (!string.IsNullOrEmpty(argumentAssemblyTypeName)) argumentAssemblyType = System.Type.GetType(argumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object);
                        else argumentAssemblyType = typeof(UnityEngine.Object);

                        UnityEventBase dummyEvent;
                        System.Type propertyTypeName = System.Type.GetType(property.FindPropertyRelative("m_TypeName").stringValue, false);
                        if (propertyTypeName == null) dummyEvent = (UnityEventBase) new UnityEvent();
                        else dummyEvent = Activator.CreateInstance(propertyTypeName) as UnityEventBase;

                        if (!UnityEventDrawer.IsPersistantListenerValid(dummyEvent, propertyMethodName.stringValue, propertyTarget.objectReferenceValue, (PersistentListenerMode)arrayElementAtIndex.FindPropertyRelative("m_Mode").enumValueIndex, argumentAssemblyType))
                        { 
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event handler function is missing");
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }   
            }            
            return false;
        }

        private void appendErrorLine(string error)
        {
            errorCount++;
            errorStringBuilder.Append(errorCount.ToString());
            errorStringBuilder.Append(") ");
            errorStringBuilder.AppendLine(error);
        }
    }
}

