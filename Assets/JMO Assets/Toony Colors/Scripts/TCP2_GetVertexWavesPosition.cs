// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

using UnityEngine;
#if UNITY_EDITOR
using ToonyColorsPro.Runtime;
using UnityEditor;
#endif

// Script to get the water height from a specific world position
// Useful to easily make objects float on water for example

namespace ToonyColorsPro
{
	namespace Runtime
	{
		public class TCP2_GetVertexWavesPosition : MonoBehaviour
		{
			public Transform WaterPlane;

			[Space]
			[Tooltip("Will make the object stick to the water plane")]
			public bool followWaterHeight = true;
			public float heightOffset = 0;
			[Space]
			[Tooltip("Will align the object to the wave normal based on its position")]
			public bool followWaterNormal;
			[Tooltip("Determine the object's up axis (when following wave normal)")]
			public Vector3 upAxis = new Vector3(0, 1, 0);
			[Tooltip("Rotation of the object once it's been affected by the water normal")]
			public Vector3 postRotation = new Vector3(0, 0, 0);

			[Header("Water Shader Configuration")]
			[Space]
			public int sineCount = 1;
			[Space]
			public float WavesSpeed = 2.0f;
			public float WavesHeight = 0.1f;
			public float WavesFrequency = 1.0f;
			[Space]
			public bool useCustomTime;

			public bool customSineValues;

			// Shader Generator 2 values:
			[HideInInspector] public Vector4 sinOffsets1 = new Vector4(1.0f, 2.2f, 0.6f, 1.3f);
			[HideInInspector] public Vector4 phaseOffsets1 = new Vector4(1.0f, 1.3f, 2.2f, 0.4f);

			[HideInInspector] public Vector4 sinOffsets2 = new Vector4(0.6f, 1.3f, 3.1f, 2.4f);
			[HideInInspector] public Vector4 phaseOffsets2 = new Vector4(2.2f, 0.4f, 3.3f, 2.9f);
			
			[HideInInspector] public Vector4 sinOffsets3 =	new Vector4(1.4f, 1.8f, 4.2f, 3.6f);
			[HideInInspector] public Vector4 phaseOffsets3 =	new Vector4(0.2f, 2.6f, 0.7f, 3.1f);
			[HideInInspector] public Vector4 sinOffsets4 =	new Vector4(1.1f, 2.8f, 1.7f, 4.3f);
			[HideInInspector] public Vector4 phaseOffsets4 = new Vector4(0.5f, 4.8f, 3.1f, 2.3f);
			
			static readonly int _Time = Shader.PropertyToID("_Time");
			static int LastFrameTimeSampling;
			static float ShaderTime = 0;

			void LateUpdate()
			{
				float time;
				if (useCustomTime)
				{
					time = Time.time;
				}
				else
				{
					// Avoid fetching the _Time value multiple times per frame
					if (LastFrameTimeSampling < Time.frameCount)
					{
						ShaderTime = Shader.GetGlobalVector(_Time).y;
						LastFrameTimeSampling = Time.frameCount;
					}

					time = ShaderTime;
				}
				
				if (followWaterHeight)
				{
					Vector3 worldPosition = GetPositionOnWater_SG2(time, transform.position);
					transform.position = worldPosition;
				}

				if (followWaterNormal)
				{
					transform.rotation = Quaternion.FromToRotation(upAxis, GetNormalOnWater_SG2(time, transform.position));
					transform.Rotate(postRotation, Space.Self);
				}
			}

			Vector4 CalculateSinePosition(float v1, float v2, Vector4 sinOffsets, Vector4 phaseOffsets, ref float phase)
			{
				return new Vector4(
					Mathf.Sin((v1 * sinOffsets.x) + (phase * phaseOffsets.x)),
					Mathf.Sin((v1 * sinOffsets.y) + (phase * phaseOffsets.y)),
					Mathf.Sin((v2 * sinOffsets.z) + (phase * phaseOffsets.z)),
					Mathf.Sin((v2 * sinOffsets.w) + (phase * phaseOffsets.w))
				);
			}
			
			Vector4 CalculateSineNormal(float v1, float v2, Vector4 sinOffsets, Vector4 phaseOffsets, ref float phase)
			{
				return new Vector4(
					Mathf.Cos((v1 * sinOffsets.x) + (phase * phaseOffsets.x)) * sinOffsets.x,
					Mathf.Cos((v1 * sinOffsets.y) + (phase * phaseOffsets.y)) * sinOffsets.y,
					Mathf.Cos((v2 * sinOffsets.z) + (phase * phaseOffsets.z)) * sinOffsets.z,
					Mathf.Cos((v2 * sinOffsets.w) + (phase * phaseOffsets.w)) * sinOffsets.w
				);
			}
			
			//Returns a world space position on a water plane, based on its material
			public Vector3 GetPositionOnWater_SG2(float time, Vector3 worldPosition)
			{
				float phase = time * WavesSpeed;
				float x = worldPosition.x * WavesFrequency;
				float z = worldPosition.z * WavesFrequency;

				float height = WavesHeight * WaterPlane.transform.lossyScale.y;

				float waveFactorX = 0;
				float waveFactorZ = 0;

				switch (sineCount)
				{
					case 2:
					{
						Vector4 sinWaves = CalculateSinePosition(x, z, sinOffsets1, phaseOffsets1, ref phase);
						waveFactorX = (sinWaves.x + sinWaves.y) * height / 2.0f;
						waveFactorZ = (sinWaves.z + sinWaves.w) * height / 2.0f;
						break;
					}

					case 4:
					{
						Vector4 sinWavesX = CalculateSinePosition(x, x, sinOffsets1, phaseOffsets1, ref phase);
						Vector4 sinWavesZ = CalculateSinePosition(z, z, sinOffsets2, phaseOffsets2, ref phase);
						waveFactorX = (sinWavesX.x + sinWavesX.y + sinWavesX.z + sinWavesX.w) * height / 4.0f;
						waveFactorZ = (sinWavesZ.x + sinWavesZ.y + sinWavesZ.z + sinWavesZ.w) * height / 4.0f;
						break;
					}

					case 8:
					{
						Vector4 sinWavesX = CalculateSinePosition(x, x, sinOffsets1, phaseOffsets1, ref phase);
						Vector4 sinWavesZ = CalculateSinePosition(z, z, sinOffsets2, phaseOffsets2, ref phase);
						Vector4 sinWavesX2 = CalculateSinePosition(x, x, sinOffsets3, phaseOffsets3, ref phase);
						Vector4 sinWavesZ2 = CalculateSinePosition(z, z, sinOffsets4, phaseOffsets4, ref phase);
						waveFactorX = (sinWavesX.x + sinWavesX.y + sinWavesX.z + sinWavesX.w + sinWavesX2.x + sinWavesX2.y + sinWavesX2.z + sinWavesX2.w) * height / 8.0f;
						waveFactorZ = (sinWavesZ.x + sinWavesZ.y + sinWavesZ.z + sinWavesZ.w + sinWavesZ2.x + sinWavesZ2.y + sinWavesZ2.z + sinWavesZ2.w) * height / 8.0f;
						break;
					}

					case 1:
					{
						waveFactorX = Mathf.Sin(x + phase) * height;
						waveFactorZ = Mathf.Sin(z + phase) * height;
						break;
					}
				}

				worldPosition.y = (waveFactorX + waveFactorZ) + WaterPlane.transform.position.y + heightOffset;
				return worldPosition;
			}

			public Vector3 GetNormalOnWater_SG2(float time, Vector3 worldPosition)
			{
				float phase = time * WavesSpeed;
				float x = worldPosition.x * WavesFrequency;
				float z = worldPosition.z * WavesFrequency;

				float height = WavesHeight * WaterPlane.transform.lossyScale.y;

				float waveNormalX = 0;
				float waveNormalZ = 0;

				switch (sineCount)
				{
					case 2:
					{
						Vector4 sinWaves = CalculateSineNormal(x, z, sinOffsets1, phaseOffsets1, ref phase);
						waveNormalX = (sinWaves.x + sinWaves.y) * -height / 2.0f;
						waveNormalZ = (sinWaves.z + sinWaves.w) * -height / 2.0f;
						break;
					}

					case 4:
					{
						Vector4 sinWavesX = CalculateSineNormal(x, x, sinOffsets1, phaseOffsets1, ref phase);
						Vector4 sinWavesZ = CalculateSineNormal(z, z, sinOffsets2, phaseOffsets2, ref phase);
						waveNormalX = (sinWavesX.x + sinWavesX.y + sinWavesX.z + sinWavesX.w) * -height / 4.0f;
						waveNormalZ = (sinWavesZ.x + sinWavesZ.y + sinWavesZ.z + sinWavesZ.w) * -height / 4.0f;
						break;
					}

					case 8:
					{
						Vector4 sinWavesX = CalculateSineNormal(x, x, sinOffsets1, phaseOffsets1, ref phase);
						Vector4 sinWavesZ = CalculateSineNormal(z, z, sinOffsets2, phaseOffsets2, ref phase);
						Vector4 sinWavesX2 = CalculateSineNormal(x, x, sinOffsets3, phaseOffsets3, ref phase);
						Vector4 sinWavesZ2 = CalculateSineNormal(z, z, sinOffsets4, phaseOffsets4, ref phase);
						waveNormalX = (sinWavesX.x + sinWavesX.y + sinWavesX.z + sinWavesX.w + sinWavesX2.x + sinWavesX2.y + sinWavesX2.z + sinWavesX2.w) * -height / 8.0f;
						waveNormalZ = (sinWavesZ.x + sinWavesZ.y + sinWavesZ.z + sinWavesZ.w + sinWavesZ2.x + sinWavesZ2.y + sinWavesZ2.z + sinWavesZ2.w) * -height / 8.0f;

						break;
					}

					case 1:
					{
						waveNormalX = Mathf.Cos(x + phase) * -height;
						waveNormalZ = Mathf.Cos(z + phase) * -height;
						break;
					}
				}

				return new Vector3(waveNormalX, 1, waveNormalZ).normalized;
			}
		}
	}

	namespace Inspector
	{
#if UNITY_EDITOR
		[CustomEditor(typeof(TCP2_GetVertexWavesPosition)), CanEditMultipleObjects]
		class TCP2_GetVertexWavesPosition_Editor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				var customSineValues = this.serializedObject.FindProperty("customSineValues");
				if (customSineValues.boolValue)
				{
					var sinOffsets1 = this.serializedObject.FindProperty("sinOffsets1");
					var phaseOffsets1 = this.serializedObject.FindProperty("phaseOffsets1");
					var sinOffsets2 = this.serializedObject.FindProperty("sinOffsets2");
					var phaseOffsets2 = this.serializedObject.FindProperty("phaseOffsets2");
					var sinOffsets3 = this.serializedObject.FindProperty("sinOffsets3");
					var phaseOffsets3 = this.serializedObject.FindProperty("phaseOffsets3");
					var sinOffsets4 = this.serializedObject.FindProperty("sinOffsets4");
					var phaseOffsets4 = this.serializedObject.FindProperty("phaseOffsets4");

					EditorGUILayout.PropertyField(sinOffsets1);
					EditorGUILayout.PropertyField(phaseOffsets1);
					EditorGUILayout.PropertyField(sinOffsets2);
					EditorGUILayout.PropertyField(phaseOffsets2);
					EditorGUILayout.PropertyField(sinOffsets3);
					EditorGUILayout.PropertyField(phaseOffsets3);
					EditorGUILayout.PropertyField(sinOffsets4);
					EditorGUILayout.PropertyField(phaseOffsets4);
					
					EditorGUILayout.HelpBox("Change the phase and offset values if you have changed the default ones in the shader using the Shader Generator 2's Shader Properties system.", MessageType.Info);
				}

				GUILayout.Space(8);
				if (GUILayout.Button("Try to fetch values\nfrom water material", GUILayout.Height(30)))
				{
					foreach (var t in targets)
					{
						var script = t as TCP2_GetVertexWavesPosition;
						if (script != null && script.WaterPlane != null)
						{
							var renderer = script.WaterPlane.GetComponent<Renderer>();
							if (renderer != null && renderer.sharedMaterial != null)
							{
								var mat = renderer.sharedMaterial;

								if (mat.HasProperty("_WavesSpeed"))
								{
									serializedObject.FindProperty("WavesSpeed").floatValue = mat.GetFloat("_WavesSpeed");
								}

								if (mat.HasProperty("_WavesHeight"))
								{
									serializedObject.FindProperty("WavesHeight").floatValue = mat.GetFloat("_WavesHeight");
								}

								if (mat.HasProperty("_WavesFrequency"))
								{
									serializedObject.FindProperty("WavesFrequency").floatValue = mat.GetFloat("_WavesFrequency");
								}

								var sineCountProperty = serializedObject.FindProperty("sineCount");
								if (mat.shader != null)
								{
									int count = ShaderUtil.GetPropertyCount(mat.shader);
									sineCountProperty.intValue = 1;
									for (int i = 0; i < count; i++)
									{
										string name = ShaderUtil.GetPropertyName(mat.shader, i);
										if (name == "_SineCount8")
										{
											sineCountProperty.intValue = 8;
											break;
										}

										if (name == "_SineCount4")
										{
											sineCountProperty.intValue = 4;
											break;
										}

										if (name == "_SineCount2")
										{
											sineCountProperty.intValue = 2;
											break;
										}
									}
								}

								serializedObject.ApplyModifiedProperties();
							}
						}
					}
				}
			}
			
			bool FetchMaterialValue(Material material, string property, out float value)
			{
				if (material.HasProperty(property))
				{
					value = material.GetFloat(property);
					return true;
				}

				value = -1f;
				return false;
			}
		}
#endif
	}
}
