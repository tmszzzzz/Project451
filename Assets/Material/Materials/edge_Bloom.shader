Shader "Custom/Outline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
		[HDR]_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth ("Outline Width", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        LOD 200

        Pass
        {
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

			half4 _OutlineColor;
			float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
				float4 viewPos = float4(UnityObjectToViewPos(v.vertex), 1.0);
				float3 viewNormal = mul(UNITY_MATRIX_IT_MV, v.normal);

				viewNormal.z = -0.5;
				float3 normal = normalize(viewNormal);
				viewPos += float4(normal, 1.0) * _OutlineWidth;

				o.pos = mul(UNITY_MATRIX_P, viewPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;

                return col;
            }
            ENDCG
        }
    }
}