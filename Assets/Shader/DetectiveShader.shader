Shader "Custom/TextureWithTransparencyAndColor"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}       // 纹理输入
        _ColorTint ("Tint Color", Color) = (1,1,1,1)     // 颜色叠加
        _Alpha ("Alpha", Range(0, 1)) = 1.0              // 透明度调节
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            // 使用透明混合模式
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 引用Unity的内置库
            #include "UnityCG.cginc"

            // 输入定义
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;          // 主纹理
            float4 _ColorTint;           // 颜色叠加
            float _Alpha;                // 透明度

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 读取纹理颜色
                half4 texColor = tex2D(_MainTex, i.uv);

                // 应用透明度和附加颜色
                texColor.rgb *= _ColorTint.rgb;
                texColor.a *= _Alpha;

                return texColor;
            }
            ENDCG
        }
    }
}
