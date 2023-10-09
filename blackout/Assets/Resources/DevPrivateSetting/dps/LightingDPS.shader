Shader "Unlit/LightingDPS"
{
    Properties
    {
        [Toggle]_UseDPS("Enable setting", Float) = 1
        [Space(10)]
        _SolarAngle("Solar angle", Range(0, 360)) = 90
        _SolarDirection("Solar direction", Range(0,360)) = 0

        [Toggle]_SunlightColorOverride("SunlightColorOverride", Float) = 1
        _SunlightColor("Sunlight color", Color) = (1,1,1,1)

        


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
