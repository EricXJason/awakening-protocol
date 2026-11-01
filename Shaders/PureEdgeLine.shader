// ============================================================================
// 檔案名稱: PureEdgeLine.shader
// 所屬模組: Shaders
// 責任描述: Universal Render Pipeline (URP) 頂點外擴與拓撲純邊緣線著色器。
// 技術實作: 本 Shader 代碼為 AI 輔助生成之客製著色器 (AI-Generated / Prompt-Driven)，
//           經人工工程審查、相容性驗證與 URP 渲染管線參數微調，專供拓撲純邊緣發光視覺表現。
// 架構分層: Presentation / URP Custom Shader
// ============================================================================
Shader "Custom/PureEdgeLine"
{
    Properties
    {
        [MainColor] _LineColor ("Line Color", Color) = (1, 0.6, 0.1, 1)
        _Intensity ("Intensity", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off           
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        
        Offset -1, -1 

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _LineColor;
                float  _Intensity;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID 
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT); 
                
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionCS = TransformWorldToHClip(worldPos);
                
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN); 
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float3 finalColor = _LineColor.rgb * _Intensity;
                return float4(finalColor, _LineColor.a);
            }
            ENDHLSL
        }
    }
}