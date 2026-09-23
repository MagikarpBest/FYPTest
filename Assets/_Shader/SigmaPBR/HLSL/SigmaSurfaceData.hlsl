#ifndef _INCLUDE_SIGMASURFACEDATA
#define _INCLUDE_SIGMASURFACEDATA
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
float _Surface;
float _Cutoff;

float _TriplanarTile;
float _TriplanarBlendOffset;
float _TriplanarBlendExponent;

//Base map
float4 _BaseColor;
float4 _BaseTexture_ST;
float _NormalStrength;
float _Metallic;
float3 _SpecularColor;
float _Smoothness;
float _HeightMapStrength;
float _OcclusionStrength;
float3 _EmissionColor;

//Top map
float4 _TopBaseColor;
float4 _TopBaseTexture_ST;
float _TopNormalStrength;
float _TopMetallic;
float3 _TopSpecularColor;
float _TopSmoothness;
float _TopHeightMapStrength;
float _TopOcclusionStrength;
float3 _TopEmissionColor;
CBUFFER_END

TEXTURE2D(_BaseTexture);

#ifdef _SPECULAR_SETUP
    TEXTURE2D(_SpecularMap);
#else
    TEXTURE2D(_MetallicMap);
#endif
                
TEXTURE2D(_SmoothnessMap);
TEXTURE2D(_NormalTexture);
TEXTURE2D(_HeightMap);
TEXTURE2D(_OcclusionMap);
TEXTURE2D(_EmissionMap);

#ifdef _SEPARATE_TOP_MAP
    TEXTURE2D(_TopBaseTexture);

    #ifdef _SPECULAR_SETUP
        TEXTURE2D(_TopSpecularMap);
    #else
        TEXTURE2D(_TopMetallicMap);
    #endif
                    
    TEXTURE2D(_TopSmoothnessMap);
    TEXTURE2D(_TopNormalTexture);
    TEXTURE2D(_TopHeightMap);
    TEXTURE2D(_TopOcclusionMap);
    TEXTURE2D(_TopEmissionMap);
#endif

#if defined(_TEXTUREFILTER_LINEAR) && defined(_TEXTUREWRAP_REPEAT)
    #define SIGMA_SAMPLER sampler_LinearRepeat
#elif defined(_TEXTUREFILTER_LINEAR) && defined(_TEXTUREWRAP_CLAMP)
    #define SIGMA_SAMPLER sampler_LinearClamp
#elif defined(_TEXTUREFILTER_POINT) && defined(_TEXTUREWRAP_REPEAT)
    #define SIGMA_SAMPLER sampler_PointRepeat
#elif defined(_TEXTUREFILTER_POINT) && defined(_TEXTUREWRAP_CLAMP)
    #define SIGMA_SAMPLER sampler_PointClamp
#endif

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

struct TriplanarUV 
{
    float2 x, y, z;
};

struct TriplanarViewDir
{
    float3 x, y, z; 
};

void ParallaxOffset(float3 viewDirTS, inout float2 uv, Texture2D heightMap, SamplerState heightSampler, float heightmapStrength)
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
    
    uv.xy += viewDirTS.xy * height * heightmapStrength;
}

TriplanarViewDir GetTriplanarViewDir(SigmaSurfaceParameters sp)
{
    TriplanarViewDir triView;
    float3 v = sp.viewDirWS;
    
    //The Z component is perpendicular to the projection plane
    //For triplanar mapping both sides of the plane use the same depth direction
    //so we use its magnitude and remove the sign
    triView.x = float3(v.z, v.y, abs(v.x));
    triView.y = float3(v.x, v.z, abs(v.y));
    triView.z = float3(v.x, v.y, abs(v.z));
    
    //prevent mirror
     if (sp.normalWS.x < 0) 
     {
         triView.x.x = -triView.x.x;
     }
     if (sp.normalWS.y < 0) 
     {
         triView.y.x = -triView.y.x;
     }
     if (sp.normalWS.z >= 0) 
     {
         triView.z.x = -triView.z.x;
     }
    
    return triView;
}

TriplanarUV GetTriplanarUV(SigmaSurfaceParameters sp) 
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
    
    //Parallax
    TriplanarViewDir triView = GetTriplanarViewDir(sp);
    
    ParallaxOffset(triView.x, triUV.x, _HeightMap, SIGMA_SAMPLER, _HeightMapStrength);
    
    #ifdef _SEPARATE_TOP_MAP
        if (sp.normalWS.y > 0)
        {
            ParallaxOffset(triView.y, triUV.y, _TopHeightMap, SIGMA_SAMPLER, _TopHeightMapStrength);
        }
        else
        {
            ParallaxOffset(triView.y, triUV.y, _HeightMap, SIGMA_SAMPLER, _HeightMapStrength);
        }
    #else
        ParallaxOffset(triView.y, triUV.y, _HeightMap, SIGMA_SAMPLER, _HeightMapStrength);
    #endif
   
    ParallaxOffset(triView.z, triUV.z, _HeightMap, SIGMA_SAMPLER, _HeightMapStrength);
    
    return triUV;
}

float3 GetTriplanarWeights(SigmaSurfaceParameters sp) 
{
    float3 triW = abs(sp.normalWS);
    triW = saturate(triW - _TriplanarBlendOffset);
    triW = pow(triW, _TriplanarBlendExponent);
    
    float sum = triW.x + triW.y + triW.z;
    return triW / max(sum, 1e-5);
}

void InitSurfaceParameters(v2f i, out SigmaSurfaceParameters sp)
{
    sp.uv = i.uv;  
    sp.positionWS = i.positionWS;
    sp.normalWS = NormalizeNormalPerPixel(i.normalWS);
    sp.tangentWS = float4(normalize(i.tangentWS.xyz), i.tangentWS.w);
    sp.viewDirWS = normalize(i.viewWS);
    sp.screenUV = GetNormalizedScreenSpaceUV(i.positionCS);
    
    //Parallax
    float3 bitangentWS = cross(sp.normalWS, sp.tangentWS.xyz) * sp.tangentWS.w * unity_WorldTransformParams.w; 
    float3x3 TBN = float3x3(sp.tangentWS.xyz, bitangentWS, sp.normalWS);
    float3 viewDirTS = normalize(mul(TBN, sp.viewDirWS)); //world to tangent
    ParallaxOffset(viewDirTS, sp.uv, _HeightMap, SIGMA_SAMPLER, _HeightMapStrength);
    
    //Triplanar
    TriplanarUV triUV = GetTriplanarUV(sp);
    float3 triW = GetTriplanarWeights(sp);
}

float4 GetBaseColor(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = sp.triUV;
        float3 triW = sp.triW;

        float4 baseColorX = SAMPLE_TEXTURE2D(_BaseTexture, SIGMA_SAMPLER, triUV.x) * _BaseColor;
        float4 baseColorY = SAMPLE_TEXTURE2D(_BaseTexture, SIGMA_SAMPLER, triUV.y) * _BaseColor;
        float4 baseColorZ = SAMPLE_TEXTURE2D(_BaseTexture, SIGMA_SAMPLER, triUV.z) * _BaseColor;
    
        #ifdef _SEPARATE_TOP_MAP
            if (sp.normalWS.y > 0) 
            {
                baseColorY = SAMPLE_TEXTURE2D(_TopBaseTexture, SIGMA_SAMPLER, triUV.y) * _TopBaseColor;
            }
        #endif

        float4 baseColor = baseColorX * triW.x + baseColorY * triW.y + baseColorZ * triW.z;
    
    #else
        float4 baseColor = SAMPLE_TEXTURE2D(_BaseTexture, SIGMA_SAMPLER, sp.uv) * _BaseColor;
    #endif

    return baseColor;
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
        TriplanarUV triUV = sp.triUV;
        float3 triW = sp.triW;
        
        float3 normalTS_X = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, SIGMA_SAMPLER, triUV.x), _NormalStrength);
        float3 normalTS_Y = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, SIGMA_SAMPLER, triUV.y), _NormalStrength);
        float3 normalTS_Z = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, SIGMA_SAMPLER, triUV.z), _NormalStrength);
        
        #ifdef _SEPARATE_TOP_MAP
            if (sp.normalWS.y > 0)
            {
                normalTS_Y = UnpackNormalScale(SAMPLE_TEXTURE2D(_TopNormalTexture, SIGMA_SAMPLER, triUV.y), _TopNormalStrength);
            }
        #endif
    
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
        //assumes Z is pointing up, so convert the surface normal to the projected space
        //perform the blend in this tangent space, then convert the result to world space
        float3 normalWS_X =
            BlendTriplanarNormal(normalTS_X, sp.normalWS.zyx).zyx;
        float3 normalWS_Y =
            BlendTriplanarNormal(normalTS_Y, sp.normalWS.xzy).xzy;  
        float3 normalWS_Z =
            BlendTriplanarNormal(normalTS_Z, sp.normalWS);
        
        float3 normal = normalWS_X * triW.x + normalWS_Y * triW.y + normalWS_Z * triW.z;
    
    #else
        float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTexture, SIGMA_SAMPLER, sp.uv), _NormalStrength);
        normalTS = normalize(normalTS);
                	    
        float3 bitangentWS = cross(sp.normalWS, sp.tangentWS.xyz) * sp.tangentWS.w * unity_WorldTransformParams.w; 
        //tangentWS.w corrects for handedness issues baked into the mesh
        //unity_WorldTransformParams.w corrects for handedness issues introduced by the instance's transform (negative scale)
        
        float3x3 TBN = float3x3(sp.tangentWS.xyz, bitangentWS, sp.normalWS);
        float3 normal = normalize(mul(normalTS, TBN));
    #endif
    
    return normalize(normal);
}

#ifdef _SPECULAR_SETUP
    float GetSpecular(SigmaSurfaceParameters sp)
    {
        #ifdef _TRIPLANAR_MAPPING
            TriplanarUV triUV = sp.triUV;
            float3 triW = sp.triW;
            
            float3 specularX = SAMPLE_TEXTURE2D(_SpecularMap, SIGMA_SAMPLER, triUV.x).rgb * _SpecularColor;
            float3 specularY = SAMPLE_TEXTURE2D(_SpecularMap, SIGMA_SAMPLER, triUV.y).rgb * _SpecularColor;
            float3 specularZ = SAMPLE_TEXTURE2D(_SpecularMap, SIGMA_SAMPLER, triUV.z).rgb * _SpecularColor;

            #ifdef _SEPARATE_TOP_MAP
                if (sp.normalWS.y > 0)
                {
                    specularY = SAMPLE_TEXTURE2D(_TopSpecularMap, SIGMA_SAMPLER, triUV.y).rgb * _TopSpecularColor;
                }
            #endif
    
            float3 specular = specularX * triW.x + specularY * triW.y + specularZ * triW.z;
    
        #else
            float3 specular = SAMPLE_TEXTURE2D(_SpecularMap, SIGMA_SAMPLER, sp.uv).rgb * _SpecularColor;
        #endif
        
        return specular;
    }
#else
    float GetMetallic(SigmaSurfaceParameters sp)
    {
        #ifdef _TRIPLANAR_MAPPING
            TriplanarUV triUV = sp.triUV;
            float3 triW = sp.triW;
        
            float metallicX = SAMPLE_TEXTURE2D(_MetallicMap, SIGMA_SAMPLER, triUV.x).r * _Metallic;
            float metallicY = SAMPLE_TEXTURE2D(_MetallicMap, SIGMA_SAMPLER, triUV.y).r * _Metallic;
            float metallicZ = SAMPLE_TEXTURE2D(_MetallicMap, SIGMA_SAMPLER, triUV.z).r * _Metallic;

            #ifdef _SEPARATE_TOP_MAP
                if (sp.normalWS.y > 0)
                {
                    metallicY = SAMPLE_TEXTURE2D(_TopMetallicMap, SIGMA_SAMPLER, triUV.y).r * _TopMetallic;
                }
            #endif
            
            float metallic = metallicX * triW.x + metallicY * triW.y + metallicZ * triW.z;
    
        #else
            float metallic = SAMPLE_TEXTURE2D(_MetallicMap, SIGMA_SAMPLER, sp.uv).r * _Metallic;
        #endif
        
        return metallic;
    }
#endif

float GetSmoothness(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = sp.triUV;
        float3 triW = sp.triW;
    
        float smoothnessX = SAMPLE_TEXTURE2D(_SmoothnessMap, SIGMA_SAMPLER, triUV.x).r * _Smoothness;
        float smoothnessY = SAMPLE_TEXTURE2D(_SmoothnessMap, SIGMA_SAMPLER, triUV.y).r * _Smoothness;
        float smoothnessZ = SAMPLE_TEXTURE2D(_SmoothnessMap, SIGMA_SAMPLER, triUV.z).r * _Smoothness;

        #ifdef _SEPARATE_TOP_MAP
            if (sp.normalWS.y > 0)
            {
                smoothnessY = SAMPLE_TEXTURE2D(_TopSmoothnessMap, SIGMA_SAMPLER, triUV.y).r * _TopSmoothness;
            }
        #endif
        
        float smoothness = smoothnessX * triW.x + smoothnessY * triW.y + smoothnessZ * triW.z;
    
    #else
        float smoothness = SAMPLE_TEXTURE2D(_SmoothnessMap, SIGMA_SAMPLER, sp.uv).r * _Smoothness;
    #endif
    
    #ifdef _CONVERT_FROM_ROUGHNESS 
        return 1.0 - smoothness;
    #endif
    
    return smoothness;
}

float GetOcclusion(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = sp.triUV;
        float3 triW = sp.triW;
        
        float occlusionX = lerp(1.0f, SAMPLE_TEXTURE2D(_OcclusionMap, SIGMA_SAMPLER, triUV.x).r, _OcclusionStrength);
        float occlusionY = lerp(1.0f, SAMPLE_TEXTURE2D(_OcclusionMap, SIGMA_SAMPLER, triUV.y).r, _OcclusionStrength);
        float occlusionZ = lerp(1.0f, SAMPLE_TEXTURE2D(_OcclusionMap, SIGMA_SAMPLER, triUV.z).r, _OcclusionStrength);

        #ifdef _SEPARATE_TOP_MAP
            if (sp.normalWS.y > 0)
            {
                occlusionY = lerp(1.0f, SAMPLE_TEXTURE2D(_TopOcclusionMap, SIGMA_SAMPLER, triUV.y).r, _TopOcclusionStrength);
            }
        #endif
        
        float occlusion = occlusionX * triW.x + occlusionY * triW.y + occlusionZ * triW.z;
    
    #else
        float occlusion = lerp(1.0f, SAMPLE_TEXTURE2D(_OcclusionMap, SIGMA_SAMPLER, sp.uv).r, _OcclusionStrength);
    #endif
    
    return occlusion;
}

float3 GetEmissive(SigmaSurfaceParameters sp)
{
    #ifdef _TRIPLANAR_MAPPING
        TriplanarUV triUV = sp.triUV;
        float3 triW = sp.triW;
        
        float3 emissionX = SAMPLE_TEXTURE2D(_EmissionMap, SIGMA_SAMPLER, triUV.x).rgb * _EmissionColor;
        float3 emissionY = SAMPLE_TEXTURE2D(_EmissionMap, SIGMA_SAMPLER, triUV.y).rgb * _EmissionColor;
        float3 emissionZ = SAMPLE_TEXTURE2D(_EmissionMap, SIGMA_SAMPLER, triUV.z).rgb * _EmissionColor;

        #ifdef _SEPARATE_TOP_MAP
            if (sp.normalWS.y > 0)
            {
                emissionY = SAMPLE_TEXTURE2D(_TopEmissionMap, SIGMA_SAMPLER, triUV.y).rgb * _TopEmissionColor;
            }
        #endif
    
        float3 emission = emissionX * triW.x + emissionY * triW.y + emissionZ * triW.z;
    #else
        float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, SIGMA_SAMPLER, sp.uv).rgb * _EmissionColor;
    #endif
    
    return emission;
}

v2f Initv2f(appdata v)
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
