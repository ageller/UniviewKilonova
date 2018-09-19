in vec3 uv_vertexAttrib;
// in vec3 uv_normalAttrib;

// out vec3 vPosition;
// out vec3 vNormal;


void main()
{
    gl_Position = vec4(uv_vertexAttrib , 1.0);
    // vPosition = uv_vertexAttrib;
    // vNormal = uv_normalAttrib;
}
