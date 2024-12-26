Shader "Custom/DamageIndicator"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,0,0,0.8)
        _ArcAngle ("Arc Angle", Range(5, 360)) = 30
        _EdgeSoftness ("Edge Softness", Range(0.01, 0.5)) = 0.1
        _FlashSpeed ("Flash Speed", Range(0, 100)) = 2
        _FlashIntensity ("Flash Intensity", Range(0, 1)) = 0.3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float _ArcAngle;
            float _EdgeSoftness;
            float _FlashSpeed;
            float _FlashIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float radius = length(i.uv);
                float angle = degrees(atan2(i.uv.y, i.uv.x));

                // Create pizza slice - note the changed logic here
                float halfAngle = _ArcAngle * 0.5;
                float angleAlpha = smoothstep(-halfAngle, -halfAngle + _EdgeSoftness * 30, -abs(angle));
                
                // Circular edge fade
                float circleEdge = smoothstep(1.0 - _EdgeSoftness, 1.0, radius);
                float finalAlpha = (1 - angleAlpha) * (1 - circleEdge);

                float flash = (sin(_Time.y * _FlashSpeed) * 0.5 + 0.5) * _FlashIntensity;
                
                float4 col = _MainColor + flash;
                col.a = finalAlpha * _MainColor.a;
                
                return col;
            }
            ENDCG
        }
    }
}