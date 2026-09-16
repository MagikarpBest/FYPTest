Shader "SigmaShader/SigmaPBR"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
    	
    	[Toggle(_SPECULAR_SETUP)] _UseSpecularSetup("Use Specular Setup", Integer) = 0

		[NoScaleOffset] _MetallicMap("Metallic", 2D) = "white" {}
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0

		[NoScaleOffset] _SpecularMap("SpecularMap", 2D) = "white" {}
		_SpecularColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)

		[NoScaleOffset] _SmoothnessMap("Smoothness Map", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
    	[Toggle(_CONVERT_FROM_ROUGHNESS)] _ConvertFromRoughness("Convert From Roughness", Integer) = 0

		[NoScaleOffset] [Normal] _NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range(0.0, 2.0)) = 1.0

		[NoScaleOffset] _HeightMap("Height Map", 2D) = "white" {}
		_HeightMapStrength("Height Map Strength", Range(0.0, 0.1)) = 0.0

		[NoScaleOffset] _OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

		[NoScaleOffset] _EmissionMap("Emission Map", 2D) = "white" {}
		[HDR] _EmissionColor("Emission Color", Color) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            ZWrite On
            
            ZTest LEqual

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
				#pragma multi_compile_fragment _ _LIGHT_COOKIES
				#pragma multi_compile _ _ADDITIONAL_LIGHTS
				#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
				#pragma multi_compile _ _CLUSTER_LIGHT_LOOP
       
	            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
				#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
				#pragma multi_compile_fragment _ _REFLECTION_PROBE_ATLAS
	            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                
                #pragma shader_feature_local _ _CONVERT_FROM_ROUGHNESS
                #pragma shader_feature_local _ _SPECULAR_SETUP

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "PBRCommon.hlsl"
                
                //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/BSDF.hlsl"

                CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
				float _NormalStrength;
				float _Metallic;
				float3 _SpecularColor;
				float _Smoothness;
				float _HeightMapStrength;
				float _OcclusionStrength;
				float3 _EmissionColor;
                CBUFFER_END

				TEXTURE2D(_BaseTexture);
				SAMPLER(sampler_BaseTexture);

                #ifdef _SPECULAR_SETUP
				TEXTURE2D(_SpecularMap);
				SAMPLER(sampler_SpecularMap);
				#else
				TEXTURE2D(_MetallicMap);
				SAMPLER(sampler_MetallicMap);
                #endif
                
				TEXTURE2D(_SmoothnessMap);
				SAMPLER(sampler_SmoothnessMap);

				TEXTURE2D(_NormalTexture);
				SAMPLER(sampler_NormalTexture);

				TEXTURE2D(_HeightMap);
				SAMPLER(sampler_HeightMap);

				TEXTURE2D(_OcclusionMap);
				SAMPLER(sampler_OcclusionMap);

				TEXTURE2D(_EmissionMap);
				SAMPLER(sampler_EmissionMap);

                struct appdata
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					float2 dynamicLightmapUV : TEXCOORD2;

                };

                struct v2f
                {
                    float4 positionCS : SV_Position;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    float3 positionWS : TEXCOORD2;
                    float3 viewWS : TEXCOORD3;
					float4 tangentWS : TEXCOORD4;
					float2 dynamicLightmapUV : TEXCOORD5;
                };

                v2f vert(appdata v)
                {
                    v2f o = (v2f)0;

                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                    o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                    o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                    o.viewWS = GetWorldSpaceViewDir(o.positionWS);
					o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), v.tangentOS.w);
					o.dynamicLightmapUV = v.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;

                    return o;
                }
                
                float4 frag(v2f i) : SV_Target
                {
                	float3 normalWS = NormalizeNormalPerPixel(i.normalWS);
                	float3 viewDirWS = normalize(i.viewWS);
                	float3 viewDirTS = GetViewDirectionTangentSpace(i.tangentWS, i.normalWS, viewDirWS);
                	float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
					float4 shadowMask = SAMPLE_SHADOWMASK(i.dynamicLightmapUV);
                	
                	i.uv += ParallaxMapping(TEXTURE2D_ARGS(_HeightMap, sampler_HeightMap), viewDirTS, _HeightMapStrength, i.uv);
                	
                	float4 baseColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv) * _BaseColor;
                	
                	//AO
                	float occlusion = lerp(1.0f, SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, i.uv).r, _OcclusionStrength);
                	
                	//Normal
                	float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, i.uv), _NormalStrength);
                	normalTS = normalize(normalTS);
                	
                	float3 binormalWS = cross(normalWS, i.tangentWS.xyz) * i.tangentWS.w * unity_WorldTransformParams.w;
                	
					normalWS = normalize(
                    normalTS.x * i.tangentWS.xyz +
                    normalTS.y * binormalWS +
                    normalTS.z * normalWS);
                	
                	//Emission
                	float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, i.uv).rgb * _EmissionColor;
                	
                	//Specular/Metallic
#ifdef _SPECULAR_SETUP
                	float metallic = 0.0f;
                	float3 specularMap = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, i.uv).rgb * _SpecularColor;
					float3 F0 = specularMap; // specular color is F0 directly
#else 
                	float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, i.uv).r * _Metallic;
					float3 F0 = lerp(0.04, baseColor.rgb, metallic);
#endif
                	
                	//Smoothness/Roughness
#ifdef _CONVERT_FROM_ROUGHNESS
                	float smoothness = (1.0 - SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, i.uv).r) * _Smoothness;
#else
                	float smoothness = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, i.uv).r * _Smoothness;
#endif
                	float roughness = 1.0 - smoothness;
                	roughness = clamp(roughness, 0.04, 0.99);
                	
                	//Main light
                	Light mainLight = GetMainLight(shadowCoord);
					float3 lightColor = mainLight.distanceAttenuation * mainLight.shadowAttenuation * mainLight.color;
                	float3 halfVector = normalize(mainLight.direction + viewDirWS);
                	
                	//Direct light
					//Cook-Torrance BRDF
                	float HdotV = saturate(dot(halfVector, viewDirWS));
                	float3 ks = F_FresnelSchlick(HdotV, F0); //specular coefficient
					float3 kd = 1.0 - ks; //diffuse coefficient
                	kd *= 1.0 - metallic;
                	
                	//Specular
                	float3 specular = Specular_CookTorance(mainLight.direction, normalWS, viewDirWS, halfVector, F0, roughness);
           
                	//Diffuse
                	float3 diffuse = kd * baseColor.rgb;
                	
                	//BRDF = kdfdiffuse + ksfspecular
                	//Cook torrance u get rid of ks in specular cause it already has fresnel so if u dont remove you doubling
                	
                	//Rendering equation
                	float NdotL = saturate(dot(normalWS, mainLight.direction));
                	
                	float3 directLight = (diffuse + specular) * lightColor * NdotL;
                	
                	//Indirect light
                	float NdotV = saturate(dot(normalWS, viewDirWS)); //no single light dir/half vector we can use since its from all angles
                	float3 ksIndirect = F_FresnelSchlickRoughness(NdotV, F0, roughness);
                	float3 kdIndirect = 1.0 - ksIndirect; //diffuse coefficient
                	kdIndirect *= 1.0 - metallic;
                	half3 R = reflect(-viewDirWS, normalWS); 
                	
                	//Indirect specular
                	//The Split Sum: 1nd Stage
                	half3 envSpecularPrefilted = GlossyEnvironmentReflection(R, i.positionWS, roughness, 1.0h, GetNormalizedScreenSpaceUV(i.positionCS));
                	
                	//The Split Sum: 2nd Stage
                	float2 envBRDF = EnvBRDFApprox_UE4(roughness, NdotV);
                	
                	float specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, occlusion, roughness);
					float3 specularAO = GTAOMultiBounce(specularOcclusion, F0);
                	
                	float3 specularIndirect = envSpecularPrefilted * (ksIndirect * envBRDF.r + envBRDF.g) * specularAO;
                	
                	//Indirect diffuse
                	float3 irradianceSH = SampleSH(normalWS); //irradiance spherical harmonics
                	
                	float3 diffuseAO = GTAOMultiBounce(occlusion, baseColor);
                	
                	float3 diffuseIndirect = irradianceSH * kdIndirect * baseColor * diffuseAO;
                	
                	float3 indirectLight = diffuseIndirect + specularIndirect;
                	
#ifdef _ADDITIONAL_LIGHTS
	                InputData inputData = (InputData)0;
	                inputData.positionWS = i.positionWS;
	                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                	uint lightCount = GetAdditionalLightsCount();

	                LIGHT_LOOP_BEGIN(lightCount)

	                    Light light = GetAdditionalLight(lightIndex, i.positionWS, shadowMask);
	                    float3 lightColorAdd = light.distanceAttenuation * light.shadowAttenuation * light.color;
                		float3 halfVectorAdd = normalize(light.direction + viewDirWS);
                		
                		float HdotVAdd = saturate(dot(halfVectorAdd, viewDirWS));
                		float3 ksAdd = F_FresnelSchlick(HdotVAdd, F0); //specular coefficient
						float3 kdAdd = 1.0 - ksAdd; //diffuse coefficient
                		kdAdd *= 1.0 - metallic;
                		
                		//Specular
                		float3 specularAdd = Specular_CookTorance(light.direction, normalWS, viewDirWS, halfVectorAdd, F0, roughness);
                		
                		//Diffuse
                		float3 diffuseAdd = kdAdd * baseColor.rgb;
                		
                		float NdotLAdd = saturate(dot(normalWS, light.direction));
                		
                		directLight += (diffuseAdd + specularAdd) * lightColorAdd * NdotLAdd;
	                LIGHT_LOOP_END
#endif
                	
                	float3 finalColor = emission + directLight + indirectLight;
					return float4(finalColor, baseColor.a);
                }

            ENDHLSL
        }

		Pass
		{
			Tags
            {
                "LightMode" = "ShadowCaster"
            }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma vertex shadowPassVert
			#pragma fragment shadowPassFrag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			float3 _LightDirection;
			float3 _LightPosition;

			struct appdata
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
			};

			struct v2f
			{
				float4 positionCS : SV_POSITION;
			};

			float4 GetShadowPositionHClip(float3 positionOS, float3 normalOS)
			{
				float3 positionWS = TransformObjectToWorld(positionOS);
				float3 normalWS = TransformObjectToWorldNormal(normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
				positionCS = ApplyShadowClamping(positionCS);

				return positionCS;
			}

			v2f shadowPassVert(appdata v)
			{
				v2f o = (v2f)0;

				o.positionCS = GetShadowPositionHClip(v.positionOS, v.normalOS);

				return o;
			}

			float4 shadowPassFrag(v2f i) : SV_TARGET
			{
				return 0;
			}
			ENDHLSL
		}

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask R

            HLSLPROGRAM
                #pragma vertex depthOnlyVert
                #pragma fragment depthOnlyFrag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct appdata
                {
                    float4 positionOS : POSITION;
                };

                struct v2f
                {
                    float4 positionCS : SV_Position;
                };

                v2f depthOnlyVert(appdata v)
                {
                    v2f o = (v2f)0;

                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);

                    return o;
                }

                float depthOnlyFrag(v2f i) : SV_Target
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

            ZWrite On

            HLSLPROGRAM

                #pragma vertex depthNormalVert
                #pragma fragment depthNormalFrag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
				float _NormalStrength;
				float _Metallic;
				float3 _SpecularColor;
				float _Smoothness;
				float _HeightMapStrength;
				float _OcclusionStrength;
				float3 _EmissionColor;
                CBUFFER_END
                
				TEXTURE2D(_NormalTexture);
				SAMPLER(sampler_NormalTexture);

                struct appdata
                {
                    float4 positionOS : POSITION;
					float2 uv : TEXCOORD0;
                    float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
                };

                struct v2f
                {
                    float4 positionCS : SV_Position;
					float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
					float4 tangentWS : TEXCOORD2;
                };

                v2f depthNormalVert(appdata v)
                {
                    v2f o = (v2f)0;

                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
					o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                    float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                    o.normalWS = NormalizeNormalPerVertex(normalWS);
					o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), v.tangentOS.w);

                    return o;
                }

                float4 depthNormalFrag(v2f i) : SV_Target
                {
                    float3 normalWS = NormalizeNormalPerPixel(i.normalWS);

					float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, i.uv), _NormalStrength);
					float3 bitangentWS = cross(normalWS, i.tangentWS.xyz) * i.tangentWS.w * unity_WorldTransformParams.w;

					normalWS = normalize(
						normalTS.x * i.tangentWS.xyz +
						normalTS.y * bitangentWS +
						normalTS.z * normalWS);

                    return float4(normalWS, 0.0f);
                }

            ENDHLSL
        }
    }
}