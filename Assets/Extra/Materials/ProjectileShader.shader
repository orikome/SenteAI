Shader "Custom/ProjectileShader" {
    Properties{
        _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _InnerGlowWidth("Inner Glow Width", Range(0, 5)) = 0.1
        _InnerGlowColor("Inner Glow Color", Color) = (1, 1, 1, 1)
        _OrbSpeed("Orb Speed", Float) = 1
        _OrbAmplitude("Orb Amplitude", Float) = 0.2
        _OrbSize("Orb Size", Float) = 0.1
        _OrbColor("Orb Color", Color) = (1, 1, 1, 1)
    }

    SubShader{
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        float _OrbSpeed;
        float _OrbAmplitude;
        float _OrbSize;
        half4 _MainColor;
        half4 _InnerGlowColor;
        half4 _OrbColor;
        float _InnerGlowWidth;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
            float3 objPos : POSITION;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            // Mving orb position in object space
            float3 orbCenter = float3(sin(_Time.y * _OrbSpeed) * _OrbAmplitude, 0, 0);

            // Distance from current position to orb center
            float dist = distance(IN.objPos, orbCenter);

            // Orb effect
            float orb = 1.0 - smoothstep(_OrbSize, 0.0, dist);

            // Fresnel effect
            float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), IN.worldNormal)), 2);

            // Inner glow line based on the fresnel effect and the inner glow width
            float innerGlow = step(1 - _InnerGlowWidth, fresnel);

            // Combine colors
            half3 orbColor = lerp(_MainColor.rgb, _OrbColor.rgb, orb);
            o.Albedo = orbColor + _InnerGlowColor.rgb * innerGlow;
            o.Alpha = _MainColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
