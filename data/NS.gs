layout(triangles) in; //seems necessary to have this even if I don't need any data?
layout(triangle_strip, max_vertices = 4) out;

uniform mat4 uv_modelViewProjectionMatrix;
uniform mat4 uv_modelViewInverseMatrix;
uniform int uv_simulationtimeDays;
uniform float uv_simulationtimeSeconds;

uniform float eventTime;
uniform vec2 pfit;
uniform float starNum;
uniform float NSrad;


out vec2 texcoord;
out vec3 NSpos;
out vec4 color;
out float fstarNum;

const float PI = 3.141592653589793;
const float Grav = 132712440000.0; //km**3 / (s**2 solMass)

// axis should be normalized
mat3 rotationMatrix(vec3 axis, float angle)
{
	float s = sin(angle);
	float c = cos(angle);
	float oc = 1.0 - c;
	
	return mat3(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
				oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
				oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}

float periodFunc(float t)
{
//returns the period in seconds (from a fit to the LIGO frequency vs. time plot)
	return pow(-1.*pfit[0]*t, pfit[1]);
}

vec3 getbinxyz(vec3 xb, float m1, float m2, float ecc, float per, float omega, float pa, float zi, float tim, float sNum)
{
//from an old IDL code of mine, used for generating binary orbits for N-body
//working in units of solar mass, km, s

// calculate the semi-major axis    
	float a3 = per*per * Grav * (m1 + m2) / (4.0 * PI*PI);
	float semi = pow( a3 , 1./3.);

//Set values at aopcentre. 
	vec2 xorb = vec2(semi*(1.0 + ecc), 0.);
	
// Set transformation elements (Brouwer & Clemence p. 35).
// also Murray & Dermott page 51
// O=pa
// w=omega
// I=zi
	vec3 px = vec3(0.);
	vec3 qx = vec3(0.);
	float f = mod(tim, per)/per*2.*PI;
	px[0] =     cos(pa)*cos(omega + f) - sin(pa)*sin(omega + f)*cos(zi);
	qx[0] = -1.*sin(pa)*cos(omega + f) - cos(pa)*sin(omega + f)*cos(zi);
	px[1] =     sin(pa)*cos(omega + f) + cos(pa)*sin(omega + f)*cos(zi);
	qx[1] = -1.*cos(pa)*cos(omega + f) + sin(pa)*sin(omega + f)*cos(zi);
	px[2] =     sin(omega + f)*sin(zi);
	qx[2] =     cos(omega + f)*sin(zi);
	
// // Transform to relative variables.
	vec3 xrel = px*xorb[0] + qx*xorb[1];

// calculate the true xyz positions
	vec3 xsb = vec3(0);
	xsb = xb + m2*xrel/(m1 + m2);
	if (sNum > 1.){
		xsb -= xrel;
	}
	return xsb;
}

void drawSprite(vec4 position, float radius, float rotation)
{
	vec3 objectSpaceUp = vec3(0, 0, 1);
	vec3 objectSpaceCamera = (uv_modelViewInverseMatrix * vec4(0, 0, 0, 1)).xyz;
	vec3 cameraDirection = normalize(objectSpaceCamera - position.xyz);
	vec3 orthogonalUp = normalize(objectSpaceUp - cameraDirection * dot(cameraDirection, objectSpaceUp));
	vec3 rotatedUp = rotationMatrix(cameraDirection, rotation) * orthogonalUp;
	vec3 side = cross(rotatedUp, cameraDirection);
	texcoord = vec2(0, 1);
	gl_Position = uv_modelViewProjectionMatrix * vec4(position.xyz + radius * (-side + rotatedUp), 1);
	EmitVertex();
	texcoord = vec2(0, 0);
	gl_Position = uv_modelViewProjectionMatrix * vec4(position.xyz + radius * (-side - rotatedUp), 1);
	EmitVertex();
	texcoord = vec2(1, 1);
	gl_Position = uv_modelViewProjectionMatrix * vec4(position.xyz + radius * (side + rotatedUp), 1);
	EmitVertex();
	texcoord = vec2(1, 0);
	gl_Position = uv_modelViewProjectionMatrix * vec4(position.xyz + radius * (side - rotatedUp), 1);
	EmitVertex();
	EndPrimitive();
}

void main()
{
	fstarNum = starNum;
	color = vec4(0);
	vec3 xb = vec3(gl_in[0].gl_Position.x, gl_in[0].gl_Position.y, gl_in[0].gl_Position.z); //and it is also apparently necessary to access the data??

	if (eventTime <= 0){
		//NS
		color = vec4(0.,0.,1.,1.);

		//initial parameters for the NSs
		float m1 = 1.8;
		float m2 = 1.1;
		float ecc = 0.;
		float omega = 0.;
		float pa = 0.;
		float zi = 0.;

		float period = periodFunc(eventTime);
		NSpos = getbinxyz(xb, m1, m2, ecc, period, omega, pa, zi, eventTime, starNum);

		vec4 pos = vec4(NSpos, 1.);
		drawSprite(pos, NSrad, 0);
	} 


}
