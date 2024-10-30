Shader "test/Tessellation"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _DispTex("Disp Texture", 2D) = "gray" {}
        _NormalMap("Normalmap", 2D) = "bump" {}
        _RoughnessMap("RoughnessMap", 2D) = "white" {}
        _AoMap("AoMap", 2D) = "white" {}
        _Displacement("Displacement", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 4.5
        #pragma multi_compile _ _DETAIL_NORMALMAP
        #pragma multi_compile _ _REFLECTION_BUFFERS
        #pragma multi_compile _ _SHADOWS_DEPTH

        sampler2D _MainTex;
        sampler2D _DispTex;
        sampler2D _NormalMap;
        sampler2D _RoughnessMap;
        sampler2D _AoMap;
        float _Displacement;

        struct Input
        {
            float2 uvMainTex;
        };

        void disp(inout appdata v)
        {
            float d = tex2D(_DispTex, v.uvMainTex).r * _Displacement;
            v.vertex.xyz += v.normal * d;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = tex2D(_MainTex, IN.uvMainTex).rgb;
            o.Metallic = 0.0; // Assuming no metallic property is needed
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uvMainTex));
            o.Smoothness = 1 - tex2D(_RoughnessMap, IN.uvMainTex).r;
            o.Occlusion = tex2D(_AoMap, IN.uvMainTex).r;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
