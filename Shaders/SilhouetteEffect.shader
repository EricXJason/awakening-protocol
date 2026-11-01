// ============================================================================
// 檔案名稱: SilhouetteEffect.shader
// 所屬模組: Shaders
// 責任描述: 物件遮擋剪影與外輪廓著色器 (Silhouette / Contour)。
// 技術實作: 本 Shader 代碼為 AI 輔助生成之客製著色器 (AI-Generated / Prompt-Driven)，
//           經人工工程審查、相容性驗證與深度測試 (ZTest) 調校，用於 VR 空間物件透視剪影效果。
// 架構分層: Presentation / URP Custom Shader
// ============================================================================
Shader "Custom/SilhouetteEffect"
{
    Properties
    {
        _LineColor("Line Color", Color) = (0, 1, 0.4, 1)
        _Thickness("Thickness", Float) = 2.0   
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "CONTOUR"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _LineColor;
            float  _Thickness;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 viewDirWS  : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);

                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.normalWS   = normalize(normalWS);
                OUT.viewDirWS  = normalize(GetCameraPositionWS() - worldPos);

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float ndotv = saturate(dot(IN.normalWS, IN.viewDirWS));
                float edge  = pow(1.0 - ndotv, _Thickness);

                float alpha = edge * _LineColor.a;
                float3 rgb  = _LineColor.rgb * edge;

                return float4(rgb, alpha);
            }

            ENDHLSL
        }
    }
}
