﻿int LightCount;
float ScreenWidth;
float ScreenHeight;

float3 positionLight[64];
float3 colorLight[64];
float invRadiusLight[64];

 
sampler DiffuseSampler : register(s0);

texture NormalMap;
SamplerState NormalMapSampler {
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture LightMap;
SamplerState LightMapSampler {
	Texture = (LightMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};



float3 AmbientColor;

float3 CalculateLight(int index, float3 normal, float3 pixelPosition, float2 texCoords)
{
	float3 lightPos = float3(positionLight[index].x, positionLight[index].y, 1);
	float3 direction = lightPos - pixelPosition;
		float atten = length(direction);

	direction /= atten;

	float dotValue = dot(normal, direction);
	float amount = 0;


	if (dotValue < 0.1 && dotValue > -0.1) {
		amount = 1;
	}

	amount = 1.0 + max(dotValue, 0) * 15.0;
	atten *= invRadiusLight[index];


	float modifer = max((1 - atten), 0);
	return colorLight[index] * modifer * amount;
}

float4 DeferredNormalPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
	float4 base = tex2D(DiffuseSampler, texCoords);  
	float3 normal = (tex2D(NormalMapSampler, texCoords) * 2.0f - 1.0f);
	float3 pixelPosition = float3(ScreenWidth * texCoords.x, ScreenHeight * texCoords.y, 0);
	float3 finalColor = 0;

	for (int i = 0; i < LightCount; i++)
	{
		finalColor += CalculateLight(i, normal, pixelPosition, texCoords);
	}

	float4 lm = tex2D(LightMapSampler, texCoords);
	finalColor += tex2D(DiffuseSampler, texCoords) * lm;

	return float4((AmbientColor + finalColor) * base.rgb, base.a);
}
 
 

technique DeferredLight
{
	pass Pass0
	{
		PixelShader = compile ps_5_0 DeferredNormalPS();
	} 
}
