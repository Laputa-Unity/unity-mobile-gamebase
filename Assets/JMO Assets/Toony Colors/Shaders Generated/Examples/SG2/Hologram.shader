// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Toony Colors Pro 2/Examples/SG2/Hologram"
{
	Properties
	{

		[TCP2HeaderHelp(Emission)]
		[NoScaleOffset] _MainTex ("Emission Texture", 2D) = "white" {}
		[TCP2Separator]
		
		[TCP2HeaderHelp(Outline)]
		_OutlineWidth ("Width", Range(0.1,4)) = 1
		// Outline Normals
		[TCP2MaterialKeywordEnumNoPrefix(Regular, _, Vertex Colors, TCP2_COLORS_AS_NORMALS, Tangents, TCP2_TANGENT_AS_NORMALS, UV1, TCP2_UV1_AS_NORMALS, UV2, TCP2_UV2_AS_NORMALS, UV3, TCP2_UV3_AS_NORMALS, UV4, TCP2_UV4_AS_NORMALS)]
		_NormalsSource ("Outline Normals Source", Float) = 0
		[TCP2MaterialKeywordEnumNoPrefix(Full XYZ, TCP2_UV_NORMALS_FULL, Compressed XY, _, Compressed ZW, TCP2_UV_NORMALS_ZW)]
		_NormalsUVType ("UV Data Type", Float) = 0
		[TCP2Separator]
		_NDVMinFrag ("NDV Min", Range(0,2)) = 0.5
		_NDVMaxFrag ("NDV Max", Range(0,2)) = 1
		[TCP2Separator]
		// Custom Material Properties
		[TCP2ColorNoAlpha] [HDR] _HologramColor ("Hologram Color", Color) = (0,0.502,1,1)
		 _ScanlinesTex ("Scanlines Texture", 2D) = "white" {}
		[TCP2UVScrolling] _ScanlinesTex_SC ("Scanlines Texture UV Scrolling", Vector) = (1,1,0,0)

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"IgnoreProjectors"="True"
		}

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Custom Material Properties

		sampler2D _ScanlinesTex;

		// Shader Properties
		sampler2D _MainTex;
		
		// Custom Material Properties
		half4 _HologramColor;
		float4 _ScanlinesTex_ST;
		float4 _ScanlinesTex_TexelSize;
		half4 _ScanlinesTex_SC;

		// Shader Properties
		float _OutlineWidth;
		float _NDVMinFrag;
		float _NDVMaxFrag;

		ENDCG

		// Outline Include
		CGINCLUDE

		struct appdata_outline
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			#if TCP2_UV1_AS_NORMALS
			float4 texcoord0 : TEXCOORD0;
		#elif TCP2_UV2_AS_NORMALS
			float4 texcoord1 : TEXCOORD1;
		#elif TCP2_UV3_AS_NORMALS
			float4 texcoord2 : TEXCOORD2;
		#elif TCP2_UV4_AS_NORMALS
			float4 texcoord3 : TEXCOORD3;
		#endif
		#if TCP2_COLORS_AS_NORMALS
			float4 vertexColor : COLOR;
		#endif
		#if TCP2_TANGENT_AS_NORMALS
			float4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f_outline
		{
			float4 vertex : SV_POSITION;
			float4 vcolor : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f_outline vertex_outline (appdata_outline v)
		{
			v2f_outline output;
			UNITY_INITIALIZE_OUTPUT(v2f_outline, output);
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			// Shader Properties Sampling
			float __outlineWidth = ( _OutlineWidth );
			float4 __outlineColorVertex = ( _HologramColor.rgba );

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
			v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
		
		#ifdef TCP2_COLORS_AS_NORMALS
			//Vertex Color for Normals
			float3 normal = (v.vertexColor.xyz*2) - 1;
		#elif TCP2_TANGENT_AS_NORMALS
			//Tangent for Normals
			float3 normal = v.tangent.xyz;
		#elif TCP2_UV1_AS_NORMALS || TCP2_UV2_AS_NORMALS || TCP2_UV3_AS_NORMALS || TCP2_UV4_AS_NORMALS
			#if TCP2_UV1_AS_NORMALS
				#define uvChannel texcoord0
			#elif TCP2_UV2_AS_NORMALS
				#define uvChannel texcoord1
			#elif TCP2_UV3_AS_NORMALS
				#define uvChannel texcoord2
			#elif TCP2_UV4_AS_NORMALS
				#define uvChannel texcoord3
			#endif
		
			#if TCP2_UV_NORMALS_FULL
			//UV for Normals, full
			float3 normal = v.uvChannel.xyz;
			#else
			//UV for Normals, compressed
			#if TCP2_UV_NORMALS_ZW
				#define ch1 z
				#define ch2 w
			#else
				#define ch1 x
				#define ch2 y
			#endif
			float3 n;
			//unpack uvs
			v.uvChannel.ch1 = v.uvChannel.ch1 * 255.0/16.0;
			n.x = floor(v.uvChannel.ch1) / 15.0;
			n.y = frac(v.uvChannel.ch1) * 16.0 / 15.0;
			//- get z
			n.z = v.uvChannel.ch2;
			//- transform
			n = n*2 - 1;
			float3 normal = n;
			#endif
		#else
			float3 normal = v.normal;
		#endif
		
		#if TCP2_ZSMOOTH_ON
			//Correct Z artefacts
			normal = UnityObjectToViewPos(normal);
			normal.z = -_ZSmooth;
		#endif
			float size = 1;
		
		#if !defined(SHADOWCASTER_PASS)
			output.vertex = UnityObjectToClipPos(v.vertex.xyz);
			normal = mul(unity_ObjectToWorld, float4(normal, 0)).xyz;
			float2 clipNormals = normalize(mul(UNITY_MATRIX_VP, float4(normal,0)).xy);
			clipNormals.xy *= output.vertex.w;
			clipNormals.xy = (clipNormals.xy / _ScreenParams.xy) * 2.0;
			half outlineWidth = __outlineWidth * size;
			output.vertex.xy += clipNormals.xy * outlineWidth;
		#else
			v.vertex = v.vertex + float4(normal,0) * __outlineWidth * size * 0.01;
		#endif
		
			output.vcolor.xyzw = __outlineColorVertex;

			return output;
		}

		float4 fragment_outline (v2f_outline input) : SV_Target
		{

			// Shader Properties Sampling
			float4 __outlineColor = ( float4(1,1,1,1) );

			half4 outlineColor = __outlineColor * input.vcolor.xyzw;

			return outlineColor;
		}

		ENDCG
		// Outline Include End

		//Depth pre-pass
		Pass
		{
			Name "Depth Prepass"
			Tags
			{
				"LightMode"="ForwardBase"
			}
			ColorMask 0
			ZWrite On

			CGPROGRAM
			#pragma vertex vertex_depthprepass
			#pragma fragment fragment_depthprepass
			#pragma target 3.0

			struct appdata_sil
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_depthprepass
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_depthprepass vertex_depthprepass (appdata_sil v)
			{
				v2f_depthprepass output;
				UNITY_INITIALIZE_OUTPUT(v2f_depthprepass, output);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
				v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				output.vertex = UnityObjectToClipPos(v.vertex);

				return output;
			}

			half4 fragment_depthprepass (v2f_depthprepass input) : SV_Target
			{

				return 0;
			}
			ENDCG
		}
		// Main Surface Shader
		Blend One One

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha noforwardadd novertexlights nolightmap nofog nolppv keepalpha
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
			half3 worldNormal; INTERNAL_DATA
			float4 screenPosition;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0 = v.texcoord0.xy;

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
			v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
			float4 clipPos = UnityObjectToClipPos(v.vertex);

			//Screen Position
			float4 screenPos = ComputeScreenPos(clipPos);
			output.screenPosition = screenPos;

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
			float3 __highlightColor;
			float3 __shadowColor;
			float __ambientIntensity;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			//Screen Space UV
			float2 screenUV = input.screenPosition.xy / input.screenPosition.w;
			
			// Custom Material Properties Sampling
			half4 value__ScanlinesTex = tex2D(_ScanlinesTex, screenUV * _ScanlinesTex_TexelSize.xy * _ScreenParams.xy * _ScanlinesTex_ST.xy + frac(_Time.yy * _ScanlinesTex_SC.xy) + _ScanlinesTex_ST.zw).rgba;

			// Shader Properties Sampling
			float __ndvMinFrag = ( _NDVMinFrag );
			float __ndvMaxFrag = ( _NDVMaxFrag );
			float4 __albedo = ( float4(0,0,0,0) );
			float4 __mainColor = ( float4(0,0,0,0) );
			float __alpha = ( __albedo.a * __mainColor.a );
			float3 __emission = ( tex2D(_MainTex, input.texcoord0.xy).rgb * _HologramColor.rgb * float3(0.5,0.5,0.5) * value__ScanlinesTex.aaa );
			output.__highlightColor = ( float3(0,0,0) );
			output.__shadowColor = ( float3(0,0,0) );
			output.__ambientIntensity = ( 1.0 );

			output.input = input;

			half3 worldNormal = WorldNormalVector(input, output.Normal);
			output.worldNormal = worldNormal;

			half ndv = abs(dot(input.viewDir, normalize(output.Normal.xyz)));
			half ndvRaw = ndv;
			ndv = 1 - ndv;
			ndv = smoothstep(__ndvMinFrag, __ndvMaxFrag, ndv);
			output.ndv = ndv;
			output.ndvRaw = ndvRaw;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			output.Albedo *= __mainColor.rgb;
			output.Emission += ( __emission * ndv.xxx );
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
			
			ndl = saturate(ndl);
			ramp = ndl.xxx;
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

		//Outline
		Pass
		{
			Name "Outline"
			Tags
			{
				"LightMode"="ForwardBase"
			}
			Cull Front

			CGPROGRAM
			#pragma vertex vertex_outline
			#pragma fragment fragment_outline
			#pragma target 3.0
			#pragma multi_compile _ TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV1_AS_NORMALS TCP2_UV2_AS_NORMALS TCP2_UV3_AS_NORMALS TCP2_UV4_AS_NORMALS
			#pragma multi_compile _ TCP2_UV_NORMALS_FULL TCP2_UV_NORMALS_ZW
			#pragma multi_compile_instancing
			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2018.4.11f1";ver:"2.4.0";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","EMISSION","SHADER_BLENDING","DEPTH_PREPASS","OUTLINE","ADDITIVE_BLENDING","OUTLINE_CONSTANT_SIZE","OUTLINE_CLIP_SPACE","OUTLINE_PIXEL_PERFECT","NO_RAMP","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3"];flags:list["noforwardadd","novertexlights"];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_constant(type:color_rgba;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 0);guid:"0aaff59c-53d6-4adc-93cb-337afbb75d11";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Main Color";imps:list[imp_constant(type:color_rgba;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 0);guid:"b40d8c7c-dd89-4f49-ab73-ee4992cbab20";op:Multiply;lbl:"Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,sp(name:"Highlight Color";imps:list[imp_constant(type:color;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"d3ae243d-185a-4c42-a250-eab77ac426cd";op:Multiply;lbl:"Highlight Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Shadow Color";imps:list[imp_constant(type:color;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"306e5603-964e-4557-809f-619b2d67c2c6";op:Multiply;lbl:"Shadow Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Emission";imps:list[imp_mp_texture(uto:False;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:3;chan:"RGB";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_MainTex";md:"";custom:False;refs:"";guid:"be90ab57-5c55-48a8-a72c-b9f1647d2637";op:Multiply;lbl:"Emission Texture";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_HologramColor";cc:3;chan:"RGB";avchan:"RGBA";guid:"7655f924-32e1-4821-9256-82d2da0e55e0";op:Multiply;lbl:"Emission Color";gpu_inst:False;locked:False;impl_index:-1),imp_constant(type:color;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0.5, 0.5, 0.5, 1);guid:"b601da56-3778-4f04-8fb2-2aeeade9d605";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_ScanlinesTex";cc:3;chan:"AAA";avchan:"RGBA";guid:"3f63e034-a8e7-4c3d-bc23-38b1341a67b1";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1),imp_generic(cc:3;chan:"XXX";source_id:"float ndv3fragment";needed_features:"USE_NDV_FRAGMENT";custom_code_compatible:False;options_v:dict[Use Min/Max Properties=True,Invert=True,Ignore Normal Map=False];guid:"37f7053a-35b7-43f9-9580-fe265dd06dca";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,sp(name:"Outline Color Vertex";imps:list[imp_ct(lct:"_HologramColor";cc:4;chan:"RGBA";avchan:"RGBA";guid:"f2484deb-7964-41a4-8f47-8f48c6fed160";op:Multiply;lbl:"Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,sp(name:"Vertex Position World";imps:list[imp_hook(guid:"83b4dcf6-1174-4906-bb5a-d084d44b39f6";op:Multiply;lbl:"worldPos.xyz";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"+ float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30)";guid:"378021e9-e245-4ac9-b2bd-9046996c8b7e";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,,,,,sp(name:"Ramp Threshold";imps:list[imp_constant(type:float;fprc:float;fv:0;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(1, 1, 1, 1);guid:"145c9946-8c3a-4367-a6fa-8ce114c28841";op:Multiply;lbl:"Threshold";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Ramp Smoothing";imps:list[imp_constant(type:float;fprc:float;fv:0;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(1, 1, 1, 1);guid:"151ba2ba-b535-4ff5-b0e9-c0e029fb5d6e";op:Multiply;lbl:"Smoothing";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Blend Destination";imps:list[imp_enum(value_type:0;value:0;enum_type:"ToonyColorsPro.ShaderGenerator.BlendFactor";guid:"93c10d78-183c-4b57-934a-4357e8db8813";op:Multiply;lbl:"Blend Destination";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Outline Blend Source";imps:list[imp_enum(value_type:0;value:0;enum_type:"ToonyColorsPro.ShaderGenerator.BlendFactor";guid:"110d47bf-1749-44dc-a5e0-5af19b354040";op:Multiply;lbl:"Blend Source";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Outline Blend Destination";imps:list[imp_enum(value_type:0;value:1;enum_type:"ToonyColorsPro.ShaderGenerator.BlendFactor";guid:"7fe5638a-715e-41a3-87f4-d80e4cf5fe32";op:Multiply;lbl:"Blend Destination";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[ct(cimp:imp_mp_color(def:RGBA(0, 0.502, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_HologramColor";md:"[TCP2ColorNoAlpha]";custom:True;refs:"Emission, Color (Per-Vertex)";guid:"d98e1807-216f-481e-98ca-23550db36d24";op:Multiply;lbl:"Hologram Color";gpu_inst:False;locked:False;impl_index:-1);exp:False;uv_exp:False;imp_lbl:"Color"),ct(cimp:imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:True;scr:True;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:4;cc:4;chan:"RGBA";mip:0;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:ScreenSpace;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_ScanlinesTex";md:"";custom:True;refs:"Emission";guid:"e059c82d-333e-41a0-b42d-83d58469534e";op:Multiply;lbl:"Scanlines Texture";gpu_inst:False;locked:False;impl_index:-1);exp:True;uv_exp:False;imp_lbl:"Texture")];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 9ba896dc7fff5186fc9d699112e137f8 */
