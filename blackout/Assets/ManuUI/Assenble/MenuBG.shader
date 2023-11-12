Shader "Unlit/MenuBG"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Rad ("Rad", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        GrabPass{"_BGTex"}

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
                float4 grabPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BGTex;
            fixed4 _BGTex_TexelSize;
            float _Rad;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            //ƒTƒ“ƒvƒŠƒ“ƒO
            fixed4 GrabSampling(float delta, v2f g) {
                fixed4 col = fixed4(0, 0, 0, 0);
                col += tex2Dproj(_BGTex, g.grabPos + float4(delta * _BGTex_TexelSize.x, 0,0,0));
                col += tex2Dproj(_BGTex, g.grabPos + float4(-delta * _BGTex_TexelSize.x, 0,0,0));
                col += tex2Dproj(_BGTex, g.grabPos + float4(0, delta * _BGTex_TexelSize.y,0,0));
                col += tex2Dproj(_BGTex, g.grabPos + float4(0, -delta * _BGTex_TexelSize.y,0,0));
                return col / 4;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,1);
                float rad = max(_Rad, 1);
                float weightCnt = 0;
                [loop]
                for (int n = 1; n <= rad; n += 1) {
                    float weight = (sin(((float)n / rad * 3.14) - 3.14 / 2) / 2 + 0.5);
                    weightCnt += weight;
                    col += GrabSampling(n, i) * weight;
                }
                col /= weightCnt;
                fixed4 mt = tex2D(_MainTex, i.uv) * _Color;
                col *= mt;
                col.a = mt.a;

                return col;
            }
            ENDCG
        }
    }
}
