// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Toony Colors Pro 2/Examples/Material Layers/Snow (Y Position Based)"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_Color ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_MainTex ("Albedo", 2D) = "white" {}
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Normal Mapping)]
		[NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Scale", Float) = 1
		[TCP2Separator]
		
		[TCP2Separator]
		[TCP2HeaderHelp(MATERIAL LAYERS)]

		[TCP2Separator]
		[TCP2Header(Snow)]
		_PositionThreshold_snow ("Position Threshold", Float) = 1
		 _PositionRange_snow ("Position Range", Float) = 1
		_contrast_snow ("Contrast", Range(0,1)) = 0.5
		_NoiseTexture_snow ("Noise Texture", 2D) = "gray" {}
		 _NoiseStrength_snow ("Noise Strength", Range(0,1)) = 0.1
		_BumpMap_snow ("Normal Map", 2D) = "bump" {}
		_BumpScale_snow ("Scale", Float) = 1
		_Albedo_snow ("Albedo", Color) = (1,1,1,1)
		_RampSmoothing_snow ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2ColorNoAlpha] _SColor_snow ("Shadow Color", Color) = (0.2,0.2,0.2,1)

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		
		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Shader Properties
		sampler2D _BumpMap;
		sampler2D _BumpMap_snow;
		
		sampler2D _MainTex;
		
		sampler2D _NoiseTexture_snow;
		
		// Shader Properties
		
		float4 _BumpMap_snow_ST;
		float _BumpScale;
		float _BumpScale_snow;
		float4 _MainTex_ST;
		fixed4 _Albedo_snow;
		fixed4 _Color;
		float _RampThreshold;
		float _RampSmoothing;
		float _RampSmoothing_snow;
		fixed4 _HColor;
		fixed4 _SColor;
		fixed4 _SColor_snow;
		float _PositionThreshold_snow;
		float _PositionRange_snow;
		float _contrast_snow;
		float4 _NoiseTexture_snow_ST;
		float _NoiseStrength_snow;
		
		ENDCG

		// Main Surface Shader

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha nolightmap nofog nolppv
		#pragma target 3.0

		//================================================================
		// STRUCTS

		//Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord0 : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			half4 tangent : TANGENT;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			half3 tangent;
			float3 objPos;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0 = v.texcoord0.xy;

			output.objPos = v.vertex.xyz;

			output.tangent = mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0)).xyz;

		}

		//================================================================

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;
			float3 normalTS;

			Input input;
			
			// Shader Properties
			float __rampThreshold;
			float __rampSmoothing;
			float3 __highlightColor;
			float3 __shadowColor;
			float __ambientIntensity;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{

			// Sampled in Custom Code
			float4 imp_100 = _NoiseStrength_snow;
			// Shader Properties Sampling
			float4 __normalMap = ( tex2D(_BumpMap, input.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw).rgba );
			float4 __normalMap_snow = ( tex2D(_BumpMap_snow, input.texcoord0.xy * _BumpMap_snow_ST.xy + _BumpMap_snow_ST.zw).rgba );
			float __bumpScale = ( _BumpScale );
			float __bumpScale_snow = ( _BumpScale_snow );
			float4 __albedo = ( tex2D(_MainTex, input.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw).rgba );
			float4 __albedo_snow = ( _Albedo_snow.rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampSmoothing = ( _RampSmoothing );
			float __rampSmoothing_snow = ( _RampSmoothing_snow );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			float3 __shadowColor_snow = ( _SColor_snow.rgb );
			output.__ambientIntensity = ( 1.0 );
			float __layer_snow = saturate(  ( input.objPos.y * _PositionRange_snow ) + _PositionThreshold_snow );
			float __contrast_snow = ( _contrast_snow );
			float __noise_snow = (  saturate( tex2D(_NoiseTexture_snow, input.texcoord0.xy * _NoiseTexture_snow_ST.xy + _NoiseTexture_snow_ST.zw).r * imp_100 ) - imp_100 / 2.0 );

			// Material Layers Blending
			 __normalMap = lerp(__normalMap, __normalMap_snow, saturate(((__layer_snow + __noise_snow) + (__contrast_snow * 0.5 - 0.5)) / __contrast_snow));
			 __bumpScale = lerp(__bumpScale, __bumpScale_snow, saturate(((__layer_snow + __noise_snow) + (__contrast_snow * 0.5 - 0.5)) / __contrast_snow));
			 __albedo = lerp(__albedo, __albedo_snow, saturate(((__layer_snow + __noise_snow) + (__contrast_snow * 0.5 - 0.5)) / __contrast_snow));
			 output.__rampSmoothing = lerp(output.__rampSmoothing, __rampSmoothing_snow, saturate(((__layer_snow + __noise_snow) + (__contrast_snow * 0.5 - 0.5)) / __contrast_snow));
			 output.__shadowColor = lerp(output.__shadowColor, __shadowColor_snow, saturate(((__layer_snow + __noise_snow) + (__contrast_snow * 0.5 - 0.5)) / __contrast_snow));

			output.input = input;

			half4 normalMap = half4(0,0,0,0);
			normalMap = __normalMap;
			output.Normal = UnpackScaleNormal(normalMap, __bumpScale);
			output.normalTS = output.Normal;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			output.Albedo *= __mainColor.rgb;

		}

		//================================================================
		// LIGHTING FUNCTION

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, UnityGI gi)
		{
			half3 lightDir = gi.light.dir;
			#if defined(UNITY_PASS_FORWARDBASE)
				half3 lightColor = _LightColor0.rgb;
				half atten = surface.atten;
			#else
				//extract attenuation from point/spot lights
				half3 lightColor = _LightColor0.rgb;
				half atten = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b)) / max(_LightColor0.r, max(_LightColor0.g, _LightColor0.b));
			#endif

			half3 normal = normalize(surface.Normal);
			half ndl = dot(normal, lightDir);
			half3 ramp;
			
			#define		RAMP_THRESHOLD	surface.__rampThreshold
			#define		RAMP_SMOOTH		surface.__rampSmoothing
			ndl = saturate(ndl);
			ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, ndl);
			half3 rampGrayscale = ramp;

			//Apply attenuation (shadowmaps & point/spot lights attenuation)
			ramp *= atten;

			//Highlight/Shadow Colors
			#if !defined(UNITY_PASS_FORWARDBASE)
				ramp = lerp(half3(0,0,0), surface.__highlightColor, ramp);
			#else
				ramp = lerp(surface.__shadowColor, surface.__highlightColor, ramp);
			#endif

			//Output color
			half4 color;
			color.rgb = surface.Albedo * lightColor.rgb * ramp;
			color.a = surface.Alpha;

			// Apply indirect lighting (ambient)
			half occlusion = 1;
			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				half3 ambient = gi.indirect.diffuse;
				ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;

				color.rgb += ambient;
			#endif

			return color;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
		{
			half3 normal = surface.Normal;

			//GI without reflection probes
			gi = UnityGlobalIllumination(data, 1.0, normal); // occlusion is applied in the lighting function, if necessary

			surface.atten = data.atten; // transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb; // remove attenuation

		}

		ENDCG

	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2018.4.11f1";ver:"2.6.4";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","BUMP","BUMP_SCALE"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_MainTex";md:"";custom:False;refs:"";guid:"c5c51c70-cbf2-433d-a60b-68ad34702603";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:0)];layers:list["1f64b1"];unlocked:list["1f64b1"];clones:dict[1f64b1=sp(name:"Albedo_1f64b1";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:False;cc:4;chan:"RGBA";prop:"_Albedo_1f64b1";md:"";custom:False;refs:"";guid:"1a5d445c-f75c-4e7c-a739-3f8b4353f1c9";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:True)];isClone:False),,,,,sp(name:"Ramp Smoothing";imps:list[imp_mp_range(def:0.5;min:0.001;max:1;prop:"_RampSmoothing";md:"";custom:False;refs:"";guid:"8413f877-519a-4ebd-95c1-7061a38c4160";op:Multiply;lbl:"Smoothing";gpu_inst:False;locked:False;impl_index:0)];layers:list["1f64b1"];unlocked:list[];clones:dict[];isClone:False),,sp(name:"Shadow Color";imps:list[imp_mp_color(def:RGBA(0.2, 0.2, 0.2, 1);hdr:False;cc:3;chan:"RGB";prop:"_SColor";md:"";custom:False;refs:"";guid:"7d7a8d7d-38b4-4c3c-b81d-1cf098b230d7";op:Multiply;lbl:"Shadow Color";gpu_inst:False;locked:False;impl_index:0)];layers:list["1f64b1"];unlocked:list[];clones:dict[];isClone:False),sp(name:"Normal Map";imps:list[imp_mp_texture(uto:True;tov:"_MainTex_ST";tov_lbl:"_MainTex_ST";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"bump";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BumpMap";md:"";custom:False;refs:"";guid:"8d536404-7c92-48b7-943b-fa9991736ca1";op:Multiply;lbl:"Normal Map";gpu_inst:False;locked:False;impl_index:0)];layers:list["1f64b1"];unlocked:list["1f64b1"];clones:dict[1f64b1=sp(name:"Normal Map_1f64b1";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"bump";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BumpMap_1f64b1";md:"";custom:False;refs:"";guid:"00000000-0000-0000-0000-000000000000";op:Multiply;lbl:"Normal Map";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:True)];isClone:False),sp(name:"Bump Scale";imps:list[imp_mp_float(def:1;prop:"_BumpScale";md:"";custom:False;refs:"";guid:"2706f148-0266-4544-b546-1865322c1b58";op:Multiply;lbl:"Scale";gpu_inst:False;locked:False;impl_index:0)];layers:list["1f64b1"];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[ml(uid:"1f64b1";name:"Snow";src:sp(name:"layer_1f64b1";imps:list[imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"( {2}.y * {4} ) + {3}";guid:"1ffb586a-daaa-45ff-8a70-61a046655b73";op:Multiply;lbl:"layer_1f64b1";gpu_inst:False;locked:False;impl_index:-1),imp_localpos(cc:1;chan:"X";guid:"edaa4c64-e6a6-4b61-b0e5-4bbf2c7ebaca";op:Multiply;lbl:"layer_1f64b1";gpu_inst:False;locked:False;impl_index:-1),imp_mp_float(def:1;prop:"_PositionThreshold_1f64b1";md:"";custom:False;refs:"";guid:"2d541af6-f531-465e-9989-600c63b020e6";op:Multiply;lbl:"Position Threshold";gpu_inst:False;locked:False;impl_index:-1),imp_mp_float(def:1;prop:"_PositionRange_1f64b1";md:"";custom:False;refs:"";guid:"9abf602f-24bd-45da-a698-8d100edb3e63";op:Multiply;lbl:"Position Range";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False);use_contrast:True;ctrst:sp(name:"contrast_1f64b1";imps:list[imp_mp_range(def:0.5;min:0;max:1;prop:"_contrast_1f64b1";md:"";custom:False;refs:"";guid:"834c2e8f-50c9-45c4-913a-7705842d3535";op:Multiply;lbl:"Contrast";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False);use_noise:True;noise:sp(name:"noise_1f64b1";imps:list[imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"saturate( {2}.r * {3} ) - {3} / 2.0";guid:"e89689f2-04cb-4558-98cc-0e62a187da51";op:Multiply;lbl:"noise_1f64b1";gpu_inst:False;locked:False;impl_index:-1),imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"gray";locked_uv:False;uv:0;cc:1;chan:"R";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_NoiseTexture_1f64b1";md:"";custom:False;refs:"";guid:"c71df622-aa37-4383-8511-0207eee05e55";op:Multiply;lbl:"Noise Texture";gpu_inst:False;locked:False;impl_index:-1),imp_mp_range(def:0.1;min:0;max:1;prop:"_NoiseStrength_1f64b1";md:"";custom:False;refs:"";guid:"853af456-8954-4e6d-86bb-772349424796";op:Multiply;lbl:"Noise Strength";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False))]) */
/* TCP_HASH e8e09773e2906d6bf8ea677d0da1768d */
