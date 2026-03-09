Shader "Custom/Red_Dot_Threshold"
{
    Properties
    {
      _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Range(0, 0.5)) = 0.45 // Inner보다 살짝 커야 합니다.
        _EdgeSoftness ("Edge Softness", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // 핵심: 스텐실 버퍼 값이 1이 아닐(NotEqual) 때만 그립니다.
        Stencil {
            Ref 1
            Comp NotEqual
            Pass Keep
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            float4 _BorderColor;
            float _Radius;
            float _EdgeSoftness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, float2(0.5, 0.5));
                float alpha = smoothstep(_Radius + _EdgeSoftness, _Radius - _EdgeSoftness, dist);
                
                clip(alpha - 0.01);

                fixed4 color = _BorderColor;
                color.a *= alpha;
                return color;
            }
            ENDCG
        }
    }
}
