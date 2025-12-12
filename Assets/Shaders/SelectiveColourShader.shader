Shader "Custom/SelectiveColorPostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tolerance ("Tolerance", Range(0, 1)) = 0.01
        [Toggle] _UseNormalizedColor ("Match Normalized Color", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "SelectiveColorPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _USENORMALIZEDCOLOR_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Tolerance;
            float4 _PassColors[16];
            int _PassColorCount;

            // Normalize color to remove lighting influence
            float3 NormalizeColor(float3 color)
            {
                float maxComponent = max(max(color.r, color.g), color.b);
                if (maxComponent < 0.01) return float3(0, 0, 0); // Too dark, return black
                return color / maxComponent; // Normalize to keep color ratios
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                // Calculate grayscale using proper luminance values
                float luminance = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));
                half3 grayscale = half3(luminance, luminance, luminance);

                // Check if color matches any pass-through colors
                float isMatch = 0.0;

                // Only check non-black pixels
                if (luminance > 0.01)
                {
                    float3 pixelColor = col.rgb;

                    #ifdef _USENORMALIZEDCOLOR_ON
                    // Normalize colors to remove brightness/lighting influence
                    // This preserves color ratios (hue) regardless of lighting
                    pixelColor = NormalizeColor(pixelColor);
                    #endif

                    for (int j = 0; j < _PassColorCount; j++)
                    {
                        float3 passColor = _PassColors[j].rgb;

                        #ifdef _USENORMALIZEDCOLOR_ON
                        passColor = NormalizeColor(passColor);
                        #endif

                        // Calculate distance between colors
                        float colorDistance = distance(pixelColor, passColor);

                        // If distance is less than tolerance, this is a match
                        if (colorDistance <= _Tolerance)
                        {
                            isMatch = 1.0;
                            break;
                        }
                    }
                }

                // Lerp between grayscale and original based on match
                col.rgb = lerp(grayscale, col.rgb, isMatch);

                return col;
            }
            ENDHLSL
        }
    }
}
