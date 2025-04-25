// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/StreetLights"
{
	Properties
	{
		_Color("Color", Color) = (0.5807742,0.7100198,0.9632353,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Columns("Columns", Range( 0 , 128)) = 0
		_Rows("Rows", Range( 0 , 128)) = 16
		_MovieSpeed("Movie Speed", Range( 0 , 50)) = 1
		_Specular("Specular", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_EmissionAlbedo("Emission Albedo", Range( 0 , 1)) = 1
		_Distortion("Distortion", 2D) = "black" {}
		_Distort("Distort", Range( 0 , 1)) = 0.1
		_DistortSpeed("Distort Speed", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float _Columns;
		uniform float _Rows;
		uniform float _MovieSpeed;
		uniform sampler2D _Distortion;
		uniform float _DistortSpeed;
		uniform float _Distort;
		uniform float _EmissionAlbedo;
		uniform float _Specular;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_TexCoord258 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles4 = _Columns * _Rows;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset4 = 1.0f / _Columns;
			float fbrowsoffset4 = 1.0f / _Rows;
			// Speed of animation
			float fbspeed4 = _Time[ 1 ] * _MovieSpeed;
			// UV Tiling (col and row offset)
			float2 fbtiling4 = float2(fbcolsoffset4, fbrowsoffset4);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex4 = round( fmod( fbspeed4 + 0.0, fbtotaltiles4) );
			fbcurrenttileindex4 += ( fbcurrenttileindex4 < 0) ? fbtotaltiles4 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox4 = round ( fmod ( fbcurrenttileindex4, _Columns ) );
			// Multiply Offset X by coloffset
			float fboffsetx4 = fblinearindextox4 * fbcolsoffset4;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy4 = round( fmod( ( fbcurrenttileindex4 - fblinearindextox4 ) / _Columns, _Rows ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy4 = (int)(_Rows-1) - fblinearindextoy4;
			// Multiply Offset Y by rowoffset
			float fboffsety4 = fblinearindextoy4 * fbrowsoffset4;
			// UV Offset
			float2 fboffset4 = float2(fboffsetx4, fboffsety4);
			// Flipbook UV
			half2 fbuv4 = uv_TexCoord258 * fbtiling4 + fboffset4;
			// *** END Flipbook UV Animation vars ***
			float temp_output_287_0 = ( _Time.y * _DistortSpeed );
			float2 uv_TexCoord271 = i.uv_texcoord * float2( 0.3,0.3 ) + float2( 0,0 );
			float2 panner273 = ( uv_TexCoord271 + temp_output_287_0 * float2( -3,6 ));
			float2 panner274 = ( uv_TexCoord271 + temp_output_287_0 * float2( 3,4 ));
			float2 panner272 = ( uv_TexCoord271 + temp_output_287_0 * float2( -3.3,-2 ));
			float4 tex2DNode66 = tex2D( _Albedo, ( fbuv4 + ( ( ( tex2D( _Distortion, panner273 ).a * 0.5 ) + ( tex2D( _Distortion, panner274 ).a + tex2D( _Distortion, panner272 ).a ) ) * (0.0 + (_Distort - 0.0) * (0.01 - 0.0) / (1.0 - 0.0)) ) ) );
			o.Albedo = ( _Color * tex2DNode66 ).rgb;
			float4 temp_output_263_0 = ( tex2DNode66 * _EmissionAlbedo );
			o.Emission = temp_output_263_0.rgb;
			float3 temp_cast_2 = (_Specular).xxx;
			o.Specular = temp_cast_2;
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
162;218;1391;815;2543.572;996.7694;1.99381;True;True
Node;AmplifyShaderEditor.TimeNode;281;-2459.438,-573.0873;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;288;-2465.479,-356.7407;Float;False;Property;_DistortSpeed;Distort Speed;10;0;Create;True;0;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;271;-2086.971,-639.2286;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.3,0.3;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;-2174.359,-415.8233;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;272;-1507.43,-603.8416;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-3.3,-2;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;282;-1864.282,-850.8271;Float;True;Property;_Distortion;Distortion;8;0;Create;True;7264660150c07a642a7be33ecbadad56;7264660150c07a642a7be33ecbadad56;False;black;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;274;-1493.723,-773.726;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;3,4;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;273;-1565.016,-211.2551;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-3,6;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;275;-1306.321,-623.1364;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;276;-1344.819,-213.3704;Float;True;Property;_TextureSample1;Texture Sample 1;11;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;277;-1302.964,-815.4236;Float;True;Property;_TextureSample2;Texture Sample 2;11;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-893.4141,-434.6983;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;279;-937.702,-711.5442;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-1167.572,-948.0432;Float;False;Property;_Distort;Distort;9;0;Create;True;0.1;0.29;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;255;-1152.974,-1246.255;Float;False;Property;_Columns;Columns;2;0;Create;True;0;2;0;128;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1155.332,-1078.443;Float;False;Property;_MovieSpeed;Movie Speed;4;0;Create;True;1;1;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-1155.085,-1166.181;Float;False;Property;_Rows;Rows;3;0;Create;True;16;1;0;128;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-1131.194,-1412.979;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;289;-841.3623,-926.4801;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;280;-781.526,-693.0245;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;284;-650.826,-857.2214;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;4;-858.1899,-1244.822;Float;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;8.0;False;2;FLOAT;8.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;286;-532.3095,-1153.138;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;119;449.4766,-1535.839;Float;False;Property;_Color;Color;0;0;Create;True;0.5807742,0.7100198,0.9632353,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-405.1431,-1241.17;Float;True;Property;_Albedo;Albedo;1;0;Create;True;None;ac54dfbeb88db6d4ea273fa92a98c1da;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;266;215.9142,-1040.848;Float;False;Property;_EmissionAlbedo;Emission Albedo;7;0;Create;True;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;292;-237.2436,-1524.988;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;291;-241.5753,-1680.726;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;290;-240.2165,-1600.903;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;-420.4929,-1409.83;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;295;-38.51777,-1514.558;Float;False;4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;-427.8544,-1685.274;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;296;100.8133,-1499.749;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;294;-417.0244,-1503.328;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;708.8938,-1279.815;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;-632.1276,-1602.382;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;20.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;484.4798,-1136.186;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;300;234.1975,-1406.76;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;792.5818,-777.2242;Float;False;Property;_Smoothness;Smoothness;6;0;Create;True;0.5;0.71;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;298;-240.7121,-1431.49;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;269;710.3283,-1158.369;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;220;797.4521,-859.725;Float;False;Property;_Specular;Specular;5;0;Create;True;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1178.656,-908.5953;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;MK4/StreetLights;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0.0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;287;0;281;2
WireConnection;287;1;288;0
WireConnection;272;0;271;0
WireConnection;272;1;287;0
WireConnection;274;0;271;0
WireConnection;274;1;287;0
WireConnection;273;0;271;0
WireConnection;273;1;287;0
WireConnection;275;0;282;0
WireConnection;275;1;272;0
WireConnection;276;0;282;0
WireConnection;276;1;273;0
WireConnection;277;0;282;0
WireConnection;277;1;274;0
WireConnection;278;0;276;4
WireConnection;279;0;277;4
WireConnection;279;1;275;4
WireConnection;289;0;285;0
WireConnection;280;0;278;0
WireConnection;280;1;279;0
WireConnection;284;0;280;0
WireConnection;284;1;289;0
WireConnection;4;0;258;0
WireConnection;4;1;255;0
WireConnection;4;2;256;0
WireConnection;4;3;6;0
WireConnection;286;0;4;0
WireConnection;286;1;284;0
WireConnection;66;1;286;0
WireConnection;292;0;294;0
WireConnection;291;0;293;0
WireConnection;290;0;297;0
WireConnection;299;0;297;0
WireConnection;295;0;291;0
WireConnection;295;1;290;0
WireConnection;295;2;292;0
WireConnection;295;3;298;0
WireConnection;293;0;297;0
WireConnection;296;0;295;0
WireConnection;294;0;297;0
WireConnection;121;0;119;0
WireConnection;121;1;66;0
WireConnection;297;0;281;2
WireConnection;263;0;66;0
WireConnection;263;1;266;0
WireConnection;300;0;296;0
WireConnection;298;0;299;0
WireConnection;269;0;300;0
WireConnection;269;1;263;0
WireConnection;0;0;121;0
WireConnection;0;2;263;0
WireConnection;0;3;220;0
WireConnection;0;4;216;0
ASEEND*/
//CHKSM=6D73955A0063C58ADA4FB1F3F6BB3E41DB24281D