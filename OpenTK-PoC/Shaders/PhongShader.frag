#version 460

uniform sampler2D textureSampler;
uniform sampler2D shadowSampler;

uniform vec3 lightPos;

uniform mat4 modelMatrix;
uniform vec3 cameraPos;

in vec4 lightSpacePos;
in vec3 worldPosition;
in vec2 fUv;
in vec3 fNormal;

out vec4 fragColor;

const float lightPower = 20.0;
const vec4 lightColor = vec4(1.0, 1.0, 1.0, 1.0);
const float phongExponent = 1;
const float ka = 0.5;
const float kd = 0.45;
const float ks = 0.05;
const float shadowBias = 0.0005;
const float diff = 1;
const float shadowOffset = 0.001;
const float delta = 0.0005;
const int shadowSamplesPerRow = 5;
const int shadowSamplesHalfed = 2;

// A single iteration of Bob Jenkins' One-At-A-Time hashing algorithm.
uint hash( uint x ) {
    x += ( x << 10u );
    x ^= ( x >>  6u );
    x += ( x <<  3u );
    x ^= ( x >> 11u );
    x += ( x << 15u );
    return x;
}

// Construct a float with half-open range [0:1] using low 23 bits.
// All zeroes yields 0.0, all ones yields the next smallest representable value below 1.0.
float floatConstruct( uint m ) {
    const uint ieeeMantissa = 0x007FFFFFu; // binary32 mantissa bitmask
    const uint ieeeOne      = 0x3F800000u; // 1.0 in IEEE binary32

    m &= ieeeMantissa;                     // Keep only mantissa bits (fractional part)
    m |= ieeeOne;                          // Add fractional part to 1.0

    float  f = uintBitsToFloat( m );       // Range [1:2]
    return f - 1.0;                        // Range [0:1]
}

float shadowFactor(float currentDepth, vec2 tex)
{
    float closestDepth = float(texture(shadowSampler, tex));
    //check neighbours for smoother shadow?
    return currentDepth - shadowBias > closestDepth ? 0.0 : 1.0;
}

float ShadowCalculation(vec4 fragPosLightSpace)
{
    // perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    if (projCoords.x < 0) return 1.0;
    if (projCoords.x > 1) return 1.0;
    if (projCoords.y < 0) return 1.0;
    if (projCoords.y > 1) return 1.0;
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow

    float a[shadowSamplesPerRow * shadowSamplesPerRow];

    for(int x = 0; x < shadowSamplesPerRow; x++){
        for(int y = 0; y < shadowSamplesPerRow; y++){
            vec2 randomOffset = (delta * vec2(floatConstruct(x), floatConstruct(y))) / (2.0*shadowSamplesPerRow);
            vec2 gridOffset = vec2((x-shadowSamplesHalfed) * delta, (y-shadowSamplesHalfed) * delta);
            a[y*shadowSamplesPerRow+x] = shadowFactor(currentDepth, projCoords.xy + gridOffset );;
        }
    }

    float sum = 0.0;
    for(int i = 0; i < shadowSamplesPerRow * shadowSamplesPerRow; i++){
        sum += a[i];
    }

    return sum / (shadowSamplesPerRow * shadowSamplesPerRow);
}

void main()
{
    vec4 baseColor = texture(textureSampler, fUv);
    vec3 n = normalize(vec3(modelMatrix * vec4(fNormal, 0.0)));

    vec3 l = lightPos - worldPosition;
    vec3 ln = normalize(l);

    vec4 ambient = baseColor;

    float ldotn = dot(ln, n);
    vec4 diffuse = baseColor * max(ldotn, 0.0);

    vec3 v = normalize(cameraPos - worldPosition);
    vec3 r = 2 * ldotn * n - ln;
    vec4 specular = lightColor * pow(max(dot(r,v),0.0), phongExponent);

    float localLightIntensity = lightPower / dot(l,l);

    float shadowColor = ShadowCalculation(lightSpacePos);

    vec4 color = ka * ambient + localLightIntensity * shadowColor * (kd * diffuse +  ks * specular);
    if(baseColor.a < 0.99){
        color.a = 0.0;
    } else {
        color.a = 1.0;
    }

    fragColor = color;
}