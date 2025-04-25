// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/Billboards_tilex4"
{
	Properties
	{
		_TextureSample6("Texture Sample 6", 2D) = "gray" {}
		_AlbedoPower("Albedo Power", Float) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_EmissionMultiply("Emission Multiply", Color) = (0,0,0,0)
		_MaskTexture("Mask Texture", 2D) = "gray" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MaskTexture;
		uniform sampler2D _TextureSample6;
		uniform float _AlbedoPower;
		uniform float4 _EmissionMultiply;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord368 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 appendResult436 = (float2(( uv_TexCoord368.x * 0.5 ) , uv_TexCoord368.y));
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles392 = 4.0 * 1.0;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset392 = 1.0f / 4.0;
			float fbrowsoffset392 = 1.0f / 1.0;
			// Speed of animation
			float fbspeed392 = _Time.y * 0.25;
			// UV Tiling (col and row offset)
			float2 fbtiling392 = float2(fbcolsoffset392, fbrowsoffset392);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex392 = round( fmod( fbspeed392 + 0.0, fbtotaltiles392) );
			fbcurrenttileindex392 += ( fbcurrenttileindex392 < 0) ? fbtotaltiles392 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox392 = round ( fmod ( fbcurrenttileindex392, 4.0 ) );
			// Multiply Offset X by coloffset
			float fboffsetx392 = fblinearindextox392 * fbcolsoffset392;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy392 = round( fmod( ( fbcurrenttileindex392 - fblinearindextox392 ) / 4.0, 1.0 ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy392 = (int)(1.0-1) - fblinearindextoy392;
			// Multiply Offset Y by rowoffset
			float fboffsety392 = fblinearindextoy392 * fbrowsoffset392;
			// UV Offset
			float2 fboffset392 = float2(fboffsetx392, fboffsety392);
			// Flipbook UV
			half2 fbuv392 = uv_TexCoord368 * fbtiling392 + fboffset392;
			// *** END Flipbook UV Animation vars ***
			float2 appendResult420 = (float2(( uv_TexCoord368.x * 0.0625 ) , uv_TexCoord368.y));
			float2 panner418 = ( appendResult420 + _Time.y * float2( 0.25,0 ));
			float4 tex2DNode369 = tex2D( _TextureSample6, ( fbuv392 + tex2D( _MaskTexture, panner418 ).g ) );
			o.Albedo = ( ( tex2D( _MaskTexture, ( appendResult436 * float2( 2,2 ) ) ).r + tex2DNode369 ) * _AlbedoPower ).rgb;
			o.Emission = ( _EmissionMultiply * tex2DNode369 ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
186;423;1357;671;-1533.042;1094.439;2.16399;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;368;1733.997,88.1252;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;419;2128.122,382.7834;Float;False;FLOAT2;1;0;FLOAT2;0.0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;421;2451.38,356.7175;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0625;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;437;2174.022,-603.8766;Float;False;FLOAT2;1;0;FLOAT2;0.0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TimeNode;379;2325.608,79.629;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;420;2634.903,395.3997;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;434;2476.573,-648.3516;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;418;2883.117,315.3855;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.25,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;427;2834.899,97.02029;Float;True;Property;_MaskTexture;Mask Texture;7;0;Create;True;None;220eb875928304d47ae47181662831d6;False;gray;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;436;2661.541,-604.4078;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;395;3093.232,284.6966;Float;True;Property;_Mask;Mask;6;0;Create;True;None;220eb875928304d47ae47181662831d6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;392;2902.874,-109.083;Float;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;4.0;False;2;FLOAT;1.0;False;3;FLOAT;0.25;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;432;2760.992,-710.9677;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;422;3366.165,11.82732;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;428;3056.541,-692.2167;Float;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;369;3486.932,-42.78382;Float;True;Property;_TextureSample6;Texture Sample 6;0;0;Create;True;83d0f357a484d214fbb7ee5693061487;83d0f357a484d214fbb7ee5693061487;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;426;3904.213,-313.8653;Float;False;Property;_AlbedoPower;Albedo Power;1;0;Create;True;0;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;433;3765.019,-625.6966;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;423;3767.477,221.421;Float;False;Property;_EmissionMultiply;Emission Multiply;5;0;Create;True;0,0,0,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;435;2478.738,-425.4604;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;332;3163.266,-320.1281;Float;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;333;3161.966,-208.3281;Float;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;425;4175.465,-302.9426;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;424;4156.138,135.7652;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4411.476,97.59846;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;MK4/Billboards_tilex4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;False;0;Masked;0.28;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;1;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;419;0;368;0
WireConnection;421;0;419;0
WireConnection;437;0;368;0
WireConnection;420;0;421;0
WireConnection;420;1;419;1
WireConnection;434;0;437;0
WireConnection;418;0;420;0
WireConnection;418;1;379;2
WireConnection;436;0;434;0
WireConnection;436;1;437;1
WireConnection;395;0;427;0
WireConnection;395;1;418;0
WireConnection;392;0;368;0
WireConnection;392;5;379;2
WireConnection;432;0;436;0
WireConnection;422;0;392;0
WireConnection;422;1;395;2
WireConnection;428;0;427;0
WireConnection;428;1;432;0
WireConnection;369;1;422;0
WireConnection;433;0;428;1
WireConnection;433;1;369;0
WireConnection;435;1;437;1
WireConnection;425;0;433;0
WireConnection;425;1;426;0
WireConnection;424;0;423;0
WireConnection;424;1;369;0
WireConnection;0;0;425;0
WireConnection;0;2;424;0
WireConnection;0;3;332;0
WireConnection;0;4;333;0
ASEEND*/
//CHKSM=698F859DA8E8C22AE3C01D923EB68F303ED9BB3B