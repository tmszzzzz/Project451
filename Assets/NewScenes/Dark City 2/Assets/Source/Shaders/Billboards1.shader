// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/Billboards"
{
	Properties
	{
		_Logo("Logo", 2D) = "black" {}
		_PannerX("Panner X", Range( 0 , 1)) = 0
		_PannerY("Panner Y", Range( 0 , 1)) = 0
		_DistortPower("Distort Power", Range( 0 , 1)) = 0
		_Distortions("Distortions", 2D) = "white" {}
		_AlbedoColor("Albedo Color", Color) = (0.490566,0.490566,0.490566,0)
		_Background("Background", 2D) = "gray" {}
		_BackgroundEm("Background Em", Range( 0 , 1)) = 0
		_LEDint("LED int", Range( 0 , 1)) = 0
		_LED("LED", 2D) = "white" {}
		_GlitchIntensity("Glitch Intensity", Range( 0 , 1)) = 0
		_LEDGlow("LED Glow", 2D) = "white" {}
		_Glitch1("Glitch1", Color) = (0.5197807,0.4306336,0.9926471,0)
		_Glitch2("Glitch2", Color) = (0.5588235,0.08093307,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
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

		uniform float4 _AlbedoColor;
		uniform sampler2D _Background;
		uniform sampler2D _Logo;
		uniform float4 _Logo_ST;
		uniform sampler2D _Distortions;
		uniform sampler2D _LED;
		uniform float4 _LED_ST;
		uniform float _DistortPower;
		uniform float _GlitchIntensity;
		uniform sampler2D _LEDGlow;
		uniform float4 _Glitch1;
		uniform float4 _Glitch2;
		uniform float _LEDint;
		uniform float _PannerX;
		uniform float _PannerY;
		uniform float _BackgroundEm;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Logo = i.uv_texcoord * _Logo_ST.xy + _Logo_ST.zw;
			float2 uv_LED = i.uv_texcoord * _LED_ST.xy + _LED_ST.zw;
			float4 tex2DNode249 = tex2D( _LED, uv_LED );
			float2 temp_output_272_0 = ( uv_Logo + ( tex2DNode249.a * 0.1 ) );
			float2 panner223 = ( 1.0 * _Time.y * float2( 1,2 ) + temp_output_272_0);
			float2 panner226 = ( 1.0 * _Time.y * float2( -1.8,0.3 ) + temp_output_272_0);
			float2 panner229 = ( 1.0 * _Time.y * float2( 0.8,-1.5 ) + temp_output_272_0);
			float2 panner231 = ( 1.0 * _Time.y * float2( -0.8,-1.5 ) + temp_output_272_0);
			float temp_output_216_0 = ( ( ( ( tex2D( _Distortions, panner223 ).r + tex2D( _Distortions, panner226 ).g ) + tex2D( _Distortions, panner229 ).b ) + tex2D( _Distortions, panner231 ).a ) * (0.0 + (_DistortPower - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) );
			float4 tex2DNode244 = tex2D( _Background, ( uv_Logo + temp_output_216_0 ) );
			o.Albedo = ( _AlbedoColor * tex2DNode244 ).rgb;
			float2 panner267 = ( 1.0 * _Time.y * float2( -6,5 ) + temp_output_272_0);
			float clampResult295 = clamp( ( _GlitchIntensity + tex2D( _LEDGlow, panner267 ).a ) , 0.0 , 1.0 );
			float2 panner257 = ( 1.0 * _Time.y * float2( 2,3 ) + temp_output_272_0);
			float2 panner255 = ( 1.0 * _Time.y * float2( 0,0.6 ) + temp_output_272_0);
			float clampResult280 = clamp( (0.7 + (( _SinTime.w * ( _SinTime.w * 2.5 ) * ( _SinTime.w * 1.3 ) ) - -2.0) * (1.0 - 0.7) / (1.0 - -2.0)) , 0.0 , 1.0 );
			float2 panner263 = ( 1.0 * _Time.y * float2( -5,-2.3 ) + temp_output_272_0);
			float2 appendResult286 = (float2(_PannerX , _PannerY));
			float2 panner219 = ( 1.0 * _Time.y * appendResult286 + uv_Logo);
			float4 clampResult284 = clamp( ( ( ( clampResult295 * ( ( ( _Glitch1 * tex2D( _LEDGlow, panner257 ).g ) + ( ( _Glitch2 * tex2D( _LEDGlow, panner255 ).r ) + ( tex2DNode249 * clampResult280 ) ) ) * _LEDint ) ) + ( tex2D( _LEDGlow, panner263 ).b * tex2D( _Logo, ( panner219 + temp_output_216_0 ) ) ) ) + ( tex2DNode244 * _BackgroundEm ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Emission = clampResult284.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
202;201;1200;752;-2464.005;609.8164;1.143181;True;True
Node;AmplifyShaderEditor.SamplerNode;249;1801.119,85.17075;Float;True;Property;_LED;LED;10;0;Create;True;0;0;False;0;None;18e1965a87226254b8eb7f01462acc78;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;273;-434.681,698.6474;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;213;-313.429,-556.2374;Float;False;0;210;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;272;-179.4507,701.5998;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinTimeNode;275;2220.253,1377.427;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;223;251.7749,285.6696;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;224;478.1371,373.2149;Float;True;Property;_Distortions;Distortions;5;0;Create;True;0;0;False;0;None;3dd9bc17f8d555d47b674afa5665325a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;226;247.9896,414.4718;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1.8,0.3;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;2529.076,1555.208;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;2533.389,1429.933;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;215;783.5212,156.4628;Float;True;Property;_Distort;Distort;4;0;Create;True;0;0;False;0;None;63fe5b98ec4b4364dbbb1da0dce1ce3c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;225;773.539,364.8753;Float;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;False;0;None;63fe5b98ec4b4364dbbb1da0dce1ce3c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;229;240.6166,550.2777;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.8,-1.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;2707.098,1334.953;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;255;1347.064,950.5872;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.6;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;253;1344.555,762.0715;Float;True;Property;_LEDGlow;LED Glow;12;0;Create;True;0;0;False;0;None;27642ca2a9fa91649812bb3bade99fd6;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;228;768.9166,564.8228;Float;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;False;0;None;63fe5b98ec4b4364dbbb1da0dce1ce3c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;227;1170.35,339.8283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;283;2919.491,1128.174;Float;False;5;0;FLOAT;0;False;1;FLOAT;-2;False;2;FLOAT;1;False;3;FLOAT;0.7;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;231;255.0305,679.7548;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.8,-1.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;257;1350.547,1157.059;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,3;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;254;1666.207,769.5079;Float;True;Property;_TextureSample4;Texture Sample 4;13;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;265;1733.723,600.1031;Float;False;Property;_Glitch2;Glitch2;14;0;Create;True;0;0;False;0;0.5588235,0.08093307,0,0;0,0.3158722,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;287;900.2725,-330.0593;Float;False;Property;_PannerX;Panner X;2;0;Create;True;0;0;False;0;0;0.029;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;230;1345.318,417.1234;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;280;1912.744,323.019;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;896.0468,-243.4251;Float;False;Property;_PannerY;Panner Y;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;217;1006.36,55.72845;Float;False;Property;_DistortPower;Distort Power;4;0;Create;True;0;0;False;0;0;0.029;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;232;766.8814,785.6386;Float;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;False;0;None;63fe5b98ec4b4364dbbb1da0dce1ce3c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;266;2090.903,867.355;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;261;1963.257,1026.448;Float;False;Property;_Glitch1;Glitch1;13;0;Create;True;0;0;False;0;0.5197807,0.4306336,0.9926471,0;0,0.4130404,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;267;1356.308,1551.777;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-6,5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;258;1671.789,1032.217;Float;True;Property;_TextureSample5;Texture Sample 5;13;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;286;1227.792,-250.8208;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;233;1491.991,413.0472;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;279;2092.634,276.1865;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;218;1323.952,49.29295;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;256;2266.693,277.9997;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;291;2258.916,510.9701;Float;False;Property;_GlitchIntensity;Glitch Intensity;11;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;268;1674.47,1455.005;Float;True;Property;_TextureSample7;Texture Sample 7;13;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;219;1468.361,-280.0379;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;216;1599.441,64.26983;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;2269.417,1041.076;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;294;2600.46,609.2111;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;259;2425.517,276.1189;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;214;1817.879,-176.8494;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;263;1350.463,1375.835;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-5,-2.3;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;252;2152.547,46.00183;Float;False;Property;_LEDint;LED int;9;0;Create;True;0;0;False;0;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;295;2699.431,394.0644;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;262;1671.705,1254.391;Float;True;Property;_TextureSample6;Texture Sample 6;13;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;210;2003.236,-221.4626;Float;True;Property;_Logo;Logo;1;0;Create;True;0;0;False;0;None;537b210ed4792ab46b2436bbe1e155de;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;285;1250.652,-642.3152;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;2451.6,32.07762;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;2377.378,-195.9778;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;298;2306.176,-352.4445;Float;False;Property;_BackgroundEm;Background Em;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;244;1547.187,-670.547;Float;True;Property;_Background;Background;7;0;Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;269;2623.405,34.90611;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;2628.46,-381.3949;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;250;2748.355,-115.3429;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;2921.904,-157.3116;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;246;1560.503,-908.5312;Float;False;Property;_AlbedoColor;Albedo Color;6;0;Create;True;0;0;False;0;0.490566,0.490566,0.490566,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;299;2921.714,-508.6087;Float;False;Property;_Smoothness;Smoothness;15;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;284;3070.742,-111.8337;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;2059.359,-672.8325;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3235.687,-277.6361;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;MK4/Billboards;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.28;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;1;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;273;0;249;4
WireConnection;272;0;213;0
WireConnection;272;1;273;0
WireConnection;223;0;272;0
WireConnection;226;0;272;0
WireConnection;276;0;275;4
WireConnection;277;0;275;4
WireConnection;215;0;224;0
WireConnection;215;1;223;0
WireConnection;225;0;224;0
WireConnection;225;1;226;0
WireConnection;229;0;272;0
WireConnection;278;0;275;4
WireConnection;278;1;277;0
WireConnection;278;2;276;0
WireConnection;255;0;272;0
WireConnection;228;0;224;0
WireConnection;228;1;229;0
WireConnection;227;0;215;1
WireConnection;227;1;225;2
WireConnection;283;0;278;0
WireConnection;231;0;272;0
WireConnection;257;0;272;0
WireConnection;254;0;253;0
WireConnection;254;1;255;0
WireConnection;230;0;227;0
WireConnection;230;1;228;3
WireConnection;280;0;283;0
WireConnection;232;0;224;0
WireConnection;232;1;231;0
WireConnection;266;0;265;0
WireConnection;266;1;254;1
WireConnection;267;0;272;0
WireConnection;258;0;253;0
WireConnection;258;1;257;0
WireConnection;286;0;287;0
WireConnection;286;1;288;0
WireConnection;233;0;230;0
WireConnection;233;1;232;4
WireConnection;279;0;249;0
WireConnection;279;1;280;0
WireConnection;218;0;217;0
WireConnection;256;0;266;0
WireConnection;256;1;279;0
WireConnection;268;0;253;0
WireConnection;268;1;267;0
WireConnection;219;0;213;0
WireConnection;219;2;286;0
WireConnection;216;0;233;0
WireConnection;216;1;218;0
WireConnection;260;0;261;0
WireConnection;260;1;258;2
WireConnection;294;0;291;0
WireConnection;294;1;268;4
WireConnection;259;0;260;0
WireConnection;259;1;256;0
WireConnection;214;0;219;0
WireConnection;214;1;216;0
WireConnection;263;0;272;0
WireConnection;295;0;294;0
WireConnection;262;0;253;0
WireConnection;262;1;263;0
WireConnection;210;1;214;0
WireConnection;285;0;213;0
WireConnection;285;1;216;0
WireConnection;251;0;259;0
WireConnection;251;1;252;0
WireConnection;264;0;262;3
WireConnection;264;1;210;0
WireConnection;244;1;285;0
WireConnection;269;0;295;0
WireConnection;269;1;251;0
WireConnection;297;0;244;0
WireConnection;297;1;298;0
WireConnection;250;0;269;0
WireConnection;250;1;264;0
WireConnection;296;0;250;0
WireConnection;296;1;297;0
WireConnection;284;0;296;0
WireConnection;247;0;246;0
WireConnection;247;1;244;0
WireConnection;0;0;247;0
WireConnection;0;2;284;0
WireConnection;0;4;299;0
ASEEND*/
//CHKSM=3670F35E292A0FEF53FD578A75EB505128C3D8A7