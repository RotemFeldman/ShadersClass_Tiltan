Shader "Custom/SimpleURPDisplacement"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _DisplacementMap ("Displacement Map", 2D) = "gray" {}
        _DisplacementStrength ("Displacement Strength", Range(0, 10)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_DisplacementMap);
            SAMPLER(sampler_DisplacementMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _DisplacementMap_ST;
                half4 _BaseColor;
                float _DisplacementStrength;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Sample displacement map
                float2 displacementUV = TRANSFORM_TEX(input.uv, _DisplacementMap);
                float4 dispColor = SAMPLE_TEXTURE2D_LOD(_DisplacementMap, sampler_DisplacementMap, displacementUV, 0);
                float displacement = (dispColor.r + dispColor.g + dispColor.b) / 3.0;
                
                // Apply displacement
                float3 positionOS = input.positionOS.xyz + input.normalOS * displacement * _DisplacementStrength;
                
                // Transform position and normal
                VertexPositionInputs positionInputs = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample base color
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Get main light
                Light mainLight = GetMainLight();
                
                // Basic diffuse lighting
                float3 normalWS = normalize(input.normalWS);
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 ambient = SampleSH(normalWS) * 0.5;
                float3 diffuse = mainLight.color * NdotL;
                
                float3 finalColor = baseColor.rgb * (ambient + diffuse);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        // This simple version doesn't include shadow caster or depth only passes
        // Add them if needed for proper shadow casting
    }
}