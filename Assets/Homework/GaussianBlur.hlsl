void GaussianBlur(float2 UV, Texture2D SceneColor, SamplerState _sampler, float BlurStrength, out float4 OutColor)
{
    float2 offsets[9] = {
        float2(-1, -1), float2(0, -1), float2(1, -1),
        float2(-1,  0), float2(0,  0), float2(1,  0),
        float2(-1,  1), float2(0,  1), float2(1,  1)
    };

    float weights[9] = { 
        1, 2, 1, 
        2, 4, 2, 
        1, 2, 1 
    };

    float4 color = 0;
    float weightSum = 0;

    for (int i = 0; i < 9; i++)
    {
        float2 newUV = UV + offsets[i] * BlurStrength * 0.005;
        color += SceneColor.Sample(_sampler, newUV) * weights[i];
        weightSum += weights[i];
    }

    OutColor = color / weightSum;
}
