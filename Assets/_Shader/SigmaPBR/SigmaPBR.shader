Shader "SigmaShader/SigmaPBR"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
    	[Toggle(_TRIPLANAR_MAPPING)] _UseTriplanarMapping("Use Triplanar Mapping", Integer) = 0
    	_TriplanarTile("Triplanar Tile", Float) = 0.1
    	_TriplanarBlend("Triplanar Blend", Float) = 1
    	
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
    	
    	[HideInInspector] _Surface("_Surface", Float) = 0
		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[HideInInspector] _SrcBlend("_SrcBlend", Float) = 1
		[HideInInspector] _DstBlend("_DstBlend", Float) = 0
		[HideInInspector] _SrcBlendAlpha("_SrcBlendAlpha", Float) = 1
		[HideInInspector] _DstBlendAlpha("_DstBlendAlpha", Float) = 0
		[HideInInspector] _ZWrite("_ZWrite", Float) = 1
		[HideInInspector] _ZTest("_ZTest", Float) = 4
		[HideInInspector] _Cull("_Cull", Float) = 2
		[HideInInspector] _AlphaToMask("_AlphaToMask", Float) = 0
    	
    	[HideInInspector] _CastShadows("_CastShadows", Float) = 1
		[HideInInspector] _ReceiveShadows("Receive Shadows", Float) = 1.0
		[HideInInspector] _Blend("_Blend", Float) = 0
		[HideInInspector] _AlphaClip("_AlphaClip", Float) = 0
		[HideInInspector] _ZWriteControl("_ZWriteControl", Float) = 0
		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl("_QueueControl", Float) = 0
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

            Cull [_Cull]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
			AlphaToMask [_AlphaToMask]
			
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
				#pragma target 4.5
                
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
				#pragma multi_compile_fragment _ _LIGHT_COOKIES
				#pragma multi_compile _ _ADDITIONAL_LIGHTS
				#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
				#pragma multi_compile _ _CLUSTER_LIGHT_LOOP
       
	            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
				#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
	            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                
                #pragma shader_feature_local _ _CONVERT_FROM_ROUGHNESS
                #pragma shader_feature_local _ _SPECULAR_SETUP
                #pragma shader_feature_local _ _TRIPLANAR_MAPPING
                #pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF
                #pragma shader_feature_local _ _ALPHATEST_ON
                
                #pragma multi_compile_instancing
	            #pragma instancing_options renderinglayer
	            #pragma multi_compile _ DOTS_INSTANCING_ON
                
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "SigmaPBRCommon.hlsl"
                #include "SigmaSurfaceData.hlsl"
                //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/BSDF.hlsl"
                
                v2f vert(appdata v)
                {
                    v2f o = (v2f)0;

                	o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
					o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
					o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), v.tangentOS.w);
                    o.viewWS = GetWorldSpaceViewDir(o.positionWS);
					o.dynamicLightmapUV = v.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;

                    return o;
                }
                
                float4 frag(v2f i) : SV_Target
                {
                	SigmaSurfaceParameters sp;
                	InitSurfaceParameters(i, sp);
                	
                	SigmaSurfaceData surface;
                	InitSurfaceData(sp, surface);
                	
                	#ifdef _ALPHATEST_ON
                		if (surface.alpha < _Cutoff)
                		{
                			discard;
                		}
                	#endif
                	
                	float3 normalWS = surface.normal;
                	float3 viewDirWS = normalize(i.viewWS);
                	float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
					float4 shadowMask = SAMPLE_SHADOWMASK(i.dynamicLightmapUV);
                	
                	float roughness = 1.0 - surface.smoothness;
                	roughness = max(roughness, 0.085);
                	
                	#ifdef _SPECULAR_SETUP
                		float3 F0 = surface.specular;
						half oneMinusReflectivity = 1.0;
                	#else
                		//0.04 is kDieletricSpec.a
                		float3 F0 = lerp(0.04, surface.albedo, surface.metallic);
					    half oneMinusReflectivity = OneMinusReflectivityMetallic(surface.metallic); 
                	#endif
                	
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
					
					//Unity does not use this I suppose?
					//Use OneMinusReflectivityMetallic(surface.metallic);
					//Also unity does not / PI for diffuse
					
                	// float HdotV = saturate(dot(halfVector, viewDirWS));
                	// float3 ks = F_FresnelSchlick(HdotV, F0); //specular coefficient
                	//  float3 kd = 1.0 - ks; //diffuse coefficient
                	// kd *= 1.0 - surface.metallic;
                	
                	//Specular
                	float3 specular = SpecularGGX(mainLight.direction, normalWS, viewDirWS, halfVector, F0, roughness) * PI;
           
                	//Diffuse
                	//float3 diffuse = kd * albedo / PI 
                	float3 diffuse = surface.albedo * oneMinusReflectivity;
                	
                	//BRDF = kdfdiffuse + ksfspecular
                	//Cook torrance u get rid of ks in specular cause it already has fresnel so if u dont remove you doubling
                	
                	//Rendering equation
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
                	
                	float specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(NdotV, surface.occlusion, roughness);
					float3 specularAO = GTAOMultiBounce(specularOcclusion, F0);
                	
                	float3 specularIndirect = envSpecularPrefilted * (F0 * envBRDF.r + envBRDF.g) * specularAO;
                	
                	//Indirect diffuse
                	float3 irradianceSH = SampleSH(normalWS); //irradiance spherical harmonics
                	
                	float3 diffuseAO = GTAOMultiBounce(surface.occlusion, surface.albedo);
     
                	float3 diffuseIndirect = irradianceSH * surface.albedo * oneMinusReflectivity * diffuseAO;
                	
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
                			float3 specularAdd = SpecularGGX(light.direction, normalWS, viewDirWS, halfVectorAdd, F0, roughness) * PI;

                			//Diffuse
                			float3 diffuseAdd = surface.albedo * oneMinusReflectivity;
                	
                			float NdotLAdd = saturate(dot(normalWS, light.direction));
                			
                			directLight += (diffuseAdd + specularAdd) * lightColorAdd * NdotLAdd;
		                LIGHT_LOOP_END
					#endif
                	
                	float3 finalColor = surface.emission + directLight + indirectLight;
                	float alpha = OutputAlpha(surface.alpha, IsSurfaceTypeTransparent(_Surface));
                	
					return float4(finalColor, alpha);
                }

            ENDHLSL
        }

		Pass
		{
			Tags
            {
                "LightMode" = "ShadowCaster"
            }

			Cull [_Cull]
			ZTest LEqual
			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma vertex shadowPassVert
			#pragma fragment shadowPassFrag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "SigmaSurfaceData.hlsl"

			#pragma shader_feature_local _ _ALPHATEST_ON
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			float3 _LightDirection;
			float3 _LightPosition;

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
				o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);

				return o;
			}

			float4 shadowPassFrag(v2f i) : SV_TARGET
			{
				SigmaSurfaceParameters sp;
                InitSurfaceParameters(i, sp);
				
				float4 baseColor = GetBaseColor(sp);
				AlphaDiscard(baseColor.a, _Cutoff);
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

            Cull [_Cull]
            ZTest LEqual
            ZWrite On
            ColorMask R

            HLSLPROGRAM
                #pragma vertex depthOnlyVert
                #pragma fragment depthOnlyFrag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "SigmaSurfaceData.hlsl"
				#pragma shader_feature_local _ _ALPHATEST_ON

                v2f depthOnlyVert(appdata v)
                {
                    v2f o = (v2f)0;

                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
					o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);

                    return o;
                }

                float depthOnlyFrag(v2f i) : SV_Target
                {
					SigmaSurfaceParameters sp;
	                InitSurfaceParameters(i, sp);
					
					float4 baseColor = GetBaseColor(sp);
					AlphaDiscard(baseColor.a, _Cutoff);
				
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
                #include "SigmaSurfaceData.hlsl"
                
                v2f depthNormalVert(appdata v)
                {
                    v2f o = (v2f)0;

                    o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                    o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
					o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
					o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), v.tangentOS.w);
                    o.viewWS = GetWorldSpaceViewDir(o.positionWS);

                    return o;
                }
                
                float4 depthNormalFrag(v2f i) : SV_Target
                {
                    SigmaSurfaceParameters sp;
                	InitSurfaceParameters(i, sp);
                	
					float4 baseColor = GetBaseColor(sp);
					AlphaDiscard(baseColor.a, _Cutoff);
				
                	//get surface normal
                	float3 normalWS = GetNormal(sp);
					return float4(normalWS, 0.0);
                }

            ENDHLSL
        }
    }
	CustomEditor "SigmaPBRGUI"
}