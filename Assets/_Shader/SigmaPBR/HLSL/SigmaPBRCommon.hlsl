#ifndef _INCLUDE_SIGMAPBRCOMMON
#define _INCLUDE_SIGMAPBRCOMMON

//Notes
//No negative dot products always saturate to keep within 0 to 1
//No divisions by 0 clamp to small episilon like 1e-5 (0.00001) using max

//https://talkartist.cn/article/1936746818916368384 PBR implementation
//D Normal distribution function
float D_DistributionGGX(float3 N, float3 H, float roughness) //GGX/Trowbridge-Reitz
{
    float a = roughness * roughness; 
    float a2 = a * a;
    float NdotH = saturate(dot(N, H));
    float NdotHSqr = NdotH * NdotH;
    
    float denominator = NdotHSqr * (a2 - 1.0) + 1.0;
    denominator = PI * denominator * denominator;
    return a2 / denominator;
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

//https://github.com/Nuomi-Chobits/Unity-URP-PBR/blob/main/Assets/Shaders/CustomLighting.hlsl implementation
float D_GGX_UE5(float roughness, float NdotH)
{
    float a = roughness * roughness; 
    float a2 = a * a;
    float d = ( NdotH * a2 - NdotH ) * NdotH + 1;	
    return a2 / ( PI * d * d );			
}

// Appoximation of joint Smith term for GGX
// [Heitz 2014, "Understanding the Masking-Shadowing Function in Microfacet-Based BRDFs"]
float Vis_SmithJointApprox(float roughness, float NdotV, float NdotL)
{
    float a = roughness * roughness; 
    float Vis_SmithV = NdotL * ( NdotV * ( 1 - a ) + a );
    float Vis_SmithL = NdotV * ( NdotL * ( 1 - a ) + a );
    return 0.5 * rcp( max(Vis_SmithV + Vis_SmithL, 1e-5));
}

float3 F_Schlick_UE5(float VdotH, float3 F0)
{
    float Fc = pow(1.0 - VdotH, 5);
        
    // Anything less than 2% is physically impossible and is instead considered to be shadowing
    return saturate(50.0 * F0.g) * Fc + (1 - Fc) * F0;
}

// Unity's combined V*F term, same math as DirectBRDFSpecular
float Vis_Unity(float roughness, float LdotH)
{
    float a = max(roughness * roughness, 0.0078125);
    float normalizationTerm = a * 4.0 + 2.0;
    return 1.0 / (max(0.1, LdotH * LdotH) * normalizationTerm);
}

float3 SpecularGGX(float3 L, float3 N, float3 V, float3 H, float3 F0, float roughness)
{
    float HdotV = saturate(dot(H, V));
    // float NdotV = saturate(dot(N, V));
    // float NdotL = saturate(dot(N, L));
    float NdotH = saturate(dot(N, H));
    float LdotH = saturate(dot(L, H));
    
    float D = D_GGX_UE5(roughness, NdotH);
    //float Vis = Vis_SmithJointApprox(roughness, NdotV, NdotL); //Vis is just the denom (4 · NoL · NoV) built in Vis = G / (4 · NoL · NoV)
    float Vis = Vis_Unity(roughness, LdotH);
    //float3 F = F_Schlick_UE5(HdotV, F0); //Unity seems to drop this?

    return (D * Vis) * F0;
}
#endif
