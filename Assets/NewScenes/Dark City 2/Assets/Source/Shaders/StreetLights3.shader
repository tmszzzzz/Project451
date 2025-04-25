// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/StreetLights3"
{
	Properties
	{
		_Emission("Emission", 2D) = "white" {}
		_Texture0("Texture 0", 2D) = "black" {}
		_Specular("Specular", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Color1("Color 1", Color) = (1,1,1,0)
		_Background("Background", Color) = (0,0,0,0)
		_AlbedoPower("Albedo Power", Range( 0 , 1)) = 0
		_EmissionPower("Emission Power", Range( 0 , 1)) = 0
		_Distort("Distort", Range( 0 , 1)) = 0.35
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
		uniform sampler2D _Texture0;
		uniform sampler2D _Emission;
		uniform float _Distort;
		uniform float _AlbedoPower;
		uniform float _EmissionPower;
		uniform float _Specular;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 panner337 = ( _Time.y * float2( 0,0.5 ) + i.uv_texcoord);
			float2 panner360 = ( 1.0 * _Time.y * float2( 7,9 ) + i.uv_texcoord);
			float temp_output_362_0 = ( _Time.w * 20.0 );
			float clampResult372 = clamp( ( sin( ( temp_output_362_0 * 0.7 ) ) + sin( temp_output_362_0 ) + sin( ( temp_output_362_0 * 1.3 ) ) + sin( ( temp_output_362_0 * 2.5 ) ) ) , 0.0 , 1.0 );
			float2 temp_output_361_0 = ( i.uv_texcoord + ( ( tex2D( _Texture0, panner360 ).b * clampResult372 ) * _Distort ) );
			float4 tex2DNode308 = tex2D( _Emission, temp_output_361_0 );
			float2 panner312 = ( _Time.y * float2( 0,0.2 ) + temp_output_361_0);
			float2 panner331 = ( _Time.y * float2( -0.2,0 ) + temp_output_361_0);
			float temp_output_343_0 = ( _Time.y * 20.0 );
			float clampResult349 = clamp( ( sin( ( temp_output_343_0 * 0.7 ) ) + sin( temp_output_343_0 ) + sin( ( temp_output_343_0 * 1.3 ) ) + sin( ( temp_output_343_0 * 2.5 ) ) ) , 0.7 , 1.0 );
			float4 lerpResult335 = lerp( _Background , _Color1 , ( ( tex2D( _Texture0, panner337 ).r + ( ( ( tex2DNode308.g * tex2D( _Emission, panner312 ).a ) + ( tex2DNode308.b * tex2D( _Emission, panner331 ).a ) ) + tex2DNode308.r ) ) + (0.0 + (( clampResult349 * tex2D( _Texture0, i.uv_texcoord ).g ) - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) ));
			o.Albedo = ( lerpResult335 * _AlbedoPower ).rgb;
			o.Emission = ( lerpResult335 * _EmissionPower ).rgb;
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
Version=16100
320;318;1353;556;2122.754;296.2015;1.626415;True;True
Node;AmplifyShaderEditor.TimeNode;373;-2272.373,145.1848;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;362;-2068.26,232.3546;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;363;-1853.156,331.4087;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;365;-1863.986,149.4627;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;364;-1856.625,424.9066;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;359;-2070.463,-92.18007;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;366;-1676.844,403.2466;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;369;-1676.348,233.8337;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;368;-1673.375,309.7485;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;367;-1677.707,154.0108;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;370;-1474.65,286.8183;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;360;-1741.11,-46.01112;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;7,9;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;336;-1682.933,-414.9448;Float;True;Property;_Texture0;Texture 0;1;0;Create;True;0;0;False;0;9d8591398e82f5f49a8dbfd8625fa822;9d8591398e82f5f49a8dbfd8625fa822;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;358;-1539.884,-103.7258;Float;True;Property;_TextureSample5;Texture Sample 5;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;372;-1320.38,291.8666;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-1124.845,244.8962;Float;False;Property;_Distort;Distort;8;0;Create;True;0;0;False;0;0.35;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;371;-1182.785,66.67143;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;315;-927.661,-280.0968;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;374;-1013.074,46.10257;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;311;-1259.455,-850.2502;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;343;-638.699,269.0811;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;361;-895.5206,-875.6347;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;331;-585.1158,-508.9855;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;312;-571.993,-694.0075;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;352;-434.4254,186.1892;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;303;-838.1248,-1136.365;Float;True;Property;_Emission;Emission;0;0;Create;True;0;0;False;0;9c12171962006f64fb55b7fa2a583f24;bde6c1c7fd22de1409f3fcafc8221ff7;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;350;-423.5954,368.135;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;346;-427.0639,461.6329;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;348;-248.1461,190.7373;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;330;-317.3147,-577.1044;Float;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;302;-312.5336,-795.0953;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;308;-297.4555,-1035.902;Float;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;345;-246.7873,270.5602;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;347;-243.8144,346.475;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;344;-247.2829,439.9729;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;326;115.9396,-908.3363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;351;-45.08864,356.9051;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;92.6304,-657.3023;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;333;224.9297,-769.5903;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;349;94.24246,371.714;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.7;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;356;-243.4718,-92.30885;Float;True;Property;_TextureSample4;Texture Sample 4;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;337;-554.1598,-105.7197;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;338;-226.7079,-299.1002;Float;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;333.1997,-7.345862;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;334;413.1037,-794.2369;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;339;564.325,-780.2708;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;353;599.3132,9.19714;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;304;479.1469,-1144.242;Float;False;Property;_Background;Background;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;306;434.4192,-1011.991;Float;False;Property;_Color1;Color 1;4;0;Create;True;0;0;False;0;1,1,1,0;0,0.5815589,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;357;775.0073,-776.3047;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;335;991.6193,-852.8586;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;325;889.8395,-1020.572;Float;False;Property;_AlbedoPower;Albedo Power;6;0;Create;True;0;0;False;0;0;0.277;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;323;871.088,-617.338;Float;False;Property;_EmissionPower;Emission Power;7;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;324;1177.695,-986.512;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;1140.568,-747.0165;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;220;874.8212,-513.7319;Float;False;Property;_Specular;Specular;2;0;Create;True;0;0;False;0;0.5;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;876.2542,-416.5237;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0.5;0.83;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1369.141,-935.4241;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;MK4/StreetLights3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;362;0;373;4
WireConnection;363;0;362;0
WireConnection;365;0;362;0
WireConnection;364;0;362;0
WireConnection;366;0;364;0
WireConnection;369;0;362;0
WireConnection;368;0;363;0
WireConnection;367;0;365;0
WireConnection;370;0;367;0
WireConnection;370;1;369;0
WireConnection;370;2;368;0
WireConnection;370;3;366;0
WireConnection;360;0;359;0
WireConnection;358;0;336;0
WireConnection;358;1;360;0
WireConnection;372;0;370;0
WireConnection;371;0;358;3
WireConnection;371;1;372;0
WireConnection;374;0;371;0
WireConnection;374;1;375;0
WireConnection;343;0;315;2
WireConnection;361;0;311;0
WireConnection;361;1;374;0
WireConnection;331;0;361;0
WireConnection;331;1;315;2
WireConnection;312;0;361;0
WireConnection;312;1;315;2
WireConnection;352;0;343;0
WireConnection;350;0;343;0
WireConnection;346;0;343;0
WireConnection;348;0;352;0
WireConnection;330;0;303;0
WireConnection;330;1;331;0
WireConnection;302;0;303;0
WireConnection;302;1;312;0
WireConnection;308;0;303;0
WireConnection;308;1;361;0
WireConnection;345;0;343;0
WireConnection;347;0;350;0
WireConnection;344;0;346;0
WireConnection;326;0;308;2
WireConnection;326;1;302;4
WireConnection;351;0;348;0
WireConnection;351;1;345;0
WireConnection;351;2;347;0
WireConnection;351;3;344;0
WireConnection;332;0;308;3
WireConnection;332;1;330;4
WireConnection;333;0;326;0
WireConnection;333;1;332;0
WireConnection;349;0;351;0
WireConnection;356;0;336;0
WireConnection;356;1;311;0
WireConnection;337;0;311;0
WireConnection;337;1;315;2
WireConnection;338;0;336;0
WireConnection;338;1;337;0
WireConnection;354;0;349;0
WireConnection;354;1;356;2
WireConnection;334;0;333;0
WireConnection;334;1;308;1
WireConnection;339;0;338;1
WireConnection;339;1;334;0
WireConnection;353;0;354;0
WireConnection;357;0;339;0
WireConnection;357;1;353;0
WireConnection;335;0;304;0
WireConnection;335;1;306;0
WireConnection;335;2;357;0
WireConnection;324;0;335;0
WireConnection;324;1;325;0
WireConnection;322;0;335;0
WireConnection;322;1;323;0
WireConnection;0;0;324;0
WireConnection;0;2;322;0
WireConnection;0;3;220;0
WireConnection;0;4;216;0
ASEEND*/
//CHKSM=85A631EE39A15AA9EC1771A59EBF99E8ADC0E5E9