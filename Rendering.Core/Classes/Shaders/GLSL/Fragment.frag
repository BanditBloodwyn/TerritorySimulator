#version 420 core

in vec3 normal;
in vec2 texCoord;
in vec3 FragPos;

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

void main()
{
	// Ambient
	vec3 ambient = light.ambient * vec3(texture(material.diffuse, texCoord));
	
    // Diffuse 
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(vec3(light.position - FragPos));
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, texCoord));
    	
	vec3 result = ambient + diffuse;
	
    //if(result.a < 0.1)
    //    discard;

    
    FragColor = vec4(result, 1.0);
}