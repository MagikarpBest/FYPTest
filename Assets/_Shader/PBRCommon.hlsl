#ifndef _INCLUDE_PBRCOMMON
#define _INCLUDE_PBRCOMMON
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

//Notes
//No negative dot products always saturate to keep within 0 to 1
//No divisions by 0 clamp to small episilon like 1e-5 (0.00001) using max

//D Normal distribution function
float D_DistributionGGX(float3 N, float3 H, float roughness) //GGX/Trowbridge-Reitz
{
    float a = roughness * roughness; 
    float aSqr = a * a;
    float NdotH = saturate(dot(N, H));
    float NdotHSqr = NdotH * NdotH;
    
    float denominator = NdotHSqr * (aSqr - 1.0) + 1.0;
    denominator = PI * denominator * denominator;
    return aSqr / denominator;
}

//G Geometry shadowing function
float G_GeometrySchlickGGX(float NdotX, float roughness) //X can be L or V
{
    float r = roughness + 1.0;
    float k = r * r / 8.0;
    
    float denominator = NdotX * (1.0 - k) + k;
    return NdotX / max(denominator, 1e-5);
}

float G_GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = saturate(dot(N, V));
    float NdotL = saturate(dot(N, L));
    
    float GGX1 = G_GeometrySchlickGGX(NdotV, roughness);
    float GGX2 = G_GeometrySchlickGGX(NdotL, roughness);
    
    return GGX1 * GGX2;
}
    
//F Fresnel function
float3 F_FresnelSchlick(float VdotH, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - VdotH, 5.0);
}

float3 Specular_CookTorance(float3 L, float3 N, float3 V, float3 H, float3 F0, float roughness)
{
    float HdotV = saturate(dot(H, V));
	float NdotV = saturate(dot(N, V));
	float NdotL = saturate(dot(N, L));
    
    float D = D_DistributionGGX(N, H, roughness);
	float3 F = F_FresnelSchlick(HdotV, F0);
	float G = G_GeometrySmith(N, V, L, roughness);
    
    float3 nominator = D * G * F;
    float denominator = 4 * NdotV * NdotL;
        
    return nominator / max(denominator, 1e-5);
}

float3 F_FresnelSchlickRoughness(float NdotV, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(1.0 - NdotV, 5.0);
}

//Modified version of UE4 for Black Ops 2
float2 EnvBRDFApprox_UE4(float roughness, float NdotV)
{
    // [ Lazarov 2013, "Getting More Physical in Call of Duty: Black Ops II" ]
    // Adaptation to fit our G term.
    const float4 c0 = { -1, -0.0275, -0.572, 0.022 };
    const float4 c1 = { 1, 0.0425, 1.04, -0.04 };
    float4 r = roughness * c0 + c1;
    float a004 = min( r.x * r.x, exp2( -9.28 * NdotV ) ) * r.x + r.y;
    float2 AB = float2( -1.04, 1.04 ) * a004 + r.zw;
    return AB;
}

#endif
