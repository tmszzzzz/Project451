Shader "Qilin/Translucent" 
{
  Properties 
  {
    _MainTex ("Texture", 2D) = "white" {}
    _Cube ("Env Cubemap", CUBE) = "" {}
    _AlphaCubemap ("Alpha Cubemap", CUBE) = "" {}

    _Solidity ("Solidity", Range(0,1)) = 1.0
    _FresnelScale ("Fresnel Scale", Range(0.1, 15.0)) = 3.0
    _FresnelBias ("Fresnel Bias", Range(-5, 5.0)) = 3.0
    _AlphaPower ("Alpha Power", Range(0.1, 3.0)) = 2.0
    _Refract ("Refract", Range(0, 1)) = 0.3
  }

  SubShader 
  {    
    Tags 
    {
      "Queue"="Transparent" 
      "RenderType"="Transparent" 
    }

    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows alpha:fade

    struct Input 
    {
      float2 uv_MainTex;
      float3 worldNormal;
      float3 worldRefl;
      float3 viewDir;
    };
    
    sampler2D _MainTex;
    samplerCUBE _Cube;
    samplerCUBE _AlphaCubemap;

    float _FresnelScale;
    float _FresnelBias;
    float _AlphaPower;
    float _Solidity;
    float _Refract;

    void surf (Input IN, inout SurfaceOutputStandard o) 
    {
      float3 textureSample = tex2D(_MainTex, IN.uv_MainTex);
      float3 cubeSample = texCUBE (_Cube, IN.worldRefl);
      float3 alphaSample = texCUBE (_AlphaCubemap, IN.worldRefl);
      alphaSample = pow (alphaSample, _AlphaPower);

      float fresnel = (1.0 - dot(IN.viewDir, IN.worldNormal)) * _FresnelScale + _FresnelBias;
      float refract = (1.0 - fresnel) * _Refract;

      float3 color = lerp (cubeSample, cubeSample * textureSample, 1 - alphaSample);
      float alpha = lerp(_Solidity, 1.0, refract + alphaSample);
      
      o.Albedo = color;
      o.Emission = color;
      o.Alpha = alpha;
    }
    ENDCG
  } 

  Fallback "Diffuse"
}