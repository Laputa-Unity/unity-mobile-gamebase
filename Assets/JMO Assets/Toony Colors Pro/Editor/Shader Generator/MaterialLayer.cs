using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace ToonyColorsPro
{
    namespace  ShaderGenerator
    {
        [Serialization.SerializeAs("ml")]
        public class MaterialLayer
        {
            internal enum BlendType
            {
                LinearInterpolation,
                NormalMap
            }
            
            [Serialization.SerializeAs("uid")] public readonly string uid;
            [Serialization.SerializeAs("name")] public string name = "Material Layer";
            [Serialization.SerializeAs("src"), Serialization.ForceSerialization] public ShaderProperty sourceShaderProperty;
            [Serialization.SerializeAs("use_contrast")] bool useContrastProperty;
            [Serialization.SerializeAs("ctrst"), Serialization.ForceSerialization] public ShaderProperty contrastProperty;
            [Serialization.SerializeAs("use_noise")] bool useNoiseProperty;
            [Serialization.SerializeAs("noise"), Serialization.ForceSerialization] public ShaderProperty noiseProperty;
            internal bool expanded;

            internal bool UseContrastProperty
            {
                get { return useContrastProperty; }
                set
                {
                    useContrastProperty = value;
                    if (useContrastProperty)
                    {
                        if (contrastProperty == null)
                        {
                            contrastProperty = new ShaderProperty("contrast_" + uid, ShaderProperty.VariableType.@float);
                            contrastProperty.materialLayerUid = uid;
                            contrastProperty.SetDefaultImplementations(new ShaderProperty.Implementation[]
                            {
                                new ShaderProperty.Imp_MaterialProperty_Range(contrastProperty)
                                {
                                    Label = "Contrast",
                                    Min = 0,
                                    Max = 1,
                                    DefaultValue = 0.5f
                                }
                            });
                            contrastProperty.DisplayName = uid + " Layer Contrast";
                        }
                    }
                    else
                    {
                        contrastProperty = null;
                    }
                }
            }

            internal bool UseNoiseProperty
            {
                get { return useNoiseProperty; }
                set
                {
                    useNoiseProperty = value;
                    if (useNoiseProperty)
                    {
                        if (noiseProperty == null)
                        {
                            noiseProperty = new ShaderProperty("noise_" + uid, ShaderProperty.VariableType.@float);
                            noiseProperty.materialLayerUid = uid;
                            noiseProperty.SetDefaultImplementations(new ShaderProperty.Implementation[]
                            {
                                new ShaderProperty.Imp_CustomCode(noiseProperty)
                                {
                                    code = "saturate( {2}.r * {3} ) - {3} / 2.0"
                                },
                                new ShaderProperty.Imp_MaterialProperty_Texture(noiseProperty)
                                {
                                    Label = "Noise Texture",
                                    PropertyName = string.Format("_NoiseTexture_{0}", uid),
                                    DefaultValue = "gray"
                                },
                                new ShaderProperty.Imp_MaterialProperty_Range(noiseProperty)
                                {
                                    Label = "Noise Strength",
                                    PropertyName = string.Format("_NoiseStrength_{0}", uid),
                                    Min = 0,
                                    Max = 1,
                                    DefaultValue = 0.1f
                                }
                            });
                            noiseProperty.DisplayName = uid + " Layer Noise";
                        }
                    }
                    else
                    {
                        noiseProperty = null;
                    }
                }
            }

            [Serialization.CustomDeserializeCallback]
            static MaterialLayer Deserialize(string data, object[] args)
            {
                var materialLayer = new MaterialLayer();
                
                // custom callback for ShaderProperty
                Func<object, string, object> onDeserializeShaderProperty = (spObj, spData) =>
                {
                    if (spData == "__NULL__")
                    {
                        return null;
                    }
                    
                    // HACK figure out which property is being deserialized based on name
                    // substring(11) will strip:  sp(name:"
                    ShaderProperty targetProperty = materialLayer.sourceShaderProperty;
                    if (spData.Substring(9).StartsWith("contrast_"))
                    {
                        // Can't deserialize to null, so we need to create the Shader Property first
                        materialLayer.contrastProperty = new ShaderProperty("temp_contrast", ShaderProperty.VariableType.@float);
                        targetProperty = materialLayer.contrastProperty;
                    }
                    if (spData.Substring(9).StartsWith("noise_"))
                    {
                        // Can't deserialize to null, so we need to create the Shader Property first
                        materialLayer.noiseProperty = new ShaderProperty("temp_noise", ShaderProperty.VariableType.@float);
                        targetProperty = materialLayer.noiseProperty;
                    }

                    if (targetProperty == null)
                    {
                        return null;
                    }

                    // custom callback for Implementations
                    Func<object, string, object> onDeserializeImplementation = (impObj, impData) =>
                    {
                        return ShaderGenerator2.CurrentConfig.DeserializeImplementationHandler(impObj, impData, targetProperty);
                    };
                    var implementationHandling = new Dictionary<Type, Func<object, string, object>> { { typeof(ShaderProperty.Implementation), onDeserializeImplementation } };
                    
                    return Serialization.DeserializeTo(targetProperty, spData, typeof(ShaderProperty), null, implementationHandling);
                };
                var shaderPropertyHandling = new Dictionary<Type, Func<object, string, object>> { { typeof(ShaderProperty), onDeserializeShaderProperty } };

                return (MaterialLayer)Serialization.DeserializeTo(materialLayer, data, typeof(MaterialLayer), null, shaderPropertyHandling);
            }

            [Serialization.OnDeserializeCallback]
            void OnDeserialized()
            {
                sourceShaderProperty.materialLayerUid = uid;
                sourceShaderProperty.DisplayName = uid + " Source";
                if (contrastProperty != null)
                {
                    contrastProperty.DisplayName = uid + " Layer Contrast";
                    contrastProperty.materialLayerUid = uid;
                }
                if (noiseProperty != null)
                {
                    noiseProperty.DisplayName = uid + " Layer Noise";
                    noiseProperty.materialLayerUid = uid;
                }
            }

            public MaterialLayer()
            {
                uid = GenerateUID();
                sourceShaderProperty = new ShaderProperty("layer_" + uid, ShaderProperty.VariableType.@float);
                sourceShaderProperty.materialLayerUid = uid;
                sourceShaderProperty.SetDefaultImplementations(new ShaderProperty.Implementation[]
                {
                    new ShaderProperty.Imp_MaterialProperty_Texture(sourceShaderProperty)
                    {
                        Label = "Source Texture"
                    }
                });
                sourceShaderProperty.DisplayName = uid + " Source";
            }

            internal string GetVariableName()
            {
                return ShaderProperty.ToLowerCamelCase(name);
            }
            
            internal string PrintSourceProperties(string indent)
            {
                string output = sourceShaderProperty.PrintProperties(indent);;
                if (UseContrastProperty)
                {
                    output += "\n" + indent + contrastProperty.PrintProperties(indent);
                }
                if (useNoiseProperty)
                {
                    output += "\n" + indent + noiseProperty.PrintProperties(indent);
                }
                return output;
            }

            static string GenerateUID()
            {
                string uid;
                bool valid = true;
                
                do
                {
                    uid = Random.Range(0x100000, 0xFFFFFF).ToString("x");
                    foreach (var materialLayer in ShaderGenerator2.CurrentConfig.materialLayers)
                    {
                        if (materialLayer.uid == uid)
                        {
                            valid = false;
                            break;
                        }
                    }
                } while (!valid);

                return uid;
            }

            internal void ShowPresetsMenu()
            {
                var menu = new GenericMenu();
                foreach (var preset in Presets)
                {
                    menu.AddItem(preset, false, OnSelectPreset, preset.text); 
                }
                menu.ShowAsContext();
            }

            void OnSelectPreset(object presetObj)
            {
                bool ok = EditorUtility.DisplayDialog("Load Source Preset", "Warning: this will replace all implementations and custom settings for the Source property of this layer.", "Ok", "Cancel");
                if (!ok)
                {
                    return;
                }

                foreach (var implementation in sourceShaderProperty.implementations)
                {
                    var imp_mp = implementation as ShaderProperty.Imp_MaterialProperty;
                    if (imp_mp != null)
                    {
                        imp_mp.ignoreUniquePropertyName = true;
                    }
                }
                
                var imps = CreateImplementationsFromPreset(presetObj as string, this.sourceShaderProperty);
                if (imps != null)
                {
                    this.sourceShaderProperty.SetDefaultImplementations(imps);
                    this.sourceShaderProperty.expanded = true;
                }
                else
                {
                    Debug.LogError("Couldn't create implementations from preset: " + presetObj);
                }
            }

            ShaderProperty.Implementation[] CreateImplementationsFromPreset(string method, ShaderProperty shaderProperty)
            {
                ShaderProperty.Implementation[] imps = null;
                switch (method)
                {
                    case "Normal-Based/Local/X":
                    case "Normal-Based/Local/Y":
                    case "Normal-Based/Local/Z":
                    case "Normal-Based/World/X":
                    case "Normal-Based/World/Y":
                    case "Normal-Based/World/Z":
                    {
                        bool worldSpace = method.Contains("World");
                        char axis = char.ToLowerInvariant(method[method.Length - 1]);

                        ShaderProperty.Implementation imp_normal = null;
                        if (worldSpace)
                        {
                            imp_normal = new ShaderProperty.Imp_WorldNormal(shaderProperty) { Channels = method[method.Length - 1].ToString() };
                        }
                        else
                        {
                            imp_normal = new ShaderProperty.Imp_LocalNormal(shaderProperty) { Channels = method[method.Length - 1].ToString() };
                        }

                        imps = new ShaderProperty.Implementation[]
                        {
                            new ShaderProperty.Imp_CustomCode(shaderProperty) { code = "{2}." + axis + " + {3}" },
                            imp_normal,
                            new ShaderProperty.Imp_MaterialProperty_Float(shaderProperty)
                            {
                                Label = "Normal Threshold",
                                PropertyName = string.Format("_NormalThreshold_{0}", uid)
                            }
                        };
                        break;
                    }

                    case "Position-Based/Local/X":
                    case "Position-Based/Local/Y":
                    case "Position-Based/Local/Z":
                    case "Position-Based/World/X":
                    case "Position-Based/World/Y":
                    case "Position-Based/World/Z":
                    {
                        bool worldSpace = method.Contains("World");
                        char axis = char.ToLowerInvariant(method[method.Length - 1]);

                        ShaderProperty.Implementation imp_position = null;
                        if (worldSpace)
                        {
                            imp_position = new ShaderProperty.Imp_WorldPosition(shaderProperty) { Channels = method[method.Length - 1].ToString() };
                        }
                        else
                        {
                            imp_position = new ShaderProperty.Imp_LocalPosition(shaderProperty) { Channels = method[method.Length - 1].ToString() };
                        }

                        imps = new ShaderProperty.Implementation[]
                        {
                            new ShaderProperty.Imp_CustomCode(shaderProperty) { code = "( {2}." + axis + " * {4} ) + {3}" },
                            imp_position,
                            new ShaderProperty.Imp_MaterialProperty_Float(shaderProperty)
                            {
                                Label = "Position Threshold",
                                PropertyName = string.Format("_PositionThreshold_{0}", uid)
                            },
                            new ShaderProperty.Imp_MaterialProperty_Float(shaderProperty)
                            {
                                Label = "Position Range",
                                PropertyName = string.Format("_PositionRange_{0}", uid),
                            }
                        };
                        break;
                    }
                    
                    case "Vertex Colors/R":
                    case "Vertex Colors/G":
                    case "Vertex Colors/B":
                    case "Vertex Colors/A":
                    {
                        char channel = char.ToLowerInvariant(method[method.Length - 1]);
                        imps = new ShaderProperty.Implementation[]
                        {
                            new ShaderProperty.Imp_VertexColor(shaderProperty)
                            {
                                Channels = channel.ToString().ToUpperInvariant()
                            }
                        };
                        break;
                    }
                }

                return imps;
            }
            
            static readonly GUIContent[] Presets = new[]
            {
                new GUIContent("Vertex Colors/R"),
                new GUIContent("Vertex Colors/G"),
                new GUIContent("Vertex Colors/B"),
                new GUIContent("Vertex Colors/A"),
                new GUIContent("Normal-Based/Local/X"),
                new GUIContent("Normal-Based/Local/Y"),
                new GUIContent("Normal-Based/Local/Z"),
                new GUIContent("Normal-Based/World/X"),
                new GUIContent("Normal-Based/World/Y"),
                new GUIContent("Normal-Based/World/Z"),
                new GUIContent("Position-Based/Local/X"),
                new GUIContent("Position-Based/Local/Y"),
                new GUIContent("Position-Based/Local/Z"),
                new GUIContent("Position-Based/World/X"),
                new GUIContent("Position-Based/World/Y"),
                new GUIContent("Position-Based/World/Z")
            };
        }
    }
}