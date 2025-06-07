Shader "UI/BranchPulse"
{
    Properties
    {
        _MainTex ("Highlight Mask", 2D) = "white" {}
        _ColorA ("Start Color (Green)", Color) = (0.3, 1, 0.3, 1)
        _ColorB ("End Color (Purple)", Color) = (0.7, 0.3, 1, 1)
        _BreathStrength ("Breath Strength", Range(0, 1)) = 0.5
        _BreathSpeed ("Breath Speed", Range(0.1, 10)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _ColorA;
            fixed4 _ColorB;
            float _BreathStrength;
            float _BreathSpeed;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 maskColor = tex2D(_MainTex, i.uv);
                float breath = sin(_Time.y * _BreathSpeed) * 0.5 + 0.5;

                // 颜色渐变：绿色 -> 紫色
                fixed3 pulseColor = lerp(_ColorA.rgb, _ColorB.rgb, breath);

                // 强度控制
                fixed3 finalColor = pulseColor * _BreathStrength;

                // 透明度由贴图 alpha 和呼吸强度共同决定
                return fixed4(finalColor, maskColor.a * breath);
            }
            ENDCG
        }
    }
}
