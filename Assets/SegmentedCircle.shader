Shader "UI/SegmentedCircle"
{
    Properties
    {
        _ColorOn ("Active Segment Color", Color) = (0,1,1,1)
        _ColorOff ("Inactive Segment Color", Color) = (0.8,0.8,0.8,1)
        _ColorHighlight ("Highlight Color", Color) = (1,1,1,1)
        _Segments ("Total Segments", Float) = 6
        _Active ("Active Segments", Float) = 2
        _Gap ("Gap Angle (Degrees)", Float) = 5
        _Thickness ("Ring Thickness", Range(0,1)) = 0.3
        _HighlightStart ("Highlight Start Segment", Float) = 0
        _HighlightCount ("Highlight Segment Count", Float) = 0
        _HighlightExpand ("Highlight Expansion", Range(0,0.2)) = 0.1
        _BorderSize ("Border Size", Range(0,0.1)) = 0.02
        _Radius ("Circle Radius", Range(0,1)) = 0.8
        // Triangle indicator properties
        _Triangle1Index ("Triangle 1 Gap Index", Float) = 0
        _Triangle2Index ("Triangle 2 Gap Index", Float) = 1
        _Triangle1Color ("Triangle 1 Color", Color) = (1,0,0,1)
        _Triangle2Color ("Triangle 2 Color", Color) = (0,1,0,1)
        _TriangleSize ("Triangle Size", Range(0.01,0.2)) = 0.1
        _TriangleWidth ("Triangle Width", Range(0.1,2)) = 1
        _TriangleOffset ("Triangle Distance from Circle", Range(0,0.2)) = 0.05
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

            fixed4 _ColorOn, _ColorOff, _ColorHighlight;
            fixed4 _Triangle1Color, _Triangle2Color;
            float _Segments, _Active, _Gap, _Thickness;
            float _HighlightStart, _HighlightCount, _HighlightExpand, _BorderSize;
            float _Radius;
            float _Triangle1Index, _Triangle2Index;
            float _TriangleSize, _TriangleWidth, _TriangleOffset;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // Center UV (-1 to 1)
                return o;
            }

            bool IsHighlighted(float segmentID) {
                return segmentID >= _HighlightStart && segmentID < (_HighlightStart + _HighlightCount);
            }

            bool IsInTriangle(float2 uv, float segmentID, float baseRadius) {
                float segAngle = 360.0 / _Segments;
                float gapCenterAngle = -(segmentID + 0.5) * segAngle + 90;
                
                // Check if either of the adjacent segments is highlighted
                bool leftHighlighted = IsHighlighted(segmentID);
                bool rightHighlighted = IsHighlighted((segmentID + 1) % int(_Segments));
                
                // Use the expanded radius if either adjacent segment is highlighted
                float effectiveRadius = baseRadius;
                if (leftHighlighted || rightHighlighted) {
                    effectiveRadius *= (1.0 + _HighlightExpand);
                }
                
                // Convert gap center angle to radians for rotation
                float angleRad = radians(gapCenterAngle);
                float2x2 rotationMatrix = float2x2(cos(angleRad), -sin(angleRad),
                                                 sin(angleRad), cos(angleRad));
                
                // Transform UV to triangle space
                float2 rotatedUV = mul(rotationMatrix, uv);
                
                // Triangle bounds - pointing inward
                float triangleHeight = _TriangleSize;
                float triangleBase = _TriangleSize * _TriangleWidth;
                float2 triangleCenter = float2(0, effectiveRadius + _TriangleOffset + triangleHeight * 0.5);
                
                // Move UV relative to triangle center
                float2 localUV = rotatedUV - triangleCenter;
                
                // Flip the Y coordinates to make triangle point inward
                localUV.y = -localUV.y;
                
                // Triangle test
                float halfBase = triangleBase * 0.5;
                float normalizedY = localUV.y / triangleHeight + 0.5;
                return abs(localUV.x) < halfBase * normalizedY && 
                       normalizedY >= 0 && normalizedY <= 1;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float r = length(uv);
                
                float angle = atan2(uv.y, uv.x);
                angle = degrees(angle);
                if (angle < 0) angle += 360;

                float segAngle = 360.0 / _Segments;
                float angleWithGap = segAngle - _Gap;
                float segmentID = floor(angle / segAngle);
                float localAngle = fmod(angle, segAngle);

                // Check for triangle indicators first
                if (IsInTriangle(uv, _Triangle1Index, _Radius)) {
                    return _Triangle1Color;
                }
                if (IsInTriangle(uv, _Triangle2Index, _Radius)) {
                    return _Triangle2Color;
                }

                if (localAngle > angleWithGap)
                    discard;

                bool isHighlighted = IsHighlighted(segmentID);
                float baseRadius = _Radius;
                float expandedRadius = isHighlighted ? baseRadius * (1.0 + _HighlightExpand) : baseRadius;
                
                // Discard pixels outside the segment's radius
                if (r > expandedRadius || r < (1 - _Thickness) * baseRadius)
                    discard;

                // Border effect for highlighted segments
                if (isHighlighted) {
                    float borderInner = (1 - _Thickness) * baseRadius + _BorderSize;
                    float borderOuter = expandedRadius - _BorderSize;
                    
                    // Radial borders (inner and outer)
                    bool isRadialBorder = (r < borderInner || r > borderOuter);
                    
                    // Angular borders (sides)
                    float borderAngle = _BorderSize * 360.0 / (3.14159 * baseRadius);
                    bool isAngularBorder = (localAngle < borderAngle || localAngle > angleWithGap - borderAngle);
                    
                    if (isRadialBorder || isAngularBorder) {
                        return _ColorHighlight;
                    }
                }

                // Support for fractional _Active values using fill amount
                float fractionalPart = _Active - floor(_Active);
                float fillAmount = 1.0;
                
                if (segmentID == floor(_Active)) {
                    fillAmount = fractionalPart;
                }
                
                // Calculate the fill position based on local angle
                float fillPosition = localAngle / angleWithGap;
                
                // Discard pixels that are beyond the fill amount
                if (fillPosition > fillAmount) {
                    discard;
                }
                
                return segmentID < floor(_Active) ? _ColorOn : _ColorOff;
            }
            ENDCG
        }
    }
}
