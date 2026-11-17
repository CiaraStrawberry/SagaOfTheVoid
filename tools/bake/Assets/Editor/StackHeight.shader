// Sprite-stack height map: drawn looking straight down, each pixel gets the height of the top surface there, as a
// fraction of the ship's height (0 = lowest point, 1 = highest), split over red (coarse) and green (fine).
Shader "Hidden/StackHeight"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _StackMinY, _StackSizeY;
            struct v2f { float4 pos : SV_POSITION; float y : TEXCOORD0; };
            v2f vert (appdata_base v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.y = mul(unity_ObjectToWorld, v.vertex).y; return o; }
            float4 frag (v2f i) : SV_Target
            {
                float h = saturate((i.y - _StackMinY) / _StackSizeY) * 255.0;
                return float4(floor(h) / 255.0, frac(h), 0, 1);
            }
            ENDCG
        }
    }
}
