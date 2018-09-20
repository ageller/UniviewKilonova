uniform float uv_fade;
uniform mat4 uv_modelViewInverseMatrix;

in vec2 texcoord;
in vec4 color;
in vec3 NSpos;

in float fstarNum;
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
	fragColor = color;
	vec2 fromCenter = texcoord * 2 - vec2(1);
	float dist = dot(fromCenter,fromCenter);
	float angle = atan((texcoord.x - 0.5)*2., (texcoord.y - 0.5)*2.);

	float u = 1.;
	float Rstar2 = 1.0;
	float brightness = (1. - u*(1. - sqrt((Rstar2 - dist)/Rstar2)));
	fragColor.a *= uv_fade;

	vec3 cameraPosition = (uv_modelViewInverseMatrix * vec4(0, 0, 0, 1)).xyz;

	vec3 cNorm = normalize(cameraPosition);
	vec3 pNorm = vec3(angle, 0., fstarNum) + cNorm;// + NSpos;

// Offset distance with noise
	float cn = noise(pNorm, 7, 1.4, 0.7, 0);
	dist /= cn;

	float alpha = clamp( 1. - pow(dist, 0.3), 0.0, 1.0);

	fragColor.a *= alpha;

	fragColor.rgb = vec3(mix(vec3(1.0, 1.0, 1.0), fragColor.rgb, clamp(dist ,0. , 1.)));

	//fragColor.rgb = vec3(tn);

}
