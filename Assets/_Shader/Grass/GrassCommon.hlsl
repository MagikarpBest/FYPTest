#ifndef _INCLUDE_GRASSCOMMON
#define _INCLUDE_GRASSCOMMON
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Assets/_Shader/Resources/Random.cginc"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Assets/_Shader/Resources/Quarternion.hlsl"

struct GrassData 
{
    float4 position;
    float3 up;
    float3 forward;
    float2 scale;
    float2 terrainUV;
    float densityThreshold;
};

StructuredBuffer<GrassData> _GrassDataBuffer;

float2 _WindDirection;
float _WindStrength;
float _WindSpeed;
float _GrassBend;
float4 _WindTexture_ST;
float _WindScale;
TEXTURE2D(_WindTexture);
SAMPLER(sampler_WindTexture);

float3 GetGrassPosition(float3 positionOS, float2 uv, uint instanceID)
{
    //position from buffer
    GrassData grass = _GrassDataBuffer[instanceID];
    
    //forward direction is randomized and grass alligned to surface normal
    float4 facingRot = from_to_rotation(float3(0, 0, 1), grass.forward);
    //float4 upRot = from_to_rotation(float3(0, 1, 0), grass.up);
    //float4 grassRot = qmul(upRot, facingRot);
    float3 localPosition = rotate_vector(positionOS, facingRot);
    
    //scale
    localPosition.xz *= grass.scale.x;
    localPosition.y  *= grass.scale.y;
    
    float3 positionWS = grass.position.xyz + localPosition.xyz; //final 
    
    float2 windDir = normalize(_WindDirection);
    float2 windUV = positionWS.xz + _Time.y * _WindSpeed * windDir * _WindScale;
    windUV = TRANSFORM_TEX(windUV, _WindTexture);
    // float2 wind = SimplexNoise(windUV) * _WindStrength * uv.y;
    float4 windTex = SAMPLE_TEXTURE2D_LOD(_WindTexture, sampler_WindTexture, windUV, 0);
    float2 wind = (windTex.r * 2 - 1) * _WindStrength * uv.y;
    positionWS.xz += wind;
    
    return positionWS;
}

float3 GetMeshNormal(float3 normalOS, uint instanceID)
{
    GrassData grass = _GrassDataBuffer[instanceID];
    float4 facingRot = from_to_rotation(float3(0, 0, 1), grass.forward);
    float3 normalWS = normalize(rotate_vector(normalOS, facingRot));
    
    return normalWS;
}

#endif
