Shader "QAtmo/QAtmo_Decals" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _SpecColor("SpecColor", Color) = (1,1,1,1)
        _Specular("Specular", Range(0.01, 1.0)) = 1.0
        _Gloss("Gloss", Range(0.0, 1.0)) = 1.0
    }
 
    SubShader {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
       
 
        CGPROGRAM
        #pragma exclude_renderers flash
        #pragma target 3.0
        #pragma surface surf BlinnPhong fullforwardshadows dualforward decal:blend exclude_path:prepass
 
        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        float _Specular, _Gloss;
 
        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float4 color: Color;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            UNITY_INITIALIZE_OUTPUT (SurfaceOutput, o);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color  * IN.color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Specular = _Specular;
            o.Gloss = _Gloss;
        }
        ENDCG
    }
 
    FallBack "Decal/Transparent Diffuse"
}