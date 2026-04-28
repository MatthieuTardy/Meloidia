Shader "Roystan/Toon/GeometryGrass_URP" {
    Properties{
        [Header(Colors)]
        _Color("Main Color", Color) = (1,1,1,1)
        _Albedo1("Albedo Base", Color) = (0.1, 0.5, 0.1, 1)
        _Albedo2("Albedo Top", Color) = (0.2, 0.8, 0.2, 1)
        _AOColor("Ambient Occlusion", Color) = (0.05, 0.2, 0.05, 1)
        _TipColor("Tip Color", Color) = (0.5, 1.0, 0.3, 1)
        _MainTex("Texture", 2D) = "white" {}

        [Header(Color Variation)]
        _Color2("Variation Color (Patches)", Color) = (0.7, 0.8, 0.4, 1)
        _ColorNoiseScale("Patch Noise Scale", Range(0.01, 2.0)) = 0.1

        [Header(Grass Shape)]
        _Height("Grass Height", float) = 3
        _Width("Grass Width", range(0, 0.2)) = 0.08
        _Curvature("Curvature Amount", range(0, 3)) = 1.2
        _Roundness("Tip Roundness", range(0, 1)) = 0.9
        _BulgeAmount("Bulge (belly)", range(0, 2)) = 0.6

        [Header(Toon Lighting)]
        _ToonOffset("Toon Ramp Offset", Range(0, 1)) = 0.3
        _ToonOffsetPoint("Toon Ramp Offset (Point Lights)", Range(0, 1)) = 0.3
        _ToonSmooth("Toon Ramp Smoothness", Range(0.01, 0.5)) = 0.1
        _ToonTint("Toon Ramp Tinting", Color) = (0,0,0,0)

        [Header(Wind)]
        _WindDirection("Wind Direction (X,Z)", Vector) = (1, 0, 0, 0)
        _WindSpeed("Wind Speed", Range(0.0, 5.0)) = 1.0
        _WindStrength("Wind Strength", Range(0.0, 2.0)) = 0.5
        _WindScale("Wind Noise Scale", Range(0.1, 5.0)) = 1.0

        [Header(Interaction)]
        _InteractorRadius("Interactor Radius", float) = 1.5
        _InteractorStrength("Interactor Strength", float) = 1.5

        [Header(Fog)]
        _FogColor("Fog Color", Color) = (0.7, 0.85, 1.0, 1)
        _FogDensity("Fog Density", Range(0.0, 1.0)) = 0.0
        _FogOffset("Fog Offset", Range(0.0, 10.0)) = 0.0
    }

        SubShader{
            Tags {
                "RenderType" = "Opaque"
                "Queue" = "Geometry"
                "RenderPipeline" = "UniversalPipeline"
            }

            ZWrite On
            Cull Off

            Pass {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vp
                #pragma geometry gp
                #pragma fragment fp
                #pragma require geometry

                #pragma target 4.5

            // Multi-compiles pour les ombres et les lumières
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // ==========================================
            // RANDOM & NOISE
            // ==========================================

            float randValue(float n) { return frac(sin(n) * 43758.5453123); }

            float3 mod289_3(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 mod289_4(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 permute(float4 x) { return mod289_4(((x * 34.0) + 1.0) * x); }
            float4 taylorInvSqrt(float4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

            float snoise(float3 v) {
                const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
                const float4 D = float4(0.0, 0.5, 1.0, 2.0);

                float3 i = floor(v + dot(v, C.yyy));
                float3 x0 = v - i + dot(i, C.xxx);
                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1.0 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);

                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy;
                float3 x3 = x0 - D.yyy;

                i = mod289_3(i);
                float4 p = permute(permute(permute(
                    i.z + float4(0.0, i1.z, i2.z, 1.0))
                  + i.y + float4(0.0, i1.y, i2.y, 1.0))
                  + i.x + float4(0.0, i1.x, i2.x, 1.0));

                float n_ = 0.142857142857;
                float3 ns = n_ * D.wyz - D.xzx;
                float4 j = p - 49.0 * floor(p * ns.z * ns.z);
                float4 x_ = floor(j * ns.z);
                float4 y_ = floor(j - 7.0 * x_);
                float4 x = x_ * ns.x + ns.yyyy;
                float4 y = y_ * ns.x + ns.yyyy;
                float4 h = 1.0 - abs(x) - abs(y);
                float4 b0 = float4(x.xy, y.xy);
                float4 b1 = float4(x.zw, y.zw);
                float4 s0 = floor(b0) * 2.0 + 1.0;
                float4 s1 = floor(b1) * 2.0 + 1.0;
                float4 sh = -step(h, float4(0.0, 0.0, 0.0, 0.0));
                float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
                float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
                float3 p0 = float3(a0.xy, h.x);
                float3 p1 = float3(a0.zw, h.y);
                float3 p2 = float3(a1.xy, h.z);
                float3 p3 = float3(a1.zw, h.w);

                float4 norm = taylorInvSqrt(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
                p0 *= norm.x; p1 *= norm.y; p2 *= norm.z; p3 *= norm.w;

                float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
                m = m * m;
                return 42.0 * dot(m * m, float4(dot(p0, x0), dot(p1, x1), dot(p2, x2), dot(p3, x3)));
            }

            // ==========================================
            // STRUCTS
            // ==========================================

            struct VertexData {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2g {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            struct g2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float fogFactor : TEXCOORD2;
                float colorVar : TEXCOORD3; // <-- Nouvelle variable pour la variation de couleur
            };

            // ==========================================
            // PROPERTIES
            // ==========================================

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _Albedo1, _Albedo2, _AOColor, _TipColor, _FogColor;

                // Variations
                float4 _Color2;
                float _ColorNoiseScale;

                float _FogDensity, _FogOffset;
                float _Height, _Width;
                float _Curvature, _Roundness, _BulgeAmount;

                float _ToonOffset;
                float _ToonOffsetPoint;
                float _ToonSmooth;
                float4 _ToonTint;

                float4 _WindDirection;
                float _WindSpeed;
                float _WindStrength;
                float _WindScale;

                float _InteractorRadius;
                float _InteractorStrength;
            CBUFFER_END

                // Position globale depuis le script (doit rester hors du CBUFFER)
                float4 _InteractorPosition;

            // ==========================================
            // VERTEX SHADER
            // ==========================================

            v2g vp(VertexData v) {
                v2g o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                return o;
            }

            // ==========================================
            // HELPERS
            // ==========================================

            float4 RotateAroundY(float4 vertex, float degrees) {
                float alpha = degrees * PI / 180.0;
                float sina, cosa; sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float4(mul(m, vertex.xz), vertex.yw).xzyw;
            }

            float3 CubicBezier(float3 p0, float3 p1, float3 p2, float3 p3, float t) {
                float omt = 1.0 - t;
                return omt * omt * omt * p0 + 3.0 * omt * omt * t * p1 + 3.0 * omt * t * t * p2 + t * t * t * p3;
            }

            float BulgeProfile(float t, float roundness, float bulge) {
                float belly = sin(t * PI) * bulge;
                float closeCurve = sqrt(max(0.0, 1.0 - pow(t, lerp(1.5, 4.0, roundness))));
                float openCurve = smoothstep(0.0, 0.15, t);
                return (closeCurve + belly) * openCurve;
            }

            // ==========================================
            // GEOMETRY SHADER
            // ==========================================

            [maxvertexcount(60)]
            void gp(point v2g points[1], inout TriangleStream<g2f> triStream) {
                int i;
                float4 root = points[0].vertex;

                float idHash = randValue(abs(root.x * 10000 + root.y * 100 + root.z * 0.05f + 2));
                idHash = randValue(idHash * 100000);
                float idHash2 = randValue(idHash * 54321);
                float idHash3 = randValue(idHash2 * 12345);

                float grassHeight = _Height * lerp(0.7f, 1.3f, idHash);
                float grassWidth = _Width * lerp(0.8f, 1.3f, idHash2);

                float curveAngle = idHash * 360.0;
                float cx = cos(curveAngle * PI / 180.0);
                float cz = sin(curveAngle * PI / 180.0);
                float curveMagnitude = _Curvature * grassHeight * 0.4;

                // --- VARIATION DE COULEUR (PATCHES + RANDOM) ---
                // On utilise la position du brin dans le monde pour générer des taches de couleur douces
                float3 worldRootPos = TransformObjectToWorld(root.xyz);
                float colorNoise = snoise(worldRootPos * _ColorNoiseScale) * 0.5 + 0.5;
                // On mélange le bruit (70%) avec le random individuel (30%) pour plus de naturel
                float finalColorVar = saturate(colorNoise * 0.7 + idHash2 * 0.3);

                // --- VENT ---
                float2 windDir = normalize(_WindDirection.xz + float2(0.001, 0.001));
                float2 windUV = root.xz * _WindScale * 0.1 - windDir * _Time.y * _WindSpeed;
                float windGust = snoise(float3(windUV.x, 0.0, windUV.y)) * 0.5 + 0.5;
                float microJitter = snoise(root.xyz * 2.0 + _Time.y * _WindSpeed * 2.0) * 0.1;
                float totalWindStrength = (windGust + microJitter) * _WindStrength * grassHeight;
                float2 windDisplacement = windDir * totalWindStrength;

                // --- INTERACTION ---
                float3 localInteractorPos = TransformWorldToObject(_InteractorPosition.xyz);
                float distToInteractor = distance(root.xz, localInteractorPos.xz);
                float interactFalloff = 1.0 - saturate(distToInteractor / _InteractorRadius);
                float2 pushDirXZ = normalize(root.xz - localInteractorPos.xz + float2(0.001, 0.001));
                float3 interactionDisplacement = float3(pushDirXZ.x, -0.8, pushDirXZ.y) * interactFalloff * _InteractorStrength * grassHeight;

                // --- APPLICATION AUX POINTS DE CONTROLE ---
                float3 p0 = root.xyz;
                float3 p1 = root.xyz + float3(
                    cx * curveMagnitude * 0.2 + windDisplacement.x * 0.2 + interactionDisplacement.x * 0.33,
                    max(0.1, grassHeight * 0.33 + interactionDisplacement.y * 0.33),
                    cz * curveMagnitude * 0.2 + windDisplacement.y * 0.2 + interactionDisplacement.z * 0.33
                );
                float3 p2 = root.xyz + float3(
                    cx * curveMagnitude * 0.7 + windDisplacement.x * 0.6 + interactionDisplacement.x * 0.66,
                    max(0.1, grassHeight * 0.66 + interactionDisplacement.y * 0.66),
                    cz * curveMagnitude * 0.7 + windDisplacement.y * 0.6 + interactionDisplacement.z * 0.66
                );
                float3 p3 = root.xyz + float3(
                    cx * curveMagnitude + windDisplacement.x + interactionDisplacement.x,
                    max(0.1, grassHeight + interactionDisplacement.y),
                    cz * curveMagnitude + windDisplacement.y + interactionDisplacement.z
                );

                const int SEGMENTS = 12;
                const int vertexCount = (SEGMENTS + 1) * 2;

                g2f v[vertexCount];
                for (i = 0; i < vertexCount; ++i) {
                    v[i].vertex = 0.0f; v[i].uv = 0.0f; v[i].worldPos = 0.0f; v[i].fogFactor = 0.0f; v[i].colorVar = 0.0f;
                }

                float rotation = idHash3 * 180.0f;

                for (i = 0; i <= SEGMENTS; ++i) {
                    float t_linear = float(i) / float(SEGMENTS);
                    float t = sin(t_linear * PI * 0.5); // Arrondi de la pointe

                    float3 curvePos = CubicBezier(p0, p1, p2, p3, t);

                    float widthMultiplier = BulgeProfile(t, _Roundness, _BulgeAmount);
                    float currentWidth = widthMultiplier * grassWidth;

                    float3 leftPos = curvePos + float3(-currentWidth, 0, 0);
                    float3 rightPos = curvePos + float3(currentWidth, 0, 0);

                    float4 leftLocal = float4(leftPos - root.xyz, 1);
                    float4 rightLocal = float4(rightPos - root.xyz, 1);

                    leftLocal = RotateAroundY(leftLocal, rotation);
                    rightLocal = RotateAroundY(rightLocal, rotation);

                    leftLocal.xyz += root.xyz;
                    rightLocal.xyz += root.xyz;

                    float3 worldLeft = TransformObjectToWorld(leftLocal.xyz);
                    float3 worldRight = TransformObjectToWorld(rightLocal.xyz);

                    float4 clipLeft = TransformWorldToHClip(worldLeft);
                    float4 clipRight = TransformWorldToHClip(worldRight);

                    int idx = i * 2;

                    // Assignation Vertex Gauche
                    v[idx].vertex = clipLeft;
                    v[idx].uv = float2(0, t);
                    v[idx].worldPos = worldLeft;
                    v[idx].fogFactor = ComputeFogFactor(clipLeft.z);
                    v[idx].colorVar = finalColorVar; // Transmission de la variation

                    // Assignation Vertex Droit
                    v[idx + 1].vertex = clipRight;
                    v[idx + 1].uv = float2(1, t);
                    v[idx + 1].worldPos = worldRight;
                    v[idx + 1].fogFactor = ComputeFogFactor(clipRight.z);
                    v[idx + 1].colorVar = finalColorVar; // Transmission de la variation
                }

                for (i = 0; i < SEGMENTS; ++i) {
                    int idx = i * 2;
                    triStream.Append(v[idx]); triStream.Append(v[idx + 2]); triStream.Append(v[idx + 1]);
                    triStream.Append(v[idx + 1]); triStream.Append(v[idx + 2]); triStream.Append(v[idx + 3]);
                }

                triStream.RestartStrip();
            }

            // ==========================================
            // FRAGMENT SHADER
            // ==========================================

            half4 fp(g2f i) : SV_Target {
                // --- Base Color ---
                float4 grassCol = lerp(_Albedo1, _Albedo2, i.uv.y);
                float aoMask = smoothstep(0.0, 0.4, i.uv.y);
                float4 ao = lerp(_AOColor, float4(1,1,1,1), aoMask);
                float tipMask = smoothstep(0.5, 1.0, i.uv.y);
                float4 tip = _TipColor * tipMask;

                float4 baseColor = (grassCol + tip) * ao;

                // --- Application de la variation ---
                float4 globalTint = lerp(_Color, _Color2, i.colorVar);

                float2 texUV = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texUV);

                // Mélange final de la couleur
                float3 finalAlbedo = baseColor.rgb * texCol.rgb * globalTint.rgb;

                // --- TOON LIGHTING & SHADOWS ---
                float3 stableNormal = float3(0, 1, 0);

                #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
                    float4 shadowCoord = ComputeScreenPos(i.vertex);
                #else
                    float4 shadowCoord = TransformWorldToShadowCoord(i.worldPos);
                #endif

                Light mainLight = GetMainLight(shadowCoord);

                half dMain = dot(stableNormal, mainLight.direction) * 0.5 + 0.5;
                half toonRampMain = smoothstep(_ToonOffset, _ToonOffset + _ToonSmooth, dMain);
                toonRampMain *= mainLight.shadowAttenuation;

                float3 toonLighting = mainLight.color * (toonRampMain + _ToonTint.rgb);

                // --- POINT LIGHTS / SPOT LIGHTS ---
                float3 extraLights = float3(0, 0, 0);
                int pixelLightCount = GetAdditionalLightsCount();

                for (int j = 0; j < pixelLightCount; ++j) {
                    Light aLight = GetAdditionalLight(j, i.worldPos, half4(1, 1, 1, 1));

                    float3 attenuatedLightColor = aLight.color * (aLight.distanceAttenuation * aLight.shadowAttenuation);

                    half dExtra = dot(stableNormal, aLight.direction) * 0.5 + 0.5;
                    half toonRampExtra = smoothstep(_ToonOffsetPoint, _ToonOffsetPoint + _ToonSmooth, dExtra);

                    extraLights += attenuatedLightColor * toonRampExtra;
                }

                toonLighting += extraLights;
                toonLighting += unity_AmbientSky.rgb;

                float3 finalRGB = finalAlbedo * toonLighting;

                // --- FOG ---
                float viewDistance = length(GetCameraPositionWS() - i.worldPos);
                float fogFactor = (_FogDensity / sqrt(log(2))) * (max(0.0f, viewDistance - _FogOffset));
                fogFactor = exp2(-fogFactor * fogFactor);

                float3 fogged = lerp(_FogColor.rgb, finalRGB, fogFactor);
                fogged = MixFog(fogged, i.fogFactor);

                return half4(fogged, 1.0);
            }

            ENDHLSL
        }
        }
}