Shader "Hovl/Particles/Blend_TwoSides_URP"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Float) = 0.5
        _MainTex("Main Tex", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _FrontFacesColor("Front Faces Color", Color) = (0,0.2313726,1,1)
        _BackFacesColor("Back Faces Color", Color) = (0.1098039,0.4235294,1,1)
        _Emission("Emission", Float) = 2
        [Toggle]_UseFresnel("Use Fresnel?", Float) = 1
        [Toggle]_SeparateFresnel("SeparateFresnel", Float) = 0
        _SeparateEmission("Separate Emission", Float) = 2
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _Fresnel("Fresnel", Float) = 1
        _FresnelEmission("Fresnel Emission", Float) = 1
        [Toggle]_UseCustomData("Use Custom Data?", Float) = 0
        [HideInInspector] _texcoord("", 2D) = "white" {}
        [HideInInspector] _tex4coord("", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        
        LOD 100

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            // 使用URP的雾效宏替换
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 uv4 : TEXCOORD1;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                half4 color : COLOR;
                float2 uv : TEXCOORD3;
                float4 uv4 : TEXCOORD4;
                // 使用URP的雾效变量
                half fogFactor : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
            float _Cutoff;
            float4 _MainTex_ST;
            float4 _Mask_ST;
            float4 _Noise_ST;
            float4 _SpeedMainTexUVNoiseZW;
            float4 _FrontFacesColor;
            float4 _BackFacesColor;
            float _Emission;
            float _UseFresnel;
            float _SeparateFresnel;
            float _SeparateEmission;
            float4 _FresnelColor;
            float _Fresnel;
            float _FresnelEmission;
            float _UseCustomData;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Mask);
            SAMPLER(sampler_Mask);
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;
                
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.color = input.color;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv4 = input.uv4 * _Noise_ST.xyzw + _Noise_ST.xyzw;
                
                // 计算URP的雾效因子
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Fresnel计算
                float fresnelNdotV = dot(input.normalWS, input.viewDirWS);
                float fresnelTerm = pow(1.0 - saturate(fresnelNdotV), _Fresnel);
                
                // 正面颜色带菲涅尔
                float4 frontColor = _FrontFacesColor * (1.0 - fresnelTerm) + (_FresnelEmission * _FresnelColor * fresnelTerm);
                frontColor = lerp(_FrontFacesColor, frontColor, _UseFresnel);
                
                // 背面检测
                float dotResult = dot(input.normalWS, input.viewDirWS);
                float backFaceBlend = (1.0 + (sign(dotResult) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0));
                
                // 混合前后颜色
                float4 lerpResult = lerp(frontColor, _BackFacesColor, backFaceBlend);
                
                // 主纹理动画
                float2 mainTexUV = input.uv + (float2(_SpeedMainTexUVNoiseZW.x, _SpeedMainTexUVNoiseZW.y) * _Time.y);
                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainTexUV);
                
                // 发射光计算
                float4 emission;
                if (_SeparateFresnel > 0.5)
                {
                    emission = (lerpResult + (_FresnelColor * mainTex * _SeparateEmission)) * _Emission * input.color * input.color.a;
                }
                else
                {
                    emission = lerpResult * _Emission * input.color * input.color.a * mainTex;
                }
                
                // 遮罩和噪声计算
                float2 noiseUV = input.uv4.xy + (_Time.y * float2(_SpeedMainTexUVNoiseZW.z, _SpeedMainTexUVNoiseZW.w)) + input.uv4.w;
                float4 mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, TRANSFORM_TEX(input.uv, _Mask));
                float4 noise = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, noiseUV);
                float customData = lerp(1.0, input.uv4.z, _UseCustomData);
                float alphaClip = mask.r * noise.r * customData;
                
                clip(alphaClip - _Cutoff);
                
                half4 color = half4(emission.rgb, 1.0);
                
                // 应用URP雾效
                color.rgb = MixFog(color.rgb, input.fogFactor);
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
}