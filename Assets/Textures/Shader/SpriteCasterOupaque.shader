Shader "Custom/SpriteCutoutShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            clip(tex.a - _Cutoff); // Alpha test: discard pixels below cutoff
            o.Albedo = tex.rgb;
            o.Alpha = tex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
