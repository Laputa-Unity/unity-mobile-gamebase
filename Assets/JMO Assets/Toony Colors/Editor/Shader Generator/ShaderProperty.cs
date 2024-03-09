using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ToonyColorsPro.Utilities;
using System.Linq;

// Represents a user-modifiable shader property, that will be generated and injected in the code.
// It can be defined as a Material Property, Constant, or fetched from another source (e.g. Vertex Color),
// and be combined with other source (e.g. Material Property + Vertex Color * Constant).
// It can also be locked and not modifiable by user, e.g. fixed Material Property.
//
// The Generator will fetch the ShaderProperty list and generate the relevant code for the shader:
// - Properties { } block
// - Variables declaration
// - Variables initialization

namespace ToonyColorsPro
{
	namespace ShaderGenerator
	{
		// Enums that can be used in the templates for fixed function enums
		// They have to be outside of any class to work properly with the
		// enum material property drawers

		public enum BlendFactor
		{
			[Enums.Order(0)]										One					= UnityEngine.Rendering.BlendMode.One,
			[Enums.Order(1)]										Zero				= UnityEngine.Rendering.BlendMode.Zero,
			[Enums.Order(2), Enums.Label("Source Color")]			SrcColor			= UnityEngine.Rendering.BlendMode.SrcColor,
			[Enums.Order(3), Enums.Label("1 - Source Color")]		OneMinusSrcColor	= UnityEngine.Rendering.BlendMode.OneMinusSrcColor,
			[Enums.Order(4), Enums.Label("Destination Color")]		DstColor			= UnityEngine.Rendering.BlendMode.DstColor,
			[Enums.Order(5), Enums.Label("1 - Destination Color")]	OneMinusDstColor	= UnityEngine.Rendering.BlendMode.OneMinusDstColor,
			[Enums.Order(6), Enums.Label("Source Alpha")]			SrcAlpha			= UnityEngine.Rendering.BlendMode.SrcAlpha,
			[Enums.Order(7), Enums.Label("1 - Source Alpha")]		OneMinusSrcAlpha	= UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
			[Enums.Order(8), Enums.Label("Destination Alpha")]		DstAlpha			= UnityEngine.Rendering.BlendMode.DstAlpha,
			[Enums.Order(9), Enums.Label("1 - Destination Alpha")]	OneMinusDstAlpha	= UnityEngine.Rendering.BlendMode.OneMinusDstAlpha
		}

		public enum BlendOperation
		{
			[Enums.Order(0)]									Add		= UnityEngine.Rendering.BlendOp.Add,
			[Enums.Order(1), Enums.Label("Subtract")]			Sub		= UnityEngine.Rendering.BlendOp.Subtract,
			[Enums.Order(2), Enums.Label("Reverse Subtract")]	RevSub	= UnityEngine.Rendering.BlendOp.ReverseSubtract,
			[Enums.Order(3)]									Min		= UnityEngine.Rendering.BlendOp.Min,
			[Enums.Order(4)]									Max		= UnityEngine.Rendering.BlendOp.Max
		}

		public enum DepthWrite
		{
			[Enums.Order(0)] On = 1,
			[Enums.Order(1)] Off = 0
		}

		public enum CompareFunction
		{
			[Enums.Order(0)]									Never		= UnityEngine.Rendering.CompareFunction.Never,
			[Enums.Order(1)]									Less		= UnityEngine.Rendering.CompareFunction.Less,
			[Enums.Order(2), Enums.Label("Less or Equal")]		LEqual		= UnityEngine.Rendering.CompareFunction.LessEqual,
			[Enums.Order(3)]									Equal		= UnityEngine.Rendering.CompareFunction.Equal,
			[Enums.Order(4), Enums.Label("Greater or Equal")]	GEqual		= UnityEngine.Rendering.CompareFunction.GreaterEqual,
			[Enums.Order(5)]									Greater		= UnityEngine.Rendering.CompareFunction.Greater,
			[Enums.Order(6), Enums.Label("Not Equal")]			NotEqual	= UnityEngine.Rendering.CompareFunction.NotEqual,
			[Enums.Order(7)]									Always		= UnityEngine.Rendering.CompareFunction.Always
		}

		public enum StencilOperation
		{
			[Enums.Order(0)]										Keep = UnityEngine.Rendering.StencilOp.Keep,
			[Enums.Order(1)]										Zero = UnityEngine.Rendering.StencilOp.Zero,
			[Enums.Order(2)]										Replace = UnityEngine.Rendering.StencilOp.Replace,
			[Enums.Order(3)]										Invert = UnityEngine.Rendering.StencilOp.Invert,
			[Enums.Order(4), Enums.Label("Increment Saturate")]		IncrSat = UnityEngine.Rendering.StencilOp.IncrementSaturate,
			[Enums.Order(5), Enums.Label("Decrement Saturate")]		DecrSat = UnityEngine.Rendering.StencilOp.DecrementSaturate,
			[Enums.Order(6), Enums.Label("Increment Wrap")]			IncrWrap = UnityEngine.Rendering.StencilOp.IncrementWrap,
			[Enums.Order(7), Enums.Label("Decrement Wrap")]			DecrWrap = UnityEngine.Rendering.StencilOp.DecrementWrap,
		}

		public enum Culling
		{
			[Enums.Order(0), Enums.Label("Back faces")]			Back	= UnityEngine.Rendering.CullMode.Back,
			[Enums.Order(1), Enums.Label("Front faces")]		Front	= UnityEngine.Rendering.CullMode.Front,
			[Enums.Order(2), Enums.Label("Off (double-sided)")]	Off		= UnityEngine.Rendering.CullMode.Off
		}

		/// <summary>
		/// User-friendly enum value names system
		/// </summary>
		public class Enums
		{
			[AttributeUsage(AttributeTargets.Field)]
			public class Label : Attribute
			{
				public string label;

				public Label(string label)
				{
					this.label = label;
				}
			}

			[AttributeUsage(AttributeTargets.Field)]
			public class Order : Attribute
			{
				public int order;

				public Order(int order)
				{
					this.order = order;
				}
			}

			/// <summary>
			/// Returns an array of the enum values, sorted by their [Order] attribute.
			/// This allows the custom enums to be in any order, retaining the original values that correspond to the built-in Unity enum.
			/// </summary>
			static public OrderedEnum[] GetOrderedEnumValues(Type enumType)
			{
				if(!enumType.IsEnum)
				{
					Debug.LogError("Not an enum type: " + enumType);
					return null;
				}

				List<OrderedEnum> orderedEnums = new List<OrderedEnum>();
				var fields = enumType.GetFields();
				foreach (var field in fields)
				{
					var orders = (Order[])field.GetCustomAttributes(typeof(Order), false);
					var labels = (Label[])field.GetCustomAttributes(typeof(Label), false);
					if (orders != null && orders.Length > 0)
					{
						Enum value = (Enum)field.GetValue(null);
						string name = value.ToString();
						if(labels != null && labels.Length > 0)
						{
							name = labels[0].label;
						}

						orderedEnums.Add(new OrderedEnum()
						{
							value = value,
							order = orders[0].order,
							displayName = name
						});
					}
				}
				orderedEnums.Sort((x,y) => x.order.CompareTo(y.order));
				return orderedEnums.ToArray();
			}

			public struct OrderedEnum
			{
				public Enum value;
				public string displayName;
				public int order;
			}
		}

		[Serialization.SerializeAs("sp", "manuallyModified")]
		public partial class ShaderProperty
		{
			static class UI
			{
				public const float GUI_NEWLINE_INDENT = 20;
				public const float GUI_RIGHT_BUTTONS = 40;
			}

			public enum ProgramType
			{
				Undefined,
				Vertex,
				Fragment,
				FixedFunction
			}

			internal enum OptionFeatures
			{
				VertexColors,
				NoTile_Sampling,
				NoTile_Sampling_Vertex,
				Triplanar_Sampling,
				Triplanar_Sampling_Vertex,
				Triplanar_Sampling_Global,
				Triplanar_Sampling_Local,
				HSV_Full,
				HSV_Grayscale,
				HSV_Colorize,
				Screen_Space_UV_Vertex,
				Screen_Space_UV_Fragment,
				Screen_Space_UV_Object_Offset,
				UV_Anim_Random_Offset,
				UV_Anim_Sine,
				UV_Anim_Sine_World,
				Scale_By_Texel_Size,
				World_Pos_UV_Fragment,
				World_Pos_UV_Vertex,
				Local_Pos_Fragment,
				Local_Normal_Fragment,
				World_Normal_Vertex,
				World_Normal_Fragment
			}

			[Flags]
			public enum VariableType
			{
				@float = 1,
				float2 = 2,
				float3 = 4,
				float4 = 8,
				color = 16,
				color_rgba = 32,
				fixed_function_float = 64,
				fixed_function_enum = 128,
			}

			internal static bool CheckVariableType(VariableType set, VariableType element)
			{
				return (set & element) == element;
			}

			internal static bool VariableTypeIsFixedFunction(VariableType type)
			{
				return type == VariableType.fixed_function_float || type == VariableType.fixed_function_enum;
			}

			// Doesn't include 'fixed_function' as it is a special type
			const VariableType VariableTypeAll = VariableType.@float | VariableType.float2 | VariableType.float3 | VariableType.float4 | VariableType.color | VariableType.color_rgba;

			static string VariableTypeToShaderCode(VariableType type)
			{
				//TODO Handle float precision maybe?
				switch (type)
				{
					case VariableType.color:
					case VariableType.float3:
						return "float3";
					case VariableType.color_rgba:
					case VariableType.float4:
						return "float4";
					case VariableType.@float:
						return "float";
					case VariableType.float2:
						return "float2";
				}

				return null;
			}

			string VariableTypeToName(VariableType type)
			{
				if (type == VariableType.color_rgba)
				{
					return "color (rgba)";
				}
				else if (type == VariableType.fixed_function_float)
				{
					return "float (fixed function)";
				}
				else if (type == VariableType.fixed_function_enum)
				{
					return "enum (fixed function)";
				}
				else
				{
					return type.ToString();
				}
			}

			public static int VariableTypeToChannelsCount(VariableType type)
			{
				switch (type)
				{
					case VariableType.color:
					case VariableType.float3:
						return 3;

					case VariableType.color_rgba:
					case VariableType.float4:
						return 4;

					case VariableType.@float:
						return 1;

					case VariableType.float2:
						return 2;
				}

				return -1;
			}

			public enum FloatPrecision
			{
				@float,
				half,
				@fixed
			}

			public enum ColorPrecision
			{
				LDR,
				HDR
			}

			public enum Operator
			{
				Multiply,
				Divide,
				Add,
				Subtract
			}

			static string[] OperatorSymbols = { "×", "÷", "+", "-" };

			//================================================================================================================================

			//Needed so that we can instantiate using System.Activator with ShaderProperty argument (when deserialiazing a Imp_ShaderPropertyReference):
			//a new ShaderProperty will be created, just so that we can retrieve its name, and find the correct existing one in the list (the one created is then destroyed)
			public ShaderProperty(ShaderProperty sp) { }
			public ShaderProperty() { }

			[Serialization.CustomDeserializeCallback]
			static ShaderProperty Deserialize(string data, object[] args)
			{
				var shaderProperty = new ShaderProperty();
				
				// custom callback for Implementations
				Func<object, string, object> onDeserializeImplementation = (impObj, impData) =>
				{
					return ShaderGenerator2.CurrentConfig.DeserializeImplementationHandler(impObj, impData, shaderProperty);
				};
				var implementationHandling = new Dictionary<Type, Func<object, string, object>> { { typeof(ShaderProperty.Implementation), onDeserializeImplementation } };
                    
				return (ShaderProperty)Serialization.DeserializeTo(shaderProperty, data, typeof(ShaderProperty), null, implementationHandling);
			}
			
			//================================================================================================================================

			ReorderableLayoutList layoutList = new ReorderableLayoutList();

			public string Name { get { return _name; } private set { _name = value; } }
			[Serialization.SerializeAs("name")] string _name;
			[Serialization.SerializeAs("imps")] public List<Implementation> implementations;
			[Serialization.SerializeAs("layers")] public List<string> linkedMaterialLayers = new List<string>();
			[Serialization.SerializeAs("unlocked")] public List<string> unlockedMaterialLayers = new List<string>();
			[Serialization.SerializeAs("clones"), Serialization.ForceSerialization] public Dictionary<string, ShaderProperty> clonedShaderProperties = new Dictionary<string, ShaderProperty>();

			public VariableType Type { get; private set; }
			public ProgramType Program = ProgramType.Undefined;
			public bool IsUsedInLightingFunction = false;   //TODO same process for IsUsedInVertexFunction for vert/frag shaders and automatic float4 texcoordN packing
			public List<int> usedImplementationsForCustomCode = new List<int>();

			// Material Layers
			[Serialization.SerializeAs("isClone")] internal bool isLayerClone = false;
			internal bool isMaterialLayerProperty { get { return materialLayerUid != null; } }
			internal string materialLayerUid = null;
			string layerCloneSuffix = null;
			string layersTooltip = null;

			int passBitmask;    //bitmask that determines in which passes the shader property is used
			Implementation[] defaultImplementations;
			public bool expanded;
			public List<int> implementationsExpandedStates = new List<int>();
			string helpMessage;
			string displayName = null;
			public string DisplayName
			{
				get
				{
					if (!string.IsNullOrEmpty(displayName))
					{
						if (isMaterialLayerProperty)
						{
							var ml = ShaderGenerator2.CurrentConfig.GetMaterialLayerByUID(materialLayerUid);
							if (ml != null)
							{
								return displayName.Replace(materialLayerUid, ml.name);
							}
						}

						return displayName;
					}

					return this.Name;
				}
				set { displayName = value; }
			}

			public delegate void OnImplementationsChanged();
			public OnImplementationsChanged onImplementationsChanged;

			int defaultImplementationHash = 0;
			public bool manuallyModified { get; private set; }
			public bool error { get; private set; }
			// indicates whether this property should be sampled when using its value, or at the beginning of the vert/frag functions
			public bool deferredSampling { get; set; }
			public bool cantReferenceOtherProperties { get; private set; }
			public string preventReference { get; private set; }

			public bool isHook = false;
			public string toggleFeatures = null;

			//================================================================================================================================

			public ShaderProperty(string name, VariableType type)
			{
				Name = name;
				Type = type;
				implementations = new List<Implementation> { new Imp_ConstantValue(this) };
				CallOnImplementationsChanged();
				CheckErrors();

				CustomMaterialProperty.OnCustomMaterialPropertyRemoved += OnCustomTextureRemoved;
			}

			public ShaderProperty CloneForLayer(MaterialLayer materialLayer)
			{
				var clone = new ShaderProperty();
				clone.isLayerClone = true;
				clone.Name = this.Name + "_" + materialLayer.uid;
				clone.Type = this.Type;
				clone.implementations = new List<Implementation>();
				clone.SetDefaultImplementations(this.defaultImplementations);
				clone.implementations.Clear();
				foreach (var imp in implementations)
				{
					var clonedImp = imp.CloneForNewShaderProperty(clone, materialLayer.uid);
					clone.implementations.Add(clonedImp);
				}
				clone.CallOnImplementationsChanged();
				clone.CheckErrors();
				clone.CheckHash();
				return clone;
			}

			internal IEnumerable<ShaderProperty> IterateUsedClonedProperties()
			{
				foreach (var uid in linkedMaterialLayers)
				{
					if (unlockedMaterialLayers.Contains(uid))
					{
						// print the cloned Shader Property related to this layer
						yield return clonedShaderProperties[uid];
					}
				}
			}
			
			/// Note: can return the same CustomMaterialProperty more than once
			internal IEnumerable<CustomMaterialProperty> IterateCustomMaterialProperties()
			{
				var alreadyYielded = new HashSet<CustomMaterialProperty>();
				foreach (var imp in implementations)
				{
					var imp_cmp = imp as Imp_CustomMaterialProperty;
					if (imp_cmp != null && imp_cmp.LinkedCustomMaterialProperty != null)
					{
						yield return imp_cmp.LinkedCustomMaterialProperty;
					}
				}
			}

			internal void WillBeRemoved()
			{
				foreach (var imp in implementations)
				{
					imp.WillBeRemoved();
				}
			}

			void OnCustomTextureRemoved(CustomMaterialProperty ct)
			{
				// expand this Shader Property if a linked Custom Material Property was removed to show the message
				foreach (var imp in this.implementations)
				{
					var imp_ct = imp as Imp_CustomMaterialProperty;
					if (imp_ct != null && imp_ct.LinkedCustomMaterialProperty == ct)
					{
						imp_ct.LinkedCustomMaterialProperty = null;
					}

					var imp_mp_tex = imp as Imp_MaterialProperty_Texture;
					if (imp_mp_tex != null
					    && imp_mp_tex.UvSource == ShaderProperty.Imp_MaterialProperty_Texture.UvSourceType.CustomMaterialProperty
					    && imp_mp_tex.LinkedCustomMaterialProperty == ct)
					{
						imp_mp_tex.LinkedCustomMaterialProperty = null;
					}
				}

				CallOnImplementationsChanged();
				CheckErrors();
			}

			[Serialization.OnDeserializeCallback]
			void OnDeserialize()
			{
				UpdateLayersTooltip();
				
				if (clonedShaderProperties.Count > 0)
				{
					foreach (var clonedShaderProperty in clonedShaderProperties.Values)
					{
						// non-serialized fields shared with the clones' source:
						clonedShaderProperty.Type = this.Type;
						clonedShaderProperty.Program = this.Program;

						// clones should share the same default implementations as their source
						clonedShaderProperty.defaultImplementations = this.defaultImplementations;
					}
				}
				
				CallOnImplementationsChanged();
			}

			void CallOnImplementationsChanged()
			{
				if (onImplementationsChanged != null)
				{
					onImplementationsChanged();
				}
			}

			public override string ToString()
			{
				return string.Format("[Shader Property: {0}]", Name);
			}

			public void AddPassUsage(int pass)
			{
				passBitmask |= 1 << pass;
			}

			public void SetDefaultImplementations(params Implementation[] imps)
			{
				defaultImplementations = imps;
				ResetDefaultImplementation();
			}

			int GetImplementationsHash()
			{
				string hash = "";
				foreach (var imp in implementations)
				{
					hash += imp.ToHashString();
				}
				return hash.GetHashCode();
			}

			public void ResetDefaultImplementation(bool clearMaterialLayers = true)
			{
				foreach (var imp in implementations)
				{
					imp.WillBeRemoved();
				}

				implementations.Clear();
				foreach (var imp in defaultImplementations)
				{
					implementations.Add(imp.Clone());
				}

				if (clearMaterialLayers)
				{
					linkedMaterialLayers.Clear();
					unlockedMaterialLayers.Clear();
					clonedShaderProperties.Clear();
				}

				ResolveShaderPropertyReferences();

				defaultImplementationHash = GetImplementationsHash();
				CallOnImplementationsChanged();
				CheckErrors();
				CheckHash();
			}

			public void ForceUpdateDefaultHash()
			{
				defaultImplementationHash = GetImplementationsHash();
			}

			void OnResetImplementation(object resetMaterialLayers)
			{
				ResetDefaultImplementation(resetMaterialLayers != null);
				ShaderGenerator2.NeedsShaderPropertiesUpdate = true;
			}

			public void CheckErrors()
			{
				bool wasError = this.error;
				this.error = false;
				foreach (var imp in implementations)
				{
					if (imp == null)
					{
						continue;
					}

					imp.CheckErrors();
					this.error |= imp.HasErrors;
				}

				if (wasError != error)
				{
					//ShaderGenerator2.NeedsShaderPropertiesUpdate = true;
				}
			}

			public void CheckHash()
			{
				int newHash = GetImplementationsHash();
				manuallyModified = defaultImplementationHash != newHash;
				manuallyModified |= linkedMaterialLayers.Count > 0;
				ShaderGenerator2.NeedsShaderPropertiesUpdate = true;
			}

			/// <summary>
			/// Is the Shader Property currently visible in this Config?
			/// </summary>
			public bool IsVisible()
			{
				if (ShaderGenerator2.CurrentConfig == null) return false;

				return Array.Exists(ShaderGenerator2.CurrentConfig.VisibleShaderProperties, sp => sp == this);
			}
			
			void UpdateLayersTooltip()
			{
				layersTooltip = "";
				foreach (string uid in linkedMaterialLayers)
				{
					layersTooltip += string.Format("\n- {0}", ShaderGenerator2.CurrentConfig.GetMaterialLayerByUID(uid).name);
				}
			}

			internal void AddMaterialLayer(string uid)
			{
				this.linkedMaterialLayers.Add(uid);
				UpdateLayersTooltip();
			}

			internal void RemoveMaterialLayer(string uid)
			{
				this.linkedMaterialLayers.Remove(uid);
				UpdateLayersTooltip();
			}

			string CallMethodWithCloneSuffixForEachLayer(Func<ShaderProperty, string> callback)
			{
				string output = "";
				foreach (var uid in linkedMaterialLayers)
				{
					if (unlockedMaterialLayers.Contains(uid))
					{
						// print the cloned Shader Property related to this layer
						output += callback(clonedShaderProperties[uid]);
					}
					else
					{
						// clone this Shader Property with suffix
						this.layerCloneSuffix = uid;
						output += callback(this);	
					}
				}
				this.layerCloneSuffix = null;
				return output;
			}

			string CallMethodWithCloneSuffixForLayer(string uid, Func<ShaderProperty, string> callback)
			{
				if (unlockedMaterialLayers.Contains(uid))
				{
					return callback(clonedShaderProperties[uid]);
				}
				else
				{
					this.layerCloneSuffix = uid;
					string output = callback(this);
					this.layerCloneSuffix = null;
					return output;
				}
			}

			//Print the properties from this ShaderProperty, if any
			public string PrintProperties(string indent = "")
			{
				var result = "";
				foreach (var i in implementations)
				{
					var str = i.PrintProperty(indent);
					if (!string.IsNullOrEmpty(str))
					{
						result += indent + str + "\n";
					}
				}
				if (string.IsNullOrEmpty(result.Trim()))
					return "";
				return result.TrimEnd('\n').TrimStart();
			}

			internal string PrintPropertiesForLayer(string indent, string uid)
			{
				string output = "";
				if (linkedMaterialLayers.Contains(uid))
				{
					output += CallMethodWithCloneSuffixForLayer(uid, (sp) => string.Format("\n{0}{1}", indent, sp.PrintProperties(indent)));
				}
				return output;
			}

			//Print the variables/properties declaration for this ShaderProperty, if any
			public string PrintVariableDeclare(bool gpuInstanced, string indent)
			{
				string output = PrintVariableDeclare_Internal(gpuInstanced, indent);
				output += CallMethodWithCloneSuffixForEachLayer((sp) => string.Format("\n{0}", sp.PrintVariableDeclare_Internal(gpuInstanced, indent)));
				return output;
			}
			
			public string PrintVariableDeclare_Internal(bool gpuInstanced, string indent)
			{
				var result = "";
				foreach (var i in implementations)
				{
					var str = i.PrintVariableDeclare(indent, gpuInstanced);
					if (!string.IsNullOrEmpty(str))
					{
						result += str + "\n";
					}
				}

				if (string.IsNullOrEmpty(result.Trim()))
				{
					return "";
				}

				return result.TrimEnd('\n');
			}

			//Print the variables/properties declaration that are incompatible with CBuffer/GPU instancing buffer
			public string PrintVariableDeclareOutsideCBuffer(string indent)
			{
				string output = PrintVariableDeclareOutsideCBuffer_Internal(indent);
				output += CallMethodWithCloneSuffixForEachLayer((sp) => string.Format("\n{0}", sp.PrintVariableDeclareOutsideCBuffer_Internal(indent)));
				return output;
			}
			
			public string PrintVariableDeclareOutsideCBuffer_Internal(string indent)
			{
				string result = "";
				foreach (var imp in implementations)
				{
					string prop = imp.PrintVariableDeclareOutsideCBuffer(indent);
					if (prop != null)
					{
						result += prop + "\n";
					}
				}
				return result.TrimEnd('\n');
			}

			//Print variables in SurfaceOutput so that they can be used in the Lighting function (and possibly cross-referenced in the Surface function)
			public string PrintVariableSurfaceOutput(string indent = "")
			{
				if (!IsUsedInLightingFunction || deferredSampling)
					return "";

				return string.Format("{0} {1};", VariableTypeToShaderCode(Type), GetVariableName());
			}

			//Print the variable(s) sampling/calculations for this ShaderProperty
			public string PrintVariableSample(string inputSource, string outputSource, ProgramType program, string arguments, string indent, string prefix = null, bool skipBaseProperty = false)
			{
				string output = skipBaseProperty ? "" : PrintVariableSample_Internal(inputSource, outputSource, program, arguments, prefix);
				output += CallMethodWithCloneSuffixForEachLayer((sp) => string.Format("\n{0}{1}", indent, sp.PrintVariableSample_Internal(inputSource, outputSource, program, arguments, prefix)));
				return output;
			}
			
			public string PrintVariableSample_Internal(string inputSource, string outputSource, ProgramType program, string arguments, string prefix = null)
			{
				return PrintVariableSample(inputSource, outputSource, program, arguments, true, prefix);
			}
			
			private string PrintVariableSample(string inputSource, string outputSource, ProgramType program, string arguments, bool declareVariable, string prefix = null)
			{
				var result = "";
				HashSet<Implementation> usedImplementations = new HashSet<Implementation>(); //some implementations can be used by custom code
				for (var i = 0; i < implementations.Count; i++)
				{
					var imp = implementations[i];

					var imp_cc = imp as Imp_CustomCode;
					var imp_hsv = imp as Imp_HSV;
					if (imp_cc != null && imp_cc.usesReplacementTags && string.IsNullOrEmpty(imp_cc.tagError))
					{
						//special case: use custom code with replacement tags
						result += imp_cc.PrintVariableReplacement(ref usedImplementations, inputSource, outputSource, arguments, program);
					}
					else if (imp_hsv != null)
					{
						//special case: apply hsv modifier to used implementations so far
						result = imp_hsv.PrintVariableHSV(result);
					}
					else
					{
						if (!usedImplementations.Contains(imp))
						{
							string variable = null;
							if (program == ProgramType.Vertex)
								variable = imp.PrintVariableVertex(inputSource, outputSource, arguments);
							else if (program == ProgramType.Fragment)
								variable = imp.PrintVariableFragment(inputSource, outputSource, arguments);

							if (variable == null)
							{
								continue;
							}

							if (i > 0 && imp.HasOperator())
								result += imp.PrintOperator();
							result += variable;
						}
					}
				}

				if (declareVariable)
				{
					if (IsUsedInLightingFunction && ShaderGenerator2.CurrentPassHasLightingFunction && !isLayerClone && layerCloneSuffix == null)
					{
						return string.Format("{0}.{1} = {3}( {2} );", outputSource, GetVariableName(), result, prefix);
					}
					else
					{
						return string.Format("{0} {1} = {3}( {2} );", VariableTypeToShaderCode(Type), GetVariableName(), result, prefix);
					}
				}
				else
				{
					return string.Format("( {0} )", result);
				}
			}

			public virtual string PrintVariableSampleDeferred(string inputSource, string outputSource, ProgramType program, string args, bool declareVariable)
			{
				// HACK if in lighting function, add .input to the surface output struct when deferred sampling variables
				if (program == ProgramType.Fragment && ShaderGenerator2.IsInLightingFunction)
				{
					inputSource += ".input";
				}

				string variableSample = PrintVariableSample(inputSource, outputSource, program, args, declareVariable);
				string genericImps = PrintGenericImplementations();
				if (!string.IsNullOrEmpty(genericImps))
				{
					variableSample = string.Format("( {0}{1} )", variableSample, genericImps);
				}
				return variableSample;
			}

			//Print the variable name, optionally with "input." prefix if used in lighting function
			public string PrintVariableName(string inputSource)
			{
				string variableName = null;

				if (IsUsedInLightingFunction && ShaderGenerator2.CurrentPassHasLightingFunction)
				{
					variableName = string.Format("{0}.{1}", inputSource, GetVariableName());
				}
				else
				{
					variableName = GetVariableName();
				}

				// Generic Implementations have to be calculated when the Shader Property is sampled
				string genericImps = PrintGenericImplementations();
				if (!string.IsNullOrEmpty(genericImps))
				{
					variableName = string.Format("( {0}{1} )", variableName, genericImps);
				}

				return variableName;
			}

			string PrintGenericImplementations()
			{
				string genericImps = "";
				for(int i = 0; i < implementations.Count; i++)
				{
					if (usedImplementationsForCustomCode.Contains(i))
					{
						continue;
					}

					var genImp = implementations[i] as Imp_GenericFromTemplate;
					if (genImp != null)
					{
						genericImps += genImp.Print();
					}
				}
				return genericImps;
			}

			//Returns an array of needed features for this Shader Property to work (redundant values will be trimmed afterwards)
			public string[] NeededFeatures()
			{
				var features = new List<string>();
				foreach (var imp in implementations)
				{
					foreach (var nf in imp.NeededFeatures())
					{
						features.AddRange(GetNeededFeatures(nf, Program));
					}
					features.AddRange(imp.NeededFeaturesExtra());
				}

				return features.ToArray();
			}

			static string[] GetNeededFeatures(OptionFeatures feature, ProgramType program)
			{
				switch (feature)
				{
					case OptionFeatures.VertexColors:
					{
						if (program == ProgramType.Fragment)
						{
							return new[] { "USE_VERTEX_COLORS_FRAG", "USE_VERTEX_COLORS_VERT" };
						}
						else
						{
							return new[] { "USE_VERTEX_COLORS_VERT" };
						}
					}

					case OptionFeatures.UV_Anim_Sine:
					{
						if (program == ProgramType.Fragment)
						{
							return new[] { "UV_SINE_ANIMATION_VERTEX", "UV_SINE_ANIMATION_FRAGMENT" };
						}
						else
						{
							return new[] { "UV_SINE_ANIMATION_VERTEX" };
						}
					}

					case OptionFeatures.UV_Anim_Sine_World:
					{
						if (program == ProgramType.Fragment)
						{
							return new[] { "UV_SINE_ANIMATION_VERTEX_WORLD", "UV_SINE_ANIMATION_FRAGMENT_WORLD", "USE_WORLD_POSITION_UV_VERTEX" };
						}
						else
						{
							return new[] { "UV_SINE_ANIMATION_VERTEX_WORLD", "USE_WORLD_POSITION_UV_VERTEX" };
						}
					}

					case OptionFeatures.NoTile_Sampling: return new[] { "NOTILE_SAMPLING" };
					case OptionFeatures.NoTile_Sampling_Vertex: return new[] { "NOTILE_SAMPLING_VERTEX" };
					case OptionFeatures.Triplanar_Sampling: return new[] { "TRIPLANAR_SAMPLING" };
					case OptionFeatures.Triplanar_Sampling_Global: return new[] { "TRIPLANAR_SAMPLING_GLOBAL" };
					case OptionFeatures.Triplanar_Sampling_Local: return new[] { "TRIPLANAR_SAMPLING_LOCAL" };
					case OptionFeatures.Triplanar_Sampling_Vertex: return new[] { "TRIPLANAR_SAMPLING_VERTEX" };
					case OptionFeatures.HSV_Full: return new[] { "USE_HSV_FULL" };
					case OptionFeatures.HSV_Grayscale: return new[] { "USE_HSV_GRAYSCALE" };
					case OptionFeatures.HSV_Colorize: return new[] { "USE_HSV_COLORIZE" };
					case OptionFeatures.Screen_Space_UV_Vertex: return new[] { "USE_SCREEN_SPACE_UV_VERTEX" };
					case OptionFeatures.Screen_Space_UV_Fragment: return new[] { "USE_SCREEN_SPACE_UV_FRAGMENT" };
					case OptionFeatures.Screen_Space_UV_Object_Offset: return new[] { "SCREEN_SPACE_UV_OBJECT_OFFSET" };
					case OptionFeatures.UV_Anim_Random_Offset: return new[] { "HASH_22" };
					case OptionFeatures.World_Pos_UV_Fragment: return new[] { "USE_WORLD_POSITION_FRAGMENT" };
					case OptionFeatures.World_Pos_UV_Vertex: return new[] { "USE_WORLD_POSITION_UV_VERTEX" };
					case OptionFeatures.Local_Pos_Fragment: return new[] { "USE_OBJECT_POSITION_FRAGMENT" };
					case OptionFeatures.Local_Normal_Fragment: return new[] { "USE_OBJECT_NORMAL_FRAGMENT" };
					case OptionFeatures.World_Normal_Vertex: return new[] { "USE_WORLD_NORMAL_UV_VERTEX" };
					case OptionFeatures.World_Normal_Fragment: return new[] { "USE_WORLD_NORMAL_FRAGMENT" };
				}

				return new string[0];
			}

			public static string[] AllOptionFeatures()
			{
				return new string[]
				{
					"USE_VERTEX_COLORS_FRAG",
					"USE_VERTEX_COLORS_VERT",
					"NOTILE_SAMPLING",
					"NOTILE_SAMPLING_VERTEX",
					"USE_HSV_FULL",
					"USE_HSV_GRAYSCALE",
					"USE_HSV_COLORIZE",
					"USE_SCREEN_SPACE_UV",
					"USE_SCREEN_SPACE_UV_FRAGMENT",
					"SCREEN_SPACE_UV_OBJECT_OFFSET",
					"HASH_22"
				};
			}

			internal string GetVariableName()
			{
				if (VariableTypeIsFixedFunction(Type))
				{
					// There can only be one implementation for fixed function properties
					return implementations[0].PrintVariableFixedFunction();
				}

				if (layerCloneSuffix != null)
				{
					return string.Format("__{0}_{1}", ToLowerCamelCase(this.Name), layerCloneSuffix);
				}

				return string.Format("__{0}", ToLowerCamelCase(this.Name, this.isMaterialLayerProperty || this.isLayerClone));
			}

			internal static string ToLowerCamelCase(string input, bool keepUnderscores = false)
			{
				string output = "";
				bool upper = false;
				for (int i = 0; i < input.Length; i++)
				{
					if (char.IsLetterOrDigit(input[i]) || (keepUnderscores && input[i] == '_'))
					{
						output += upper ? char.ToUpperInvariant(input[i]) : char.ToLowerInvariant(input[i]);
						upper = false;
					}
					else
					{
						upper = true;
					}
				}
				return output;
			}

			public struct MenuItem
			{
				public GUIContent guiContent;
				public bool disabled;
				public bool on;
				public GenericMenu.MenuFunction menuFunction;
				public GenericMenu.MenuFunction2 menuFunction2;
				public object args;
				public int order;
				public bool isSeparator;
				public string separatorPath;
			}

			bool IsImplementationCompatible(Type implementationType)
			{
				var compatibility = implementationType.GetProperty("VariableCompatibility", BindingFlags.Public | BindingFlags.Static);
				return (compatibility != null && CheckVariableType((VariableType)compatibility.GetValue(null, null), Type));
			}

			GenericMenu CreateImplementationsMenu(int index, bool add)
			{
				//create menu for available implementations
				var itemsList = new List<MenuItem>();
				var types = typeof(ShaderProperty).GetNestedTypes();
				bool hasGenericImpls = false;
				foreach (var t in types)
				{
					if (t.IsSubclassOf(typeof(Implementation)))
					{
						if (t == typeof(Imp_Hook))
						{
							continue;
						}

						if (t == typeof(Imp_GenericFromTemplate))
						{
							if (this.Type == VariableType.fixed_function_enum || this.Type == VariableType.fixed_function_float)
							{
								continue;
							}

							int order = Array.IndexOf(Implementation.MenuOrders, t) * 1000;
							var selectedImp = add ? null : implementations[index] as Imp_GenericFromTemplate;

							// Get available generic implementations and build menu options
							for (int i = 0; i < Imp_GenericFromTemplate.AvailableGenericImplementations.Count; i++)
							{
								var imp = Imp_GenericFromTemplate.AvailableGenericImplementations[i];
								bool selected = selectedImp != null && selectedImp.sourceIdentifier == imp.identifier;

								// different pass (note: pass 0 sets bit 1, etc.)
								if ((this.passBitmask & (1<<imp.pass)) == 0)
								{
									continue;
								}

								// same "callback" as below, except for 'newImp' being cloned instead of dynamically created
								GenericMenu.MenuFunction callback = () =>
								{
									//remove existing to prevent false positive unique name mismatch
									Implementation temp = null;
									if (!add)
									{
										//don't do anything if the same type is selected
										if (selected)
											return;

										temp = implementations[index];
										implementations[index].WillBeRemoved();
										implementations[index] = null;
									}

									var newImp = imp.CreateImplementation(this);
									if (add)
									{
										implementations.Insert(index, newImp);
									}
									else
									{
										newImp.CopyCommonProperties(temp);
										temp = null;
										implementations[index] = newImp;
									}

									CheckHash();
									CheckErrors();
									CallOnImplementationsChanged();
								};

								bool disabled = false;

								// check compatibility
								disabled = !imp.compatibleShaderProperties.Contains(this);

								if (disabled)
								{
									/*
									string suffix = " (calculated elsewhere in code)";
									itemsList.Add(new MenuItem { disabled = true, order = order + i, guiContent = new GUIContent(imp.MenuLabel + suffix) });
									*/
								}
								else
								{
									if (!hasGenericImpls)
									{
										hasGenericImpls = true;
										itemsList.Add(new MenuItem() { order = order + i - 1, isSeparator = true, separatorPath = "Special/" });
									}

									itemsList.Add(new MenuItem { order = order + i, guiContent = new GUIContent(imp.MenuLabel), on = selected, menuFunction = callback });
								}
							}

							continue;
						}

						if (IsImplementationCompatible(t))
						{
							int order = Array.IndexOf(Implementation.MenuOrders, t) * 1000;
							string label = t.GetProperty("MenuLabel", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as string;
							bool selected = add ? false : implementations[index].GetType() == t;

							//Imp_CustomMaterialProperty: disable if there isn't any custom material property defined, or add list of defined custom material property
							if (t == typeof(Imp_CustomMaterialProperty))
							{
								if (ShaderGenerator2.CurrentConfig.CustomMaterialProperties == null || ShaderGenerator2.CurrentConfig.CustomMaterialProperties.Length == 0)
								{
									itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(label), disabled = true });
								}
								else if (this.cantReferenceOtherProperties)
								{
									itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(label), disabled = true });
								}
								else
								{
									var ctImp = add ? null : implementations[index] as Imp_CustomMaterialProperty;

									GenericMenu.MenuFunction2 ctCallback = (object data) =>
									{
										var ct = data as CustomMaterialProperty;

										//only replace custom material property instance if same type
										if (!add && implementations[index].GetType() == t)
										{
											(implementations[index] as Imp_CustomMaterialProperty).LinkedCustomMaterialProperty = ct;
											(implementations[index] as Imp_CustomMaterialProperty).InitChannelsSwizzle();
										}
										//else create a new custom material property implementation
										else
										{
											var newImp = Activator.CreateInstance(t, new object[] { this }) as Imp_CustomMaterialProperty;
											newImp.LinkedCustomMaterialProperty = ct;
											newImp.InitChannelsSwizzle();
											if (add)
											{
												implementations.Insert(index, newImp);
											}
											else
											{
												newImp.CopyCommonProperties(implementations[index]);
												implementations[index].WillBeRemoved();
												implementations[index] = newImp;
											}
											CallOnImplementationsChanged();
										}

										CheckHash();
										CheckErrors();
										CallOnImplementationsChanged();
									};

									foreach (var ct in ShaderGenerator2.CurrentConfig.CustomMaterialProperties)
									{
										//add each custom material property as an option
										selected = add ? false : ctImp != null && ctImp.LinkedCustomMaterialProperty == ct;
										itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(string.Format("{0}/{1} ({2})", label, ct.Label, ct.PropertyName)), on = selected, menuFunction2 = ctCallback, args = ct });
									}
								}
							}
							//Imp_ShaderPropertyReference: disable if there isn't any other shader property available, or add list of other shader properties
							else if (t == typeof(Imp_ShaderPropertyReference))
							{
								if (ShaderGenerator2.CurrentConfig.VisibleShaderProperties == null || ShaderGenerator2.CurrentConfig.VisibleShaderProperties.Length == 0)
									itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(label), disabled = true });
								else if (this.cantReferenceOtherProperties)
									itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(label), disabled = true });
								else
								{
									var spRefImp = add ? null : implementations[index] as Imp_ShaderPropertyReference;

									GenericMenu.MenuFunction2 spCallback = (object data) =>
									{
										var sp = data as ShaderProperty;

										//only replace shader property instance if same type
										if (!add && implementations[index].GetType() == t)
										{
											(implementations[index] as Imp_ShaderPropertyReference).LinkedShaderProperty = sp;
										}
										//else create a new shader property implementation
										else
										{
											var newImp = Activator.CreateInstance(t, new object[] { this }) as Imp_ShaderPropertyReference;
											newImp.LinkedShaderProperty = sp;
											if (add)
											{
												implementations.Insert(index, newImp);
											}
											else
											{
												newImp.CopyCommonProperties(implementations[index]);
												implementations[index].WillBeRemoved();
												implementations[index] = newImp;
											}
											CallOnImplementationsChanged();
										}

										CheckHash();
										CheckErrors();
										CallOnImplementationsChanged();
									};

									var list = new List<ShaderProperty>(ShaderGenerator2.CurrentConfig.VisibleShaderProperties);
									list.Sort((x, y) => string.Compare(x.Name, y.Name));
									for (int i = 0; i <list.Count; i++)
									{
										var sp = list[i];

										//avoid cyclic reference
										if (sp == this)
											continue;

										string referenceError = Imp_ShaderPropertyReference.IsReferencePossible(this, sp);

										if (referenceError != "")
										{
											//add each shader property as an option
											selected = add ? false : spRefImp != null && spRefImp.LinkedShaderProperty == sp;
											if (referenceError != null)
												itemsList.Add(new MenuItem { order = order + i, guiContent = new GUIContent(label + "/" + sp.DisplayName + " " + referenceError), disabled = true });
											else
												itemsList.Add(new MenuItem { order = order + i, guiContent = new GUIContent(label + "/" + sp.DisplayName), on = selected, menuFunction2 = spCallback, args = sp });
										}
									}
								}
							}
							//general case: just add the implementation type as new imp
							else
							{
								GenericMenu.MenuFunction callback = () =>
								{
									//remove existing to prevent false positive unique name mismatch
									Implementation temp = null;
									if (!add)
									{
										//don't do anything if the same type is selected
										if (implementations[index].GetType() == t)
											return;

										temp = implementations[index];
										implementations[index].WillBeRemoved();
										implementations[index] = null;
									}

									var newImp = Activator.CreateInstance(t, new object[] { this }) as Implementation;
									if (add)
									{
										implementations.Insert(index, newImp);
									}
									else
									{
										newImp.CopyCommonProperties(temp);
										temp = null;
										implementations[index] = newImp;
									}

									CheckHash();
									CheckErrors();
									CallOnImplementationsChanged();
								};

								bool disabled = false;
								string suffix = "";

								// can only add one Imp_HSV per Shader Property
								if (t == typeof(Imp_HSV))
								{
									if (implementations.Exists(imp => imp is Imp_HSV))
									{
										disabled = true;
										suffix = " (already added)";
									}
								}

								if (disabled)
								{
									itemsList.Add(new MenuItem { disabled = true, order = order, guiContent = new GUIContent(label + suffix) });
								}
								else
								{
									itemsList.Add(new MenuItem { order = order, guiContent = new GUIContent(label + suffix), on = selected, menuFunction = callback });
								}
							}
						}
					}
				}

				//sort items list and build menu
				var implementationsMenu = new GenericMenu();
				itemsList.Sort((item1, item2) => item1.order.CompareTo(item2.order));
				foreach (var item in itemsList)
				{
					if(item.isSeparator)
						implementationsMenu.AddSeparator(item.separatorPath);
					else if (item.disabled)
						implementationsMenu.AddDisabledItem(item.guiContent);
					else if (item.menuFunction2 != null)
						implementationsMenu.AddItem(item.guiContent, item.on, item.menuFunction2, item.args);
					else
						implementationsMenu.AddItem(item.guiContent, item.on, item.menuFunction);
				}
				return implementationsMenu;
			}

			static readonly GUIContent gc_copyImplementations = new GUIContent("Copy Implementations");
			static readonly GUIContent gc_PasteImplementations = new GUIContent("Paste Implementations");
			static GUIContent gc_cantPasteImplementations = new GUIContent();
			static readonly GUIContent gc_ExportImplementations = new GUIContent("Export Implementations...");
			static readonly GUIContent gc_ImportImplementations = new GUIContent("Import Implementations...");
			static readonly GUIContent gc_ResetImplementations = new GUIContent("Reset Default Implementation");
			static readonly GUIContent gc_ResetImplementationsML = new GUIContent("Reset Default Implementation (keep Material Layers)");
			static readonly GUIContent gc_debugCompareImplementations = new GUIContent("Debug: compare implementations with defaults");
			static List<Implementation> s_copiedImplementationsBuffer;
			static ShaderProperty.VariableType s_copiedImplementationsType;

			/// <summary>
			/// Prevent an Implementation field/property from being copied/pasted
			/// </summary>
			[AttributeUsage(AttributeTargets.Field)]
			public class ExcludeFromCopy : Attribute { }

			void ShowContextMenu()
			{
				GenericMenu menu = new GenericMenu();

				if (Type == VariableType.fixed_function_enum)
				{
					menu.AddDisabledItem(gc_copyImplementations);
					menu.AddDisabledItem(gc_PasteImplementations);
					menu.AddSeparator("");
					menu.AddDisabledItem(gc_ExportImplementations);
					menu.AddDisabledItem(gc_ImportImplementations);
				}
				else
				{
					menu.AddItem(gc_copyImplementations, false, OnCopyImplementations);

					// verify that the copied implementations can be pasted on the target
					string cantPasteMessage = "";
					if (s_copiedImplementationsBuffer != null)
					{
						if (s_copiedImplementationsType != this.Type)
						{
							cantPasteMessage = " (incompatible type)";
						}
						else
						{
							var newImplementations = FilterCopiedImplementations(s_copiedImplementationsBuffer);

							if (newImplementations.Count > 0)
							{
								cantPasteMessage = null;
								menu.AddItem(gc_PasteImplementations, false, OnPasteImplementations, newImplementations);
							}
							else
							{
								cantPasteMessage = " (incompatible type)";
							}
						}
					}

					if (cantPasteMessage != null)
					{
						gc_cantPasteImplementations.text = string.Format("{0}{1}", gc_PasteImplementations.text, cantPasteMessage);
						menu.AddDisabledItem(gc_cantPasteImplementations);
					}

					menu.AddSeparator("");
					menu.AddItem(gc_ExportImplementations, false, OnExportImplementations);
					menu.AddItem(gc_ImportImplementations, false, OnImportImplementations);
				}

				menu.AddSeparator("");
				menu.AddItem(gc_ResetImplementations, false, OnResetImplementation, true);
				menu.AddItem(gc_ResetImplementationsML, false, OnResetImplementation, null);

				if (ShaderGenerator2.DebugMode)
				{
					menu.AddItem(gc_debugCompareImplementations, false, () =>
					{
						var method = typeof(ShaderProperty.Implementation).GetMethod("CompareToDefaultImplementation", BindingFlags.Instance | BindingFlags.NonPublic);
						foreach (var imp in this.implementations)
						{
							var genericMethod = method.MakeGenericMethod(imp.GetType());
							genericMethod.Invoke(imp, null);
						}
					});
				}

				menu.ShowAsContext();
			}

			List<Implementation> FilterCopiedImplementations(List<Implementation> implementationsToCopy)
			{
				var newImplementations = new List<Implementation>();
				foreach (var imp in implementationsToCopy)
				{
					var type = imp.GetType();
					if (!IsImplementationCompatible(type))
					{
						continue;
					}

					// TODO same for Imp_MaterialProperty_Texture when using Shader Property UV ?
					if (type == typeof(Imp_ShaderPropertyReference))
					{
						if (((Imp_ShaderPropertyReference)imp).LinkedShaderProperty != null && Imp_ShaderPropertyReference.IsReferencePossible(this, ((Imp_ShaderPropertyReference)imp).LinkedShaderProperty) != null)
						{
							continue;
						}
					}

					var newImplementation = (Implementation)Activator.CreateInstance(type, new object[] { this });

					var fields = type.GetFields();
					foreach (var field in fields)
					{
						var serializedAttributes = field.GetCustomAttributes(typeof(Serialization.SerializeAsAttribute), true);
						if (serializedAttributes.Length == 0)
						{
							continue;
						}

						var excludeAttributes = field.GetCustomAttributes(typeof(ExcludeFromCopy), true);
						if (excludeAttributes.Length > 0)
						{
							continue;
						}

						var value = field.GetValue(imp);
						field.SetValue(newImplementation, value);
					}

					newImplementations.Add(newImplementation);
				}
				return newImplementations;
			}

			void OnCopyImplementations()
			{
				s_copiedImplementationsBuffer = new List<Implementation>();

				foreach (var imp in implementations)
				{
					if (imp.CanBeCopied())
					{
						s_copiedImplementationsBuffer.Add(imp);
					}
				}

				s_copiedImplementationsType = this.Type;
			}

			void OnPasteImplementations(object newImplementations)
			{
				// Clear implementations except hooks
				implementations = implementations.Where(imp =>imp is Imp_Hook).ToList();

				foreach (var imp in (List<Implementation>)newImplementations)
				{
					imp.OnPasted();
				}
				implementations.AddRange((List<Implementation>)newImplementations);
				CheckErrors();
				CheckHash();
				CallOnImplementationsChanged();
			}

			void OnExportImplementations()
			{
				var folder = ProjectOptions.data.LastImplementationExportImportPath;
				if (!System.IO.Directory.Exists(folder))
				{
					folder = Application.dataPath;
				}

				var path = EditorUtility.SaveFilePanel("Export Implementations", folder, this.Name, "tcp2imp");
				if (!string.IsNullOrEmpty(path))
				{
					ProjectOptions.data.LastImplementationExportImportPath = System.IO.Path.GetDirectoryName(path);
					string output = "";
					foreach (var imp in implementations)
					{
						output += string.Format("{0}\n", Serialization.Serialize(imp));
					}
					System.IO.File.WriteAllText(path, output);
				}
			}

			void OnImportImplementations()
			{
				var path = EditorUtility.OpenFilePanel("Import Implementations", ProjectOptions.data.LastImplementationExportImportPath, "tcp2imp");
				if (!string.IsNullOrEmpty(path))
				{
					ProjectOptions.data.LastImplementationExportImportPath = System.IO.Path.GetDirectoryName(path);

					string[] serializedImplementations = System.IO.File.ReadAllLines(path);
					if (serializedImplementations.Length > 0)
					{
						List<Implementation> importedImplementations = new List<Implementation>();
						implementations.Clear();
						foreach (var serImp in serializedImplementations)
						{
							try
							{
								var imp = (Implementation)Serialization.Deserialize(serImp, new object[] { this });
								importedImplementations.Add(imp);
							}
							catch (Exception error)
							{
								Debug.LogError(ShaderGenerator2.ErrorMsg(string.Format("Couldn't deserialize the following line from tcp2imp file:\n\"{0}\"\nError returned:\n{1}", serImp, error.ToString())));
							}
						}

						if (importedImplementations.Count > 0)
						{
							var newImplementations = FilterCopiedImplementations(importedImplementations);
							if (newImplementations.Count > 0)
							{
								OnPasteImplementations(newImplementations);
							}
							else
							{
								EditorUtility.DisplayDialog("Import Implementations", "No compatible implementations found for this Shader Property type (" + Type.ToString() + ")", "OK");
							}
						}
						else
						{
							EditorUtility.DisplayDialog("Import Implementations", "No valid implementations could be found in this file.", "OK");
						}
					}
					else
					{
						Debug.LogError(ShaderGenerator2.ErrorMsg(("Empty tcp2imp file!")));
					}
				}
			}

			public void ResolveShaderPropertyReferences()
			{
				foreach (var imp in implementations)
				{
					var impSpRef = imp as Imp_ShaderPropertyReference;
					if (impSpRef != null)
					{
						impSpRef.TryToFindLinkedShaderProperty();
					}

					var impMpTex = imp as Imp_MaterialProperty_Texture;
					if (impMpTex != null)
					{
						if (impMpTex.UvSource == Imp_MaterialProperty_Texture.UvSourceType.OtherShaderProperty)
						{
							impMpTex.TryToFindLinkedShaderProperty();
						}
						else if (impMpTex.UvSource == Imp_MaterialProperty_Texture.UvSourceType.CustomMaterialProperty)
						{
							impMpTex.TryToFindLinkedCustomMaterialProperty();
						}
					}

					var impCC = imp as Imp_CustomCode;
					if (impCC != null)
					{
						impCC.TryToFindPrependCodeBlock();
						impCC.CheckReplacementTags();
					}
				}

				CheckErrors();
			}

			// TODO move
			int matLayerTab = -1;
			float matLayerScroll;
			float matLayerScrollTarget;
			
			public void ShowGUILayout(float indentLeft = 0)
			{
				EditorGUI.BeginChangeCheck();

				var guiColor = GUI.color;
				GUI.color *= EditorGUIUtility.isProSkin || (manuallyModified && !isMaterialLayerProperty) || error ? Color.white : new Color(.75f, .75f, .75f, 1f);
				var style = EditorStyles.helpBox;
				if (error)
				{
					style = expanded ? TCP2_GUI.ErrorPropertyHelpBoxExp : TCP2_GUI.ErrorPropertyHelpBox;
				}
				else if (manuallyModified && !isMaterialLayerProperty)
				{
					style = expanded ? TCP2_GUI.EnabledPropertyHelpBoxExp : TCP2_GUI.EnabledPropertyHelpBox;
				}

				if (indentLeft > 0)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(indentLeft);
				}

				EditorGUILayout.BeginVertical(style);
				GUI.color = guiColor;

				var rect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));
				var guiContent = new GUIContent(DisplayName);
				var typeLabel = new GUIContent(": " + VariableTypeToName(Type));
				var programLabel = new GUIContent(Program.ToString());
				float labelWidth = TCP2_GUI.HeaderDropDown.CalcSize(guiContent).x;
				float typeWidth = SGUILayout.Styles.GrayMiniLabel.CalcSize(typeLabel).x;
				float programLabelWidth = SGUILayout.Styles.GrayMiniLabel.CalcSize(programLabel).x;
				float rightMenuButtonWidth = 16;

				// hover
				TCP2_GUI.DrawHoverRect(rect);

				// main foldout
				var foldoutRect = rect;
				foldoutRect.width -= rightMenuButtonWidth;
				EditorGUI.BeginChangeCheck();
				expanded = GUI.Toggle(foldoutRect, expanded, guiContent, TCP2_GUI.HeaderDropDown);
				if (EditorGUI.EndChangeCheck())
				{
					if (Event.current.alt || Event.current.control)
					{
						var state = expanded;
						foreach (var sp in ShaderGenerator2.CurrentConfig.VisibleShaderProperties)
						{
							sp.expanded = state;
						}
					}
				}

				// variable type (color, color_rgba, float, ...)
				rect = GUILayoutUtility.GetLastRect();
				var r = rect;
				r.x += labelWidth;
				r.width -= labelWidth;
				using (new EditorGUI.DisabledScope(true))
				{
					GUI.Label(r, typeLabel, EditorStyles.miniLabel);
				}

				// help icon if there's a help message
				bool hasHelpMessage = helpMessage != null;
				if (hasHelpMessage)
				{
					r = rect;
					r.x += labelWidth + typeWidth;
					r.width = 16;
					r.y += 1;
					GUI.Label(r, TCP2_GUI.TempContent(null, TCP2_GUI.SmallHelpIconTexture));

					bool mouseOver = r.Contains(Event.current.mousePosition);
					ShaderGenerator2.showDynamicTooltip |= mouseOver;
					if (mouseOver)
					{
						ShaderGenerator2.dynamicTooltip = helpMessage;
					}
				}

				// program type (vertex, fragment, lighting)
				r = rect;
				r.x += rect.width - programLabelWidth - rightMenuButtonWidth;
				r.width = programLabelWidth;
				if (!isMaterialLayerProperty)
				{
					using (new EditorGUI.DisabledScope(true))
					{
						GUI.Label(r, programLabel, EditorStyles.miniLabel);
					}
				}
				
				if (linkedMaterialLayers.Count > 0)
				{
					r.width = 20;
					r.x -= r.width;
#if !UNITY_2019_3_OR_NEWER
					r.y += 2;
#endif
					GUI.Label(r, TCP2_GUI.TempContent(null, TCP2_GUI.LayersIconTexture));

					bool mouseOver = r.Contains(Event.current.mousePosition);
					ShaderGenerator2.showDynamicTooltip |= mouseOver;
					if (mouseOver)
					{
						ShaderGenerator2.dynamicTooltip = "This property uses Material Layers:" + layersTooltip;
					}
				}

				// implementations copy/export/import menu
				r = rect;
				r.x += rect.width - rightMenuButtonWidth;
				r.width = rightMenuButtonWidth;
				bool showMenu = GUI.Button(r, GUIContent.none, TCP2_GUI.ContextMenuButton);
				showMenu |= Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.modifiers == EventModifiers.None && foldoutRect.Contains(Event.current.mousePosition);

				if (showMenu)
				{
					ShowContextMenu();
				}

				if (expanded)
				{
					// Material Layer tabs
					if (!this.isMaterialLayerProperty && ShaderGenerator2.CurrentConfig.materialLayers.Count > 0)
					{
						GUILayout.Space(6);
						EditorGUILayout.BeginHorizontal();
						{
#if UNITY_2019_3_OR_NEWER
							var labelStyle = EditorStyles.label;
#else
							var labelStyle = EditorStyles.miniLabel;
#endif
							GUILayout.Label("Material Layers:", labelStyle, GUILayout.ExpandWidth(false));
							matLayerTab = TabsHorizontalInfinite(matLayerTab + 1, ShaderGenerator2.CurrentConfig.materialLayersNames, ref matLayerScroll, ref matLayerScrollTarget) - 1;
						}
						EditorGUILayout.EndHorizontal();
					}

					bool disableUi = false;
					bool drawBaseShaderProperty = true;
					matLayerTab = Mathf.Clamp(matLayerTab, -1, ShaderGenerator2.CurrentConfig.materialLayers.Count - 1);
					if (matLayerTab >= 0)
					{
						// Draw Material Layer
						
						var ml = ShaderGenerator2.CurrentConfig.materialLayers[matLayerTab];
						bool layerIsEnabled = linkedMaterialLayers.Contains(ml.uid);
						EditorGUI.BeginChangeCheck();
						GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
						bool toggle = GUILayout.Toggle(layerIsEnabled, TCP2_GUI.TempContent(string.Format(" Enable Layer '{0}'", ml.name)));
						if (EditorGUI.EndChangeCheck())
						{
							if (toggle)
							{
								AddMaterialLayer(ml.uid);
							}
							else
							{
								RemoveMaterialLayer(ml.uid);
							}
						}

						bool layerIsLocked = !unlockedMaterialLayers.Contains(ml.uid);
						EditorGUI.BeginDisabledGroup(!layerIsEnabled);
						EditorGUI.BeginChangeCheck();
						bool locked = GUILayout.Toggle(layerIsLocked, TCP2_GUI.TempContent(" Same as Base layer"));
						if (EditorGUI.EndChangeCheck())
						{
							if (!locked)
							{
								unlockedMaterialLayers.Add(ml.uid);
								if (!clonedShaderProperties.ContainsKey(ml.uid))
								{
									var cloneSp = this.CloneForLayer(ml);
									clonedShaderProperties.Add(ml.uid, cloneSp);
								}
							}
							else
							{
								unlockedMaterialLayers.Remove(ml.uid);
							}
						}
						EditorGUI.EndDisabledGroup();

						disableUi = !layerIsEnabled || layerIsLocked;

						if (!locked)
						{
							EditorGUI.BeginDisabledGroup(disableUi);
							{
								clonedShaderProperties[ml.uid].ShowImplementationsGUI();
								drawBaseShaderProperty = false;
							}
							EditorGUI.EndDisabledGroup();
						}
					}

					if (drawBaseShaderProperty)
					{
						EditorGUI.BeginDisabledGroup(disableUi);
						ShowImplementationsGUI();
						EditorGUI.EndDisabledGroup();
					}
				}

				EditorGUILayout.EndVertical();
				if (indentLeft > 0)
				{
					EditorGUILayout.EndHorizontal();
				}

				if (EditorGUI.EndChangeCheck())
				{
					CheckHash();
					CheckErrors();
				}
			}

			void ShowImplementationsGUI()
			{
				var guiColor = GUI.color;
				int removeAt = -1;
				int insertAt = -1;
				
				//lambda function so that we can reorder drawing when one is selected
				Action<int, float> DrawImplementation = (index, indent) =>
				{
					bool usedByCustomCode = usedImplementationsForCustomCode.Contains(index);

					if (index > 0)
					{
						GUILayout.Space(1);
						SGUILayout.DrawLine();
						GUILayout.Space(2);
					}
					else
						GUILayout.Space(6);

					GUILayout.BeginHorizontal();
					GUILayout.Space(indent);

					// button with implementation name, show imp menu on click
					if (index > 0 && implementations[index].HasOperator() && !usedByCustomCode)
					{
						var op = (int) implementations[index].@operator;
						if (GUILayout.Button(OperatorSymbols[op], EditorStyles.popup, GUILayout.Width(35)))
						{
							var menu = new GenericMenu();
							for (var j = 0; j < OperatorSymbols.Length; j++)
							{
								menu.AddItem(new GUIContent(OperatorSymbols[j]), false, implementations[index].SetOperator, j);
							}

							menu.ShowAsContext();
						}
					}
					else if (usedByCustomCode)
					{
						using (new EditorGUI.DisabledScope(true))
						{
							GUILayout.Button(new GUIContent("CC", "Used by Custom Code"), EditorStyles.miniButton, GUILayout.Width(35));
						}
					}

					bool locked = implementations[index].IsLocked;
					using (new EditorGUI.DisabledGroupScope(locked))
					{
						if (locked)
						{
							SGUILayout.DrawLockIcon(Color.gray);
						}

						string text = string.Format("{0}. {1}", index + 1, implementations[index].GUILabel());
						var label = new GUIContent(text, locked ? "This implementation is locked and can't be changed for this property, as it is required by the shader.\nYou can still add more implementations for this property though." : "");
						if (GUILayout.Button(label, EditorStyles.popup))
						{
							//create & show context menu
							var implementationsMenu = CreateImplementationsMenu(index, false);
							implementationsMenu.ShowAsContext();
						}
					}

					//Add/Remove MoveUp/MoveDown buttons
					if (!VariableTypeIsFixedFunction(Type))
					{
						const float w = UI.GUI_RIGHT_BUTTONS / 2;
						if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(w)))
						{
							insertAt = index + 1;
						}

						using (new EditorGUI.DisabledGroupScope(implementations.Count <= 1 || locked))
						{
							if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(w)))
							{
								removeAt = index;
							}
						}
					}

					GUILayout.EndHorizontal();

					//Parameters depending on property type
					GUILayout.Space(1);
					implementations[index].NewLineGUI(usedByCustomCode);
				};

				//guiColor = GUI.color;
				GUI.color *= new Color(.92f, .92f, .92f, 1f);
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				GUI.color = guiColor;
				{
					bool reorder = layoutList.DoLayoutList(DrawImplementation, implementations);
					if (reorder)
					{
						CallOnImplementationsChanged();
						CheckErrors();
					}
				}
				EditorGUILayout.EndVertical();
				
				//Add/Remove from list
				if (insertAt >= 0)
				{
					//create & show context menu
					var implementationsMenu = CreateImplementationsMenu(insertAt, true);
					implementationsMenu.ShowAsContext();
				}
				if (removeAt >= 0)
				{
					implementations[removeAt].WillBeRemoved();
					implementations.RemoveAt(removeAt);
					CallOnImplementationsChanged();
				}
			}

			static string GetMaterialLayerTabLabel(string label)
			{
#if UNITY_2019_3_OR_NEWER
				return string.Format(" {0} ", label);
#else
				return label;
#endif	
			}
			
			static readonly Color DisabledLayerColor = new Color(.7f, .7f, .7f);
			int TabsHorizontalInfinite(int selected, KeyValuePair<string, string>[] options, ref float scrollPosition, ref float targetScrollPosition)
			{
				var guiColor = GUI.color;
				EditorGUILayout.BeginHorizontal();
				{
#if UNITY_2019_3_OR_NEWER
					const float buttonHeight = 18;
#else
					const float buttonHeight = 15;
#endif
					var lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(buttonHeight));
					
					// Calculate total space used by the tabs:
					float totalWidth = 0f;
					float[] widths = new float[options.Length];
					for (int i = 0; i < options.Length; i++)
					{
						var gc = TCP2_GUI.TempContent(GetMaterialLayerTabLabel(options[i].Key));
						var size = SGUILayout.Styles.MiniButtonMid.CalcSize(gc);
						widths[i] = size.x;
						totalWidth += widths[i];
					}
					float minValue = lineRect.width - totalWidth;

					// If remaining space is negative, then tabs can't fit in the current width:
					if (minValue < 0)
					{
						const float clipHeight = 18;

						Rect btnPrevRect = lineRect;
						btnPrevRect.width = 20;
						lineRect.xMin += btnPrevRect.width;
					
						Rect btnNextRect = lineRect;
						btnNextRect.width = 20;
						lineRect.xMax -= btnNextRect.width;
						btnNextRect.x = lineRect.xMax;

						// Take arrow button widths into account:
						minValue -= 40 - 2;

						lineRect.height = clipHeight;
						GUI.BeginClip(lineRect, new Vector2(scrollPosition, 0), Vector2.zero, false);
						{
							Rect r = new Rect(0, 0, 0, buttonHeight);
							for (int i = 0; i < options.Length; i++)
							{
								r.width = widths[i];

								bool layerIsEnabled = i == 0 || this.linkedMaterialLayers.Contains(options[i].Value);
								GUI.color = !layerIsEnabled ? DisabledLayerColor : guiColor;
								{
									var gc = TCP2_GUI.TempContent(GetMaterialLayerTabLabel(options[i].Key));
									if (GUI.Toggle(r, i == selected, gc, SGUILayout.Styles.MiniButtonMid))
									{
										selected = i;
									}
								}
								GUI.color = guiColor;

								r.x += r.width;
							}
						}
						GUI.EndClip();
						
						// Arrow buttons:
						using (new EditorGUI.DisabledScope(targetScrollPosition >= 0))
						{
							if (GUI.RepeatButton(btnPrevRect, TCP2_GUI.TempContent("<"), SGUILayout.Styles.MiniButtonLeft))
							{
								targetScrollPosition += 2;
							}
						}

						using (new EditorGUI.DisabledScope(targetScrollPosition <= minValue))
						{
							if (GUI.RepeatButton(btnNextRect, TCP2_GUI.TempContent(">"), SGUILayout.Styles.MiniButtonRight))
							{
								targetScrollPosition -= 2;
							}
						}

						targetScrollPosition = Mathf.Clamp(targetScrollPosition, minValue, 0);

						if (Event.current.type == EventType.Repaint)
						{
							if (Math.Abs(targetScrollPosition - scrollPosition) > 0.1f)
							{
								scrollPosition = Mathf.Lerp(scrollPosition, targetScrollPosition, Mathf.Max(0.05f, Time.deltaTime * 0.25f));
								ShaderGenerator.ShaderGenerator2.RepaintWindow();
							}
							else
							{
								scrollPosition = targetScrollPosition;
							}
						}
						
						scrollPosition = Mathf.Clamp(scrollPosition, minValue, 0);
					}
					else
					// Else the tabs can fit:
					{
						Rect rect = lineRect;
						rect.width = widths[0];
						
						// First button:
						if (GUI.Toggle(rect, selected == 0, TCP2_GUI.TempContent(GetMaterialLayerTabLabel(options[0].Key)), options.Length > 1 ? SGUILayout.Styles.MiniButtonLeft : SGUILayout.Styles.MiniButton))
						{
							selected = 0;
						}
						
						// Mid buttons
						for (int i = 1; i < options.Length - 1; i++)
						{
							rect.xMin += rect.width;
							rect.width = widths[i];

							bool layerIsEnabled = this.linkedMaterialLayers.Contains(options[i].Value);
							GUI.color = !layerIsEnabled ? DisabledLayerColor : guiColor;
							{
								if (GUI.Toggle(rect, selected == i, TCP2_GUI.TempContent(GetMaterialLayerTabLabel(options[i].Key)), SGUILayout.Styles.MiniButtonMid))
								{
									selected = i;
								}
							}
							GUI.color = guiColor;
						}
						
						// Last Button:
						rect.xMin += rect.width;
						rect.width = widths[widths.Length - 1];
						GUI.color = !linkedMaterialLayers.Contains(options[options.Length-1].Value) ? DisabledLayerColor : guiColor;
						{
							if (GUI.Toggle(rect, selected == options.Length - 1, TCP2_GUI.TempContent(GetMaterialLayerTabLabel(options[options.Length-1].Key)), options.Length > 1 ? SGUILayout.Styles.MiniButtonRight : SGUILayout.Styles.MiniButton))
							{
								selected = options.Length - 1;
							}
						}
						GUI.color = guiColor;
					}
				}
				EditorGUILayout.EndHorizontal();

				return selected;
			}

			static Dictionary<string, string> GetAssociatedData(string[] keyValuePairs, int startIndex = 0)
			{
				var associatedData = new Dictionary<string, string>();
				for (var j = startIndex; j < keyValuePairs.Length; j++)
				{
					var kvp = keyValuePairs[j].Trim();
					if (kvp.StartsWith("imp("))
					{
						continue;
					}

					var keyValue = kvp.Split('=');
					associatedData.Add(keyValue[0].Trim(), keyValue[1].Trim());
				}
				return associatedData;
			}

			static Dictionary<string, ShaderProperty> CachedShaderPropertiesFromTemplate = new Dictionary<string, ShaderProperty>();

			public static void ClearCache()
			{
				CachedShaderPropertiesFromTemplate.Clear();
			}

			public static ShaderProperty CreateFromTemplateData(string line)
			{
				if (CachedShaderPropertiesFromTemplate.ContainsKey(line))
				{
					return CachedShaderPropertiesFromTemplate[line];
				}

				var data = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				var variableType = (VariableType)Enum.Parse(typeof(VariableType), data[0]);

				//create ShaderProperty
				var shaderProperty = new ShaderProperty(data[1], variableType);
				
				//create default implementation
				// - get associated data
				var subdata = Serialization.SplitExcludingBlocks(data[2], ',', true, true, "()");
				var programType = subdata[0].ToLowerInvariant();

				// - define program type (vertex, fragment)
				try
				{
					var type = programType;
					if (programType == "frag")
						type = "fragment";
					else if (programType == "vert")
						type = "vertex";
					else if (programType == "surface")
						type = "fragment";
					else if (programType == "lighting")
					{
						type = "fragment";
						shaderProperty.IsUsedInLightingFunction = true;
					}
					else if (programType == "fixed")
					{
						type = "FixedFunction";
					}
					shaderProperty.Program = (ProgramType)Enum.Parse(typeof(ProgramType), type, true);
				}
				catch
				{
					Debug.LogError(ShaderGenerator2.ErrorMsg("Unrecognized Shader Property program type: '" + programType + "'. It should be either '<b>vertex</b>' or '<b>fragment</b>'."));
				}

				// shaderProperty-specific data
				var associatedData = GetAssociatedData(subdata, 1);

				shaderProperty.deferredSampling = GetAssociatedDataBool(associatedData, "manually_sampled", false);
				shaderProperty.preventReference = GetAssociatedDataString(associatedData, "prevent_reference", null);
				shaderProperty.cantReferenceOtherProperties = GetAssociatedDataBool(associatedData, "cant_reference", false);
				shaderProperty.helpMessage = GetAssociatedDataString(associatedData, "help", null);
				shaderProperty.displayName = GetAssociatedDataString(associatedData, "label", null);

				// create the implementation(s)
				var list = new List<Implementation>();
				int i = 0;
				foreach(var sub in subdata)
				{
					var subTrim = sub.Trim();
					if (subTrim.StartsWith("imp("))
					{
						var imp = ParseImplementation(subTrim, shaderProperty);
						if(imp == null)
						{
							Debug.LogError(ShaderGenerator2.ErrorMsg("Couldn't parse implementation:\n" + subTrim));
						}
						else
						{
							imp.DefaultImplementationIndex = i;
							i++;
							list.Add(imp);
						}
					}
				}

				shaderProperty.SetDefaultImplementations(list.ToArray());

				// add cached so that they're not recreated at each SG2 change in the UI
				CachedShaderPropertiesFromTemplate.Add(line, shaderProperty);

				return shaderProperty;
			}

			// Parse a string-represented implementation, in the form:
			// imp(key = value, key2 = value2, key3 = value3, ...)
			static Implementation ParseImplementation(string strImplementation, ShaderProperty shaderProperty)
			{
				Implementation imp = null;

				int impLength = "imp(".Length;
				string impTrim = strImplementation.Substring(impLength, strImplementation.Length - impLength - 1);
				var impData = Serialization.SplitExcludingBlocks(impTrim, ',', true, "()");
				string impType = impData[0].Trim();
				var associatedData = GetAssociatedData(impData, 1);

				switch (impType)
				{
					case "texture":
					{
						imp = new Imp_MaterialProperty_Texture(shaderProperty)
						{
							DefaultValue = GetAssociatedDataString(associatedData, "default"),
							UvChannel = GetAssociatedDataInt(associatedData, "uv_channel", 0),
							UseTilingOffset = GetAssociatedDataBool(associatedData, "tiling_offset", false),
							GlobalTilingOffset = GetAssociatedDataBool(associatedData, "global", false),
							ScaleByTexelSize = GetAssociatedDataBool(associatedData, "scale_texel", false),
							UseScrolling = GetAssociatedDataBool(associatedData, "scrolling", false),
							GlobalScrolling = GetAssociatedDataBool(associatedData, "global_scrolling", false),
							RandomOffset = GetAssociatedDataBool(associatedData, "random_offset", false),
							GlobalRandomOffset = GetAssociatedDataBool(associatedData, "global_random_offset", false),
							MaterialDrawers = GetAssociatedDataString(associatedData, "drawer", ""),
							IsUvLocked = GetAssociatedDataBool(associatedData, "locked_uv", false),
							ChannelsCount = VariableTypeToChannelsCount(shaderProperty.Type),
							TilingOffsetVariable = GetAssociatedDataString(associatedData, "tiling_offset_var", ""),
							UVTriplanarScale = GetAssociatedDataFloat(associatedData, "triplanar_scale", 1.0f)
#if UNITY_2019_4_OR_NEWER
							, SeparateSamplerName = GetAssociatedDataString(associatedData, "sampler", null)
#endif
						};

						var channels = GetAssociatedDataString(associatedData, "channels", null);
						if (channels != null)
						{
							((Imp_MaterialProperty_Texture)imp).Channels = channels.ToUpperInvariant();
							((Imp_MaterialProperty_Texture)imp).ChannelsCount = channels.Length;
						}

						var uv_screenspace = GetAssociatedDataString(associatedData, "uv_screenspace", "");
						if (!string.IsNullOrEmpty(uv_screenspace))
						{
							((Imp_MaterialProperty_Texture)imp).SetScreenSpaceUV();
						}


						var uv_world_pos = GetAssociatedDataString(associatedData, "uv_worldpos", "");
						if (!string.IsNullOrEmpty(uv_world_pos))
						{
							((Imp_MaterialProperty_Texture)imp).SetWorldPositionUV();
						}

						var uv_triplanar = GetAssociatedDataString(associatedData, "uv_triplanar", "");
						if (!string.IsNullOrEmpty(uv_triplanar))
						{
							((Imp_MaterialProperty_Texture)imp).SetTriplanarUV();
						}

						var uv_shaderproperty = GetAssociatedDataString(associatedData, "uv_shaderproperty", "");
						if (!string.IsNullOrEmpty(uv_shaderproperty))
						{
							((Imp_MaterialProperty_Texture)imp).SetShaderPropertyUV();
							((Imp_MaterialProperty_Texture)imp).LinkedShaderPropertyName = uv_shaderproperty;

							var swizzle = GetAssociatedDataString(associatedData, "swizzle", null);
							if (!string.IsNullOrEmpty(swizzle))
							{
								((Imp_MaterialProperty_Texture)imp).UVChannels = swizzle;
							}
						}

						break;
					}

					case "float":
						imp = new Imp_MaterialProperty_Float(shaderProperty)
						{
							DefaultValue = GetAssociatedDataFloat(associatedData, "default"),
							MaterialDrawers = GetAssociatedDataString(associatedData, "drawer", "")
						};
						break;

					case "range":
						imp = new Imp_MaterialProperty_Range(shaderProperty)
						{
							DefaultValue = GetAssociatedDataFloat(associatedData, "default"),
							Min = GetAssociatedDataFloat(associatedData, "min"),
							Max = GetAssociatedDataFloat(associatedData, "max"),
							MaterialDrawers = GetAssociatedDataString(associatedData, "drawer", "")
						};
						break;

					case "vector":
					{
						var values = GetAssociatedDataString(associatedData, "default", "(0, 0, 0, 0)").TrimStart('(').TrimEnd(')');
						var defaultValueSplit = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
						var defaultValue = Vector4.zero;
						defaultValue.x = defaultValueSplit.Length >= 1 ? float.Parse(defaultValueSplit[0], CultureInfo.InvariantCulture) : 0f;
						defaultValue.y = defaultValueSplit.Length >= 2 ? float.Parse(defaultValueSplit[1], CultureInfo.InvariantCulture) : 0f;
						defaultValue.z = defaultValueSplit.Length >= 3 ? float.Parse(defaultValueSplit[2], CultureInfo.InvariantCulture) : 0f;
						defaultValue.w = defaultValueSplit.Length >= 4 ? float.Parse(defaultValueSplit[3], CultureInfo.InvariantCulture) : 0f;
						imp = new Imp_MaterialProperty_Vector(shaderProperty)
						{
							DefaultValue = defaultValue,
							MaterialDrawers = GetAssociatedDataString(associatedData, "drawer", "")
						};
					}
					break;

					case "color":
					{
						var values = GetAssociatedDataString(associatedData, "default", "(0, 0, 0, 0)").TrimStart('(').TrimEnd(')');
						var defaultValueSplit = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
						var defaultValue = Color.white;
						defaultValue.r = defaultValueSplit.Length >= 1 ? float.Parse(defaultValueSplit[0], CultureInfo.InvariantCulture) : 0f;
						defaultValue.g = defaultValueSplit.Length >= 2 ? float.Parse(defaultValueSplit[1], CultureInfo.InvariantCulture) : 0f;
						defaultValue.b = defaultValueSplit.Length >= 3 ? float.Parse(defaultValueSplit[2], CultureInfo.InvariantCulture) : 0f;
						defaultValue.a = defaultValueSplit.Length >= 4 ? float.Parse(defaultValueSplit[3], CultureInfo.InvariantCulture) : 1f;
						imp = new Imp_MaterialProperty_Color(shaderProperty)
						{
							DefaultValue = defaultValue,
							Hdr = GetAssociatedDataBool(associatedData, "hdr", false),
							MaterialDrawers = GetAssociatedDataString(associatedData, "drawer", "")
						};
					}
					break;

					case "vertex_color":
					{
						imp = new Imp_VertexColor(shaderProperty);
						var channels = GetAssociatedDataString(associatedData, "swizzle", null);
						if (!string.IsNullOrEmpty(channels))
							(imp as Imp_VertexColor).Channels = channels;
					}
					break;

					case "vertex_normal":
					{
						imp = new Imp_LocalNormal(shaderProperty);
						var channels = GetAssociatedDataString(associatedData, "swizzle", null);
						if (!string.IsNullOrEmpty(channels))
							(imp as Imp_LocalNormal).Channels = channels;
					}
					break;

					case "world_position":
					{
						imp = new Imp_WorldPosition(shaderProperty);
						var channels = GetAssociatedDataString(associatedData, "swizzle", null);
						if (!string.IsNullOrEmpty(channels))
							(imp as Imp_WorldPosition).Channels = channels;
					}
					break;

					case "vertex_texcoord":
					{
						imp = new Imp_VertexTexcoord(shaderProperty);
						var channels = GetAssociatedDataString(associatedData, "swizzle", null);
						if (!string.IsNullOrEmpty(channels))
							(imp as Imp_VertexTexcoord).Channels = channels;
						var texcoordChannel = GetAssociatedDataInt(associatedData, "texcoord", -1);
						if (texcoordChannel >= 0)
							(imp as Imp_VertexTexcoord).TexcoordChannel = texcoordChannel;
					}
					break;

					case "custom_code":
					{
						imp = new Imp_CustomCode(shaderProperty)
						{
							code = GetAssociatedDataString(associatedData, "code")
						};
					}
					break;

					case "shader_property_reference":
					case "shader_property_ref":
					{
						var linkedPropertyName = GetAssociatedDataString(associatedData, "reference", null);
						var channels = GetAssociatedDataString(associatedData, "swizzle", null);
						imp = new Imp_ShaderPropertyReference(shaderProperty)
						{
							//only reference name here, the actual one will be retrieved later because it might not exist yet
							LinkedShaderPropertyName = linkedPropertyName,
							Channels = channels
						};
					}
					break;

					case "constant":
					{
						switch (shaderProperty.Type)
						{
							case VariableType.@float:
							case VariableType.fixed_function_float:
							case VariableType.fixed_function_enum:
								imp = new Imp_ConstantValue(shaderProperty)
								{
									FloatValue = GetAssociatedDataFloat(associatedData, "default", 0)
								};
								break;

							case VariableType.float2:
							case VariableType.float3:
							case VariableType.float4:
							{
								var values = GetAssociatedDataString(associatedData, "default", "(0, 0, 0, 0)").TrimStart('(').TrimEnd(')');
								var defaultValueSplit = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
								var defaultValue = Vector4.zero;
								defaultValue.x = defaultValueSplit.Length >= 1 ? float.Parse(defaultValueSplit[0], CultureInfo.InvariantCulture) : 0f;
								defaultValue.y = defaultValueSplit.Length >= 2 ? float.Parse(defaultValueSplit[1], CultureInfo.InvariantCulture) : 0f;
								defaultValue.z = defaultValueSplit.Length >= 3 ? float.Parse(defaultValueSplit[2], CultureInfo.InvariantCulture) : 0f;
								defaultValue.w = defaultValueSplit.Length >= 4 ? float.Parse(defaultValueSplit[3], CultureInfo.InvariantCulture) : 0f;
								imp = new Imp_ConstantValue(shaderProperty)
								{
									Float2Value = defaultValue,
									Float3Value = defaultValue,
									Float4Value = defaultValue
								};
							}
							break;

							case VariableType.color:
							case VariableType.color_rgba:
							{
								var values = GetAssociatedDataString(associatedData, "default", "(0, 0, 0, 0)").TrimStart('(').TrimEnd(')');
								var defaultValueSplit = values.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
								var defaultValue = Color.white;
								defaultValue.r = defaultValueSplit.Length >= 1 ? float.Parse(defaultValueSplit[0], CultureInfo.InvariantCulture) : 0f;
								defaultValue.g = defaultValueSplit.Length >= 2 ? float.Parse(defaultValueSplit[1], CultureInfo.InvariantCulture) : 0f;
								defaultValue.b = defaultValueSplit.Length >= 3 ? float.Parse(defaultValueSplit[2], CultureInfo.InvariantCulture) : 0f;
								defaultValue.a = defaultValueSplit.Length >= 4 ? float.Parse(defaultValueSplit[3], CultureInfo.InvariantCulture) : 1f;
								imp = new Imp_ConstantValue(shaderProperty)
								{
									ColorValue = defaultValue
								};
							}

							break;
						}
					}
					break;

					case "constant_float":
					{
						imp = new Imp_ConstantFloat(shaderProperty)
						{
							FloatValue = GetAssociatedDataFloat(associatedData, "default", 0)
						};
					}
					break;

					case "enum":
					{
						if (shaderProperty.Type != VariableType.fixed_function_enum)
						{
							Debug.LogError(ShaderGenerator2.ErrorMsg("Enum Implementation can only be used with Fixed Function Enum types."));
							break;
						}

						imp = new Imp_Enum(shaderProperty)
						{
							EnumType = GetAssociatedDataString(associatedData, "enum_type", null)
						};
						((Imp_Enum)imp).SetEnumType();

						int defaultValueInt = GetAssociatedDataInt(associatedData, "default", -1);
						if (defaultValueInt >= 0)
						{
							((Imp_Enum)imp).EnumValue = defaultValueInt;
						}
						else
						{
							string defaultValue = GetAssociatedDataString(associatedData, "default", null);
							if (!string.IsNullOrEmpty(defaultValue))
							{
								((Imp_Enum)imp).Parse(defaultValue.Trim('"'));
							}
						}

						break;
					}

					case "hook":
					{
						imp = new Imp_Hook(shaderProperty);
						shaderProperty.isHook = true;
						shaderProperty.deferredSampling = true;
						// shaderProperty.cantReferenceOtherProperties = true;
						shaderProperty.preventReference = "(hook)";
						shaderProperty.toggleFeatures = GetAssociatedDataString(associatedData, "toggles", null);
						break;
					}

					default:
						Debug.LogError(ShaderGenerator2.ErrorMsg("Unrecognized default property type: '" + impType + "'"));
						break;
				}

				if (imp != null)
				{
					// - common properties to all types
					imp.IsLocked |= GetAssociatedDataBool(associatedData, "locked", false);
					imp.Label = GetAssociatedDataString(associatedData, "label", shaderProperty.Name);

					// - specific to some implementations
					var imp_mp_texture = imp as Imp_MaterialProperty_Texture;
					if (imp_mp_texture != null)
					{
						if (imp_mp_texture.IsUvLocked)
						{
							// UVs are calculated in the shader, meaning that the property should be sampled when it is used rather than at the beginning of the vert or frag function
							shaderProperty.deferredSampling = true;
							shaderProperty.preventReference = "(sampled elsewhere in code)";
						}
						
						if (!string.IsNullOrEmpty(imp_mp_texture.TilingOffsetVariable))
						{
							imp_mp_texture.TilingOffsetVariableLabel = imp_mp_texture.TilingOffsetVariable;
						}
					}

					var imp_mp = imp as Imp_MaterialProperty;
					if (imp_mp != null)
					{
						// get specific variable name for material properties
						string propertyName = GetAssociatedDataString(associatedData, "variable", null);
						if (propertyName != null)
						{
							(imp as Imp_MaterialProperty).PropertyName = propertyName;
						}
						
						(imp as Imp_MaterialProperty).PropertyNameLocked = GetAssociatedDataBool(associatedData, "variable_locked", false);
					}
				}

				return imp;
			}

			//Get associated data with error/empty checks and default values
			static string GetAssociatedDataString(Dictionary<string, string> ad, string key, string defaultValue = "DefaultValue")
			{
				var str = defaultValue;
				if(ad.ContainsKey(key))
				{
					str = ad[key];

					//remove quotes
					if (str.StartsWith("\""))
					{
						str = str.Substring(1, str.Length - 2);
					}
				}
				return str;
			}
			static int GetAssociatedDataInt(Dictionary<string, string> ad, string key, int defaultValue = 0)
			{
				var ret = defaultValue;
				if (ad.ContainsKey(key))
				{
					if (!int.TryParse(ad[key], out ret))
					{
						return defaultValue;
					}
				}
				return ret;
			}
			static float GetAssociatedDataFloat(Dictionary<string, string> ad, string key, float defaultValue = 0.0f)
			{
				var ret = defaultValue;
				if (ad.ContainsKey(key))
				{
					if (!float.TryParse(ad[key], NumberStyles.Float, CultureInfo.InvariantCulture, out ret))
					{
						return defaultValue;
					}
				}
				return ret;
			}
			static bool GetAssociatedDataBool(Dictionary<string, string> ad, string key, bool defaultValue = false)
			{
				var ret = defaultValue;
				if (ad.ContainsKey(key))
				{
					if (!bool.TryParse(ad[key], out ret))
					{
						return defaultValue;
					}
				}
				return ret;
			}

			//Get arguments values
			static string TryGetArgument(string key, string arguments)
			{
				if (string.IsNullOrEmpty(arguments))
				{
					return null;
				}

				var args = arguments.Split(',');
				var heading = key + ":";
				foreach (var arg in args)
				{
					if (arg.StartsWith(heading))
					{
						return arg.Substring(arg.IndexOf(':')+1);
					}
				}

				return null;
			}

			static string AddArgument(string key, string value, string arguments)
			{
				return string.Format("{0}{1}:{2}",
					string.IsNullOrEmpty(arguments) ? "" : arguments + ",",
					key,
					value);
			}
		}
	}
}