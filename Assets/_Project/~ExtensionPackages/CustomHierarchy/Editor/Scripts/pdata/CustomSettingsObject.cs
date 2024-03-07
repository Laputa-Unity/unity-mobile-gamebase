using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace customtools.customhierarchy.pdata
{
    [System.Serializable]
    class CustomSettingsObject: ScriptableObject
    {
        [SerializeField] private List<string> settingStringNames  = new List<string>();
        [SerializeField] private List<string> settingStringValues = new List<string>();

        [SerializeField] private List<string> settingFloatNames   = new List<string>();
        [SerializeField] private List<float>  settingFloatValues  = new List<float>();

        [SerializeField] private List<string> settingIntNames     = new List<string>();
        [SerializeField] private List<int>    settingIntValues    = new List<int>();

        [SerializeField] private List<string> settingBoolNames   = new List<string>();
        [SerializeField] private List<bool>   settingBoolValues  = new List<bool>();

        public void Clear()
        {
            settingStringNames.Clear();
            settingStringValues.Clear();
            settingFloatNames.Clear();
            settingFloatValues.Clear();
            settingIntNames.Clear();
            settingIntValues.Clear();
            settingBoolNames.Clear();
            settingBoolValues.Clear();
        }

        public void Set(string settingName, object value)
        {
            if (value is bool b)
            {
                settingBoolValues[settingBoolNames.IndexOf(settingName)] = b;
            }
            else if (value is string s)
            {
                settingStringValues[settingStringNames.IndexOf(settingName)] = s;
            }
            else if (value is float f)
            {
                settingFloatValues[settingFloatNames.IndexOf(settingName)] = f;
            }
            else if (value is int i)
            {
                settingIntValues[settingIntNames.IndexOf(settingName)] = i;
            }
            EditorUtility.SetDirty(this);
        }

        public object Get(string settingName, object defaultValue)
        {
            if (defaultValue is bool)
            {
                int id = settingBoolNames.IndexOf(settingName);
                if (id == -1) 
                {
                    settingBoolNames.Add(settingName);
                    settingBoolValues.Add((bool)defaultValue);
                    return defaultValue;
                }
                else return settingBoolValues[id];
            }
            else if (defaultValue is string value)
            {
                int id = settingStringNames.IndexOf(settingName);
                if (id == -1) 
                {
                    settingStringNames.Add(settingName);
                    settingStringValues.Add(value);
                    return value;
                }
                else return settingStringValues[id];
            }
            else if (defaultValue is float f)
            {
                int id = settingFloatNames.IndexOf(settingName);
                if (id == -1) 
                {
                    settingFloatNames.Add(settingName);
                    settingFloatValues.Add(f);
                    return f;
                }
                else return settingFloatValues[id];
            }
            else if (defaultValue is int i)
            {
                int id = settingIntNames.IndexOf(settingName);
                if (id == -1) 
                {
                    settingIntNames.Add(settingName);
                    settingIntValues.Add(i);
                    return i;
                }
                else return settingIntValues[id];
            }
            return null;
        }
        
        public object GET<T>(string settingName)
        {
            if (typeof(T) == typeof(bool))
            {
                int id = settingBoolNames.IndexOf(settingName);
                if (id == -1) return null;
                else return settingBoolValues[id];
            }
            else if (typeof(T) == typeof(string))
            {
                int id = settingStringNames.IndexOf(settingName);
                if (id == -1) return null;
                else return settingStringValues[id];
            }
            else if (typeof(T) == typeof(float))
            {
                int id = settingFloatNames.IndexOf(settingName);
                if (id == -1) return null;
                else return settingFloatValues[id];
            }
            else if (typeof(T) == typeof(int))
            {
                int id = settingIntNames.IndexOf(settingName);
                if (id == -1) return null;
                else return settingIntValues[id];
            }
            return null;
        }
    }
}

