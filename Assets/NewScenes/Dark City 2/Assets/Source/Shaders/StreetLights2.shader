// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/StreetLights2"
{
	Properties
	{
		_Emission("Emission", 2D) = "white" {}
		_Specular("Specular", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Color1("Color 1", Color) = (0,0,0,0)
		_Color2("Color 2", Color) = (0,0,0,0)
		_Background("Background", Color) = (0,0,0,0)
		_AlbedoPower("Albedo Power", Range( 0 , 1)) = 0
		_EmissionPower("Emission Power", Range( 0 , 1)) = 0
		_SlideSpeed("Slide Speed", Range( 0 , 1)) = 0
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

		uniform float4 _Background;
		uniform float4 _Color1;
		uniform sampler2D _Emission;
		uniform float _SlideSpeed;
		uniform float4 _Color2;
		uniform float _AlbedoPower;
		uniform float _EmissionPower;
		uniform float _Specular;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 appendResult320 = (float2((-0.5 + (_SlideSpeed - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) , 0.0));
			float2 uv_TexCoord311 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner312 = ( uv_TexCoord311 + _Time.y * appendResult320);
			float4 lerpResult305 = lerp( _Background , _Color1 , tex2D( _Emission, panner312 ).r);
			float4 tex2DNode308 = tex2D( _Emission, uv_TexCoord311 );
			float4 lerpResult309 = lerp( _Background , _Color2 , tex2DNode308.g);
			float4 lerpResult310 = lerp( lerpResult305 , lerpResult309 , tex2DNode308.b);
			o.Albedo = ( lerpResult310 * _AlbedoPower ).rgb;
			o.Emission = ( lerpResult310 * _EmissionPower ).rgb;
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
191;202;1375;567;1709.914;715.9389;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;317;-1381.39,-582.4716;Float;False;Property;_SlideSpeed;Slide Speed;8;0;Create;True;0;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;321;-1104.604,-600.1246;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.5;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;320;-836.0235,-632.9975;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;315;-845.8283,-509.2617;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;311;-1016.399,-750.1694;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;303;-751.3838,-888.309;Float;True;Property;_Emission;Emission;0;0;Create;True;None;57db925238b3ddf41bf13d3a58918cbb;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;312;-602.6306,-669.8906;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;307;67.83746,-415.3464;Float;False;Property;_Color2;Color 2;4;0;Create;True;0,0,0,0;1,0.5172414,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;304;64.34875,-617.7548;Float;False;Property;_Background;Background;5;0;Create;True;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;302;-265.2686,-702.1739;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;306;101.1651,-872.9926;Float;False;Property;_Color1;Color 1;3;0;Create;True;0,0,0,0;0.7352941,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;308;-316.9421,-330.9744;Float;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;305;362.4967,-727.0483;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;309;339.8904,-465.6337;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;323;680.6036,-590.5092;Float;False;Property;_EmissionPower;Emission Power;7;0;Create;True;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;310;582.0493,-712.4183;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;325;623.3932,-902.5886;Float;False;Property;_AlbedoPower;Albedo Power;6;0;Create;True;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;950.083,-720.1877;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;324;974.285,-918.8168;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;216;685.7698,-389.6948;Float;False;Property;_Smoothness;Smoothness;2;0;Create;True;0.5;0.881;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;684.3368,-486.903;Float;False;Property;_Specular;Specular;1;0;Create;True;0.5;0.246;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1178.656,-908.5953;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;MK4/StreetLights2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0.0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;321;0;317;0
WireConnection;320;0;321;0
WireConnection;312;0;311;0
WireConnection;312;2;320;0
WireConnection;312;1;315;2
WireConnection;302;0;303;0
WireConnection;302;1;312;0
WireConnection;308;0;303;0
WireConnection;308;1;311;0
WireConnection;305;0;304;0
WireConnection;305;1;306;0
WireConnection;305;2;302;1
WireConnection;309;0;304;0
WireConnection;309;1;307;0
WireConnection;309;2;308;2
WireConnection;310;0;305;0
WireConnection;310;1;309;0
WireConnection;310;2;308;3
WireConnection;322;0;310;0
WireConnection;322;1;323;0
WireConnection;324;0;310;0
WireConnection;324;1;325;0
WireConnection;0;0;324;0
WireConnection;0;2;322;0
WireConnection;0;3;220;0
WireConnection;0;4;216;0
ASEEND*/
//CHKSM=5D70E5319910BCF9F7B55029473EC6BEA7DB1E00