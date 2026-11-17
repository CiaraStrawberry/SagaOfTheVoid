// Sprite-stack slicing: drawn looking straight down with the near clip plane at the slice height. A pixel whose first
// visible surface is a back face lies inside the hull at that height, so it is part of the slice: it gets the
// texture colour there. Front faces (outside the hull) are left clear.
Shader "Hidden/StackSlice"
{
    Properties { _MainTex ("Tex", 2D) = "white" {} _Color ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color;
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            v2f vert (appdata_base v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); return o; }
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                if (facing > 0) return fixed4(0, 0, 0, 0);
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                return fixed4(c.rgb, 1);
            }
            ENDCG
        }
    }
}
