#version 330
// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;
in vec3 fragNormal;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;

// Output fragment color
out vec4 finalColor;

// NOTE: Add your custom variables here

void main()
{
    // Texel color fetching from texture sampler
    vec4 texelColor = texture(texture0, fragTexCoord);
    
    if (fragNormal == vec3(0.0, 0.0, 0.0)) {
        discard;
    }

    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.3));
    float diff = (dot(fragNormal, lightDir) + 1.0) / 2.0;
    texelColor.rgb *= diff;

    // NOTE: Implement here your fragment shader code

    // final color is the color from the texture 
    //    times the tint color (colDiffuse)
    //    times the fragment color (interpolated vertex color)
    finalColor = texelColor*colDiffuse*fragColor;
}