#version 330 core
out vec4 FragColor;
in vec3 Normal;  
in vec3 FragPos;  

uniform vec3 ourColor;
uniform vec3 lightColor;
uniform vec3 lightPos;  
uniform vec3 viewPos;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
}; 

struct Light {
    vec3 position;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;  
uniform Material material;

void main()
{
    //ambient
    vec3 ambient = material.ambient * light.ambient;

    //diffuse
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);  

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = (diff * material.diffuse) * light.diffuse;

    //specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = material.specular * spec * light.specular;  

    //FragColor = vec4((ambient + diffuse + specular) * ourColor, 1.0);
    FragColor = vec4((ambient + diffuse + specular) , 1.0);
}