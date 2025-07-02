Shader "UI/CharacterMysticEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _TransparencyThreshold ("Transparency Threshold", Range(0,1)) = 0.5
        _VignetteStrength ("Vignette Strength", Range(0,5)) = 1
        _EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.5
        _EdgeColor ("Edge Color", Color) = (0,1,1,1)
        _EdgeStrength ("Edge Multiply Strength", Range(0,5)) = 2
        _NoiseOffsetX ("Noise Offset X", Range(-1,1)) = 0 // 新增
        _NoiseOffsetY ("Noise Offset Y", Range(-1,1)) = 0 // 新增
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _TransparencyThreshold;
            float _VignetteStrength;
            float _EdgeThreshold;
            float4 _EdgeColor;
            float _EdgeStrength;
            float _NoiseOffsetX;
            float _NoiseOffsetY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // 给噪声图增加手动偏移
                o.uvNoise = v.uv + float2(_NoiseOffsetX, _NoiseOffsetY);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (col.a < _TransparencyThreshold)
                    discard;

                // 暗角
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float vignette = saturate(1.0 - dist * _VignetteStrength);
                col.rgb *= vignette;

                // 噪声
                fixed noise = tex2D(_NoiseTex, i.uvNoise).r;

                if (noise > _EdgeThreshold)
                {
                    float blend = noise * _EdgeStrength;
                    col.rgb *= lerp(1, _EdgeColor.rgb, blend);
                }

                return col;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}