Shader "Custom/Outline"
{
	Properties
	{
		_OutlineColor("Outline Color",Color) = (0,1,0,1)
		_OutlineWidth("Outline Width",Range(0.001,0.01)) = 0.001
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Name "OUTLINEPASS"

			ZWrite off

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			float4 _OutlineColor;
			float _OutlineWidth;

			v2f vert(appdata IN)
			{
				v2f o;
				float camDist = distance(UnityObjectToWorldDir(IN.vertex), _WorldSpaceCameraPos);
				IN.vertex.xyz += normalize(IN.normal) * camDist * _OutlineWidth;
				o.pos = UnityObjectToClipPos(IN.vertex);
				return o;
			}

			float4 frag(v2f IN) : SV_TARGET
			{
				return _OutlineColor;
			}

			ENDCG
		}
    
    Pass
    {
      //Your own shader pass
    }
}}