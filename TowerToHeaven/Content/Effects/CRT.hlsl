sampler2D uImage0 : register(s0);

float4 PS_Main(float2 uv : TEXCOORD0) : COLOR
{
    float2 barrel = uv * 2.0 - 1.0;
    float r2 = dot(barrel, barrel);
    barrel *= 1.0 + 0.05 * r2;
    barrel = barrel * 0.5 + 0.5;
    
    if (barrel.x < 0 || barrel.x > 1 || barrel.y < 0 || barrel.y > 1)
        return 0;
        
    float2 center = barrel - 0.5;
    float dist = length(center);
    
    float4 color = tex2D(uImage0, barrel);
    
    float mask = 1 + 0.15 * sin(barrel.y * 400);
    color *= mask;
    
    color -= pow(dist, 2);
    
    return color;
}

Technique techique
{
    pass Main
    {
        PixelShader = compile ps_3_0 PS_Main();
    }
}