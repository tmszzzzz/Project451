#ifndef RAYCASTINGTANGENT_INCLUDE
#define RAYCASTINGTANGENT_INCLUDE
void raycastingTangent_float(float2 f,float3 m,float3 a,float x,float o,float P,float O,out float3 i,out float M,out float2 L,out float y){float2 J=frac(f);i=float3(J*2.-1.,1.);float3 I=i,z=1./a,G=abs(z)-i*z;float F=min(min(G.x,G.y),G.z);M=F;i+=F*a;float E=-1.;if(m.y>0)E=1.;a[2]=a[2]*o/clamp(P,O*E*o,o-.01);float D=1./a[2],C=abs(D)-I[2]*D;y=C;I+=y*a;I[1]=1.-I[1];L=I.xy;}
#endif // RAYCASTINGTANGENT_INCLUDE