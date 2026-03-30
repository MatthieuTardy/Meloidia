#ifdef SHADERGRAPH_PREVIEW
    ShadowAtten = 1.0;
#else
    #if defined(UNIVERSAL_LIGHTING_INCLUDED)
        // Calcule les coordonnées de l'ombre à partir de la position dans le monde
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        // Récupère la lumière principale et son atténuation (les ombres)
        Light mainLight = GetMainLight(shadowCoord);
        ShadowAtten = mainLight.shadowAttenuation;
    #else
        ShadowAtten = 1.0;
    #endif
#endif