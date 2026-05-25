Shader "Hovl/Particles/BlendDistort_URP"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _Flow("Flow", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}
        _Color("Color", Color) = (0.5,0.5,0.5,1)
        _Distortionpower("Distortion power", Float) = 0
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 2
        _Opacity("Opacity", Range(0, 3)) = 1
        [Toggle]_Usedepth("Use depth?", Float) = 1
        [Toggle]_Softedges("Soft edges", Float) = 0
        _Depthpower("Depth power", Float) = 1
        [HideInInspector] _texcoord("", 2D) = "white" {}
        [HideInInspector] _tex4coord("", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 uv4 : TEXCOORD1;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv4 : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float4 color : COLOR;
                float fogCoord : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Noise); SAMPLER(sampler_Noise);
            TEXTURE2D(_Flow); SAMPLER(sampler_Flow);
            TEXTURE2D(_Mask); SAMPLER(sampler_Mask);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Noise_ST;
            float4 _Flow_ST;
            float4 _Mask_ST;
            float4 _NormalMap_ST;
            float4 _Color;
            float4 _SpeedMainTexUVNoiseZW;
            float4 _DistortionSpeedXYPowerZ;
            float _Distortionpower;
            float _Emission;
            float _Opacity;
            float _Usedepth;
            float _Softedges;
            float _Depthpower;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv4 = input.uv4;
                output.color = input.color;
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Calculate distortion
                float2 appendResult22 = float2(_SpeedMainTexUVNoiseZW.z, _SpeedMainTexUVNoiseZW.w);
                float2 uv0_NormalMap = input.uv * _NormalMap_ST.xy + _NormalMap_ST.zw;
                float2 panner146 = _Time.y * appendResult22 + uv0_NormalMap;
                float3 distortion = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, panner146), _Distortionpower);
                
                // Screen position with distortion
                float4 screenPos = input.screenPos;
                screenPos.xy += distortion.xy;
                float2 screenUV = screenPos.xy / screenPos.w;
                half3 screenColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV).rgb;
                
                // Main texture animation
                float2 appendResult21 = float2(_SpeedMainTexUVNoiseZW.x, _SpeedMainTexUVNoiseZW.y);
                float2 uv0_MainTex = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 panner107 = _Time.y * appendResult21 + uv0_MainTex;
                
                // Flow distortion
                float2 appendResult100 = float2(_DistortionSpeedXYPowerZ.x, _DistortionSpeedXYPowerZ.y);
                float2 uv0_Flow = input.uv4.xy * _Flow_ST.xy + _Flow_ST.zw;
                float2 panner110 = _Time.y * appendResult100 + uv0_Flow;
                
                float2 uv_Mask = input.uv * _Mask_ST.xy + _Mask_ST.zw;
                float Flowpower102 = _DistortionSpeedXYPowerZ.z;
                
                half4 flowTex = SAMPLE_TEXTURE2D(_Flow, sampler_Flow, panner110);
                half4 maskTex = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv_Mask);
                half4 tex2DNode13 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, panner107 - (flowTex * maskTex * Flowpower102).rg);
                
                // Noise texture
                float2 uv0_Noise = input.uv * _Noise_ST.xy + _Noise_ST.zw;
                float2 panner108 = _Time.y * appendResult22 + uv0_Noise;
                float2 appendResult160 = float2(input.uv4.w, 0.0);
                half4 tex2DNode14 = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, panner108 + appendResult160);
                
                // Alpha calculation
                float temp_output_88_0 = (tex2DNode13.a * tex2DNode14.a * _Color.a * input.color.a * _Opacity);
                float temp_output_151_0 = saturate(temp_output_88_0);
                
                // Depth fade
                float2 screenUVNorm = screenUV;
                #if UNITY_UV_STARTS_AT_TOP
                screenUVNorm.y = 1.0 - screenUVNorm.y;
                #endif
                
                float sceneDepth = LinearEyeDepth(SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, screenUVNorm).r, _ZBufferParams);
                float surfaceDepth = LinearEyeDepth(screenPos.z / screenPos.w, _ZBufferParams);
                float depthFade = saturate((sceneDepth - surfaceDepth) / _Depthpower);
                
                // Soft edges based on normal
                float dotResult163 = dot(input.normalWS, normalize(input.viewDirWS));
                float temp_output_185_0 = pow(dotResult163, 3.0) * 5.0;
                float dotResult171 = dot(input.normalWS, normalize(input.viewDirWS));
                float lerpResult181 = lerp(temp_output_185_0, 
                                          (0.0 + (temp_output_185_0 - 0.0) * (1.0 - 0.0) / (-1.0 - 0.0)), 
                                          (1.0 + (sign(dotResult171) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
                float clampResult186 = clamp(lerpResult181, 0.0, 1.0);
                
                // Final alpha
                float baseAlpha = lerp(temp_output_151_0, (temp_output_151_0 * depthFade), _Usedepth);
                float finalAlpha = lerp(baseAlpha, (baseAlpha * clampResult186), _Softedges);
                
                // Color calculation
                half3 temp_output_140_0 = (tex2DNode13 * tex2DNode14 * _Color * input.color * _Emission * temp_output_88_0).rgb;
                float W158 = input.uv4.z;
                half3 lerpResult157 = lerp((screenColor + temp_output_140_0), (screenColor * temp_output_140_0), W158);
                
                half4 color = half4(lerpResult157, finalAlpha);
                color.rgb = MixFog(color.rgb, input.fogCoord);
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
}