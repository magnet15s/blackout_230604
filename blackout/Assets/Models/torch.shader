Shader "Unlit/torch"
{
    Properties
    {
        _R("R",Float) = 1
        _G("G",Float) = 1
        _B("B",Float) = 1
        _FogFactor ("fog", Range(0,1)) = 1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _MainTex_ST;
            float _FogFactor;
            fixed _R;
            fixed _G;
            fixed _B;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = 0;
                col.r = _R;
                col.g = _G;
                col.b = _B;

                fixed4 c2 = col;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col*_FogFactor + c2 * (1-_FogFactor);
            }
            ENDCG
        }
    }
}
