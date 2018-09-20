uniform float uv_fade;
uniform mat4 uv_modelViewInverseMatrix;

uniform sampler2D cmap;

in vec2 texcoord;
in vec3 color;
in float fTime;
in float fMaxT;

out vec4 fragColor;

//from https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
//simple 3D noise
float mod289(float x){return x - floor(x * (1.0 / 289.0)) * 289.0;}
vec4 mod289(vec4 x){return x - floor(x * (1.0 / 289.0)) * 289.0;}
vec4 perm(vec4 x){return mod289(((x * 34.0) + 1.0) * x);}
float snoise(vec3 p){
	vec3 a = floor(p);
	vec3 d = p - a;
	d = d * d * (3.0 - 2.0 * d);

	vec4 b = a.xxyy + vec4(0.0, 1.0, 0.0, 1.0);
	vec4 k1 = perm(b.xyxy);
	vec4 k2 = perm(k1.xyxy + b.zzww);

	vec4 c = k2 + a.zzzz;
	vec4 k3 = perm(c);
	vec4 k4 = perm(c + 1.0);

	vec4 o1 = fract(k3 * (1.0 / 41.0));
	vec4 o2 = fract(k4 * (1.0 / 41.0));

	vec4 o3 = o2 * d.z + o1 * (1.0 - d.z);
	vec2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

	return o4.y * d.y + o4.x * (1.0 - d.y);
}

// from https://www.seedofandromeda.com/blogs/49-procedural-gas-giant-rendering-with-gpu-noise
//fractal noise
float noise(vec3 position, int octaves, float frequency, float persistence, int rigid) {
	float total = 0.0; // Total value so far
	float maxAmplitude = 0.0; // Accumulates highest theoretical amplitude
	float amplitude = 1.0;
	const int largeN = 50;
	for (int i = 0; i < largeN; i++) {
		if (i > octaves){
				break;
		}
		// Get the noise sample
		if (rigid == 0){
		   total += snoise(position * frequency) * amplitude;
		} else {
		// rigid noise
			total += ((1.0 - abs(snoise(position * frequency))) * 2.0 - 1.0) * amplitude;
		}
		// Make the wavelength twice as small
		frequency *= 2.0;
		// Add to our maximum possible amplitude
		maxAmplitude += amplitude;
		// Reduce amplitude according to persistence for the next octave
		amplitude *= persistence;
	}

	// Scale the result by the maximum amplitude
	return total / maxAmplitude;
}


void main()
{
	vec2 fromCenter = texcoord * 2 - vec2(1);
    float dist = 4.*dot(fromCenter,fromCenter); //factor of 4 so that we don't reach the edge of the billboard

	vec3 cm = texture(cmap ,vec2(clamp(dist,0.,1),0.5)).rgb;
    //fragColor = vec4(color, 1.);
    fragColor = vec4(cm, 1.);
    fragColor.a *= uv_fade;
 	fragColor.a *= exp(-0.5*dist/0.1);

	if (fTime > fMaxT){
		float fac = clamp(1./pow(1 + (fTime - fMaxT), 4.), 0, 1.);
		fragColor.a *= pow(dist*fac, 0.5); //hole in center, like a shell coming out
		fragColor.a *= fac; //overall fade
	}
	
	//noise
	vec3 cameraPosition = (uv_modelViewInverseMatrix * vec4(0, 0, 0, 1)).xyz;

	vec3 cNorm = normalize(cameraPosition);
	vec3 pNorm = 10.*vec3(texcoord, fTime*0.3) + cNorm;

	//fractal noise (can play with these)
	float n1 = noise(pNorm, 7, 3., 0.7, 1); 

	// spots
	float s = 0.1;
	float frequency = 3;//
	float threshold = 0.1;// limit number of spots
	float t1 = snoise(pNorm * frequency) - s;
	float t2 = snoise((pNorm + 30.) * frequency) - s;
	float ss = (max(t1 * t2, threshold) - threshold) ;

	// Accumulate total noise
	float n =clamp(n1 - ss + 0.3, 0, 1);
	fragColor *= n;

}
