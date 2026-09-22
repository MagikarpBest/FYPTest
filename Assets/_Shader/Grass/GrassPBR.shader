Shader "SigmaShader/GrassPBR"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.25, 0.55, 0.12, 1)
        _BottomColor("Bottom Color", Color) = (0.08, 0.25, 0.04, 1)
        _ColorVariation("Color Variation", Color) = (1, 1, 1, 1)
        _ColorNoiseIntensity("Color Noise Intensity", Float) = 1
    	_ColorNoiseScale("Color Noise Scale", Float) = 1
        _Roughness("Roughness", Range(0.0, 1.0)) = 1
        _WindTexture("Wind Texture", 2D) = "white" {}
        _WindDirection("Wind Direction", Vector) = (1, 0 ,0)
        _WindStrength("Wind Strength", Float) = 0.5
        _WindSpeed("Wind Speed", Float) = 1
	    _WindScale("Wind Scale", Float) = 1
	    _GrassBend("Grass Bend", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline" 
			"RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        //Universal Forward
        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            Cull Off
            ZTest LEqual
            Zwrite On
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Required: StructuredBuffer<GrassData> below needs DX11+ feature level.
            // Target 2.5 (Unity's default) doesn't support StructuredBuffer in vert/frag shaders.
            // Do not lower this unless you remove the _GrassDataBuffer usage.
            #pragma target 4.5 
            
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP // Use _CLUSTER_LIGHT_LOOP in Unity 6.1 and above.
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "GrassCommon.hlsl"
			#include "Assets/_Shader/SigmaPBR/HLSL/SigmaPBRCommon.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
			float4 _TopColor;
			float4 _BottomColor;
			float4 _ColorVariation;
			float _ColorNoiseIntensity;
			float _ColorNoiseScale;
			float _Roughness;
			CBUFFER_END
			            
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 dynamicLightmapUV : TEXCOORD2;
                float4 color : COLOR; //vertex color
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 viewWS : TEXCOORD3;
                float2 dynamicLightmapUV : TEXCOORD4;
                float edgeMask : TEXCOORD5;
            	float4 tangentWS : TEXCOORD6;
            	float3 meshNormal : TEXCOORD7;
            };
            
            v2f vert(appdata v, uint instanceID : SV_INSTANCEID)
            {
                v2f o = (v2f)0;
                o.uv = v.uv;
                
                o.positionWS = GetGrassPosition(v.positionOS, o.uv, instanceID);
                o.positionCS = TransformWorldToHClip(o.positionWS);
            	
                o.normalWS = normalize(_GrassDataBuffer[instanceID].up);
            	o.meshNormal = GetMeshNormal(v.normalOS, instanceID);
            	
                o.viewWS = GetWorldSpaceViewDir(o.positionWS);
                o.dynamicLightmapUV = v.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                
                o.edgeMask = v.color.r; 
                o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), v.tangentOS.w);
                return o;
            }
            
            float4 frag(v2f i, bool isFrontFace : SV_IsFrontFace) : SV_TARGET
            {
					float3 normalWS = NormalizeNormalPerPixel(i.normalWS);
            		float3 meshNormal = NormalizeNormalPerPixel(i.meshNormal);

            		if (!isFrontFace)
					meshNormal = -meshNormal;
            	
                	float3 viewDirWS = normalize(i.viewWS);
                	float3 viewDirTS = GetViewDirectionTangentSpace(i.tangentWS, i.normalWS, viewDirWS);
                	float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
					float4 shadowMask = SAMPLE_SHADOWMASK(i.dynamicLightmapUV);
            	
                	float3 baseColor = lerp(_BottomColor.rgb , _TopColor.rgb, i.uv.y);
	                float  colorNoise = SimplexNoise(i.positionWS.xz * _ColorNoiseScale);
	                colorNoise = colorNoise * 0.5 + 0.5;
	                colorNoise *= _ColorNoiseIntensity;
	                baseColor = lerp(baseColor, _ColorVariation, colorNoise);
            	
            		float roughness = max(_Roughness, 0.04);
            		float metallic = 0;
            		float3 F0 = lerp(0.04, baseColor, metallic);
					half oneMinusReflectivity = OneMinusReflectivityMetallic(metallic); 
            	
                	//Main light
                	Light mainLight = GetMainLight(shadowCoord);
					float3 lightColor = mainLight.distanceAttenuation * mainLight.shadowAttenuation * mainLight.color;
            	
            		#if defined(_SCREEN_SPACE_OCCLUSION)
		                AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS));
		                lightColor *= aoFactor.directAmbientOcclusion;
		            #endif
                		
                	float3 halfVector = normalize(mainLight.direction + viewDirWS);
                	
                	//Direct light
					//Cook-Torrance BRDF
            	
                	//Specular
            		float heightMask = pow(saturate(i.uv.y), 2.0);
                	float3 specular = SpecularGGX(mainLight.direction, normalWS, viewDirWS, halfVector, F0, roughness) * heightMask;
           
                	//Diffuse
                	float3 diffuse = baseColor * oneMinusReflectivity;
            	
                	float NdotL = saturate(dot(normalWS, mainLight.direction));
                	float3 directLight = (diffuse + specular) * lightColor * NdotL;
                	
                	//Indirect light
                	float NdotV = saturate(dot(normalWS, viewDirWS)); //no single light dir/half vector we can use since its from all angles
                	half3 R = reflect(-viewDirWS, normalWS); 
                	
                	//Indirect specular
                	//The Split Sum: 1nd Stage
                	half3 envSpecularPrefilted = GlossyEnvironmentReflection(R, i.positionWS, roughness, 1.0h, GetNormalizedScreenSpaceUV(i.positionCS));
                	
                	//The Split Sum: 2nd Stage
                	float2 envBRDF = EnvBRDFApprox_UE4(roughness, NdotV);
                	
            		#if defined(_SCREEN_SPACE_OCCLUSION)
            			float specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, aoFactor.indirectAmbientOcclusion, roughness);
						float3 specularAO = GTAOMultiBounce(specularOcclusion, F0);
            			float3 diffuseAO = GTAOMultiBounce(aoFactor.indirectAmbientOcclusion, surface.albedo);
					#endif
            		float3 specularAO = 1.0;
            		float3 diffuseAO = 1.0;
            	
                	float3 specularIndirect = envSpecularPrefilted * (F0 * envBRDF.r + envBRDF.g) * specularAO * heightMask;
                	
                	//Indirect diffuse
                	float3 irradianceSH = SampleSH(normalWS); //irradiance spherical harmonics
                	float3 diffuseIndirect = irradianceSH * baseColor * oneMinusReflectivity * diffuseAO;
                	
                	float3 indirectLight = diffuseIndirect + specularIndirect;
            	
            	
            	#ifdef _ADDITIONAL_LIGHTS
	                InputData inputData = (InputData)0;
	                inputData.positionWS = i.positionWS;
	                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                	uint lightCount = GetAdditionalLightsCount();

	                LIGHT_LOOP_BEGIN(lightCount)

	                    Light light = GetAdditionalLight(lightIndex, i.positionWS, shadowMask);
            			float3 lightColorAdd = light.distanceAttenuation * light.shadowAttenuation * light.color;
            	
            			#if defined(_SCREEN_SPACE_OCCLUSION)
            				AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(GetNormalizedScreenSpaceUV(i.positionCS));
				            lightColorAdd *= aoFactor.directAmbientOcclusion;
                		#endif
            	
                		float3 halfVectorAdd = normalize(light.direction + viewDirWS);
                	
                		//Specular
                		float3 specularAdd = SpecularGGX(light.direction, normalWS, viewDirWS, halfVectorAdd, F0, roughness);
       
                		//Diffuse
                		float3 diffuseAdd = baseColor * oneMinusReflectivity;
                
                		float NdotLAdd = saturate(dot(normalWS, light.direction));
                		
                		directLight += (diffuseAdd + specularAdd) * lightColorAdd * NdotLAdd;
	                LIGHT_LOOP_END
#endif
            	
            		float3 finalColor = directLight + indirectLight;
					return float4(finalColor, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            Cull Off
            Zwrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "GrassCommon.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            v2f depthOnlyVert(appdata v, uint instanceID : SV_INSTANCEID)
            {
                v2f o = (v2f)0;

                o.positionWS = GetGrassPosition(v.positionOS, v.uv, instanceID);
                o.positionCS = TransformWorldToHClip(o.positionWS);

                return o;
            }

            float depthOnlyFrag(v2f i) : SV_TARGET
            {
                return i.positionCS.z;
            }

            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            Cull Off
            Zwrite On

            HLSLPROGRAM
            #pragma vertex depthNormalsVert
            #pragma fragment depthNormalsFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "GrassCommon.hlsl"
            
            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                 float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            v2f depthNormalsVert(appdata v, uint instanceID : SV_INSTANCEID)
            {
                v2f o = (v2f)0;
                
                o.positionWS = GetGrassPosition(v.positionOS, v.uv, instanceID);
                o.positionCS = TransformWorldToHClip(o.positionWS);
            	o.normalWS = normalize(_GrassDataBuffer[instanceID].up);

                return o;
            }

            float4 depthNormalsFrag(v2f i) : SV_TARGET
            {
                float3 normalWS = NormalizeNormalPerPixel(i.normalWS);
                
                return float4(normalWS, 0.0f);
            }

            ENDHLSL
        }
    }
}


