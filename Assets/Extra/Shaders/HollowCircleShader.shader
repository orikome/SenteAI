Shader "Custom/HollowCircleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Outer Radius", Range(0,1)) = 0.5
        _InnerRadius ("Inner Radius", Range(0,1)) = 0.4
        _Softness ("Edge Softness", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            fixed4 _Color;
            float _Radius;
            float _InnerRadius;
            float _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                float outer = 1 - smoothstep(_Radius - _Softness, _Radius + _Softness, dist);
                float inner = smoothstep(_InnerRadius - _Softness, _InnerRadius + _Softness, dist);
                
                float ring = outer * inner;
                
                fixed4 col = _Color * i.color;
                col.a *= ring;
                return col;
            }
            ENDCG
        }
    }
}