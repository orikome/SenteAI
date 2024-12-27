Shader "Custom/LaserBeam"
{
    Properties
    {
        _CoreColor ("Core Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _CoreWidth ("Core Width", Range(0,1)) = 0.1
        _GlowWidth ("Glow Width", Range(0,1)) = 0.5
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 2
        _ScrollSpeed ("Scroll Speed", Range(0,10)) = 2
        _NoiseScale ("Noise Scale", Range(0,50)) = 10
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One
        ZWrite Off
        Cull Off

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
                float3 worldPos : TEXCOORD1;
            };

            float4 _CoreColor;
            float4 _GlowColor;
            float _CoreWidth;
            float _GlowWidth;
            float _GlowIntensity;
            float _ScrollSpeed;
            float _NoiseScale;
            float _NoiseStrength;

            // Hash function for noise
            float hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * float3(.1031, .1030, .0973));
                p3 += dot(p3, p3.yxz + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            // 2D Noise
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scrolling noise
                float2 noiseUV = i.uv * _NoiseScale + float2(_Time.y * _ScrollSpeed, 0);
                float noiseVal = noise(noiseUV) * _NoiseStrength;
                
                // Apply noise to UV
                float2 distortedUV = i.uv + float2(noiseVal, 0);
                
                // Core gradient
                float center = abs(distortedUV.y - 0.5) * 2;
                float core = smoothstep(_CoreWidth, 0, center);
                float glow = smoothstep(_GlowWidth, _CoreWidth, center);
                
                // Combine effects
                fixed4 col = _CoreColor * core + _GlowColor * glow * _GlowIntensity;
                col *= (1 - noiseVal * 0.5); // Subtle noise effect on intensity
                
                return col;
            }
            ENDCG
        }
    }
}