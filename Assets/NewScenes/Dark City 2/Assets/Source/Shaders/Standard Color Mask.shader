// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/Standard Color Mask"
{
	Properties
	{
		_AlbedoMaskA("Albedo Mask (A)", 2D) = "gray" {}
		_MaskColor("Mask Color", Color) = (0,0,0,0)
		_Normalmap("Normalmap", 2D) = "bump" {}
		_MetallicGloss("Metallic Gloss", 2D) = "black" {}
		_Emission("Emission", 2D) = "black" {}
		[Toggle]_EmissionTexture("Emission Texture", Range( 0 , 1)) = 0
		[HDR]_EmissionColor("Emission Color", Color) = (0.5,0.5,0.5,0)
		_AO("AO", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normalmap;
		uniform float4 _Normalmap_ST;
		uniform sampler2D _AlbedoMaskA;
		uniform float4 _AlbedoMaskA_ST;
		uniform float4 _MaskColor;
		uniform float4 _EmissionColor;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float _EmissionTexture;
		uniform sampler2D _MetallicGloss;
		uniform float4 _MetallicGloss_ST;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normalmap = i.uv_texcoord * _Normalmap_ST.xy + _Normalmap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normalmap, uv_Normalmap ) );
			float2 uv_AlbedoMaskA = i.uv_texcoord * _AlbedoMaskA_ST.xy + _AlbedoMaskA_ST.zw;
			float4 tex2DNode2 = tex2D( _AlbedoMaskA, uv_AlbedoMaskA );
			float4 lerpResult7 = lerp( tex2DNode2 , _MaskColor , tex2DNode2.a);
			o.Albedo = lerpResult7.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 tex2DNode6 = tex2D( _Emission, uv_Emission );
			float4 lerpResult9 = lerp( float4( 0,0,0,0 ) , _EmissionColor , tex2DNode6.r);
			float4 lerpResult13 = lerp( lerpResult9 , tex2DNode6 , _EmissionTexture);
			o.Emission = lerpResult13.rgb;
			float2 uv_MetallicGloss = i.uv_texcoord * _MetallicGloss_ST.xy + _MetallicGloss_ST.zw;
			float4 tex2DNode4 = tex2D( _MetallicGloss, uv_MetallicGloss );
			o.Metallic = tex2DNode4.r;
			o.Smoothness = tex2DNode4.a;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
283;203;1200;746;1225.711;-107.1675;1.519301;True;True
Node;AmplifyShaderEditor.ColorNode;10;-559.3211,215.5461;Float;False;Property;_EmissionColor;Emission Color;6;1;[HDR];Create;True;0;0;False;0;0.5,0.5,0.5,0;0.490566,0.490566,0.490566,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-879.1072,289.7104;Float;True;Property;_Emission;Emission;4;0;Create;True;0;0;False;0;None;7667c5c22a2545f45b636501f94469a7;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-885.2346,-81.82604;Float;True;Property;_AlbedoMaskA;Albedo Mask (A);0;0;Create;True;0;0;False;0;None;e4158e979aa314040a0d67682f10de8e;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;9;-320.483,215.0815;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;8;-828.0997,-313.4736;Float;False;Property;_MaskColor;Mask Color;1;0;Create;True;0;0;False;0;0,0,0,0;0,0.2938651,0.5660378,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-415.6367,854.3281;Float;False;Property;_EmissionTexture;Emission Texture;5;1;[Toggle];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;-40.70945,303.322;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-893.7916,714.4851;Float;True;Property;_AO;AO;7;0;Create;True;0;0;False;0;None;e3eae56aad2163c41acad9f2289514d3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-885.7267,104.5547;Float;True;Property;_Normalmap;Normalmap;2;0;Create;True;0;0;False;0;None;d0e89b4083ea04e44b3c8ef9179a3f6c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-324.5096,-296.7731;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-890.6617,533.6829;Float;True;Property;_MetallicGloss;Metallic Gloss;3;0;Create;True;0;0;False;0;None;acb88c6db1230794a8aa39c01c98174a;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;169.6618,-2.827697;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MK4/Standard Color Mask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;1;10;0
WireConnection;9;2;6;0
WireConnection;13;0;9;0
WireConnection;13;1;6;0
WireConnection;13;2;11;0
WireConnection;7;0;2;0
WireConnection;7;1;8;0
WireConnection;7;2;2;4
WireConnection;0;0;7;0
WireConnection;0;1;3;0
WireConnection;0;2;13;0
WireConnection;0;3;4;0
WireConnection;0;4;4;4
WireConnection;0;5;5;0
ASEEND*/
//CHKSM=D75DF92DB032D6DEFA843901034286212D26B541