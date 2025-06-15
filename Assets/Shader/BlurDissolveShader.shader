Shader "Custom/BlurDissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 10)) = 0
        _DissolveThreshold ("Dissolve Threshold", Range(0,1)) = 0
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DissolveTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _BlurAmount;
            float _DissolveThreshold;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float dissolveValue = tex2D(_DissolveTex, i.uv).r;

                // 控制溶解
                if (dissolveValue < _DissolveThreshold)
                {
                    discard;
                }

                // 简单模糊模拟（多点采样模糊）
                float2 offset = float2(_BlurAmount / 1024, _BlurAmount / 1024);
                float4 blur = tex2D(_MainTex, i.uv) * 0.36;
                blur += tex2D(_MainTex, i.uv + offset) * 0.16;
                blur += tex2D(_MainTex, i.uv - offset) * 0.16;
                blur += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)) * 0.16;
                blur += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y)) * 0.16;

                return blur * _Color;
            }
            ENDCG
        }
    }
}
