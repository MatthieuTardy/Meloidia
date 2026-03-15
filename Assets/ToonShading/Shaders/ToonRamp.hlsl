void ToonShading_float(in float3 Normal, in float ToonRampSmoothness, in float3 ClipSpacePos,
                       in float3 WorldPos, in float ToonRampOffset, in float ShadowPower,
                       in float ToonContrast, in float LightIntensity, out float3 ToonRampOutput)
{
#ifdef SHADERGRAPH_PREVIEW
		ToonRampOutput = float3(0.5, 0.5, 0.5);
#else
		// Shadow coordinates
#if SHADOWS_SCREEN
			half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif 
		
		// Main light
#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
			Light light = GetMainLight(shadowCoord);
#else
    Light light = GetMainLight();
#endif

		// Dot product + normalize
    half d = dot(Normal, light.direction) * 0.5 + 0.5;
		
		// Toon ramp avec contrôle du contraste
    half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset + ToonRampSmoothness, d);
    toonRamp = lerp(0.3, 1.0, pow(toonRamp, ToonContrast)); // 0.3 = noir min, 1.0 = blanc max
		
		// Intensité de lumière
    toonRamp *= LightIntensity;
		
		// Shadow contrôlé
    half shadowIntensity = lerp(1.0, light.shadowAttenuation, ShadowPower);
    ToonRampOutput = light.color * toonRamp * shadowIntensity;
#endif
}