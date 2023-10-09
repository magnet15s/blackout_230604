Shader "Unlit/PostEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white"{}
        _NoiseIts ("Noise intensity", Range(0.01, 1.0)) = 0.7
        _BloomThres ("Bloom threshold", Range(0.0, 1.0)) = 0.4
        [Space(10)]
        _FogColor("Fog color", Color) = (1,1,1,1)
        _FogStart ("Fog start depth", Range(0.0, 1.0)) = 0.8
        _FogLineIntersect("Fog line intersection", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Ztest Always
        Zwrite Off

        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile _NOISE_ON

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define PI 3.14159265


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                /*UNITY_FOG_COORDS(1)*/
                float4 vertex : SV_POSITION;
            };

            //fields

            sampler2D _CameraDepthTexture;

            sampler2D _MainTex;
            sampler2D _Noise;
            float _NoiseIts;
            float4 _MainTex_ST;
            float4 _Noise_ST;
            fixed _BloomThres;

            //funcs

            fixed luma(fixed4 col) {
                return col.r * 0.299 + col.g * 0.587 + col.b * 0.114;
            }

            fixed4 contConf(fixed4 col, fixed base) {
                fixed4 rslt = saturate(col + (luma(col) - (1 - base)));
                rslt.a = 1;
                return rslt;
            }
            
            fixed4 getBoxSamp(float2 uv, float delta) {
                float2 uvd;
                half4 sumCol;
                for (int i = 0; i < 4; i++) {
                    uvd.x = (int)(i / 2) * 2 - 1;
                    uvd.y = (int)(i % 2) * 2 - 1;
                    sumCol += tex2D(_MainTex, uv + uvd * delta);
                }
                return sumCol / 4;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            #define PI 3.14159265

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.r = saturate(col.r);
                col.g = saturate(col.g);
                col.b = saturate(col.b);
                col.a = 1;
                


                //fog
                fixed depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
                
                //cont
                half a = 1.5;
                half b = 0.4;
                //col = saturate(a * (col - b) + .38) ;//saturate((sin((col - 0.5) * PI )/2.5) + 0.5);
                col.r = col.r > 0.5 ? min(a * (col.r - b) + 0.5, 1 / a * (col.r - 1) + 1) : max(a * (col.r - b) + 0.5, 1 / a * col.r);
                col.g = col.g > 0.5 ? min(a * (col.g - b) + 0.5, 1 / a * (col.g - 1) + 1) : max(a * (col.g - b) + 0.5, 1 / a * col.g);
                col.b = col.b > 0.5 ? min(a * (col.b - b) + 0.5, 1 / a * (col.b - 1) + 1) : max(a * (col.b - b) + 0.5, 1 / a * col.b);
                
                //bloom
                fixed4 blCol = contConf(getBoxSamp(i.uv, .004), _BloomThres);
                col += blCol * 0.06; 
                blCol = contConf(getBoxSamp(i.uv, .003), _BloomThres);
                col += blCol * 0.08;
                blCol = contConf(getBoxSamp(i.uv, .002), _BloomThres);
                col += blCol * 0.2;
                blCol = contConf(getBoxSamp(i.uv, .001), _BloomThres);
                col += blCol * 0.3;
                col += contConf(col, _BloomThres);



                // apply noise
                #ifdef _NOISE_ON
                    fixed4 noise = tex2D(_Noise, i.uv + _Time * 5);
                    noise = (noise / (1 / _NoiseIts)) + _NoiseIts;
                    col.r = col.r / max(noise.r, 0.01f);
                    col.g = col.g / max(noise.g, 0.01f);
                    col.b = col.b / max(noise.b, 0.01f);
                    col.a = 1;
                #else
                #endif


                return col;
            }

            
            ENDCG
        }
    }
}
