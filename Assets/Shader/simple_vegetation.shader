Shader "Philip/simple_vegetation"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Cutout("Threshold", Range(0.0, 1.0)) = 0.5
        _Strength("Wind Strength", Float) = 1
    }
    SubShader
    {
        Cull Off
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutout vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 value;
        };
        
        fixed4 _Color;
        float _Strength;
        float2 uv_MainTex;

        float turbulence(float2 uv)
        {
            float frequency = 0.8;
            float y = 0;
            float t = -_Time.y * 1.3;
            float run = uv.y;
            y += sin(run * frequency);
            y += sin(run * frequency * 2.1 + t) * 4.5;
            y += sin(run * frequency * 1.72 + t * 1.121) * 4.0;
            y += sin(run * frequency * 2.221 + t * 0.437) * 5.0;
            y += sin(run * frequency * 3.1122 + t * 4.269) * 2.5;
            y *= _Strength * 0.06;
            return y;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            float t = turbulence(v.texcoord);
            float mask = v.texcoord.y;// Local position mask using vertical position
            v.vertex.xyz += v.normal * t * mask;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
