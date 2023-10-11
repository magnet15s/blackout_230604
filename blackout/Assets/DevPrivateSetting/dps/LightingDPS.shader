Shader "Unlit/LightingDPS"
{
    Properties
    {
        [Toggle]_UseDPS("Enable setting", Float) = 1
        [Space(10)]
        _SolarAngle("Solar angle", Range(0, 360)) = 189.77
        _SolarDirection("Solar direction", Range(0,360)) = 0

        [Toggle]_SunlightColorOverride("Sunlight color override", Float) = 1
        _SunlightColor("Sunlight color", Color) = (0.57,0.54,0.51,1)

        [Toggle]_CamFXOverride("Camera effect override", Float) = 1
        [Space(20)]
        _NoiseIts("Noise intensity", Range(0.01, 1.0)) = 0.7
        [Space(10)]
        _BloomThres("Bloom threshold", Range(0.0, 1.0)) = 0.4
        _BloomIts("Bloom intensity", Float) = 1
        [Space(10)]
        _Lightness("Lightness", Float) = 1
        _Contrast("Contrast", Float) = 1
        


    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = 1;
                o.uv = 1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
