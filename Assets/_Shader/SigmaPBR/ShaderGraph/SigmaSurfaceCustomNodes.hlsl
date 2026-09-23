#ifndef _INCLUDE_SIGMASURFACECUSTOMNODES
#define _INCLUDE_SIGMASURFACECUSTOMNODES
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

void GetTriplanarUV(float3 positionWS, float3 normalWS, float triplanarTile,
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

void GetTriplanarViewDir(float3 viewDirWS, float3 normalWS,
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

void GetTriplanarWeights(float3 normalWS, float triplanarBlendOffset, float triplanarBlendExponent,
    out float3 triW) 
{
    triW = abs(normalWS);
    triW = saturate(triW - triplanarBlendOffset);
    triW = pow(triW, triplanarBlendExponent);
    
    float sum = triW.x + triW.y + triW.z;
    triW = triW / max(sum, 1e-5);
}

void GetParallaxOffsetUV(float3 viewDirTS, float2 uv, Texture2D heightMap, SamplerState heightSampler, float heightmapStrength,
    out float2 offsetUV)
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
    
    offsetUV = uv;
    offsetUV.xy += viewDirTS.xy * height * heightmapStrength;
}

void GetParallaxOffsetTriplanarUV(float3 positionWS, float3 viewDirWS, float3 normalWS, Texture2D heightMap, SamplerState heightSampler, 
    float heightmapStrength, float triplanarTile,
    out float2 offsetTriUV_X, out float2 offsetTriUV_Y, out float2 offsetTriUV_Z)
{
    float3 triViewX, triViewY, triViewZ;
    GetTriplanarViewDir(viewDirWS, normalWS, triViewX, triViewY, triViewZ);
    
    float2 triUV_X, triUV_Y, triUV_Z;
    GetTriplanarUV(positionWS, normalWS, triplanarTile, triUV_X, triUV_Y, triUV_Z);
    
    GetParallaxOffsetUV(triViewX, triUV_X, heightMap, heightSampler, heightmapStrength, offsetTriUV_X);
    GetParallaxOffsetUV(triViewY, triUV_Y, heightMap, heightSampler, heightmapStrength, offsetTriUV_Y);
    GetParallaxOffsetUV(triViewZ, triUV_Z, heightMap, heightSampler, heightmapStrength, offsetTriUV_Z);
}

void ParallaxTriplanar()
{
    
}

#endif
