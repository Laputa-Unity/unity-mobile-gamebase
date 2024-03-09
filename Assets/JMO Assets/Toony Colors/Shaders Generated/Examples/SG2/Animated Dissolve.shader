// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Toony Colors Pro 2/Examples/SG2/Animated Dissolve"
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
		
		[TCP2HeaderHelp(Dissolve)]
		_DissolveMap ("Map", 2D) = "gray" {}
		[TCP2UVScrolling] _DissolveMap_SC ("Map UV Scrolling", Vector) = (1,1,0,0)
		[HDR] _DissolveGradientTexture ("Gradient Texture", Color) = (1,1,1,1)
		_DissolveGradientWidth ("Ramp Width", Range(0,1)) = 0.2
		[TCP2Separator]
		
		[TCP2Vector4Floats(Contrast X,Contrast Y,Contrast Z,Smoothing,1,16,1,16,1,16,0.05,10)] _TriplanarSamplingStrength ("Triplanar Sampling Parameters", Vector) = (8,8,8,0.5)
		// Custom Material Properties
		 _DissolveValue ("Dissolve Value", Range(0,1)) = 0.5

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="AlphaTest"
		}

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Custom Material Properties

		// Shader Properties
		sampler2D _MainTex;
		sampler2D _DissolveMap;
		
		// Custom Material Properties
		float _DissolveValue;

		// Shader Properties
		float4 _MainTex_ST;
		float4 _DissolveMap_ST;
		half4 _DissolveMap_SC;
		float _DissolveGradientWidth;
		half4 _DissolveGradientTexture;
		fixed4 _Color;
		float _RampThreshold;
		float _RampSmoothing;
		fixed4 _HColor;
		fixed4 _SColor;

		float4 _TriplanarSamplingStrength;

		float4 tex2D_triplanar(sampler2D samp, float4 tiling_offset, float3 worldPos, float3 worldNormal)
		{
			half4 sample_y = ( tex2D(samp, worldPos.xz * tiling_offset.xy + tiling_offset.zw).rgba );
			fixed4 sample_x = ( tex2D(samp, worldPos.zy * tiling_offset.xy + tiling_offset.zw).rgba );
			fixed4 sample_z = ( tex2D(samp, worldPos.xy * tiling_offset.xy + tiling_offset.zw).rgba );
			
			//blending
			half3 blendWeights = pow(abs(worldNormal), _TriplanarSamplingStrength.xyz / _TriplanarSamplingStrength.w);
			blendWeights = blendWeights / (blendWeights.x + abs(blendWeights.y) + blendWeights.z);
			half4 triplanar = sample_x * blendWeights.x + sample_y * blendWeights.y + sample_z * blendWeights.z;
			
			return triplanar;
		}
			
		ENDCG

		// Main Surface Shader

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha addshadow nolightmap nofog nolppv
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
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			half4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			float3 objPos;
			float3 objNormal;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			worldPos.xyz = ( worldPos.xyz + pow(_DissolveValue, 2) * v.vertex.y * float3(0,1,0) );
			v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
			output.objPos = v.vertex.xyz;
			output.objNormal = v.normal.xyz;

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
			// Shader Properties Sampling
			float4 __albedo = ( tex2D(_MainTex, input.texcoord0.xy).rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			float __dissolveMap = ( tex2D_triplanar(_DissolveMap, float4(_DissolveMap_ST.xy, _DissolveMap_ST.zw + frac(_Time.yy * _DissolveMap_SC.xy)), input.objPos, input.objNormal) );
			float __dissolveValue = ( _DissolveValue.x );
			float __dissolveGradientWidth = ( _DissolveGradientWidth );
			float __dissolveGradientStrength = ( 2.0 );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampSmoothing = ( _RampSmoothing );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__ambientIntensity = ( 1.0 );

			output.input = input;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			//Dissolve
			half dissolveMap = __dissolveMap;
			half dissolveValue = __dissolveValue;
			half gradientWidth = __dissolveGradientWidth;
			float dissValue = dissolveValue*(1+2*gradientWidth) - gradientWidth;
			float dissolveUV = smoothstep(dissolveMap - gradientWidth, dissolveMap + gradientWidth, dissValue);
			clip((1-dissolveUV) - 0.001);
			half4 dissolveColor = ( _DissolveGradientTexture.rgba );
			dissolveColor *= __dissolveGradientStrength * dissolveUV;
			output.Emission += dissolveColor.rgb;
			
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

/* TCP_DATA u config(unity:"2018.4.11f1";ver:"2.5.2";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","DISSOLVE","DISSOLVE_CLIP","DISSOLVE_GRADIENT"];flags:list["addshadow"];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[,,,,,,,,sp(name:"Dissolve Map";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:False;scr:True;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:True;def:"gray";locked_uv:False;uv:6;cc:1;chan:"R";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:True;uv_type:Triplanar;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_DissolveMap";md:"";custom:False;refs:"";guid:"1bce4e69-5aa9-4194-8383-1f8da882010d";op:Multiply;lbl:"Map";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Value";imps:list[imp_ct(lct:"_DissolveValue";cc:1;chan:"X";avchan:"X";guid:"60d36b91-8298-42c4-b1a6-116eb6e231f4";op:Multiply;lbl:"Value";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Gradient Texture";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_DissolveGradientTexture";md:"";custom:False;refs:"";guid:"9cb5f671-20e0-4192-bfb3-38a15cbe6b6e";op:Multiply;lbl:"Gradient Texture";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,sp(name:"Vertex Position World";imps:list[imp_hook(guid:"e8830166-30da-4b68-8ec9-0b3c84d446dd";op:Multiply;lbl:"worldPos.xyz";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"+ pow({3}, 2) * v.vertex.y * float3(0,1,0)";guid:"7685fcb2-f26c-40f4-bf85-aa7470960080";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_DissolveValue";cc:3;chan:"XXX";avchan:"X";guid:"6c6d296d-84e4-4ba5-acdd-2b1004ced2bb";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[ct(cimp:imp_mp_range(def:0.5;min:0;max:1;prop:"_DissolveValue";md:"";custom:True;refs:"Dissolve Value, Vertex Position (World Space)";guid:"d25d99c3-bceb-4a5b-a3d0-8d46322040aa";op:Multiply;lbl:"Dissolve Value";gpu_inst:False;locked:False;impl_index:-1);exp:False;uv_exp:False;imp_lbl:"Range")];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 529b80387a0843e948acc8bbd0f4be64 */
