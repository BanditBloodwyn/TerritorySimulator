#version 420 core

in vec3 FragPos;
in vec3 Normal;
in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D texture0;
uniform sampler2D texture1;

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float     shininess;
};
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;

void main()
{
	// Ambient
	vec4 ambient = vec4(light.ambient, 1.0) * texture(texture0, texCoord);

    // Diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(vec3(light.position - FragPos));
    float diff = max(dot(norm, lightDir), 0.0);
    vec4 diffuse = vec4(light.diffuse, 1.0) * diff * texture(texture0, texCoord);

    // Specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec4 specular = vec4(light.specular, 1.0) * spec * texture(texture0, texCoord);

	vec4 result = ambient + diffuse + specular;   
	
	if(result.a < -1)
		discard;	
		
    FragColor = result;
}