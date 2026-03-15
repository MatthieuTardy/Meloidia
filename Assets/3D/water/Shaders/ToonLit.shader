Shader "Roystan/Toon/Lit_Transparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // 1. On change les Tags pour indiquer à Unity de rendre ce shader dans la queue de transparence
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        // 2. Paramètres de transparence classique (Alpha Blending)
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = normalize(mul((float3x3)UNITY_MATRIX_M, v.normal));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float NdotL = dot(i.worldNormal, _WorldSpaceLightPos0);
                
                // On garde la lumière sous forme de float3 pour éviter les soucis avec l'alpha
                float3 light = saturate(floor(NdotL * 3) / (2 - 0.5)) * _LightColor0.rgb;

                float4 col = tex2D(_MainTex, i.uv);
                
                // 3. On sépare le calcul RGB du calcul Alpha
                float3 finalRGB = (col.rgb * _Color.rgb) * (light + unity_AmbientSky.rgb);
                float finalAlpha = col.a * _Color.a;

                // On renvoie un float4 combinant la couleur et l'opacité
                return float4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}