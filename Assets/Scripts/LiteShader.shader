Shader "Custom/ShadowLite"
{
    Properties
    {
        _Color ("Shadow Color", Color) = (0, 0, 0, 0.5)
        _MainTex("Texture (optional)", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" 
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                // multiply optional texture by tint color
                return tex * _Color;
            }
            ENDCG
        }
    }
}
