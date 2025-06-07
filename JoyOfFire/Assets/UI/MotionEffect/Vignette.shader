Shader "Map/Vignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _Smoothness;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1; // ��uvת����[-1,1]��Χ
                float len = length(uv);

                fixed4 col = tex2D(_MainTex, i.uv);

                // ���㰵��ϵ����0���ģ�1��Ե��
                float vignette = smoothstep(1.0 - _Smoothness, 1.0, len);

                // ��ת����ǿ�ȣ�Խ������Ե��ɫԽ��
                col.rgb *= lerp(1.0, 1.0 - _Intensity, vignette);

                return col;
            }
            ENDCG
        }
    }
}

