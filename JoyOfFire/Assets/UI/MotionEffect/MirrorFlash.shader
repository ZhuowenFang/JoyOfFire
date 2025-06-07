Shader "UI/MirrorFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashStrength ("Flash Strength", Range(0,1)) = 0
        _FlashPos ("Flash Position", Range(0,1)) = 0.0
        _FlashWidth ("Flash Width", Range(0.01, 0.5)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
            float4 _MainTex_ST;

            float4 _FlashColor;
            float _FlashStrength;
            float _FlashPos;
            float _FlashWidth;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算扫光亮度因子
                float dist = abs(i.uv.x - _FlashPos);
                float sweep = smoothstep(_FlashWidth, 0, dist); // 越接近中心越亮
                col.rgb += _FlashColor.rgb * _FlashStrength * sweep;

                return col;
            }
            ENDCG
        }
    }
}