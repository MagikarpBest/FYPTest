#ifndef _INCLUDE_SIGMASURFACECUSTOMNODES
#define _INCLUDE_SIGMASURFACECUSTOMNODES

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

#endif
