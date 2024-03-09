// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ToonyColorsPro
{
	namespace Demo
	{
		public class TCP2_Demo_AnimateMaterial : MonoBehaviour
		{
			[Serializable]
			public class AnimatedProperty
			{
				public enum MaterialPropertyType
				{
					Float,
					Color,
					Vector4
				}

				public string Name = "_Color";
				public MaterialPropertyType Type = MaterialPropertyType.Float;
				public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				public float Duration = 1;
				[Space]
				public float FloatFrom = 0;
				public float FloatTo = 1;
				[Space]
				public Color ColorFrom = Color.black;
				public Color ColorTo = Color.white;
				[Space]
				public Vector4 VectorFrom = Vector4.zero;
				public Vector4 VectorTo = Vector4.one;

				int propertyId;

				public void Init()
				{
					this.propertyId = Shader.PropertyToID(this.Name);
				}

				public void Update(Material material)
				{
					float time = Curve.Evaluate((Time.time % Duration) / Duration);

					switch (Type)
					{
						case MaterialPropertyType.Float:
							material.SetFloat(propertyId, Mathf.Lerp(FloatFrom, FloatTo, time));
							break;
						case MaterialPropertyType.Color:
							material.SetColor(propertyId, Color.Lerp(ColorFrom, ColorTo, time));
							break;
						case MaterialPropertyType.Vector4:
							material.SetVector(propertyId, Vector4.Lerp(VectorFrom, VectorTo, time));
							break;
					}
				}
			}

			public Material material;
			public AnimatedProperty[] animatedProperties;

			void Awake()
			{
				if (animatedProperties != null)
				{
					foreach (var animatedProp in animatedProperties)
					{
						animatedProp.Init();
					}
				}
			}

			void Update()
			{
				if (animatedProperties != null)
				{
					foreach (var animatedProp in animatedProperties)
					{
						animatedProp.Update(material);
					}
				}
			}
		}
	}
}