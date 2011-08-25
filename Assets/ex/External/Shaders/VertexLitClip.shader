// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced 'glstate.lightmodel.ambient' with 'UNITY_LIGHTMODEL_AMBIENT'
// Upgrade NOTE: replaced 'glstate.matrix.invtrans.modelview[0]' with 'UNITY_MATRIX_IT_MV'

Shader "VertexLit Clipped" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Pass {
            Cull Off

            CGPROGRAM
                // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#include "UnityCG.cginc"

                struct v2f {
                    float4 pos : SV_POSITION;
                    float4 uv[2] : TEXCOORD0;
                    float4 color : COLOR0;
                };

                uniform float4x4 _ClipTextureMatrix;
                uniform float4 _Color;

                v2f vert( appdata_base v ) {
                    v2f o;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                    o.uv[0] = v.texcoord;

                    float4 c = mul( _Object2World, v.vertex );
                    c = mul( _ClipTextureMatrix, c );
                    o.uv[1] = c;

                    // lighting from single directional light + ambient
                    float4 color = UNITY_LIGHTMODEL_AMBIENT;
                    float3 lightDir = mul( glstate.light[0].position.xyz, (float3x3)UNITY_MATRIX_IT_MV );
                    float ndotl = max( 0, dot( lightDir, v.normal ) );
                    color.xyz += glstate.light[0].diffuse * _Color * ndotl;
                    o.color = color;
                    return o;
                }

            ENDCG

            AlphaTest Greater 0.1
            ColorMaterial AmbientAndDiffuse
            Lighting On
            SetTexture [_MainTex] {
                Combine texture * primary DOUBLE, texture * primary
            }
            SetTexture [_ClipTexture] { Combine previous, texture }
        }
    } 
    Fallback "VertexLit"
}
