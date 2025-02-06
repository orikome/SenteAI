Shader "Custom/ProceduralDisintegration" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _DissolveThreshold ("Dissolve Threshold", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (1,0,0,1)
        _EdgeWidth ("Edge Width", Range(0,0.2)) = 0.1
        _NoiseScale ("Noise Scale", Float) = 20
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        float _DissolveThreshold;
        fixed4 _Color;
        fixed4 _EdgeColor;
        float _EdgeWidth;
        float _NoiseScale;

        // Hash function for randomization
        float hash(float3 p) {
            p = frac(p * 0.3183099 + .1);
            p *= 17.0;
            return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
        }

        // 3D Value noise
        float noise(float3 x) {
            float3 p = floor(x);
            float3 f = frac(x);
            f = f * f * (3.0 - 2.0 * f);
    
            float n = p.x + p.y * 157.0 + 113.0 * p.z;
            return lerp(lerp(lerp(hash(p + float3(0,0,0)), 
                               hash(p + float3(1,0,0)), f.x),
                           lerp(hash(p + float3(0,1,0)), 
                               hash(p + float3(1,1,0)), f.x), f.y),
                       lerp(lerp(hash(p + float3(0,0,1)), 
                               hash(p + float3(1,0,1)), f.x),
                           lerp(hash(p + float3(0,1,1)), 
                               hash(p + float3(1,1,1)), f.x), f.y), f.z);
        }

        struct Input {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Generate noise based on world position
            float n = noise(IN.worldPos * _NoiseScale);
            
            // Calculate dissolution
            float dissolution = n - _DissolveThreshold;
            
            // Edge glow
            fixed4 col = _Color;
            if(dissolution < _EdgeWidth && dissolution > 0) {
                col = lerp(_Color, _EdgeColor, 1 - (dissolution / _EdgeWidth));
            }

            o.Albedo = col.rgb;
            o.Alpha = dissolution < 0 ? 0 : 1;
            o.Metallic = 0;
            o.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}