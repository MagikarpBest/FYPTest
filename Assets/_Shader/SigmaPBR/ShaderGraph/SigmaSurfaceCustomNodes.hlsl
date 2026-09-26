#ifndef _INCLUDE_SIGMASURFACECUSTOMNODES
#define _INCLUDE_SIGMASURFACECUSTOMNODES
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

void GetEditableSampler_float(out UnitySamplerState samplerOut)
{
    samplerOut.samplerstate = default_sampler_Linear_Repeat;

    #if defined(_TEXTUREFILTER_LINEAR) && defined(_TEXTUREWRAP_REPEAT)
    samplerOut.samplerstate = default_sampler_Linear_Repeat;
    #elif defined(_TEXTUREFILTER_LINEAR) && defined(_TEXTUREWRAP_CLAMP)
    samplerOut.samplerstate = default_sampler_Linear_Clamp;
    #elif defined(_TEXTUREFILTER_POINT) && defined(_TEXTUREWRAP_REPEAT)
    samplerOut.samplerstate = default_sampler_Point_Repeat;
    #elif defined(_TEXTUREFILTER_POINT) && defined(_TEXTUREWRAP_CLAMP)
    samplerOut.samplerstate = default_sampler_Point_Clamp;
    #endif
}

void GetTriplanarUV_float(float3 positionWS, float3 normalWS, float triplanarTile,
    out float2 triUV_X, out float2 triUV_Y, out float2 triUV_Z) 
{
    float3 p = positionWS * triplanarTile;
    triUV_X = p.zy;
    triUV_Y = p.xz;
    triUV_Z = p.xy;
    
    //prevent mirror
    if (normalWS.x < 0) 
    {
        triUV_X.x = -triUV_X.x;
    }
    if (normalWS.y < 0) 
    {
        triUV_Y.x = -triUV_Y.x;
    }
    if (normalWS.z >= 0) 
    {
        triUV_Z.x = -triUV_Z.x;
    }
}

void GetTriplanarViewDir_float(float3 viewDirWS, float3 normalWS,
    out float3 triViewX, out float3 triViewY, out float3 triViewZ)
{
    float3 v = viewDirWS;
    
    //The Z component is perpendicular to the projection plane
    //For triplanar mapping both sides of the plane use the same depth direction
    //so we use its magnitude and remove the sign
    triViewX = float3(v.z, v.y, abs(v.x));
    triViewY = float3(v.x, v.z, abs(v.y));
    triViewZ = float3(v.x, v.y, abs(v.z));
    
    //prevent mirror
    if (normalWS.x < 0) 
    {
        triViewX.x = -triViewX.x;
    }
    if (normalWS.y < 0) 
    {
        triViewY.x = -triViewY.x;
    }
    if (normalWS.z >= 0) 
    {
        triViewZ.x = -triViewZ.x;
    }
}

void GetTriplanarWeights_float(float3 normalWS, float triplanarBlendOffset, float triplanarBlendExponent,
    out float3 triWeights) 
{
    triWeights = abs(normalWS);
    triWeights = saturate(triWeights - triplanarBlendOffset);
    triWeights = pow(triWeights, triplanarBlendExponent);
    
    float sum = triWeights.x + triWeights.y + triWeights.z;
    triWeights = triWeights / max(sum, 1e-5);
}

void GetParallaxOffsetUV_float(float3 viewDirTS, float2 uv, UnityTexture2D heightMap, UnitySamplerState heightSampler, float heightmapStrength,
    out float2 parallaxUV)
{
    parallaxUV = uv;
    
    //#ifdef USE_HEIGHTMAP
        //scale view so that z is 1 no need to /z cause we dont use z
        //offset the z component so it never approaches zero, which would blow up the xy/z division at shallow (grazing) view angles
        //this trades a bit of projection accuracy it warps the perspective slightly for much more stable manageable parallax artifacts at those angles
        //0.42 is Unity's standard-shader value, chosen empirically rather than derived.
        viewDirTS = normalize(viewDirTS);
        
        float parallaxBias = 0.42;
        viewDirTS.xy /= (viewDirTS.z + parallaxBias); 

        float height = SAMPLE_TEXTURE2D(heightMap, heightSampler, uv).g;
        height -= 0.5; //centers height around 0 
        
        parallaxUV.xy += viewDirTS.xy * height * heightmapStrength;
    //#endif
}

void GetParallaxOffsetTriplanarUV_float(float3 positionWS, float3 viewDirWS, float3 normalWS, UnityTexture2D heightMap, UnitySamplerState heightSampler, 
    float heightmapStrength, float triplanarTile,
    out float2 parallaxTriUV_X, out float2 parallaxTriUV_Y, out float2 parallaxTriUV_Z)
{
    float3 triViewX, triViewY, triViewZ;
    GetTriplanarViewDir_float(viewDirWS, normalWS, triViewX, triViewY, triViewZ);
    
    float2 triUV_X, triUV_Y, triUV_Z;
    GetTriplanarUV_float(positionWS, normalWS, triplanarTile, triUV_X, triUV_Y, triUV_Z);
    
    GetParallaxOffsetUV_float(triViewX, triUV_X, heightMap, heightSampler, heightmapStrength, parallaxTriUV_X);
    GetParallaxOffsetUV_float(triViewY, triUV_Y, heightMap, heightSampler, heightmapStrength, parallaxTriUV_Y);
    GetParallaxOffsetUV_float(triViewZ, triUV_Z, heightMap, heightSampler, heightmapStrength, parallaxTriUV_Z);
}

void SigmaTriplanar_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triWeights, UnityTexture2D triTexture, UnitySamplerState triSampler,
    out float4 result)
{
    float4 sampleX = SAMPLE_TEXTURE2D(triTexture, triSampler, triUV_X);
    float4 sampleY = SAMPLE_TEXTURE2D(triTexture, triSampler, triUV_Y);
    float4 sampleZ = SAMPLE_TEXTURE2D(triTexture, triSampler, triUV_Z);
    
    result = sampleX * triWeights.x + sampleY * triWeights.y + sampleZ * triWeights.z;
}

float3 BlendTriplanarNormal(float3 mappedNormal, float3 surfaceNormal) 
{
    float3 n;
    n.xy = mappedNormal.xy + surfaceNormal.xy;
    n.z = mappedNormal.z * surfaceNormal.z;
    return n;
}

void SigmaTriplanarNormal_float(float3 normalWS, float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triWeights, UnityTexture2D normalMap, UnitySamplerState normalSampler, float normalStrength,
    out float3 result)
{
    float3 normalTS_X = UnpackNormalScale(SAMPLE_TEXTURE2D(normalMap, normalSampler, triUV_X), normalStrength);
    float3 normalTS_Y = UnpackNormalScale(SAMPLE_TEXTURE2D(normalMap, normalSampler, triUV_Y), normalStrength);
    float3 normalTS_Z = UnpackNormalScale(SAMPLE_TEXTURE2D(normalMap, normalSampler, triUV_Z), normalStrength);
    
    //prevent mirror
    if (normalWS.x < 0) 
    {
        normalTS_X.x = - normalTS_X.x;
    }
    if (normalWS.y < 0) 
    {
        normalTS_Y.x = -normalTS_Y.x;
    }
    if (normalWS.z >= 0) 
    {
        normalTS_Z.x = -normalTS_Z.x;
    }
        
    //Whiteout blending
    //assumes Z is pointing up, so convert the surface normal to the projected space
    //perform the blend in this tangent space, then convert the result to world space
    float3 normalWS_X =
        BlendTriplanarNormal(normalTS_X, normalWS.zyx).zyx;
    float3 normalWS_Y =
        BlendTriplanarNormal(normalTS_Y, normalWS.xzy).xzy;  
    float3 normalWS_Z =
        BlendTriplanarNormal(normalTS_Z, normalWS);
        
    result = normalWS_X * triWeights.x + normalWS_Y * triWeights.y + normalWS_Z * triWeights.z;
    result = normalize(result);
}

//Get surface property abstraction
void GetBaseColor_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D baseTexture, float4 baseTint,
    out float4 result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 baseColor;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, baseTexture, SIGMA_SAMPLER, baseColor);
    #else
        float4 baseColor = SAMPLE_TEXTURE2D(baseTexture, SIGMA_SAMPLER, parallaxUV);
    #endif
    
    result = baseColor * baseTint;
}

void GetNormal_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    float3 normalWS, float3 tangentWS, float3 bitangentWS,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D normalTexture, float normalStrength,
    out float3 result)
{
    #ifdef _TRIPLANAR_MAPPING
        float3 normal;
        SigmaTriplanarNormal_float(normalWS, triUV_X, triUV_Y, triUV_Z, triW, normalTexture, SIGMA_SAMPLER, normalStrength, normal);
    #else
        float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(normalTexture, SIGMA_SAMPLER, parallaxUV), normalStrength);
        normalTS = normalize(normalTS);
     
        float3x3 TBN = float3x3(tangentWS, bitangentWS, normalWS);
        float3 normal = normalize(mul(normalTS, TBN));
    #endif
    
    result = normalize(normal);
}

void GetSmoothness_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D smoothnessTexture, float smoothnessValue,
    out float result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 triSmoothness;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, smoothnessTexture, SIGMA_SAMPLER, triSmoothness);
        float smoothness = triSmoothness.r;
    #else
        float smoothness = SAMPLE_TEXTURE2D(smoothnessTexture, SIGMA_SAMPLER, parallaxUV).r;
    #endif
    
    result = smoothness * smoothnessValue;
    
    #ifdef _CONVERT_FROM_ROUGHNESS 
        result = 1.0 - smoothness;
    #endif
}

void GetEmission_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D emissionTexture, float4 emissionColor,
    out float3 result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 emission;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, emissionTexture, SIGMA_SAMPLER, emission);
    #else
        float4 emission = SAMPLE_TEXTURE2D(emissionTexture, SIGMA_SAMPLER, parallaxUV);
    #endif
    
    result = emission.rgb * emissionColor;
}

void GetOcclusion_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D occlusionTexture, float occlusionStrength,
    out float result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 triOcclusion;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, occlusionTexture, SIGMA_SAMPLER, triOcclusion);
        float occlusion = triOcclusion.r;
    #else
        float occlusion = SAMPLE_TEXTURE2D(occlusionTexture, SIGMA_SAMPLER, parallaxUV).r;
    #endif
    
    result = lerp(1.0f, occlusion, occlusionStrength);
}

void GetMetallic_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D metallicTexture, float metallicValue,
    out float result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 triMetallic;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, metallicTexture, SIGMA_SAMPLER, triMetallic);
        float metallic = triMetallic.r;
    #else
        float metallic = SAMPLE_TEXTURE2D(metallicTexture, SIGMA_SAMPLER, parallaxUV).r;
    #endif
    
    result = metallic * metallicValue;
}

void GetSpecular_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triW, float2 parallaxUV,
    UnitySamplerState SIGMA_SAMPLER, UnityTexture2D specularTexture, float4 specularStrength,
    out float3 result)
{
    #ifdef _TRIPLANAR_MAPPING
        float4 specular;
        SigmaTriplanar_float(triUV_X, triUV_Y, triUV_Z, triW, specularTexture, SIGMA_SAMPLER, specular);
    #else
        float4 specular = SAMPLE_TEXTURE2D(specularTexture, SIGMA_SAMPLER, parallaxUV);
    #endif
    
    result = specular.rgb * specularStrength;
}

void SigmaTerrainPremultiplyLayer_float(float4x4 surfaceData, float mask, out float4x4 result)
{
    result = surfaceData * mask;
}

void SigmaTerrainBlendLayersAdd_float(float4x4 surfaceData1, float4x4 surfaceData2, float4x4 surfaceData3, float4x4 surfaceData4, out float4x4 result)
{
    result = surfaceData1 + surfaceData2 + surfaceData3 + surfaceData4;
} 

void SigmaPackSurfaceData_float(float4 baseColor, float3 normalWS, float smoothness, float3 emission, float occlusion, float metallic, float3 specular,
    out float4x4 surfaceData)
{
    surfaceData[0] = baseColor;
    surfaceData[1] = float4(normalWS, smoothness);
    surfaceData[2] = float4(emission, occlusion);
    surfaceData[3] = float4(metallic, specular);
}

void SigmaUnpackSurfaceData_float(float4x4 surfaceData, 
    out float4 baseColor, out float3 normalWS, out float smoothness, out float3 emission, out float occlusion, out float metallic, out float3 specular)
{
    baseColor = surfaceData[0];
    normalWS = surfaceData[1].xyz;
    smoothness = surfaceData[1].w;
    emission = surfaceData[2].xyz;
    occlusion = surfaceData[2].w;
    metallic = surfaceData[3].x;
    specular = surfaceData[3].yzw;
}

#endif
