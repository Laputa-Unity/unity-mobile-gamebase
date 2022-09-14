// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Toony Colors Pro 2/Examples/SG2/Stylized Water"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_MainTex ("Water Texture", 2D) = "white" {}
		[TCP2Vector4FloatsDrawer(Speed,Amplitude,Frequency,Offset)] _MainTex_SinAnimParams ("Water Texture UV Sine Distortion Parameters", Float) = (1, 0.05, 1, 0)
		 _WaterColor ("Water Color", Color) = (1,1,1,1)
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Rim Lighting)]
		[TCP2ColorNoAlpha] _RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.5)
		_RimMin ("Rim Min", Range(0,2)) = 0.5
		_RimMax ("Rim Max", Range(0,2)) = 1
		[TCP2Separator]
		
		[TCP2HeaderHelp(Vertex Waves Animation)]
		_WavesSpeed ("Speed", Float) = 2
		_WavesHeight ("Height", Float) = 0.1
		_WavesFrequency ("Frequency", Range(0,10)) = 1
		
		[TCP2HeaderHelp(Depth Based Effects)]
		[TCP2ColorNoAlpha] _DepthColor ("Depth Color", Color) = (0,0,1,1)
		[PowerSlider(5.0)] _DepthColorDistance ("Depth Color Distance", Range(0.01,3)) = 0.5
		_FoamSpread ("Foam Spread", Range(0,5)) = 2
		_FoamStrength ("Foam Strength", Range(0,1)) = 0.8
		_FoamColor ("Foam Color (RGB) Opacity (A)", Color) = (0.9,0.9,0.9,1)
		_FoamTex ("Foam Texture Custom", 2D) = "black" {}
		
		_CustomTime ("Custom Time", Vector) = (0.05, 1, 2, 3)

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

		// Custom time variable overriding the built-in one
		#define _Time _CustomTime

		float4 _CustomTime;
		
		// Shader Properties
		sampler2D _MainTex;
		sampler2D _FoamTex;
		
		// Shader Properties
		float _WavesFrequency;
		float _WavesHeight;
		float _WavesSpeed;
		float4 _MainTex_ST;
		half4 _MainTex_SinAnimParams;
		fixed4 _WaterColor;
		fixed4 _DepthColor;
		float _DepthColorDistance;
		float _FoamSpread;
		float _FoamStrength;
		fixed4 _FoamColor;
		float4 _FoamTex_ST;
		float _RampThreshold;
		float _RampSmoothing;
		fixed4 _HColor;
		fixed4 _SColor;
		float _RimMin;
		float _RimMax;
		fixed4 _RimColor;

		UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

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
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			half4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			half3 viewDir;
			float3 worldPos;
			half3 worldNormal; INTERNAL_DATA
			float4 screenPosition;
			float2 sinUvAnimVertexWorldPos;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			float3 worldPosUv = mul(unity_ObjectToWorld, v.vertex).xyz;

			// Used for texture UV sine animation (world space)
			float2 sinUvAnimVertexWorldPos = worldPosUv.xy + worldPosUv.yz;
			output.sinUvAnimVertexWorldPos = sinUvAnimVertexWorldPos;

			// Texture Coordinates
			output.texcoord0 = v.texcoord0.xy;
			// Shader Properties Sampling
			float __wavesFrequency = ( _WavesFrequency );
			float __wavesHeight = ( _WavesHeight );
			float __wavesSpeed = ( _WavesSpeed );

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			
			// Vertex water waves
			float _waveFrequency = __wavesFrequency;
			float _waveHeight = __wavesHeight;
			float3 _vertexWavePos = worldPos.xyz * _waveFrequency;
			float _phase = _Time.y * __wavesSpeed;
			float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
			float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
			v.vertex.y += (waveFactorX + waveFactorZ);
			float4 clipPos = UnityObjectToClipPos(v.vertex);

			//Screen Position
			float4 screenPos = ComputeScreenPos(clipPos);
			output.screenPosition = screenPos;
			COMPUTE_EYEDEPTH(output.screenPosition.z);

		}

		//================================================================

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			half3 Albedo;
			half3 Normal;
			half3 worldNormal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;
			half ndv;
			half ndvRaw;

			Input input;
			
			// Shader Properties
			float __rampThreshold;
			float __rampSmoothing;
			float3 __highlightColor;
			float3 __shadowColor;
			float __ambientIntensity;
			float __rimMin;
			float __rimMax;
			float3 __rimColor;
			float __rimStrength;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			float2 uvSinAnim__MainTex = (input.sinUvAnimVertexWorldPos * _MainTex_SinAnimParams.z) + (_Time.yy * _MainTex_SinAnimParams.x);
			// Shader Properties Sampling
			float __depthViewCorrectionBias = ( 2.0 );
			float4 __albedo = (  lerp(float4(1,1,1,1), _WaterColor.rgba, tex2D(_MainTex, input.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw + (((sin(0.9 * uvSinAnim__MainTex + _MainTex_SinAnimParams.w) + sin(1.33 * uvSinAnim__MainTex + 3.14 * _MainTex_SinAnimParams.w) + sin(2.4 * uvSinAnim__MainTex + 5.3 * _MainTex_SinAnimParams.w)) / 3) * _MainTex_SinAnimParams.y)).a) );
			float4 __mainColor = ( float4(1,1,1,1) );
			float __alpha = ( __albedo.a * __mainColor.a );
			float3 __depthColor = ( _DepthColor.rgb );
			float __depthColorDistance = ( _DepthColorDistance );
			float __foamSpread = ( _FoamSpread );
			float __foamStrength = ( _FoamStrength );
			float4 __foamColor = ( _FoamColor.rgba );
			float3 __foamTextureCustom = ( tex2D(_FoamTex, input.texcoord0.xy * _FoamTex_ST.xy + _FoamTex_ST.zw).rgb );
			float __foamMask = ( .0 );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampSmoothing = ( _RampSmoothing );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__ambientIntensity = ( 1.0 );
			output.__rimMin = ( _RimMin );
			output.__rimMax = ( _RimMax );
			output.__rimColor = ( _RimColor.rgb );
			output.__rimStrength = ( 1.0 );

			output.input = input;

			half3 worldNormal = WorldNormalVector(input, output.Normal);
			output.worldNormal = worldNormal;

			half ndv = abs(dot(input.viewDir, normalize(output.Normal.xyz)));
			half ndvRaw = ndv;
			output.ndv = ndv;
			output.ndvRaw = ndvRaw;

			// Sample depth texture and calculate difference with local depth
			float sceneDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.screenPosition));
			if (unity_OrthoParams.w > 0.0)
			{
				// Orthographic camera
				#if defined(UNITY_REVERSED_Z)
					sceneDepth = 1.0 - sceneDepth;
				#endif
				sceneDepth = (sceneDepth * _ProjectionParams.z) + _ProjectionParams.y;
			}
			else
			{
				// Perspective camera
				sceneDepth = LinearEyeDepth(sceneDepth);
			}
			
			float localDepth = input.screenPosition.z;
			float depthDiff = abs(sceneDepth - localDepth);
			depthDiff *= ndvRaw * __depthViewCorrectionBias;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			output.Albedo *= __mainColor.rgb;
			
			// Depth-based color
			half3 depthColor = __depthColor;
			half3 depthColorDist = __depthColorDistance;
			output.Albedo.rgb = lerp(depthColor, output.Albedo.rgb, saturate(depthColorDist * depthDiff));
			
			// Depth-based water foam
			half foamSpread = __foamSpread;
			half foamStrength = __foamStrength;
			half4 foamColor = __foamColor;
			
			half3 foam = __foamTextureCustom;
			float foamDepth = saturate(foamSpread * depthDiff) * (1.0 - __foamMask);
			half foamTerm = (step(foam.rgb, saturate(foamStrength - foamDepth)) * saturate(foamStrength - foamDepth)) * foamColor.a;
			output.Albedo.rgb = lerp(output.Albedo.rgb, foamColor.rgb, foamTerm);
			output.Alpha = lerp(output.Alpha, foamColor.a, foamTerm);
		}

		//================================================================
		// LIGHTING FUNCTION

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, half3 viewDir, UnityGI gi)
		{
			half ndv = surface.ndv;
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

			// Rim Lighting
			#if !defined(UNITY_PASS_FORWARDADD)
			half rim = 1 - surface.ndvRaw;
			rim = ( rim );
			half rimMin = surface.__rimMin;
			half rimMax = surface.__rimMax;
			rim = smoothstep(rimMin, rimMax, rim);
			half3 rimColor = surface.__rimColor;
			half rimStrength = surface.__rimStrength;
			color.rgb += rim * rimColor * rimStrength;
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

	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2018.4.11f1";ver:"2.6.0";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","CUSTOM_TIME","RIM","VSW_WORLDPOS","VERTEX_SIN_WAVES","DEPTH_VIEW_CORRECTION","DEPTH_BUFFER_COLOR","DEPTH_BUFFER_FOAM"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",RIM_LABEL="Rim Lighting"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"lerp(float4(1,1,1,1), {3}.rgba, {2}.a)";guid:"ba58c624-ecf3-47ca-a874-7e22e0d7a0e2";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:True;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:5;cc:4;chan:"AAAA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:WorldPosition;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_MainTex";md:"";custom:False;refs:"";guid:"fd326b54-3683-4e3b-a22d-e384770978ae";op:Multiply;lbl:"Water Texture";gpu_inst:False;locked:False;impl_index:0),imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:False;cc:4;chan:"RGBA";prop:"_WaterColor";md:"";custom:False;refs:"";guid:"67e9afdc-7793-4680-93be-07de7c23b028";op:Multiply;lbl:"Water Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Main Color";imps:list[imp_constant(type:color_rgba;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(1, 1, 1, 1);guid:"83fe16ef-ebbd-4fc1-8e4a-15d640c48b7e";op:Multiply;lbl:"Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 1dbf06e7ab17ebdf94acf27d98aa14a1 */
