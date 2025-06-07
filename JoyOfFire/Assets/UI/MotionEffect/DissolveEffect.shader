Shader "UI/DissolveEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Cutoff ("Dissolve Amount", Range(0,1)) = 1
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
        _EdgeWidth ("Edge Width", Range(0.01, 0.2)) = 0.05
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Cutoff;
            float4 _EdgeColor;
            float _EdgeWidth;
            float4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float noise = tex2D(_NoiseTex, uv).r;
                float alpha = tex2D(_MainTex, uv).a;

                // Ö÷ÌùÍ¼ÑÕÉ«
                fixed4 mainColor = tex2D(_MainTex, uv) * _Color;

                // ÈÜ½â±ßÔµÅÐ¶Ï
                float diff = noise - _Cutoff;

                // ÔÚ±ßÔµ·¶Î§ÄÚµÄ£¬ÏÔÊ¾Îª»ðÑæ±ßÑÕÉ«
                if (diff < 0 && diff > -_EdgeWidth)
                {
                    mainColor.rgb = _EdgeColor.rgb;
                    mainColor.a = _EdgeColor.a;
                }
                else if (noise < _Cutoff)
                {
                    discard;
                }

                mainColor.a *= alpha;
                return mainColor;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}