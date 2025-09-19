Shader "Custom/PlayerPoint"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,255,0,255)
        _OutlineWidth("Outline Width", Range(0,0.01)) = 0.003
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        cull front
        // 1st Pass
        CGPROGRAM
        #pragma surface surf Nolight vertex:vert noshadow noambient

        fixed4 _OutlineColor;
        float _OutlineWidth;

        void vert(inout appdata_full v) {
            v.vertex.xyz = v.vertex.xyz + v.normal.xyz * _OutlineWidth;
        }

        struct Input
        {
            float4 color:COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
        }

        float4 LightingNolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return _OutlineColor;
        }
        ENDCG

        cull back
        // 2nd Pass
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
