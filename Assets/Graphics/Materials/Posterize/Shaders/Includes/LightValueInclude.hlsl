void GetLightValue_float(in float3 Normal, in float3 ClipSpacePos, in float3 WorldPos, out float lightValue)
{
		#ifdef SHADERGRAPH_PREVIEW
			lightValue = dot(Normal, float3(0.5, 0.5, 0.25));
		#else

		#if SHADOWS_SCREEN
			half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
		#else
			half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
		#endif

		#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
			Light light = GetMainLight(shadowCoord);
		#else
			Light light = GetMainLight();
		#endif

		half d = dot(Normal, light.direction);

		lightValue = light.color * d * light.shadowAttenuation;

		float additionalLights;
		int additionalLightCount = GetAdditionalLightsCount();
		
		for (int j = 0; j < additionalLightCount; ++j)
		{
            Light aLight = GetAdditionalLight(j, WorldPos, half4(1, 1, 1, 1));
            
            half aDot = dot(Normal, aLight.direction);
            
			float attenuation = aLight.distanceAttenuation * aLight.shadowAttenuation;
 
            additionalLights += attenuation * aDot;
		}

		lightValue += additionalLights;
		#endif
}