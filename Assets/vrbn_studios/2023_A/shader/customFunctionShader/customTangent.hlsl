#ifndef CUSTOMTANGENT_INCLUDE
#define CUSTOMTANGENT_INCLUDE
void getViewDirTangent_float(float3 f,float3 c,float3 d,float3 m,float v,float x,float y,out float3 n){n=normalize(float3(dot(d,normalize(f)),dot(d,normalize(cross(c,f)*-1.)),dot(d,normalize(c))));v/=2.;x/=2.;y/=2.;n=float3(n.x/v,n.y/x,n.z/y);}
#endif // CUSTOMTANGENT_INCLUDE