Shader "UI/SegmentedCircle"
{
    Properties
    {
        _ColorOn ("Active Segment Color", Color) = (0,1,1,1)
        _ColorOff ("Inactive Segment Color", Color) = (0.8,0.8,0.8,1)
        _Segments ("Total Segments", Float) = 6
        _Active ("Active Segments", Float) = 2
        _Gap ("Gap Angle (Degrees)", Float) = 5
        _Thickness ("Ring Thickness", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorOn, _ColorOff;
            float _Segments, _Active, _Gap, _Thickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // Center UV (-1 to 1)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float r = length(uv);
                if (r > 1 || r < (1 - _Thickness))
                    discard;

                float angle = atan2(uv.y, uv.x); // Radians
                angle = degrees(angle);
                if (angle < 0) angle += 360;

                float segAngle = 360.0 / _Segments;
                float angleWithGap = segAngle - _Gap;
                float segmentID = floor(angle / segAngle);

                float localAngle = fmod(angle, segAngle);

                if (localAngle > angleWithGap)
                    discard;

                return segmentID < _Active ? _ColorOn : _ColorOff;
            }
            ENDCG
        }
    }
}
