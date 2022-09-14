Shader "Octave3D/XZGrid"
{
	Properties
	{
		_LineColor("Line Color", Color) = (1,1,1,1)
		_PlaneColor("Plane Color", Color) = (0,0,0,0)
		_CellSizeX("Cell Size X", float) = 1.0
		_CellSizeZ("Cell Size Z", float) = 1.0
		_CamFarPlaneDist ("Camera far plane", float) = 1000			
		_CamWorldPos ("Camera position", Vector) = (0,0,0,0)		
		_CellOffset ("Cell offset", Vector) = (0,0,0,0)
	}

	CGINCLUDE
	float CalculateCamAlphaScale(float3 viewPos, float camFarPlaneDist, float3 camWorldPos)
	{
		float farPlaneDist = camFarPlaneDist;
		farPlaneDist *= (0.15f * (1000.0f / camFarPlaneDist));
		farPlaneDist *= max(1.0f, abs(camWorldPos.y) / 10.0f);
		float distFromCamPos = abs(viewPos.z);
		return saturate(1.0f - distFromCamPos / farPlaneDist);
	}
	ENDCG

	Subshader
	{	
		Tags {"Queue"="Transparent"}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragment frag
			#pragma vertex vert

			float4 _PlaneColor;
			float _CamFarPlaneDist;
			float3 _CamWorldPos;
			float3 _CellOffset;

			struct vInput
			{
				float4 vertexPos : POSITION;
			};

			struct vOutput
			{
				float3 viewPos : TEXCOORD1;
				float4 clipPos: SV_POSITION;
			};

			vOutput vert(vInput input)
			{
				vOutput output;

				// Store matrix and then multipy to avoid syntax upgrade. Unfortunately the syntax
				// is updated automatically even if the Unity version is tested with predefined macros.
				float4x4 mvp = UNITY_MATRIX_MVP;
				output.clipPos = mul(mvp, input.vertexPos);
				output.viewPos = mul(UNITY_MATRIX_MV, input.vertexPos);

				return output;
			}

			float4 frag(vOutput input) : COLOR
			{
				float alphaScale = CalculateCamAlphaScale(input.viewPos, _CamFarPlaneDist, _CamWorldPos);
				return float4(_PlaneColor.r, _PlaneColor.g, _PlaneColor.b, _PlaneColor.a * alphaScale);
			}
			ENDCG
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragment frag
			#pragma vertex vert

			float4 _LineColor;
			float4 _PlaneColor;
			float _CellSizeX;
			float _CellSizeZ;
			float _CamFarPlaneDist;
			float3 _CamWorldPos;
			float4x4 _InvRotMatrix;
			float4x4 _PlaneTransformMtx;
			float3 _CellOffset;

			struct vInput
			{
				float4 vertexPos : POSITION;
			};

			struct vOutput
			{
				float3 worldPos : TEXCOORD0;
				float3 viewPos : TEXCOORD1;
				float4 clipPos: SV_POSITION;
			};

			vOutput vert(vInput input)
			{
				vOutput output;

				// Store matrix and then multipy to avoid syntax upgrade. Unfortunately the syntax
				// is updated automatically even if the Unity version is tested with predefined macros.
				float4x4 mvp = UNITY_MATRIX_MVP;
				output.clipPos = mul(mvp, input.vertexPos);
				output.worldPos = mul(_PlaneTransformMtx, input.vertexPos);
				output.viewPos = mul(UNITY_MATRIX_MV, input.vertexPos);

				return output;
			}

			float4 frag(vOutput input) : COLOR
			{
				float4 worldPos = float4(input.worldPos.x - _CellOffset.x, input.worldPos.y - _CellOffset.y, input.worldPos.z - _CellOffset.z, 0.0f);	// - is to to the right and + is to the left :)
				float4 modelPos = mul(_InvRotMatrix, worldPos);

				float2 xzCoords = modelPos.xz * float2(1.0f / _CellSizeX, 1.0f / _CellSizeZ);
				 
				float2 grid = abs(frac(xzCoords - 0.5) - 0.5) / fwidth(xzCoords);	
				float a = min(grid.x, grid.y);

				float4 lineColor = _LineColor;
				return float4(lineColor.r, lineColor.g, lineColor.b, CalculateCamAlphaScale(input.viewPos, _CamFarPlaneDist, _CamWorldPos) * lineColor.a * (1.0 - min(a, 1.0)));
			}
			ENDCG
		}
	}
}