namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Contains the embedded GLSL shader sources used by the toroid demo.
/// </summary>
public static class ShaderSources
{
    /// <summary>
    /// Gets the vertex shader source.
    /// </summary>
    public static string VertexShader => """
#version 450
layout(push_constant) uniform PushConstants
{
    mat4 mvp;
    mat4 model;
} pc;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 0) out vec3 outWorldNormal;
layout(location = 1) out vec3 outViewDirection;

void main()
{
    vec3 worldPosition = (pc.model * vec4(inPosition, 1.0)).xyz;
    outWorldNormal = normalize((pc.model * vec4(inNormal, 0.0)).xyz);
    outViewDirection = normalize(-worldPosition);
    gl_Position = pc.mvp * vec4(inPosition, 1.0);
}
""";

    /// <summary>
    /// Gets the fragment shader source.
    /// </summary>
    public static string FragmentShader => """
#version 450
layout(location = 0) in vec3 outWorldNormal;
layout(location = 1) in vec3 outViewDirection;
layout(location = 0) out vec4 outColor;

void main()
{
    vec3 normal = normalize(outWorldNormal);
    vec3 viewDirection = normalize(outViewDirection);
    vec3 keyLightDirection = normalize(vec3(0.35, 0.75, 0.55));
    vec3 fillLightDirection = normalize(vec3(-0.40, 0.25, -0.65));
    vec3 halfVector = normalize(keyLightDirection + viewDirection);

    float ambient = 0.16;
    float diffuse = max(dot(normal, keyLightDirection), 0.0);
    float fill = max(dot(normal, fillLightDirection), 0.0) * 0.18;
    float rim = pow(1.0 - max(dot(normal, viewDirection), 0.0), 2.0) * 0.25;
    float specular = pow(max(dot(normal, halfVector), 0.0), 48.0) * 0.30;

    vec3 baseColor = vec3(0.17, 0.60, 0.94);
    vec3 litColor = baseColor * (ambient + diffuse * 0.82 + fill + rim) + vec3(specular);
    outColor = vec4(clamp(litColor, 0.0, 1.0), 1.0);
}
""";

    /// <summary>
    /// Gets the HUD vertex shader source.
    /// </summary>
    public static string HudVertexShader => """
#version 450
layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 0) out vec3 outColor;

void main()
{
    outColor = inColor;
    gl_Position = vec4(inPosition, 0.0, 1.0);
}
""";

    /// <summary>
    /// Gets the HUD fragment shader source.
    /// </summary>
    public static string HudFragmentShader => """
#version 450
layout(location = 0) in vec3 outColor;
layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = vec4(outColor, 1.0);
}
""";
}


