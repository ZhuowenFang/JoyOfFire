Shader "UI/VortexEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Strength ("Strength", Float) = 0.6
        _TwistSpeed ("Twist Speed", Float) = 2.0
        _FadeRadius ("Fade Radius", Float) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Center;
            float _Strength;
            float _TwistSpeed;
            float _FadeRadius;
            float _TimeY;

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

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = _Center.xy;
                float2 offset = i.uv - center;
                float dist = length(offset);

                // 恒定旋转角度（和距离无关）
                float angle = atan2(offset.y, offset.x);
                angle += _TwistSpeed * _Time.y * _Strength;

                float2 rotated = dist * float2(cos(angle), sin(angle));
                float2 uv = center + rotated;
                fixed4 col = tex2D(_MainTex, uv);

                // 渐隐：根据距中心的距离衰减 Alpha
                float fade = smoothstep(_FadeRadius, 1.0, dist); // 0 ~ 1 区间渐隐
                col.a *= (1.0 - fade);

                return col;
            }
            ENDCG
        }
    }
}

