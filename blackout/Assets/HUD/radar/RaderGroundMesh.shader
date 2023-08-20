Shader "Unlit/RaderGroundMesh"
{
    Properties
    {
        _MeshColor("Mesh Color",Color) = (1,1,1,1)
        _BackColor("Back Color",Color) = (0,0,0,0)
        _Size("Mesh Size",Range(0.0001,1)) = 0.1
        _Width("Width",Float) = 0.01
        _Xtpl("X Topology",Range(0,1)) = 0
        _Ytpl("Y Topology",Range(0,1)) = 0
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
            
            float4 _MeshColor;
            float4 _BackColor;
            float _Size;
            float _Width;
            float _Xtpl;
            float _Ytpl;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _BackColor;
                col = (i.uv[0] + _Size * _Xtpl) % _Size < _Width || (i.uv[1] + _Size * _Ytpl) % _Size < _Width ? _MeshColor : _BackColor;
                return col;
            }
            ENDCG
        }
    }
}
