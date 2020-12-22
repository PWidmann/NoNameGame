Shader "Philip/simple_water"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("Water Noise", 2D) = "white" {}
        _Amplitude("Amplitude", Float) = 1
        _Frequency("Frequency", Float) = 1
        _Smoothness("Smoothness", Range(0,1)) = 1
        _Threshold("Highlight Threshold", Range(0,1)) = 0.5
        _WobbleStrength("Wobble Strength", Range(0,16)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
            #pragma target 3.0

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_NoiseTex;
                float2 value;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            fixed4 _Color;
            float _Amplitude;
            float _Frequency;
            float _Smoothness;
            float _Threshold;
            float _WobbleStrength;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float f = (_Time.y / 20) * _Frequency;// TODO: multiply frequency by time/20
                fixed r1 = tex2D(_NoiseTex, IN.uv_NoiseTex + float2(1, 0) * f).r;
                fixed r2 = tex2D(_NoiseTex, IN.uv_NoiseTex + float2(0, 1) * f).r;
                float2 translation = float2(sin(r1), sin(r2)) * 0.1;
                fixed4 color = tex2D(_MainTex, IN.uv_MainTex + translation) * _Color;
                float clipValue = color.x * IN.value;

                
                if (clipValue > _Threshold)
                    o.Albedo = color.rgb + float3(1, 1, 1);
                else
                    o.Albedo = color.rgb + float3(0, 0, 0);

                o.Smoothness = _Smoothness; // TODO: Add smoothness here
                o.Metallic = 1; // TODO: Set to constant 1
            }

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                float t = _Time.y * _Frequency;
                float o1 = sin(t - v.texcoord.x * _WobbleStrength);
                float o2 = sin(t + v.texcoord.y * _WobbleStrength);
                float value = (o1 * o2);
                float offset = value * _Amplitude;
                o.value = value; // TODO: Send value to Surface Shader
                v.vertex.xyz += v.normal * offset;
            }

            ENDCG
        }
            FallBack "Diffuse"
}