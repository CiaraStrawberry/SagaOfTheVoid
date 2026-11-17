// Sprite-stacked ships and scenery (SpriteStacks.cs): a pile of flat pixel-art slices at their real heights.
// _MainTex holds one palette index per pixel (tools/sprite_stack.py; 0 = empty, bit 7 = top surface), _Palette the
// colours for this team, the vertex colour each slice's shade (lower = darker). Unlit: light is baked into the
// palette. Point-sampled with no mipmaps, so pixels stay crisp; cut out where a slice is empty; writes depth.
Shader "Saga/Stack"
{
    Properties
    {
        _MainTex ("Slice indices", 2D) = "black" {}
        _Palette ("Palette", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        // Wreck chunks (BattleView.Wrecks): each chunk lies between two cracks across the hull. A crack is a line
        // in model space: keep dot(p.xz, dirA) >= offsetA and dot(p.xz, dirB) <= offsetB. The edge steps up and
        // down per pixel by up to the jag size. Jag 0: no clipping.
        _ClipA ("Crack below (offset, dir x, dir z, jag)", Vector) = (-100000, 0, 1, 0)
        _ClipB ("Crack above (offset, dir x, dir z, pixel)", Vector) = (100000, 0, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        Cull Off
        ZWrite On

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Palette);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float4 _ClipA, _ClipB;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; half shade : TEXCOORD1; float3 posOS : TEXCOORD2; };

            // Where a break crosses this pixel column: the same for both chunks on either side of it.
            float Edge(float at, float column) { return at + (frac(sin(column * 12.9898 + at * 0.731) * 43758.5453) - 0.5) * _ClipA.w; }
            // Which pixel along the crack (and up the hull) this is, so the jagged edge steps per pixel.
            float Column(float3 p, float2 dir) { return floor(dot(p.xz, float2(dir.y, -dir.x)) / _ClipB.w) + floor(p.y / _ClipB.w) * 7.0; }

            Varyings vert (Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = i.uv;
                o.shade = i.color.r;
                o.posOS = i.positionOS.xyz;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                if (_ClipA.w > 0)
                {
                    float2 da = _ClipA.yz, db = _ClipB.yz;
                    if (dot(i.posOS.xz, da) < Edge(_ClipA.x, Column(i.posOS, da)) || dot(i.posOS.xz, db) > Edge(_ClipB.x, Column(i.posOS, db))) discard;
                }
                int idx = (int)(SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, i.uv, 0).r * 255.0 + 0.5);
                bool top = idx >= 128;
                idx = idx & 127;
                if (idx == 0) discard;
                half3 col = LOAD_TEXTURE2D(_Palette, int2(idx - 1, 0)).rgb;
                half shade = top ? max(i.shade, 0.9h) : i.shade;
                shade = round(shade * 12.0h) / 12.0h;
                return half4(saturate(col * shade * _Color.rgb), 1); // brightened scenery tops out at white, never blooms
            }
            ENDHLSL
        }
    }
}
