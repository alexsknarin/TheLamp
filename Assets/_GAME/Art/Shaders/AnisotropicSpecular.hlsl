// ANISOTROPIC SPECULAR WITH ROTATION AND POSITION SHIFT
#ifndef ANISOTROPIC_SPECULAR
#define ANISOTROPIC_SPECULAR

// Helper function to create a rotation matrix around the normal vector
float3x3 AngleAxisRotationMatrix(float3 axis, float angle)
{
    float c = cos(angle);
    float s = sin(angle);
    float t = 1.0 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
        t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
        t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
    );
}

// Helper function to calculate anisotropic specular contribution with rotation and position shift
float AnisoSpecular(float3 tangent, float3 bitangent, float3 normal, float3 viewDir, float3 lightDir, 
                   float roughnessX, float roughnessY, float rotation, float2 shift)
{
    // Rotate tangent and bitangent around normal by the rotation angle
    float3x3 rotationMatrix = AngleAxisRotationMatrix(normal, rotation);
    float3 rotatedTangent = mul(rotationMatrix, tangent);
    float3 rotatedBitangent = mul(rotationMatrix, bitangent);
    
    // Calculate half vector
    float3 halfVec = normalize(lightDir + viewDir);
    
    // Apply shift to the half vector in tangent space
    // First create a local coordinate system (tangent space)
    float3x3 tangentToWorld = float3x3(
        rotatedTangent,
        rotatedBitangent,
        normal
    );
    
    // Convert half vector to tangent space
    float3 halfVecTangentSpace = mul(transpose(tangentToWorld), halfVec);
    
    // Apply shift in tangent space (x,y directions)
    halfVecTangentSpace.xy += shift;
    
    // Normalize the shifted vector
    halfVecTangentSpace = normalize(halfVecTangentSpace);
    
    // Convert back to world space
    float3 shiftedHalfVec = mul(tangentToWorld, halfVecTangentSpace);
    
    // Compute dot products with the shifted half vector
    float NdotL = max(0.001, dot(normal, lightDir));
    float NdotV = max(0.001, dot(normal, viewDir));
    float NdotH = max(0.001, dot(normal, shiftedHalfVec));
    float TdotH = dot(rotatedTangent, shiftedHalfVec);
    float BdotH = dot(rotatedBitangent, shiftedHalfVec);
    
    // Square the roughness values to match perceptual roughness model
    float alphaX = max(0.001, roughnessX * roughnessX);
    float alphaY = max(0.001, roughnessY * roughnessY);
    
    // GGX-based anisotropic distribution
    float alpha2X = alphaX * alphaX;
    float alpha2Y = alphaY * alphaY;
    
    float denominator = (TdotH * TdotH) / alpha2X + (BdotH * BdotH) / alpha2Y + NdotH * NdotH;
    float D = 1.0 / (3.14159265359 * alphaX * alphaY * denominator * denominator);
    
    // Visibility term (simplified)
    float G = (NdotL * NdotV) / max(0.001, (NdotL + NdotV - NdotL * NdotV));
    
    return D * G * NdotL;
}

void AnisotropicSpecular_float(
    float3 WorldPosition,
    float3 CameraPosition,
    float3 Normal,
    float3 Tangent,
    float RoughnessX,
    float RoughnessY,
    float Rotation,  // Rotation angle in radians
    float2 Shift,    // Shift amount in tangent space (x,y)
    out float3 OutColor)
{
#ifdef SHADERGRAPH_PREVIEW    
    OutColor = float3(0.5, 0.5, 0.5);
#else
    // Calculate bitangent from normal and tangent
    float3 normalizedNormal = normalize(Normal);
    float3 normalizedTangent = normalize(Tangent);
    // Make sure tangent is perpendicular to normal
    normalizedTangent = normalize(normalizedTangent - normalizedNormal * dot(normalizedTangent, normalizedNormal));
    float3 bitangent = normalize(cross(normalizedNormal, normalizedTangent));
    
    // View direction
    float3 viewDir = normalize(CameraPosition - WorldPosition);
    
    // Initialize output color
    OutColor = float3(0, 0, 0);
    
    // Main directional light contribution
    Light mainLight = GetMainLight();
    float3 mainLightDir = mainLight.direction;
    float3 mainLightColor = mainLight.color;
    
    float mainSpec = AnisoSpecular(normalizedTangent, bitangent, normalizedNormal, viewDir, mainLightDir,
                                  RoughnessX, RoughnessY, Rotation, Shift);
    OutColor += mainLightColor * mainSpec;
    
    // Additional lights contribution
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_FORWARD_PLUS
        InputData inputData = (InputData)0;
        inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(WorldPosition);
        inputData.positionWS = WorldPosition;
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        #if !USE_FORWARD_PLUS
            Light light = GetAdditionalPerObjectLight(lightIndex, WorldPosition);
        #else
            Light light = GetAdditionalLight(lightIndex, WorldPosition, inputData.shadowMask);
        #endif
        
        float addSpec = AnisoSpecular(normalizedTangent, bitangent, normalizedNormal, viewDir, light.direction,
                                     RoughnessX, RoughnessY, Rotation, Shift);
        OutColor += light.color * addSpec * light.distanceAttenuation * light.shadowAttenuation;
    LIGHT_LOOP_END
#endif
}
#endif