// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "vrbnInteriorMappingURP"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector]_offSetMidUV("offSetMidUV", Vector) = (0,0,0,0)
		[HideInInspector]_multMidUV("multMidUV", Vector) = (1,1,0,0)
		[ASEBegin]_Interior_Albedo("Interior_Albedo", 2D) = "white" {}
		_Interior_emission("Interior_emission", 2D) = "white" {}
		_MidPlane_albedo("MidPlane_albedo", 2D) = "white" {}
		_MidPlane_emission("MidPlane_emission", 2D) = "white" {}
		[Toggle(_ENABLEINTERIOR_ON)] _EnableInterior("EnableInterior", Float) = 0
		[HideInInspector]_RoomType("RoomType", Float) = 1
		[HideInInspector]_Width("Width", Float) = 4
		[HideInInspector]_Height("Height", Float) = 3
		[HideInInspector]_Depth("Depth", Float) = 4
		[HideInInspector]_UvsAspectRatio("UvsAspectRatio", Vector) = (0.245,0.184,1,0)
		[HideInInspector]_DepthTest("DepthTest", Float) = 0
		[HideInInspector]_MidPlaneDepth("MidPlaneDepth", Float) = 0
		[HideInInspector]_MidPlaneExtraAdjust("MidPlaneExtraAdjust", Float) = 0
		_behindGlass_albedo("behindGlass_albedo", 2D) = "white" {}
		_behindGlass_emission("behindGlass_emission", 2D) = "white" {}
		_glasAsset_albedo("glasAsset_albedo", 2D) = "white" {}
		_glasAsset_mrmao("glasAsset_mrmao", 2D) = "white" {}
		_glasAsset_normal("glasAsset_normal", 2D) = "bump" {}
		_frontOfGlass_albedo("frontOfGlass_albedo", 2D) = "white" {}
		_frontOfGlass_emission("frontOfGlass_emission", 2D) = "white" {}
		_frontOfGlass_mrmao("frontOfGlass_mrmao", 2D) = "white" {}
		_frontOfGlass_normal("frontOfGlass_normal", 2D) = "bump" {}
		_emissionMultiplierTexture("emissionMultiplierTexture", 2D) = "white" {}
		[ASEEnd]_emissionMultiplier("emissionMultiplier", Float) = 0


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }

		Cull Back
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 5.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
		

			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#include "customFunctionShader/VrbnTransformUVs.hlsl"
			#include "customFunctionShader/raycastingTangent.hlsl"
			#include "customFunctionShader/customTangent.hlsl"
			#include "customFunctionShader/MidPlaneVrbnTransformUVs.hlsl"
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#pragma multi_compile __ _ENABLEINTERIOR_ON
			#pragma multi_compile_local __ _ENABLEINTERIOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Interior_Albedo);
			SAMPLER(sampler_Interior_Albedo);
			TEXTURE2D(_MidPlane_albedo);
			SAMPLER(sampler_MidPlane_albedo);
			TEXTURE2D(_MidPlane_emission);
			SAMPLER(sampler_MidPlane_emission);
			TEXTURE2D(_behindGlass_albedo);
			SAMPLER(sampler_behindGlass_albedo);
			TEXTURE2D(_glasAsset_albedo);
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_frontOfGlass_albedo);
			TEXTURE2D(_glasAsset_normal);
			TEXTURE2D(_frontOfGlass_normal);
			TEXTURE2D(_frontOfGlass_mrmao);
			TEXTURE2D(_Interior_emission);
			SAMPLER(sampler_Interior_emission);
			TEXTURE2D(_behindGlass_emission);
			SAMPLER(sampler_behindGlass_emission);
			TEXTURE2D(_frontOfGlass_emission);
			TEXTURE2D(_emissionMultiplierTexture);
			SAMPLER(sampler_emissionMultiplierTexture);
			TEXTURE2D(_glasAsset_mrmao);


			float3 MidPlanealbedointerpolator75_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			
			float3 MidPlaneEmissioninterpolator77_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.ase_texcoord6.xy;
				o.ase_tangent = v.ase_tangent;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord7.zw = v.ase_texcoord5.xy;
				o.ase_texcoord8.xy = v.ase_texcoord4.xy;
				o.ase_texcoord8.zw = v.texcoord.xy;
				o.ase_texcoord9.xy = v.ase_texcoord3.xy;
				o.ase_texcoord9.zw = v.ase_texcoord7.xy;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				#endif

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					o.screenPos = ComputeScreenPos(positionCS);
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord = v.texcoord;
				o.ase_texcoord6 = v.ase_texcoord6;
				o.ase_texcoord5 = v.ase_texcoord5;
				o.ase_texcoord4 = v.ase_texcoord4;
				o.ase_texcoord3 = v.ase_texcoord3;
				o.ase_texcoord7 = v.ase_texcoord7;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.ase_texcoord6 = patch[0].ase_texcoord6 * bary.x + patch[1].ase_texcoord6 * bary.y + patch[2].ase_texcoord6 * bary.z;
				o.ase_texcoord5 = patch[0].ase_texcoord5 * bary.x + patch[1].ase_texcoord5 * bary.y + patch[2].ase_texcoord5 * bary.z;
				o.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
				o.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				o.ase_texcoord7 = patch[0].ase_texcoord7 * bary.x + patch[1].ase_texcoord7 * bary.y + patch[2].ase_texcoord7 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 temp_output_167_0_g259 = IN.ase_texcoord7.xy;
				float localgetVrbnTransformUVs4_g260 = ( 0.0 );
				float localDecodeuvs6_g260 = ( 0.0 );
				float localraycastingTangent9_g260 = ( 0.0 );
				float2 uvs9_g260 = ( float3( temp_output_167_0_g259 ,  0.0 ) / _UvsAspectRatio ).xy;
				float3 normalWS9_g260 = WorldNormal;
				float3 localgetViewDirTangent20_g260 = ( float3( 0,0,0 ) );
				float3 tangentOS20_g260 = IN.ase_tangent.xyz;
				float3 normalOS20_g260 = IN.ase_normal;
				float3 worldToObjDir62_g260 = mul( GetWorldToObjectMatrix(), float4( ( WorldViewDirection * 1.0 ), 0 ) ).xyz;
				float3 cameraVectorOS20_g260 = worldToObjDir62_g260;
				float3 normalWS20_g260 = WorldNormal;
				float widht20_g260 = _Width;
				float height20_g260 = _Height;
				float temp_output_15_0_g260 = _Depth;
				float depthPosition20_g260 = temp_output_15_0_g260;
				float3 inVector20_g260 = float3( 0,0,0 );
				getViewDirTangent_float( tangentOS20_g260 , normalOS20_g260 , cameraVectorOS20_g260 , normalWS20_g260 , widht20_g260 , height20_g260 , depthPosition20_g260 , inVector20_g260 );
				float3 viewDirTangent9_g260 = ( inVector20_g260 * float3(-1,-1,-1) );
				float depthFactor9_g260 = _DepthTest;
				float roomSize9_g260 = temp_output_15_0_g260;
				float midPlaneDepth9_g260 = _MidPlaneDepth;
				float extra9_g260 = _MidPlaneExtraAdjust;
				float3 pos9_g260 = float3( 0,0,0 );
				float distance9_g260 = 0.0;
				float2 midPlanePos9_g260 = float2( 0,0 );
				float midPlaneDist9_g260 = 0.0;
				raycastingTangent_float( uvs9_g260 , normalWS9_g260 , viewDirTangent9_g260 , depthFactor9_g260 , roomSize9_g260 , midPlaneDepth9_g260 , extra9_g260 , pos9_g260 , distance9_g260 , midPlanePos9_g260 , midPlaneDist9_g260 );
				float3 v6_g260 = pos9_g260;
				float2 uv6_g260 = float2( 0,0 );
				float faceIndex6_g260 = 0;
				{
				float3 vAbs = abs(v6_g260);
				 float ma;
				 
				 if(vAbs.z >= vAbs.x && vAbs.z >= vAbs.y)
				 {
				        faceIndex6_g260 = v6_g260.z < 0.0 ? 5.0 : 4.0;
				        ma = 0.5 / vAbs.z;
				        uv6_g260 = float2(v6_g260.z < 0.0 ? -v6_g260.x : v6_g260.x, -v6_g260.y);
				}
				else if(vAbs.y >= vAbs.x)
				{
				         faceIndex6_g260 = v6_g260.y < 0.0 ? 3.0 : 2.0;
				          ma = 0.5 / vAbs.y;
				          uv6_g260 = float2(v6_g260.x, v6_g260.y < 0.0 ? -v6_g260.z : v6_g260.z);
				}
				else
				 {
				          faceIndex6_g260 = v6_g260.x < 0.0 ? 1.0 : 0.0;
				           ma = 0.5 / vAbs.x;
				           uv6_g260 = float2(v6_g260.x < 0.0 ? v6_g260.z : -v6_g260.z, -v6_g260.y);
				 }
				 uv6_g260 = uv6_g260 * ma + 0.5;
				}
				float2 uv4_g260 = uv6_g260;
				float faceId4_g260 = faceIndex6_g260;
				float temp_output_5_0_g260 = _RoomType;
				float roomType4_g260 = temp_output_5_0_g260;
				float roomColorCode4_g260 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, IN.ase_texcoord7.xy ).a;
				float2 outUV4_g260 = float2( 0,0 );
				getVrbnTransformUVs_float( uv4_g260 , faceId4_g260 , roomType4_g260 , roomColorCode4_g260 , outUV4_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch101_g259 = outUV4_g260;
				#else
				float2 staticSwitch101_g259 = temp_output_167_0_g259;
				#endif
				float4 tex2DNode49_g259 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, staticSwitch101_g259 );
				float localgetMidPlaneVrbnTransformUVs63_g260 = ( 0.0 );
				float2 uv63_g260 = ( ( midPlanePos9_g260 + _offSetMidUV ) * _multMidUV );
				float roomType63_g260 = temp_output_5_0_g260;
				float roomColorCode63_g260 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, IN.ase_texcoord7.zw ).a;
				float2 outUV63_g260 = float2( 0,0 );
				getMidPlaneVrbnTransformUVs_float( uv63_g260 , roomType63_g260 , roomColorCode63_g260 , outUV63_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch104_g259 = outUV63_g260;
				#else
				float2 staticSwitch104_g259 = IN.ase_texcoord7.zw;
				#endif
				float4 tex2DNode61_g259 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, staticSwitch104_g259 );
				float4 tex2DNode62_g259 = SAMPLE_TEXTURE2D( _MidPlane_emission, sampler_MidPlane_emission, staticSwitch104_g259 );
				float4 lerpResult107_g259 = lerp( tex2DNode49_g259 , tex2DNode61_g259 , tex2DNode62_g259.a);
				float3 colorA75_g259 = tex2DNode49_g259.rgb;
				float3 colorB75_g259 = tex2DNode61_g259.rgb;
				float temp_output_246_1_g259 = distance9_g260;
				float distA75_g259 = temp_output_246_1_g259;
				float temp_output_246_3_g259 = midPlaneDist9_g260;
				float distB75_g259 = temp_output_246_3_g259;
				float alpha75_g259 = tex2DNode62_g259.a;
				float3 localMidPlanealbedointerpolator75_g259 = MidPlanealbedointerpolator75_g259( colorA75_g259 , colorB75_g259 , distA75_g259 , distB75_g259 , alpha75_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch105_g259 = float4( localMidPlanealbedointerpolator75_g259 , 0.0 );
				#else
				float4 staticSwitch105_g259 = lerpResult107_g259;
				#endif
				float4 tex2DNode18 = SAMPLE_TEXTURE2D( _behindGlass_albedo, sampler_behindGlass_albedo, IN.ase_texcoord8.xy );
				float4 lerpResult11 = lerp( staticSwitch105_g259 , tex2DNode18 , tex2DNode18.a);
				float4 tex2DNode19 = SAMPLE_TEXTURE2D( _glasAsset_albedo, sampler_Linear_Repeat, IN.ase_texcoord8.zw );
				float4 lerpResult13 = lerp( lerpResult11 , tex2DNode19 , tex2DNode19.a);
				float4 tex2DNode43 = SAMPLE_TEXTURE2D( _frontOfGlass_albedo, sampler_Linear_Repeat, IN.ase_texcoord9.xy );
				float4 lerpResult40 = lerp( lerpResult13 , tex2DNode43 , tex2DNode43.a);
				
				float4 color27 = IsGammaSpace() ? float4(0,0,1,1) : float4(0,0,1,1);
				float3 tex2DNode25 = UnpackNormalScale( SAMPLE_TEXTURE2D( _glasAsset_normal, sampler_Linear_Repeat, IN.ase_texcoord8.zw ), 1.0f );
				float3 appendResult29 = (float3(tex2DNode25.r , -tex2DNode25.g , tex2DNode25.b));
				float4 lerpResult26 = lerp( color27 , float4( appendResult29 , 0.0 ) , 0.01);
				float3 tex2DNode46 = UnpackNormalScale( SAMPLE_TEXTURE2D( _frontOfGlass_normal, sampler_Linear_Repeat, IN.ase_texcoord9.xy ), 1.0f );
				float3 appendResult31 = (float3(tex2DNode46.r , -tex2DNode46.g , tex2DNode46.b));
				float4 tex2DNode45 = SAMPLE_TEXTURE2D( _frontOfGlass_mrmao, sampler_Linear_Repeat, IN.ase_texcoord9.xy );
				float4 lerpResult37 = lerp( lerpResult26 , float4( appendResult31 , 0.0 ) , tex2DNode45.a);
				
				float4 tex2DNode46_g259 = SAMPLE_TEXTURE2D( _Interior_emission, sampler_Interior_emission, staticSwitch101_g259 );
				float4 lerpResult108_g259 = lerp( tex2DNode46_g259 , tex2DNode62_g259 , tex2DNode62_g259.a);
				float3 colorA77_g259 = tex2DNode46_g259.rgb;
				float3 colorB77_g259 = tex2DNode62_g259.rgb;
				float distA77_g259 = temp_output_246_1_g259;
				float distB77_g259 = temp_output_246_3_g259;
				float alpha77_g259 = tex2DNode62_g259.a;
				float3 localMidPlaneEmissioninterpolator77_g259 = MidPlaneEmissioninterpolator77_g259( colorA77_g259 , colorB77_g259 , distA77_g259 , distB77_g259 , alpha77_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch106_g259 = float4( localMidPlaneEmissioninterpolator77_g259 , 0.0 );
				#else
				float4 staticSwitch106_g259 = lerpResult108_g259;
				#endif
				float4 lerpResult17 = lerp( staticSwitch106_g259 , SAMPLE_TEXTURE2D( _behindGlass_emission, sampler_behindGlass_emission, IN.ase_texcoord8.xy ) , tex2DNode18.a);
				float4 lerpResult12 = lerp( lerpResult17 , SAMPLE_TEXTURE2D( _frontOfGlass_emission, sampler_Linear_Repeat, IN.ase_texcoord9.xy ) , tex2DNode43.a);
				
				float4 tex2DNode24 = SAMPLE_TEXTURE2D( _glasAsset_mrmao, sampler_Linear_Repeat, IN.ase_texcoord8.zw );
				float lerpResult14 = lerp( tex2DNode24.r , tex2DNode45.r , tex2DNode43.a);
				
				float lerpResult15 = lerp( ( 1.0 - tex2DNode24.g ) , ( 1.0 - tex2DNode45.g ) , tex2DNode43.a);
				
				float lerpResult41 = lerp( 1.0 , tex2DNode45.b , tex2DNode43.a);
				

				float3 BaseColor = lerpResult40.rgb;
				float3 Normal = lerpResult37.rgb;
				float3 Emission = ( lerpResult12 * _emissionMultiplier * SAMPLE_TEXTURE2D( _emissionMultiplierTexture, sampler_emissionMultiplierTexture, IN.ase_texcoord9.zw ) ).rgb;
				float3 Specular = 0.5;
				float Metallic = lerpResult14;
				float Smoothness = lerpResult15;
				float Occlusion = lerpResult41;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent, WorldBiTangent, WorldNormal));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif
					inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData,
					BaseColor,
					Metallic,
					Specular,
					Smoothness,
					Occlusion,
					Emission,
					Alpha);

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += BaseColor * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += BaseColor * transmission;
						}
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += BaseColor * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += BaseColor * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			
			float3 _LightDirection;
			#if ASE_SRP_VERSION >= 110000
				float3 _LightPosition;
			#endif

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				#if ASE_SRP_VERSION >= 110000
					#if _CASTING_PUNCTUAL_LIGHT_SHADOW
						float3 lightDirectionWS = normalize(_LightPosition - positionWS);
					#else
						float3 lightDirectionWS = _LightDirection;
					#endif

					float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

					#if UNITY_REVERSED_Z
						clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
					#else
						clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
					#endif
				#else
					float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
					#if UNITY_REVERSED_Z
						clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
					#else
						clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = clipPos;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

			#include "customFunctionShader/VrbnTransformUVs.hlsl"
			#include "customFunctionShader/raycastingTangent.hlsl"
			#include "customFunctionShader/customTangent.hlsl"
			#include "customFunctionShader/MidPlaneVrbnTransformUVs.hlsl"
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile __ _ENABLEINTERIOR_ON
			#pragma multi_compile_local __ _ENABLEINTERIOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Interior_Albedo);
			SAMPLER(sampler_Interior_Albedo);
			TEXTURE2D(_MidPlane_albedo);
			SAMPLER(sampler_MidPlane_albedo);
			TEXTURE2D(_MidPlane_emission);
			SAMPLER(sampler_MidPlane_emission);
			TEXTURE2D(_behindGlass_albedo);
			SAMPLER(sampler_behindGlass_albedo);
			TEXTURE2D(_glasAsset_albedo);
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_frontOfGlass_albedo);
			TEXTURE2D(_Interior_emission);
			SAMPLER(sampler_Interior_emission);
			TEXTURE2D(_behindGlass_emission);
			SAMPLER(sampler_behindGlass_emission);
			TEXTURE2D(_frontOfGlass_emission);
			TEXTURE2D(_emissionMultiplierTexture);
			SAMPLER(sampler_emissionMultiplierTexture);


			float3 MidPlanealbedointerpolator75_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			
			float3 MidPlaneEmissioninterpolator77_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord2.xy = v.ase_texcoord6.xy;
				o.ase_tangent = v.ase_tangent;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord2.zw = v.ase_texcoord5.xy;
				o.ase_texcoord4.xy = v.ase_texcoord4.xy;
				o.ase_texcoord4.zw = v.ase_texcoord.xy;
				o.ase_texcoord5.xy = v.ase_texcoord3.xy;
				o.ase_texcoord5.zw = v.ase_texcoord7.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord6 = v.ase_texcoord6;
				o.ase_tangent = v.ase_tangent;
				o.ase_texcoord5 = v.ase_texcoord5;
				o.ase_texcoord4 = v.ase_texcoord4;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord3;
				o.ase_texcoord7 = v.ase_texcoord7;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord6 = patch[0].ase_texcoord6 * bary.x + patch[1].ase_texcoord6 * bary.y + patch[2].ase_texcoord6 * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_texcoord5 = patch[0].ase_texcoord5 * bary.x + patch[1].ase_texcoord5 * bary.y + patch[2].ase_texcoord5 * bary.z;
				o.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				o.ase_texcoord7 = patch[0].ase_texcoord7 * bary.x + patch[1].ase_texcoord7 * bary.y + patch[2].ase_texcoord7 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_167_0_g259 = IN.ase_texcoord2.xy;
				float localgetVrbnTransformUVs4_g260 = ( 0.0 );
				float localDecodeuvs6_g260 = ( 0.0 );
				float localraycastingTangent9_g260 = ( 0.0 );
				float2 uvs9_g260 = ( float3( temp_output_167_0_g259 ,  0.0 ) / _UvsAspectRatio ).xy;
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float3 normalWS9_g260 = ase_worldNormal;
				float3 localgetViewDirTangent20_g260 = ( float3( 0,0,0 ) );
				float3 tangentOS20_g260 = IN.ase_tangent.xyz;
				float3 normalOS20_g260 = IN.ase_normal;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldToObjDir62_g260 = mul( GetWorldToObjectMatrix(), float4( ( ase_worldViewDir * 1.0 ), 0 ) ).xyz;
				float3 cameraVectorOS20_g260 = worldToObjDir62_g260;
				float3 normalWS20_g260 = ase_worldNormal;
				float widht20_g260 = _Width;
				float height20_g260 = _Height;
				float temp_output_15_0_g260 = _Depth;
				float depthPosition20_g260 = temp_output_15_0_g260;
				float3 inVector20_g260 = float3( 0,0,0 );
				getViewDirTangent_float( tangentOS20_g260 , normalOS20_g260 , cameraVectorOS20_g260 , normalWS20_g260 , widht20_g260 , height20_g260 , depthPosition20_g260 , inVector20_g260 );
				float3 viewDirTangent9_g260 = ( inVector20_g260 * float3(-1,-1,-1) );
				float depthFactor9_g260 = _DepthTest;
				float roomSize9_g260 = temp_output_15_0_g260;
				float midPlaneDepth9_g260 = _MidPlaneDepth;
				float extra9_g260 = _MidPlaneExtraAdjust;
				float3 pos9_g260 = float3( 0,0,0 );
				float distance9_g260 = 0.0;
				float2 midPlanePos9_g260 = float2( 0,0 );
				float midPlaneDist9_g260 = 0.0;
				raycastingTangent_float( uvs9_g260 , normalWS9_g260 , viewDirTangent9_g260 , depthFactor9_g260 , roomSize9_g260 , midPlaneDepth9_g260 , extra9_g260 , pos9_g260 , distance9_g260 , midPlanePos9_g260 , midPlaneDist9_g260 );
				float3 v6_g260 = pos9_g260;
				float2 uv6_g260 = float2( 0,0 );
				float faceIndex6_g260 = 0;
				{
				float3 vAbs = abs(v6_g260);
				 float ma;
				 
				 if(vAbs.z >= vAbs.x && vAbs.z >= vAbs.y)
				 {
				        faceIndex6_g260 = v6_g260.z < 0.0 ? 5.0 : 4.0;
				        ma = 0.5 / vAbs.z;
				        uv6_g260 = float2(v6_g260.z < 0.0 ? -v6_g260.x : v6_g260.x, -v6_g260.y);
				}
				else if(vAbs.y >= vAbs.x)
				{
				         faceIndex6_g260 = v6_g260.y < 0.0 ? 3.0 : 2.0;
				          ma = 0.5 / vAbs.y;
				          uv6_g260 = float2(v6_g260.x, v6_g260.y < 0.0 ? -v6_g260.z : v6_g260.z);
				}
				else
				 {
				          faceIndex6_g260 = v6_g260.x < 0.0 ? 1.0 : 0.0;
				           ma = 0.5 / vAbs.x;
				           uv6_g260 = float2(v6_g260.x < 0.0 ? v6_g260.z : -v6_g260.z, -v6_g260.y);
				 }
				 uv6_g260 = uv6_g260 * ma + 0.5;
				}
				float2 uv4_g260 = uv6_g260;
				float faceId4_g260 = faceIndex6_g260;
				float temp_output_5_0_g260 = _RoomType;
				float roomType4_g260 = temp_output_5_0_g260;
				float roomColorCode4_g260 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, IN.ase_texcoord2.xy ).a;
				float2 outUV4_g260 = float2( 0,0 );
				getVrbnTransformUVs_float( uv4_g260 , faceId4_g260 , roomType4_g260 , roomColorCode4_g260 , outUV4_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch101_g259 = outUV4_g260;
				#else
				float2 staticSwitch101_g259 = temp_output_167_0_g259;
				#endif
				float4 tex2DNode49_g259 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, staticSwitch101_g259 );
				float localgetMidPlaneVrbnTransformUVs63_g260 = ( 0.0 );
				float2 uv63_g260 = ( ( midPlanePos9_g260 + _offSetMidUV ) * _multMidUV );
				float roomType63_g260 = temp_output_5_0_g260;
				float roomColorCode63_g260 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, IN.ase_texcoord2.zw ).a;
				float2 outUV63_g260 = float2( 0,0 );
				getMidPlaneVrbnTransformUVs_float( uv63_g260 , roomType63_g260 , roomColorCode63_g260 , outUV63_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch104_g259 = outUV63_g260;
				#else
				float2 staticSwitch104_g259 = IN.ase_texcoord2.zw;
				#endif
				float4 tex2DNode61_g259 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, staticSwitch104_g259 );
				float4 tex2DNode62_g259 = SAMPLE_TEXTURE2D( _MidPlane_emission, sampler_MidPlane_emission, staticSwitch104_g259 );
				float4 lerpResult107_g259 = lerp( tex2DNode49_g259 , tex2DNode61_g259 , tex2DNode62_g259.a);
				float3 colorA75_g259 = tex2DNode49_g259.rgb;
				float3 colorB75_g259 = tex2DNode61_g259.rgb;
				float temp_output_246_1_g259 = distance9_g260;
				float distA75_g259 = temp_output_246_1_g259;
				float temp_output_246_3_g259 = midPlaneDist9_g260;
				float distB75_g259 = temp_output_246_3_g259;
				float alpha75_g259 = tex2DNode62_g259.a;
				float3 localMidPlanealbedointerpolator75_g259 = MidPlanealbedointerpolator75_g259( colorA75_g259 , colorB75_g259 , distA75_g259 , distB75_g259 , alpha75_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch105_g259 = float4( localMidPlanealbedointerpolator75_g259 , 0.0 );
				#else
				float4 staticSwitch105_g259 = lerpResult107_g259;
				#endif
				float4 tex2DNode18 = SAMPLE_TEXTURE2D( _behindGlass_albedo, sampler_behindGlass_albedo, IN.ase_texcoord4.xy );
				float4 lerpResult11 = lerp( staticSwitch105_g259 , tex2DNode18 , tex2DNode18.a);
				float4 tex2DNode19 = SAMPLE_TEXTURE2D( _glasAsset_albedo, sampler_Linear_Repeat, IN.ase_texcoord4.zw );
				float4 lerpResult13 = lerp( lerpResult11 , tex2DNode19 , tex2DNode19.a);
				float4 tex2DNode43 = SAMPLE_TEXTURE2D( _frontOfGlass_albedo, sampler_Linear_Repeat, IN.ase_texcoord5.xy );
				float4 lerpResult40 = lerp( lerpResult13 , tex2DNode43 , tex2DNode43.a);
				
				float4 tex2DNode46_g259 = SAMPLE_TEXTURE2D( _Interior_emission, sampler_Interior_emission, staticSwitch101_g259 );
				float4 lerpResult108_g259 = lerp( tex2DNode46_g259 , tex2DNode62_g259 , tex2DNode62_g259.a);
				float3 colorA77_g259 = tex2DNode46_g259.rgb;
				float3 colorB77_g259 = tex2DNode62_g259.rgb;
				float distA77_g259 = temp_output_246_1_g259;
				float distB77_g259 = temp_output_246_3_g259;
				float alpha77_g259 = tex2DNode62_g259.a;
				float3 localMidPlaneEmissioninterpolator77_g259 = MidPlaneEmissioninterpolator77_g259( colorA77_g259 , colorB77_g259 , distA77_g259 , distB77_g259 , alpha77_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch106_g259 = float4( localMidPlaneEmissioninterpolator77_g259 , 0.0 );
				#else
				float4 staticSwitch106_g259 = lerpResult108_g259;
				#endif
				float4 lerpResult17 = lerp( staticSwitch106_g259 , SAMPLE_TEXTURE2D( _behindGlass_emission, sampler_behindGlass_emission, IN.ase_texcoord4.xy ) , tex2DNode18.a);
				float4 lerpResult12 = lerp( lerpResult17 , SAMPLE_TEXTURE2D( _frontOfGlass_emission, sampler_Linear_Repeat, IN.ase_texcoord5.xy ) , tex2DNode43.a);
				

				float3 BaseColor = lerpResult40.rgb;
				float3 Emission = ( lerpResult12 * _emissionMultiplier * SAMPLE_TEXTURE2D( _emissionMultiplierTexture, sampler_emissionMultiplierTexture, IN.ase_texcoord5.zw ) ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;

				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "customFunctionShader/VrbnTransformUVs.hlsl"
			#include "customFunctionShader/raycastingTangent.hlsl"
			#include "customFunctionShader/customTangent.hlsl"
			#include "customFunctionShader/MidPlaneVrbnTransformUVs.hlsl"
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile __ _ENABLEINTERIOR_ON
			#pragma multi_compile_local __ _ENABLEINTERIOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Interior_Albedo);
			SAMPLER(sampler_Interior_Albedo);
			TEXTURE2D(_MidPlane_albedo);
			SAMPLER(sampler_MidPlane_albedo);
			TEXTURE2D(_MidPlane_emission);
			SAMPLER(sampler_MidPlane_emission);
			TEXTURE2D(_behindGlass_albedo);
			SAMPLER(sampler_behindGlass_albedo);
			TEXTURE2D(_glasAsset_albedo);
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_frontOfGlass_albedo);


			float3 MidPlanealbedointerpolator75_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord2.xy = v.ase_texcoord6.xy;
				o.ase_tangent = v.ase_tangent;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord2.zw = v.ase_texcoord5.xy;
				o.ase_texcoord4.xy = v.ase_texcoord4.xy;
				o.ase_texcoord4.zw = v.ase_texcoord.xy;
				o.ase_texcoord5.xy = v.ase_texcoord3.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord5.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord3 : TEXCOORD3;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord6 = v.ase_texcoord6;
				o.ase_tangent = v.ase_tangent;
				o.ase_texcoord5 = v.ase_texcoord5;
				o.ase_texcoord4 = v.ase_texcoord4;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord3;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord6 = patch[0].ase_texcoord6 * bary.x + patch[1].ase_texcoord6 * bary.y + patch[2].ase_texcoord6 * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_texcoord5 = patch[0].ase_texcoord5 * bary.x + patch[1].ase_texcoord5 * bary.y + patch[2].ase_texcoord5 * bary.z;
				o.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_167_0_g259 = IN.ase_texcoord2.xy;
				float localgetVrbnTransformUVs4_g260 = ( 0.0 );
				float localDecodeuvs6_g260 = ( 0.0 );
				float localraycastingTangent9_g260 = ( 0.0 );
				float2 uvs9_g260 = ( float3( temp_output_167_0_g259 ,  0.0 ) / _UvsAspectRatio ).xy;
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float3 normalWS9_g260 = ase_worldNormal;
				float3 localgetViewDirTangent20_g260 = ( float3( 0,0,0 ) );
				float3 tangentOS20_g260 = IN.ase_tangent.xyz;
				float3 normalOS20_g260 = IN.ase_normal;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldToObjDir62_g260 = mul( GetWorldToObjectMatrix(), float4( ( ase_worldViewDir * 1.0 ), 0 ) ).xyz;
				float3 cameraVectorOS20_g260 = worldToObjDir62_g260;
				float3 normalWS20_g260 = ase_worldNormal;
				float widht20_g260 = _Width;
				float height20_g260 = _Height;
				float temp_output_15_0_g260 = _Depth;
				float depthPosition20_g260 = temp_output_15_0_g260;
				float3 inVector20_g260 = float3( 0,0,0 );
				getViewDirTangent_float( tangentOS20_g260 , normalOS20_g260 , cameraVectorOS20_g260 , normalWS20_g260 , widht20_g260 , height20_g260 , depthPosition20_g260 , inVector20_g260 );
				float3 viewDirTangent9_g260 = ( inVector20_g260 * float3(-1,-1,-1) );
				float depthFactor9_g260 = _DepthTest;
				float roomSize9_g260 = temp_output_15_0_g260;
				float midPlaneDepth9_g260 = _MidPlaneDepth;
				float extra9_g260 = _MidPlaneExtraAdjust;
				float3 pos9_g260 = float3( 0,0,0 );
				float distance9_g260 = 0.0;
				float2 midPlanePos9_g260 = float2( 0,0 );
				float midPlaneDist9_g260 = 0.0;
				raycastingTangent_float( uvs9_g260 , normalWS9_g260 , viewDirTangent9_g260 , depthFactor9_g260 , roomSize9_g260 , midPlaneDepth9_g260 , extra9_g260 , pos9_g260 , distance9_g260 , midPlanePos9_g260 , midPlaneDist9_g260 );
				float3 v6_g260 = pos9_g260;
				float2 uv6_g260 = float2( 0,0 );
				float faceIndex6_g260 = 0;
				{
				float3 vAbs = abs(v6_g260);
				 float ma;
				 
				 if(vAbs.z >= vAbs.x && vAbs.z >= vAbs.y)
				 {
				        faceIndex6_g260 = v6_g260.z < 0.0 ? 5.0 : 4.0;
				        ma = 0.5 / vAbs.z;
				        uv6_g260 = float2(v6_g260.z < 0.0 ? -v6_g260.x : v6_g260.x, -v6_g260.y);
				}
				else if(vAbs.y >= vAbs.x)
				{
				         faceIndex6_g260 = v6_g260.y < 0.0 ? 3.0 : 2.0;
				          ma = 0.5 / vAbs.y;
				          uv6_g260 = float2(v6_g260.x, v6_g260.y < 0.0 ? -v6_g260.z : v6_g260.z);
				}
				else
				 {
				          faceIndex6_g260 = v6_g260.x < 0.0 ? 1.0 : 0.0;
				           ma = 0.5 / vAbs.x;
				           uv6_g260 = float2(v6_g260.x < 0.0 ? v6_g260.z : -v6_g260.z, -v6_g260.y);
				 }
				 uv6_g260 = uv6_g260 * ma + 0.5;
				}
				float2 uv4_g260 = uv6_g260;
				float faceId4_g260 = faceIndex6_g260;
				float temp_output_5_0_g260 = _RoomType;
				float roomType4_g260 = temp_output_5_0_g260;
				float roomColorCode4_g260 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, IN.ase_texcoord2.xy ).a;
				float2 outUV4_g260 = float2( 0,0 );
				getVrbnTransformUVs_float( uv4_g260 , faceId4_g260 , roomType4_g260 , roomColorCode4_g260 , outUV4_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch101_g259 = outUV4_g260;
				#else
				float2 staticSwitch101_g259 = temp_output_167_0_g259;
				#endif
				float4 tex2DNode49_g259 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, staticSwitch101_g259 );
				float localgetMidPlaneVrbnTransformUVs63_g260 = ( 0.0 );
				float2 uv63_g260 = ( ( midPlanePos9_g260 + _offSetMidUV ) * _multMidUV );
				float roomType63_g260 = temp_output_5_0_g260;
				float roomColorCode63_g260 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, IN.ase_texcoord2.zw ).a;
				float2 outUV63_g260 = float2( 0,0 );
				getMidPlaneVrbnTransformUVs_float( uv63_g260 , roomType63_g260 , roomColorCode63_g260 , outUV63_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch104_g259 = outUV63_g260;
				#else
				float2 staticSwitch104_g259 = IN.ase_texcoord2.zw;
				#endif
				float4 tex2DNode61_g259 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, staticSwitch104_g259 );
				float4 tex2DNode62_g259 = SAMPLE_TEXTURE2D( _MidPlane_emission, sampler_MidPlane_emission, staticSwitch104_g259 );
				float4 lerpResult107_g259 = lerp( tex2DNode49_g259 , tex2DNode61_g259 , tex2DNode62_g259.a);
				float3 colorA75_g259 = tex2DNode49_g259.rgb;
				float3 colorB75_g259 = tex2DNode61_g259.rgb;
				float temp_output_246_1_g259 = distance9_g260;
				float distA75_g259 = temp_output_246_1_g259;
				float temp_output_246_3_g259 = midPlaneDist9_g260;
				float distB75_g259 = temp_output_246_3_g259;
				float alpha75_g259 = tex2DNode62_g259.a;
				float3 localMidPlanealbedointerpolator75_g259 = MidPlanealbedointerpolator75_g259( colorA75_g259 , colorB75_g259 , distA75_g259 , distB75_g259 , alpha75_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch105_g259 = float4( localMidPlanealbedointerpolator75_g259 , 0.0 );
				#else
				float4 staticSwitch105_g259 = lerpResult107_g259;
				#endif
				float4 tex2DNode18 = SAMPLE_TEXTURE2D( _behindGlass_albedo, sampler_behindGlass_albedo, IN.ase_texcoord4.xy );
				float4 lerpResult11 = lerp( staticSwitch105_g259 , tex2DNode18 , tex2DNode18.a);
				float4 tex2DNode19 = SAMPLE_TEXTURE2D( _glasAsset_albedo, sampler_Linear_Repeat, IN.ase_texcoord4.zw );
				float4 lerpResult13 = lerp( lerpResult11 , tex2DNode19 , tex2DNode19.a);
				float4 tex2DNode43 = SAMPLE_TEXTURE2D( _frontOfGlass_albedo, sampler_Linear_Repeat, IN.ase_texcoord5.xy );
				float4 lerpResult40 = lerp( lerpResult13 , tex2DNode43 , tex2DNode43.a);
				

				float3 BaseColor = lerpResult40.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if ASE_SRP_VERSION >= 110000
					return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
				#elif ASE_SRP_VERSION >= 100900
					return float4(PackNormalOctRectEncode(normalize(IN.worldNormal)), 0.0, 0.0);
				#else
					return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 110000
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
		

			#pragma multi_compile_fragment _ _SHADOWS_SOFT

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#include "customFunctionShader/VrbnTransformUVs.hlsl"
			#include "customFunctionShader/raycastingTangent.hlsl"
			#include "customFunctionShader/customTangent.hlsl"
			#include "customFunctionShader/MidPlaneVrbnTransformUVs.hlsl"
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#pragma multi_compile __ _ENABLEINTERIOR_ON
			#pragma multi_compile_local __ _ENABLEINTERIOR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float3 _UvsAspectRatio;
			float2 _offSetMidUV;
			float2 _multMidUV;
			float _Width;
			float _Height;
			float _Depth;
			float _DepthTest;
			float _MidPlaneDepth;
			float _MidPlaneExtraAdjust;
			float _RoomType;
			float _emissionMultiplier;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Interior_Albedo);
			SAMPLER(sampler_Interior_Albedo);
			TEXTURE2D(_MidPlane_albedo);
			SAMPLER(sampler_MidPlane_albedo);
			TEXTURE2D(_MidPlane_emission);
			SAMPLER(sampler_MidPlane_emission);
			TEXTURE2D(_behindGlass_albedo);
			SAMPLER(sampler_behindGlass_albedo);
			TEXTURE2D(_glasAsset_albedo);
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_frontOfGlass_albedo);
			TEXTURE2D(_glasAsset_normal);
			TEXTURE2D(_frontOfGlass_normal);
			TEXTURE2D(_frontOfGlass_mrmao);
			TEXTURE2D(_Interior_emission);
			SAMPLER(sampler_Interior_emission);
			TEXTURE2D(_behindGlass_emission);
			SAMPLER(sampler_behindGlass_emission);
			TEXTURE2D(_frontOfGlass_emission);
			TEXTURE2D(_emissionMultiplierTexture);
			SAMPLER(sampler_emissionMultiplierTexture);
			TEXTURE2D(_glasAsset_mrmao);


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			float3 MidPlanealbedointerpolator75_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			
			float3 MidPlaneEmissioninterpolator77_g259( float3 colorA, float3 colorB, float distA, float distB, float alpha )
			{
				float3 result = colorA;
				if (distB < distA)
				   result =lerp(colorA,colorB,alpha);
				return result;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.ase_texcoord6.xy;
				o.ase_tangent = v.ase_tangent;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord7.zw = v.ase_texcoord5.xy;
				o.ase_texcoord8.xy = v.ase_texcoord4.xy;
				o.ase_texcoord8.zw = v.texcoord.xy;
				o.ase_texcoord9.xy = v.ase_texcoord3.xy;
				o.ase_texcoord9.zw = v.ase_texcoord7.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH(normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz);
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

					o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					o.screenPos = ComputeScreenPos(positionCS);
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord7 : TEXCOORD7;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord = v.texcoord;
				o.ase_texcoord6 = v.ase_texcoord6;
				o.ase_texcoord5 = v.ase_texcoord5;
				o.ase_texcoord4 = v.ase_texcoord4;
				o.ase_texcoord3 = v.ase_texcoord3;
				o.ase_texcoord7 = v.ase_texcoord7;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.ase_texcoord6 = patch[0].ase_texcoord6 * bary.x + patch[1].ase_texcoord6 * bary.y + patch[2].ase_texcoord6 * bary.z;
				o.ase_texcoord5 = patch[0].ase_texcoord5 * bary.x + patch[1].ase_texcoord5 * bary.y + patch[2].ase_texcoord5 * bary.z;
				o.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
				o.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				o.ase_texcoord7 = patch[0].ase_texcoord7 * bary.x + patch[1].ase_texcoord7 * bary.y + patch[2].ase_texcoord7 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			FragmentOutput frag ( VertexOutput IN
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 temp_output_167_0_g259 = IN.ase_texcoord7.xy;
				float localgetVrbnTransformUVs4_g260 = ( 0.0 );
				float localDecodeuvs6_g260 = ( 0.0 );
				float localraycastingTangent9_g260 = ( 0.0 );
				float2 uvs9_g260 = ( float3( temp_output_167_0_g259 ,  0.0 ) / _UvsAspectRatio ).xy;
				float3 normalWS9_g260 = WorldNormal;
				float3 localgetViewDirTangent20_g260 = ( float3( 0,0,0 ) );
				float3 tangentOS20_g260 = IN.ase_tangent.xyz;
				float3 normalOS20_g260 = IN.ase_normal;
				float3 worldToObjDir62_g260 = mul( GetWorldToObjectMatrix(), float4( ( WorldViewDirection * 1.0 ), 0 ) ).xyz;
				float3 cameraVectorOS20_g260 = worldToObjDir62_g260;
				float3 normalWS20_g260 = WorldNormal;
				float widht20_g260 = _Width;
				float height20_g260 = _Height;
				float temp_output_15_0_g260 = _Depth;
				float depthPosition20_g260 = temp_output_15_0_g260;
				float3 inVector20_g260 = float3( 0,0,0 );
				getViewDirTangent_float( tangentOS20_g260 , normalOS20_g260 , cameraVectorOS20_g260 , normalWS20_g260 , widht20_g260 , height20_g260 , depthPosition20_g260 , inVector20_g260 );
				float3 viewDirTangent9_g260 = ( inVector20_g260 * float3(-1,-1,-1) );
				float depthFactor9_g260 = _DepthTest;
				float roomSize9_g260 = temp_output_15_0_g260;
				float midPlaneDepth9_g260 = _MidPlaneDepth;
				float extra9_g260 = _MidPlaneExtraAdjust;
				float3 pos9_g260 = float3( 0,0,0 );
				float distance9_g260 = 0.0;
				float2 midPlanePos9_g260 = float2( 0,0 );
				float midPlaneDist9_g260 = 0.0;
				raycastingTangent_float( uvs9_g260 , normalWS9_g260 , viewDirTangent9_g260 , depthFactor9_g260 , roomSize9_g260 , midPlaneDepth9_g260 , extra9_g260 , pos9_g260 , distance9_g260 , midPlanePos9_g260 , midPlaneDist9_g260 );
				float3 v6_g260 = pos9_g260;
				float2 uv6_g260 = float2( 0,0 );
				float faceIndex6_g260 = 0;
				{
				float3 vAbs = abs(v6_g260);
				 float ma;
				 
				 if(vAbs.z >= vAbs.x && vAbs.z >= vAbs.y)
				 {
				        faceIndex6_g260 = v6_g260.z < 0.0 ? 5.0 : 4.0;
				        ma = 0.5 / vAbs.z;
				        uv6_g260 = float2(v6_g260.z < 0.0 ? -v6_g260.x : v6_g260.x, -v6_g260.y);
				}
				else if(vAbs.y >= vAbs.x)
				{
				         faceIndex6_g260 = v6_g260.y < 0.0 ? 3.0 : 2.0;
				          ma = 0.5 / vAbs.y;
				          uv6_g260 = float2(v6_g260.x, v6_g260.y < 0.0 ? -v6_g260.z : v6_g260.z);
				}
				else
				 {
				          faceIndex6_g260 = v6_g260.x < 0.0 ? 1.0 : 0.0;
				           ma = 0.5 / vAbs.x;
				           uv6_g260 = float2(v6_g260.x < 0.0 ? v6_g260.z : -v6_g260.z, -v6_g260.y);
				 }
				 uv6_g260 = uv6_g260 * ma + 0.5;
				}
				float2 uv4_g260 = uv6_g260;
				float faceId4_g260 = faceIndex6_g260;
				float temp_output_5_0_g260 = _RoomType;
				float roomType4_g260 = temp_output_5_0_g260;
				float roomColorCode4_g260 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, IN.ase_texcoord7.xy ).a;
				float2 outUV4_g260 = float2( 0,0 );
				getVrbnTransformUVs_float( uv4_g260 , faceId4_g260 , roomType4_g260 , roomColorCode4_g260 , outUV4_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch101_g259 = outUV4_g260;
				#else
				float2 staticSwitch101_g259 = temp_output_167_0_g259;
				#endif
				float4 tex2DNode49_g259 = SAMPLE_TEXTURE2D( _Interior_Albedo, sampler_Interior_Albedo, staticSwitch101_g259 );
				float localgetMidPlaneVrbnTransformUVs63_g260 = ( 0.0 );
				float2 uv63_g260 = ( ( midPlanePos9_g260 + _offSetMidUV ) * _multMidUV );
				float roomType63_g260 = temp_output_5_0_g260;
				float roomColorCode63_g260 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, IN.ase_texcoord7.zw ).a;
				float2 outUV63_g260 = float2( 0,0 );
				getMidPlaneVrbnTransformUVs_float( uv63_g260 , roomType63_g260 , roomColorCode63_g260 , outUV63_g260 );
				#ifdef _ENABLEINTERIOR_ON
				float2 staticSwitch104_g259 = outUV63_g260;
				#else
				float2 staticSwitch104_g259 = IN.ase_texcoord7.zw;
				#endif
				float4 tex2DNode61_g259 = SAMPLE_TEXTURE2D( _MidPlane_albedo, sampler_MidPlane_albedo, staticSwitch104_g259 );
				float4 tex2DNode62_g259 = SAMPLE_TEXTURE2D( _MidPlane_emission, sampler_MidPlane_emission, staticSwitch104_g259 );
				float4 lerpResult107_g259 = lerp( tex2DNode49_g259 , tex2DNode61_g259 , tex2DNode62_g259.a);
				float3 colorA75_g259 = tex2DNode49_g259.rgb;
				float3 colorB75_g259 = tex2DNode61_g259.rgb;
				float temp_output_246_1_g259 = distance9_g260;
				float distA75_g259 = temp_output_246_1_g259;
				float temp_output_246_3_g259 = midPlaneDist9_g260;
				float distB75_g259 = temp_output_246_3_g259;
				float alpha75_g259 = tex2DNode62_g259.a;
				float3 localMidPlanealbedointerpolator75_g259 = MidPlanealbedointerpolator75_g259( colorA75_g259 , colorB75_g259 , distA75_g259 , distB75_g259 , alpha75_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch105_g259 = float4( localMidPlanealbedointerpolator75_g259 , 0.0 );
				#else
				float4 staticSwitch105_g259 = lerpResult107_g259;
				#endif
				float4 tex2DNode18 = SAMPLE_TEXTURE2D( _behindGlass_albedo, sampler_behindGlass_albedo, IN.ase_texcoord8.xy );
				float4 lerpResult11 = lerp( staticSwitch105_g259 , tex2DNode18 , tex2DNode18.a);
				float4 tex2DNode19 = SAMPLE_TEXTURE2D( _glasAsset_albedo, sampler_Linear_Repeat, IN.ase_texcoord8.zw );
				float4 lerpResult13 = lerp( lerpResult11 , tex2DNode19 , tex2DNode19.a);
				float4 tex2DNode43 = SAMPLE_TEXTURE2D( _frontOfGlass_albedo, sampler_Linear_Repeat, IN.ase_texcoord9.xy );
				float4 lerpResult40 = lerp( lerpResult13 , tex2DNode43 , tex2DNode43.a);
				
				float4 color27 = IsGammaSpace() ? float4(0,0,1,1) : float4(0,0,1,1);
				float3 tex2DNode25 = UnpackNormalScale( SAMPLE_TEXTURE2D( _glasAsset_normal, sampler_Linear_Repeat, IN.ase_texcoord8.zw ), 1.0f );
				float3 appendResult29 = (float3(tex2DNode25.r , -tex2DNode25.g , tex2DNode25.b));
				float4 lerpResult26 = lerp( color27 , float4( appendResult29 , 0.0 ) , 0.01);
				float3 tex2DNode46 = UnpackNormalScale( SAMPLE_TEXTURE2D( _frontOfGlass_normal, sampler_Linear_Repeat, IN.ase_texcoord9.xy ), 1.0f );
				float3 appendResult31 = (float3(tex2DNode46.r , -tex2DNode46.g , tex2DNode46.b));
				float4 tex2DNode45 = SAMPLE_TEXTURE2D( _frontOfGlass_mrmao, sampler_Linear_Repeat, IN.ase_texcoord9.xy );
				float4 lerpResult37 = lerp( lerpResult26 , float4( appendResult31 , 0.0 ) , tex2DNode45.a);
				
				float4 tex2DNode46_g259 = SAMPLE_TEXTURE2D( _Interior_emission, sampler_Interior_emission, staticSwitch101_g259 );
				float4 lerpResult108_g259 = lerp( tex2DNode46_g259 , tex2DNode62_g259 , tex2DNode62_g259.a);
				float3 colorA77_g259 = tex2DNode46_g259.rgb;
				float3 colorB77_g259 = tex2DNode62_g259.rgb;
				float distA77_g259 = temp_output_246_1_g259;
				float distB77_g259 = temp_output_246_3_g259;
				float alpha77_g259 = tex2DNode62_g259.a;
				float3 localMidPlaneEmissioninterpolator77_g259 = MidPlaneEmissioninterpolator77_g259( colorA77_g259 , colorB77_g259 , distA77_g259 , distB77_g259 , alpha77_g259 );
				#ifdef _ENABLEINTERIOR_ON
				float4 staticSwitch106_g259 = float4( localMidPlaneEmissioninterpolator77_g259 , 0.0 );
				#else
				float4 staticSwitch106_g259 = lerpResult108_g259;
				#endif
				float4 lerpResult17 = lerp( staticSwitch106_g259 , SAMPLE_TEXTURE2D( _behindGlass_emission, sampler_behindGlass_emission, IN.ase_texcoord8.xy ) , tex2DNode18.a);
				float4 lerpResult12 = lerp( lerpResult17 , SAMPLE_TEXTURE2D( _frontOfGlass_emission, sampler_Linear_Repeat, IN.ase_texcoord9.xy ) , tex2DNode43.a);
				
				float4 tex2DNode24 = SAMPLE_TEXTURE2D( _glasAsset_mrmao, sampler_Linear_Repeat, IN.ase_texcoord8.zw );
				float lerpResult14 = lerp( tex2DNode24.r , tex2DNode45.r , tex2DNode43.a);
				
				float lerpResult15 = lerp( ( 1.0 - tex2DNode24.g ) , ( 1.0 - tex2DNode45.g ) , tex2DNode43.a);
				
				float lerpResult41 = lerp( 1.0 , tex2DNode45.b , tex2DNode43.a);
				

				float3 BaseColor = lerpResult40.rgb;
				float3 Normal = lerpResult37.rgb;
				float3 Emission = ( lerpResult12 * _emissionMultiplier * SAMPLE_TEXTURE2D( _emissionMultiplierTexture, sampler_emissionMultiplierTexture, IN.ase_texcoord9.zw ) ).rgb;
				float3 Specular = 0.5;
				float Metallic = lerpResult14;
				float Smoothness = lerpResult15;
				float Occlusion = lerpResult41;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = 0; // we don't apply fog in the gbuffer pass
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				BRDFData brdfData;
				InitializeBRDFData( BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);
				half4 color;
				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	Fallback Off
	
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.WireNode;10;-222.7813,-680.0966;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;11;-419.7062,267.9061;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;12;297.4254,529.6744;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;13;26.48898,219.1712;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;14;228.7221,-995.4286;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;15;219.4755,-722.6496;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;16;-488.4584,-329.3588;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;17;-346.5943,698.2668;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;-1391.4,557.9639;Inherit;True;Property;_behindGlass_albedo;behindGlass_albedo;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-1805.676,-372.8093;Inherit;True;Property;_glasAsset_albedo;glasAsset_albedo;24;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;638.8314,716.3009;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;21;-1387.483,810.0397;Inherit;True;Property;_behindGlass_emission;behindGlass_emission;23;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;22;-204.9438,482.3187;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;24;-1805.676,-154.5156;Inherit;True;Property;_glasAsset_mrmao;glasAsset_mrmao;25;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-1809.407,67.50833;Inherit;True;Property;_glasAsset_normal;glasAsset_normal;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;26;-1012.315,34.42413;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NegateNode;28;-1485.952,111.3345;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-1347.746,68.22446;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;30;-1241.236,-703.9537;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-1105.566,-758.4756;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;32;-2227.336,-221.1454;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;33;-2991.373,1023.981;Inherit;False;6;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;34;-2992.397,1249.215;Inherit;False;5;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;35;-2703.617,1254.149;Inherit;True;Property;_MidPlane_albedo;MidPlane_albedo;18;0;Create;False;0;0;0;False;0;False;-1;None;None;True;5;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-2731.212,1032.53;Inherit;True;Property;_Interior_Albedo;Interior_Albedo;17;0;Create;False;0;0;0;False;0;False;-1;None;None;True;6;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;37;412.216,-183.2258;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;38;-1044.156,-939.6627;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;39;-1110.431,-834.1336;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;41;746.4913,285.7977;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;-235.1197,1168.974;Inherit;True;Property;_emissionMultiplierTexture;emissionMultiplierTexture;31;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-1584.654,-1479.79;Inherit;True;Property;_frontOfGlass_albedo;frontOfGlass_albedo;27;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;44;-1578.055,-1254.77;Inherit;True;Property;_frontOfGlass_emission;frontOfGlass_emission;28;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;45;-1572.458,-1043.94;Inherit;True;Property;_frontOfGlass_mrmao;frontOfGlass_mrmao;29;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;46;-1580.882,-810.7206;Inherit;True;Property;_frontOfGlass_normal;frontOfGlass_normal;30;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerStateNode;47;-2197.008,-1135.316;Inherit;False;0;0;0;1;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;48;-638.4274,1228.942;Inherit;False;7;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;49;-1653.419,726.412;Inherit;False;4;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;50;-2221.956,-1320.923;Inherit;False;3;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;-1507.95,223.6882;Inherit;False;Constant;_normal_reduction;normal_reduction;19;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;88.08493,896.845;Inherit;False;Property;_emissionMultiplier;emissionMultiplier;32;0;Create;True;0;0;0;False;0;False;0;1000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;515.3871,305.1465;Inherit;False;Constant;_Float0;Float 0;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;576.9621,60.90749;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1370.3,-30.97674;Float;False;True;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;vrbnInteriorMappingURP;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;7;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;38;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,;0;Translucency;0;0;  Translucency Strength;1,False,;0;  Normal Distortion;0.5,False,;0;  Scattering;2,False,;0;  Direct;0.9,False,;0;  Ambient;0.1,False,;0;  Shadow;0.5,False,;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;8;False;True;True;True;True;True;True;True;False;;True;0
Node;AmplifyShaderEditor.SamplerStateNode;64;-2165.247,-30.92801;Inherit;False;0;0;0;1;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.ColorNode;27;-1242.493,-74.09265;Inherit;False;Constant;_Color0;Color 0;24;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-2624.404,22.62103;Inherit;False;Property;_Width;Width;13;1;[HideInInspector];Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2576.389,144.4984;Inherit;False;Property;_Height;Height;14;1;[HideInInspector];Create;True;0;0;0;False;0;False;3;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;53;-2603.233,247.2173;Inherit;False;Property;_UvsAspectRatio;UvsAspectRatio;16;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.245,0.184,1;0.2828757,0.2828753,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;54;-2604.819,404.6443;Inherit;False;Property;_DepthTest;DepthTest;19;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2595.607,489.1001;Inherit;False;Property;_Depth;Depth;15;1;[HideInInspector];Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-2620.175,583.3043;Inherit;False;Property;_MidPlaneDepth;MidPlaneDepth;20;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-2630.924,664.1531;Inherit;False;Property;_MidPlaneExtraAdjust;MidPlaneExtraAdjust;21;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-2813.942,767.9208;Inherit;False;Property;_RoomType;RoomType;12;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;68;-2087.086,324.0424;Inherit;False;New 6UV InteriorMaste;0;;259;2fe64262a9c2c0d4f9bcd958930337cc;0;13;53;FLOAT;0;False;54;FLOAT;0;False;55;FLOAT3;0,0,0;False;234;FLOAT3;0,0,0;False;56;FLOAT;0;False;57;FLOAT;0;False;58;FLOAT;0;False;59;FLOAT;0;False;60;FLOAT;0;False;146;FLOAT;0;False;165;FLOAT;0;False;167;FLOAT2;0,0;False;168;FLOAT2;0,0;False;2;COLOR;50;COLOR;51
WireConnection;10;0;43;4
WireConnection;11;0;68;50
WireConnection;11;1;18;0
WireConnection;11;2;18;4
WireConnection;12;0;17;0
WireConnection;12;1;22;0
WireConnection;12;2;10;0
WireConnection;13;0;11;0
WireConnection;13;1;19;0
WireConnection;13;2;19;4
WireConnection;14;0;24;1
WireConnection;14;1;45;1
WireConnection;14;2;10;0
WireConnection;15;0;16;0
WireConnection;15;1;38;0
WireConnection;15;2;10;0
WireConnection;16;0;24;2
WireConnection;17;0;68;51
WireConnection;17;1;21;0
WireConnection;17;2;18;4
WireConnection;18;1;49;0
WireConnection;19;1;32;0
WireConnection;19;7;64;0
WireConnection;20;0;12;0
WireConnection;20;1;60;0
WireConnection;20;2;42;0
WireConnection;21;1;49;0
WireConnection;22;0;44;0
WireConnection;24;1;32;0
WireConnection;24;7;64;0
WireConnection;25;1;32;0
WireConnection;25;7;64;0
WireConnection;26;0;27;0
WireConnection;26;1;29;0
WireConnection;26;2;59;0
WireConnection;28;0;25;2
WireConnection;29;0;25;1
WireConnection;29;1;28;0
WireConnection;29;2;25;3
WireConnection;30;0;46;2
WireConnection;31;0;46;1
WireConnection;31;1;30;0
WireConnection;31;2;46;3
WireConnection;35;1;34;0
WireConnection;36;1;33;0
WireConnection;37;0;26;0
WireConnection;37;1;31;0
WireConnection;37;2;45;4
WireConnection;38;0;45;2
WireConnection;39;0;45;3
WireConnection;41;0;61;0
WireConnection;41;1;39;0
WireConnection;41;2;10;0
WireConnection;42;1;48;0
WireConnection;43;1;50;0
WireConnection;43;7;47;0
WireConnection;44;1;50;0
WireConnection;44;7;47;0
WireConnection;45;1;50;0
WireConnection;45;7;47;0
WireConnection;46;1;50;0
WireConnection;46;7;47;0
WireConnection;40;0;13;0
WireConnection;40;1;43;0
WireConnection;40;2;10;0
WireConnection;1;0;40;0
WireConnection;1;1;37;0
WireConnection;1;2;20;0
WireConnection;1;3;14;0
WireConnection;1;4;15;0
WireConnection;1;5;41;0
WireConnection;68;53;51;0
WireConnection;68;54;52;0
WireConnection;68;55;53;0
WireConnection;68;56;54;0
WireConnection;68;57;55;0
WireConnection;68;58;56;0
WireConnection;68;59;57;0
WireConnection;68;60;58;0
WireConnection;68;146;36;4
WireConnection;68;165;35;4
WireConnection;68;167;33;0
WireConnection;68;168;34;0
ASEEND*/
//CHKSM=F2B9AE03E08855BFF19FCD4C0CF77FA8FAE40716