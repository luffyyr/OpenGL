#version 330 core

out vec4 FragColor;

in vec3 Normal;  
in vec3 FragPos;  
in vec2 TexCoords;

uniform vec3 viewPos;

struct Material {
    sampler2D diffuse;
    sampler2D specular  ;
    float shininess;
}; 

struct Light {
    vec3 position;
    vec3 direction;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;

    float cutOff;
    float outerCutOff;
};

uniform Light light;  
uniform Material material;

void main()
{  
        //ambient
        vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

        //diffuse
        vec3 norm = normalize(Normal);
        vec3 lightDir = normalize(light.position - FragPos);     // for spot light 
        //vec3 lightDir = normalize(-light.direction);   // for directional light

        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * vec3(texture(material.diffuse, TexCoords)) * light.diffuse;

        //specular
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);  

        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = vec3(texture(material.specular, TexCoords)) * spec * light.specular;  

        // spotlight (soft edges)
        float theta = dot(lightDir, normalize(-light.direction)); 
        float epsilon = (light.cutOff - light.outerCutOff);
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
        diffuse  *= intensity;
        specular *= intensity;

        // attenuation
        float distance    = length(light.position - FragPos);
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));  

        ambient  *= attenuation; 
        diffuse  *= attenuation;
        specular *= attenuation;   

        FragColor = vec4((ambient + diffuse + specular) , 1.0);
}