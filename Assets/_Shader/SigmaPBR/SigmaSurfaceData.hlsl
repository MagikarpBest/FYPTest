#ifndef _INCLUDE_SIGMASURFACEDATA
#define _INCLUDE_SIGMASURFACEDATA
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
float _Surface;
float _Cutoff;
float4 _BaseColor;
float4 _BaseTexture_ST;
float _TriplanarTile;
float _TriplanarBlend;
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

struct SigmaSurfaceParameters
{
    float2 uv;   
    float3 positionWS;
    float3 normalWS;
    float4 tangentWS;  
    float3 viewDirWS;
    float2 screenUV;
};

struct SigmaSurfaceData
{
    float3 albedo;
    float alpha;
    float3 normal;
    float metallic;
    float3 specular;
    float smoothness;
    float occlusion;
    float3 emission;
};

struct appdata
{
    float2 uv : TEXCOORD0;
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 dynamicLightmapUV : TEXCOORD1;
};

struct v2f
{
    float4 positionCS : SV_Position;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 tangentWS : TEXCOORD3;
    float3 viewWS : TEXCOORD4;
    float2 dynamicLightmapUV : TEXCOORD5;
};

void InitSurfaceParameters(v2f i, out SigmaSurfaceParameters sp)
{
    sp.uv = i.uv;  
    sp.positionWS = i.positionWS;
    sp.normalWS = NormalizeNormalPerPixel(i.normalWS);
    sp.tangentWS = float4(normalize(i.tangentWS.xyz), i.tangentWS.w);
    sp.viewDirWS = normalize(i.viewWS);
    sp.screenUV = GetNormalizedScreenSpaceUV(i.positionCS);
}

struct TriplanarUV 
{
    float2 x, y, z;
};


TriplanarUV GetTriplanarUV (SigmaSurfaceParameters sp) 
{
    TriplanarUV triUV;
    float3 p = sp.positionWS * _TriplanarTile;
    triUV.x = p.zy;
    triUV.y = p.xz;
    triUV.z = p.xy;
    
    //prevent mirror
    if (sp.normalWS.x < 0) 
    {
        triUV.x.x = -triUV.x.x;
    }
    if (sp.normalWS.y < 0) 
    {
        triUV.y.x = -triUV.y.x;
    }
    if (sp.normalWS.z >= 0) 
    {
        triUV.z.x = -triUV.z.x;
    }
    
    return triUV;
}

float3 GetTriplanarWeights(SigmaSurfaceParameters sp) 
{
    float3 triW = abs(sp.normalWS);
    return triW / (triW.x + triW.y + triW.z);
}

float4 GetBaseColor(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = GetTriplanarUV(sp);
        float3 triW = GetTriplanarWeights(sp);

        float4 baseColorX = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, triUV.x);
        float4 baseColorY = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, triUV.y);
        float4 baseColorZ = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, triUV.z);

        float4 baseColor = baseColorX * triW.x + baseColorY * triW.y + baseColorZ * triW.z;
    #else
        float4 baseColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, sp.uv);
    #endif

    return baseColor * _BaseColor;
}

float3 BlendTriplanarNormal(float3 mappedNormal, float3 surfaceNormal) 
{
    float3 n;
    n.xy = mappedNormal.xy + surfaceNormal.xy;
    n.z = mappedNormal.z * surfaceNormal.z;
    return n;
}

float3 GetNormal(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = GetTriplanarUV(sp);
        float3 triW = GetTriplanarWeights(sp);
        
        float3 normalTS_X = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, triUV.x), _NormalStrength);
        float3 normalTS_Y = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, triUV.y), _NormalStrength);
        float3 normalTS_Z = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, triUV.z), _NormalStrength);
        
        //prevent mirror
        if (sp.normalWS.x < 0) 
        {
            normalTS_X.x = - normalTS_X.x;
        }
        if (sp.normalWS.y < 0) 
        {
            normalTS_Y.x = -normalTS_Y.x;
        }
        if (sp.normalWS.z >= 0) 
        {
            normalTS_Z.x = -normalTS_Z.x;
        }
        
        //Whiteout blending
        //assumes Z is pointing up. So convert the surface normal to the projected space, 
        //perform the blend in this tangent space, then convert the result to world space.
        float3 normalWS_X =
            BlendTriplanarNormal(normalTS_X, sp.normalWS.zyx).zyx;
        float3 normalWS_Y =
            BlendTriplanarNormal(normalTS_Y, sp.normalWS.xzy).xzy;  
        float3 normalWS_Z =
            BlendTriplanarNormal(normalTS_Z, sp.normalWS);
        
        float3 normal = normalWS_X * triW.x + normalWS_Y * triW.y + normalWS_Z * triW.z;
    
    #else
        float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, sampler_NormalTexture, sp.uv), _NormalStrength);
        normalTS = normalize(normalTS);
                	    
        float3 binormalWS = cross(sp.normalWS, sp.tangentWS.xyz) * sp.tangentWS.w * unity_WorldTransformParams.w;
                	    
        float3 normal = normalize(
        normalTS.x * sp.tangentWS.xyz +
        normalTS.y * binormalWS +
        normalTS.z * sp.normalWS);	
    
    #endif
    
    return normalize(normal);
}

#ifdef _SPECULAR_SETUP
    float GetSpecular(SigmaSurfaceParameters sp)
    {
        #ifdef _TRIPLANAR_MAPPING
            TriplanarUV triUV = GetTriplanarUV(sp);
            float3 triW = GetTriplanarWeights(sp);
            
            float3 specularProjX = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, triUV.x).rgb;
            float3 specularProjY = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, triUV.y).rgb;
            float3 specularProjZ = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, triUV.z).rgb;

            float3 specular = specularProjX * triW.x + specularProjY * triW.y + specularProjZ * triW.z;
        #else
            float3 specular = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, sp.uv).rgb;
        #endif
        
        return specular * _SpecularColor;
    }
#else
    float GetMetallic(SigmaSurfaceParameters sp)
    {
        #ifdef _TRIPLANAR_MAPPING
            TriplanarUV triUV = GetTriplanarUV(sp);
            float3 triW = GetTriplanarWeights(sp);
        
            float metallicX = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, triUV.x).r;
            float metallicY = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, triUV.y).r;
            float metallicZ = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, triUV.z).r;

            float metallic = metallicX * triW.x + metallicY * triW.y + metallicZ * triW.z;
        #else
            float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, sp.uv).r;
        #endif
        
        return metallic * _Metallic;
    }
#endif

float GetSmoothness(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = GetTriplanarUV(sp);
        float3 triW = GetTriplanarWeights(sp);
    
        float smoothnessX = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, triUV.x).r;
        float smoothnessY = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, triUV.y).r;
        float smoothnessZ = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, triUV.z).r;

        float smoothness = smoothnessX * triW.x + smoothnessY * triW.y + smoothnessZ * triW.z;
    #else
        float smoothness = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, sp.uv).r;
    #endif
    
    #ifdef _CONVERT_FROM_ROUGHNESS 
        return 1.0 - (smoothness * _Smoothness);
    #endif
    
    return smoothness * _Smoothness;
}

float GetOcclusion(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = GetTriplanarUV(sp);
        float3 triW = GetTriplanarWeights(sp);
    
        float occlusionX = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, triUV.x).r;
        float occlusionY = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, triUV.y).r;
        float occlusionZ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, triUV.z).r;

        float occlusion = occlusionX * triW.x + occlusionY * triW.y + occlusionZ * triW.z;
    #else
        float occlusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, sp.uv).r;
    #endif
    
    return lerp(1.0f, occlusion, _OcclusionStrength);
}

float3 GetEmissive(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = GetTriplanarUV(sp);
        float3 triW = GetTriplanarWeights(sp);
    
        float3 emissionX = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, triUV.x).rgb;
        float3 emissionY = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, triUV.y).rgb;
        float3 emissionZ = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, triUV.z).rgb;

        float3 emission = emissionX * triW.x + emissionY * triW.y + emissionZ * triW.z;
    #else
        float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, sp.uv).rgb;
    #endif
    
    return emission * _EmissionColor;
}

void InitSurfaceData(SigmaSurfaceParameters sp, out SigmaSurfaceData surface)
{
    float4 baseColor = GetBaseColor(sp);
    surface.albedo = baseColor.rgb;
    surface.alpha = baseColor.a;
    surface.normal = GetNormal(sp);
    #ifdef _SPECULAR_SETUP
    surface.specular = GetSpecular(sp);
    surface.metallic = 0;
    #else
    surface.metallic = GetMetallic(sp);
    surface.specular = 0;
    #endif
    surface.smoothness = GetSmoothness(sp);
    surface.occlusion = GetOcclusion(sp);
                	
    #if defined(_SCREEN_SPACE_OCCLUSION)
    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(sp.screenUV);
    surface.occlusion = min(surface.occlusion, aoFactor.indirectAmbientOcclusion);
    #endif
                	
    surface.emission = GetEmissive(sp);
}
#endif
