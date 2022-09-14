// Source: https://github.com/keijiro/GaussianBlur
// (c) 2017 Keijiro Takahashi - MIT License

// (c) 2020 Modified by Jean Moreno for "Toony Colors Pro 2"

Shader "Hidden/TCP2 Gaussian Blur Filter"
{
	Properties
	{
		_MainTex("-", 2D) = "white" {}
		_SamplingDistance ("Sampling Distance", Float) = 1
		
		_UseReflectionDepth ("Use Reflection Depth", Float) = 0
		_ReflectionDepthTex ("Reflection Depth", 2D) = "white" {}
		_ReflectivePlaneY ("Reflective Plane Y", Float) = 0
		_ReflectionDepthRange ("Reflection Depth Range", Float) = 2
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	sampler2D _ReflectionDepthTex;
	float _ReflectivePlaneY;
	float _ReflectionDepthRange;
	half _UseReflectionDepth;

	float _SamplingDistance;

	// 9-tap Gaussian filter with linear sampling
	// http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/
	half4 gaussian_filter(float2 uv, float2 stride)
	{
		half4 s = tex2D(_MainTex, uv) * 0.227027027;

		// Modern GPUs should nicely handle uniform-based branch, and older GPUs (> 10 yrs old) are unlikely to use planar reflections efficiently anyway
		half depth = 1;
		if (_UseReflectionDepth > 0)
		{
			depth = saturate((tex2D(_ReflectionDepthTex, uv).r - _ReflectivePlaneY) / _ReflectionDepthRange);
		}
		
		float2 d1 = stride * 1.3846153846 * depth;
		s += tex2D(_MainTex, uv + d1) * 0.3162162162;
		s += tex2D(_MainTex, uv - d1) * 0.3162162162;

		float2 d2 = stride * 3.2307692308 * depth;
		s += tex2D(_MainTex, uv + d2) * 0.0702702703;
		s += tex2D(_MainTex, uv - d2) * 0.0702702703;

		return s;
	}

	// Quarter downsampler
	half4 frag_quarter(v2f_img i) : SV_Target
	{
		float4 d = _MainTex_TexelSize.xyxy * float4(1, 1, -1, -1);

		if (_UseReflectionDepth > 0)
		{
			d *= saturate((tex2D(_ReflectionDepthTex, i.uv).r - _ReflectivePlaneY) / _ReflectionDepthRange);
		}
		
		half4 s;
		s  = tex2D(_MainTex, i.uv + d.xy);
		s += tex2D(_MainTex, i.uv + d.xw);
		s += tex2D(_MainTex, i.uv + d.zy);
		s += tex2D(_MainTex, i.uv + d.zw);
		return s * 0.25;
	}

	// Separable Gaussian filters
	half4 frag_blur_h(v2f_img i) : SV_Target
	{
		return gaussian_filter(i.uv, float2(_MainTex_TexelSize.x * _SamplingDistance, 0)); 
	}

	half4 frag_blur_v(v2f_img i) : SV_Target
	{
		return gaussian_filter(i.uv, float2(0, _MainTex_TexelSize.y * _SamplingDistance));
	}

	ENDCG

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_quarter
			ENDCG
		}
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_blur_h
			#pragma target 3.0
			ENDCG
		}
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_blur_v
			#pragma target 3.0
			ENDCG
		}
	}
}
