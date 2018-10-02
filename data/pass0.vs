in vec3 uv_vertexAttrib;
out vec2 texCoord;

void main(){
	
	gl_Position = vec4(sign(uv_vertexAttrib.xy),0.,1);
	texCoord = .5*gl_Position.xy+.5;
}