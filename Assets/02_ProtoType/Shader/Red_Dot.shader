Shader "Custom/Red_Dot"
{
    Properties
    {
      _FillColor ("Fill Color", Color) = (0.98, 0.23, 0.3, 1)
        _Radius ("Radius", Range(0, 0.5)) = 0.4
        _EdgeSoftness ("Edge Softness", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // 핵심: 스텐실 버퍼에 1을 무조건 기록합니다.
        Stencil {
            Ref 1
            Comp Always
            Pass Replace
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

            float4 _FillColor;
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
                
                // 중요: 투명한 배경 부분은 스텐실에 기록하지 않도록 렌더링을 완전히 취소(clip)합니다.
                clip(alpha - 0.01); 

                fixed4 color = _FillColor;
                color.a *= alpha;
                return color;
            }
            ENDCG
        }
    }
}
