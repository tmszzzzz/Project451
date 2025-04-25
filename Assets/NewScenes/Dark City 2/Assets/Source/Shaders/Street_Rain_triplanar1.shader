// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/Rain_triplanar_street1"
{
	Properties
	{
		_Color("Color", Color) = (0.5807742,0.7100198,0.9632353,0)
		_Albedo("Albedo", 2D) = "gray" {}
		_UVTiling("UV Tiling", Range( 0 , 1)) = 0.2
		_Normalmap("Normalmap", 2D) = "bump" {}
		_SpecularSmoothness("Specular Smoothness", 2D) = "gray" {}
		_Specular("Specular", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_RainMask("Rain Mask", Range( 0 , 1)) = 0.5
		_RainDropsNormal("RainDrops Normal", 2D) = "bump" {}
		_Raindropsint("Raindrops  int", Range( 0 , 5)) = 0
		_RaindropsUVTile("Raindrops UV Tile", Range( 0 , 1)) = 0
		_RainSpeed("Rain Speed", Range( 0 , 50)) = 0
		_WaveNormal("Wave Normal", 2D) = "bump" {}
		_WaveNormalint("Wave Normal int", Range( 0 , 5)) = 0
		_WaveSpeed("Wave Speed", Range( 0 , 1)) = 0
		_WaveUVTile("Wave UV Tile", Range( 0 , 1)) = 0
		_RoadSymbols("Road Symbols", 2D) = "black" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _Normalmap;
		uniform float _UVTiling;
		uniform float _Raindropsint;
		uniform sampler2D _RainDropsNormal;
		uniform float _RaindropsUVTile;
		uniform float _RainSpeed;
		uniform sampler2D _WaveNormal;
		uniform float _WaveNormalint;
		uniform float _WaveSpeed;
		uniform float _WaveUVTile;
		uniform sampler2D _SpecularSmoothness;
		uniform float _RainMask;
		uniform sampler2D _RoadSymbols;
		uniform float4 _RoadSymbols_ST;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float _Specular;
		uniform float _Smoothness;


		inline float3 TriplanarSamplingSNF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float temp_output_366_0 = (0.01 + (_UVTiling - 0.0) * (2.0 - 0.01) / (1.0 - 0.0));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar362 = TriplanarSamplingSNF( _Normalmap, ase_worldPos, ase_worldNormal, 1.0, temp_output_366_0, 0 );
			float3 tanTriplanarNormal362 = mul( ase_worldToTangent, triplanar362 );
			float temp_output_254_0 = (0.05 + (_RaindropsUVTile - 0.0) * (3.0 - 0.05) / (1.0 - 0.0));
			float2 appendResult75 = (float2(frac( ( ase_worldPos.x * temp_output_254_0 ) ) , frac( ( ase_worldPos.z * temp_output_254_0 ) )));
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles4 = 8.0 * 8.0;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset4 = 1.0f / 8.0;
			float fbrowsoffset4 = 1.0f / 8.0;
			// Speed of animation
			float fbspeed4 = _Time[ 1 ] * _RainSpeed;
			// UV Tiling (col and row offset)
			float2 fbtiling4 = float2(fbcolsoffset4, fbrowsoffset4);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex4 = round( fmod( fbspeed4 + 0.0, fbtotaltiles4) );
			fbcurrenttileindex4 += ( fbcurrenttileindex4 < 0) ? fbtotaltiles4 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox4 = round ( fmod ( fbcurrenttileindex4, 8.0 ) );
			// Multiply Offset X by coloffset
			float fboffsetx4 = fblinearindextox4 * fbcolsoffset4;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy4 = round( fmod( ( fbcurrenttileindex4 - fblinearindextox4 ) / 8.0, 8.0 ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy4 = (int)(8.0-1) - fblinearindextoy4;
			// Multiply Offset Y by rowoffset
			float fboffsety4 = fblinearindextoy4 * fbrowsoffset4;
			// UV Offset
			float2 fboffset4 = float2(fboffsetx4, fboffsety4);
			// Flipbook UV
			half2 fbuv4 = appendResult75 * fbtiling4 + fboffset4;
			// *** END Flipbook UV Animation vars ***
			float temp_output_241_0 = ( _Time.y * (0.0 + (_WaveSpeed - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) );
			float2 appendResult183 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner245 = ( appendResult183 + temp_output_241_0 * float2( 0.27,-0.25 ));
			float temp_output_222_0 = (0.05 + (_WaveUVTile - 0.0) * (3.0 - 0.05) / (1.0 - 0.0));
			float2 panner231 = ( appendResult183 + temp_output_241_0 * float2( -0.15,-0.23 ));
			float2 panner91 = ( appendResult183 + temp_output_241_0 * float2( 0.18,0.2 ).x);
			float4 triplanar364 = TriplanarSamplingSF( _SpecularSmoothness, ase_worldPos, ase_worldNormal, 1.0, temp_output_366_0, 0 );
			float clampResult228 = clamp( ((-1.0 + (_RainMask - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (triplanar364.w - 0.0) * (1.0 - (-1.0 + (_RainMask - 0.0) * (1.0 - -1.0) / (1.0 - 0.0))) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float3 lerpResult88 = lerp( tanTriplanarNormal362 , BlendNormals( tanTriplanarNormal362 , ( UnpackScaleNormal( tex2D( _RainDropsNormal, fbuv4 ) ,_Raindropsint ) + ( UnpackScaleNormal( tex2D( _WaveNormal, ( panner245 * temp_output_222_0 ) ) ,_WaveNormalint ) + UnpackScaleNormal( tex2D( _WaveNormal, ( ( panner231 * float2( 0.9,0.9 ) ) * temp_output_222_0 ) ) ,_WaveNormalint ) + UnpackScaleNormal( tex2D( _WaveNormal, ( panner91 * temp_output_222_0 ) ) ,_WaveNormalint ) ) ) ) , clampResult228);
			float3 normalizeResult249 = normalize( lerpResult88 );
			o.Normal = normalizeResult249;
			float2 uv_RoadSymbols = i.uv_texcoord * _RoadSymbols_ST.xy + _RoadSymbols_ST.zw;
			float clampResult375 = clamp( ( (1.0 + (triplanar364.w - 0.0) * (0.2 - 1.0) / (1.0 - 0.0)) * tex2D( _RoadSymbols, uv_RoadSymbols ).r ) , 0.0 , 1.0 );
			float4 lerpResult120 = lerp( float4( 1,1,1,0 ) , _Color , clampResult228);
			float4 triplanar363 = TriplanarSamplingSF( _Albedo, ase_worldPos, ase_worldNormal, 1.0, temp_output_366_0, 0 );
			o.Albedo = ( clampResult375 + ( lerpResult120 * triplanar363 ) ).rgb;
			float clampResult214 = clamp( ((-1.0 + (_Specular - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (triplanar364.x - 0.0) * ((0.0 + (_Specular - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) - (-1.0 + (_Specular - 0.0) * (1.0 - -1.0) / (1.0 - 0.0))) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float3 temp_cast_3 = (clampResult214).xxx;
			o.Specular = temp_cast_3;
			float clampResult212 = clamp( ((-1.0 + (_Smoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (triplanar364.w - 0.0) * (1.0 - (-1.0 + (_Smoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0))) / (1.0 - 0.0)) , 0.0 , 1.0 );
			o.Smoothness = clampResult212;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
538;162;1391;815;1300.444;2114.325;2.341252;True;True
Node;AmplifyShaderEditor.RangedFloatNode;240;-1850.364,1102.581;Float;False;Property;_WaveSpeed;Wave Speed;14;0;Create;True;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;207;-1740.444,-145.1366;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;108;-1607.632,931.8244;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;253;-2043.14,-374.5881;Float;False;Property;_RaindropsUVTile;Raindrops UV Tile;10;0;Create;True;0;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;239;-1562.2,1087.039;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;254;-1752.892,-347.7936;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.05;False;4;FLOAT;3.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;235;-1290.908,706.9866;Float;False;Constant;_WaveUV2;Wave UV2;12;0;Create;True;-0.15,-0.23;-0.15,-0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-1382.023,1016.797;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;183;-1473.6,74.86832;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;231;-1054.722,694.7736;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;246;-1291.862,523.527;Float;False;Constant;_Vector0;Vector 0;12;0;Create;True;0.27,-0.25;-0.15,-0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-1472.912,-184.8229;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-1469.608,-87.20174;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-1351.089,1219.989;Float;False;Property;_WaveUVTile;Wave UV Tile;15;0;Create;True;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;232;-1276.196,860.0075;Float;False;Constant;_WaveUV1;Wave UV1;13;0;Create;True;0.18,0.2;0.1,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FractNode;74;-1207.243,-95.36623;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;222;-1054.351,1125.094;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.05;False;4;FLOAT;3.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;365;-1389.698,-1273.307;Float;False;Property;_UVTiling;UV Tiling;2;0;Create;True;0.2;0.13;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;91;-1038.328,841.2997;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT;0.1;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;73;-1212.723,-176.1976;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;-864.3458,708.8533;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.9,0.9;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;245;-1051.393,550.4917;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;75;-979.0115,-157.8594;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;238;-623.578,384.2714;Float;True;Property;_WaveNormal;Wave Normal;12;0;Create;True;None;20e16ccd543ebdf4fbc4a9bcb13374b1;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TFHCRemapNode;366;-1090.385,-1276.719;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.01;False;4;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;-551.9796,-773.2418;Float;False;Property;_RainMask;Rain Mask;7;0;Create;True;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;234;-725.8802,704.1143;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-852.7296,562.0222;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-843.4626,-74.19863;Float;False;Property;_RainSpeed;Rain Speed;11;0;Create;True;0;33;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-675.1719,834.9395;Float;False;Property;_WaveNormalint;Wave Normal int;13;0;Create;True;0;0.2;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-847.3842,850.6929;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;226;-219.4937,-761.6447;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;364;-835.2589,-1343.186;Float;True;Spherical;World;False;Specular Smoothness;_SpecularSmoothness;gray;4;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Base_Smooth;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;244;-341.8859,523.6908;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;None;24bf0fdf59c5c364f9ce0fae906b6754;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;102;-340.1845,927.4704;Float;True;Property;_WaterNormal;Water Normal;6;0;Create;True;None;24bf0fdf59c5c364f9ce0fae906b6754;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;250;-571.3339,32.2523;Float;False;Property;_Raindropsint;Raindrops  int;9;0;Create;True;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;4;-577.3209,-222.5767;Float;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;8.0;False;2;FLOAT;8.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;230;-341.8411,729.6231;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;None;24bf0fdf59c5c364f9ce0fae906b6754;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;248;335.8223,196.254;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;227;3.859527,-774.0189;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.8;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-135.3428,-38.99204;Float;True;Property;_RainDropsNormal;RainDrops Normal;8;0;Create;True;None;fd09dc3c530e8654dacba5a7ee36cb20;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;367;69.56835,-1922.404;Float;True;Property;_RoadSymbols;Road Symbols;16;0;Create;True;None;9b5b74151dc00fe4983a96ea38e99959;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;362;-834.7076,-1148.32;Float;True;Spherical;World;True;Normalmap;_Normalmap;bump;3;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Base_Normalmap;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;228;184.7062,-764.761;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-505.1732,-559.656;Float;False;Property;_Smoothness;Smoothness;6;0;Create;True;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-209.7496,-1163.032;Float;False;Property;_Specular;Specular;5;0;Create;True;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;381;172.3663,-1719.902;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;1.0;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;461.6678,-33.37104;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;119;183.4844,-1459.574;Float;False;Property;_Color;Color;0;0;Create;True;0.5807742,0.7100198,0.9632353,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;218;113.4896,-1174.304;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;363;-825.4651,-1572.753;Float;True;Spherical;World;False;Albedo;_Albedo;gray;1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Base_Albedo;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;215;-193.0442,-568.278;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;243;526.923,-189.0499;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;378;415.9626,-1720.684;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;219;118.2222,-1019.957;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;120;475.216,-1298.346;Float;False;3;0;COLOR;1,1,1,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;213;316.6418,-1166.876;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;1.0;False;4;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;651.6189,-1273.678;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;211;7.196518,-561.9423;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.8;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;375;577.9767,-1728.337;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;921.1825,-228.6158;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;214;541.0101,-1130.346;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;382;823.8713,-1279.396;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;249;1084.586,-233.8844;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;212;247.9411,-547.5029;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1306.817,-390.812;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;MK4/Rain_triplanar_street1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0.0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;239;0;240;0
WireConnection;254;0;253;0
WireConnection;241;0;108;2
WireConnection;241;1;239;0
WireConnection;183;0;207;1
WireConnection;183;1;207;3
WireConnection;231;0;183;0
WireConnection;231;2;235;0
WireConnection;231;1;241;0
WireConnection;251;0;207;1
WireConnection;251;1;254;0
WireConnection;252;0;207;3
WireConnection;252;1;254;0
WireConnection;74;0;252;0
WireConnection;222;0;223;0
WireConnection;91;0;183;0
WireConnection;91;2;232;0
WireConnection;91;1;241;0
WireConnection;73;0;251;0
WireConnection;236;0;231;0
WireConnection;245;0;183;0
WireConnection;245;2;246;0
WireConnection;245;1;241;0
WireConnection;75;0;73;0
WireConnection;75;1;74;0
WireConnection;366;0;365;0
WireConnection;234;0;236;0
WireConnection;234;1;222;0
WireConnection;247;0;245;0
WireConnection;247;1;222;0
WireConnection;221;0;91;0
WireConnection;221;1;222;0
WireConnection;226;0;229;0
WireConnection;364;3;366;0
WireConnection;244;0;238;0
WireConnection;244;1;247;0
WireConnection;244;5;237;0
WireConnection;102;0;238;0
WireConnection;102;1;221;0
WireConnection;102;5;237;0
WireConnection;4;0;75;0
WireConnection;4;3;6;0
WireConnection;230;0;238;0
WireConnection;230;1;234;0
WireConnection;230;5;237;0
WireConnection;248;0;244;0
WireConnection;248;1;230;0
WireConnection;248;2;102;0
WireConnection;227;0;364;4
WireConnection;227;3;226;0
WireConnection;11;1;4;0
WireConnection;11;5;250;0
WireConnection;362;3;366;0
WireConnection;228;0;227;0
WireConnection;381;0;364;4
WireConnection;126;0;11;0
WireConnection;126;1;248;0
WireConnection;218;0;220;0
WireConnection;363;3;366;0
WireConnection;215;0;216;0
WireConnection;243;0;362;0
WireConnection;243;1;126;0
WireConnection;378;0;381;0
WireConnection;378;1;367;1
WireConnection;219;0;220;0
WireConnection;120;1;119;0
WireConnection;120;2;228;0
WireConnection;213;0;364;1
WireConnection;213;3;218;0
WireConnection;213;4;219;0
WireConnection;121;0;120;0
WireConnection;121;1;363;0
WireConnection;211;0;364;4
WireConnection;211;3;215;0
WireConnection;375;0;378;0
WireConnection;88;0;362;0
WireConnection;88;1;243;0
WireConnection;88;2;228;0
WireConnection;214;0;213;0
WireConnection;382;0;375;0
WireConnection;382;1;121;0
WireConnection;249;0;88;0
WireConnection;212;0;211;0
WireConnection;0;0;382;0
WireConnection;0;1;249;0
WireConnection;0;3;214;0
WireConnection;0;4;212;0
ASEEND*/
//CHKSM=424FEF442E83EB03211CE3DF902EAB056DA907D1