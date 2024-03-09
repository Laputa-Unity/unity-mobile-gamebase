// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

using UnityEngine;

// Script that will update the custom time value for relevant water materials (using "Custom Time" from the Shader Generator)

// This allows:
// - getting the world height position of the wave with the TCP2_GetPosOnWater script
// - syncing to Unity's Time.timeScale value

namespace ToonyColorsPro
{
	namespace Runtime
	{
		public class TCP2_ShaderUpdateUnityTime : MonoBehaviour
		{
			static readonly int UnityTime = Shader.PropertyToID("unityTime");
			static readonly int CustomTime = Shader.PropertyToID("_CustomTime");

			void LateUpdate()
			{
				Shader.SetGlobalFloat(UnityTime, Time.time);
				Shader.SetGlobalVector(CustomTime, new Vector4(Time.time / 20f, Time.time, Time.time * 2, Time.time * 3));
			}
		}
	}
}