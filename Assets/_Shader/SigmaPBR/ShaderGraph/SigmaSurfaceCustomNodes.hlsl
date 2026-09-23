#ifndef _INCLUDE_SIGMASURFACECUSTOMNODES
#define _INCLUDE_SIGMASURFACECUSTOMNODES
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

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
    //scale view so that z is 1 no need to /z cause we dont use z
    //offset the z component so it never approaches zero, which would blow up the xy/z division at shallow (grazing) view angles
    //this trades a bit of projection accuracy it warps the perspective slightly for much more stable manageable parallax artifacts at those angles
    //0.42 is Unity's standard-shader value, chosen empirically rather than derived.
    viewDirTS = normalize(viewDirTS);
    
    float parallaxBias = 0.42;
    viewDirTS.xy /= (viewDirTS.z + parallaxBias); 

    float height = SAMPLE_TEXTURE2D(heightMap, heightSampler, uv).g;
    height -= 0.5; //centers height around 0 
    
    parallaxUV = uv;
    parallaxUV.xy += viewDirTS.xy * height * heightmapStrength;
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

void SigmaTriplanar_float(float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triWeights, UnityTexture2D _texture, UnitySamplerState _sampler,
    out float4 result)
{
    float4 sampleX = SAMPLE_TEXTURE2D(_texture, _sampler, triUV_X);
    float4 sampleY = SAMPLE_TEXTURE2D(_texture, _sampler, triUV_Y);
    float4 sampleZ = SAMPLE_TEXTURE2D(_texture, _sampler, triUV_Z);
    
    result = sampleX * triWeights.x + sampleY * triWeights.y + sampleZ * triWeights.z;
}

float3 BlendTriplanarNormal(float3 mappedNormal, float3 surfaceNormal) 
{
    float3 n;
    n.xy = mappedNormal.xy + surfaceNormal.xy;
    n.z = mappedNormal.z * surfaceNormal.z;
    return n;
}

void SigmaTriplanarNormal(float3 normalWS, float2 triUV_X, float2 triUV_Y, float2 triUV_Z, float3 triWeights, UnityTexture2D normalMap, UnitySamplerState normalSampler, float normalStrength,
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

#endif
