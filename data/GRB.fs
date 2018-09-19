uniform float uv_fade;

in vec2 texcoord;
in vec3 color;

out vec4 fragColor;

void main()
{
    fragColor = vec4(color, 1.);
    fragColor.a *= uv_fade;
    vec2 fromCenter = texcoord * 2 - vec2(1);
    float dist = dot(fromCenter,fromCenter);
    if (dist > 1){
    	discard;
    }
	fragColor.a *= exp(-0.5*dist/0.1);
    //fragColor.a *= smoothstep(-1.5, -0.5, -length(fwidth(texcoord.xy)));
    //fragColor.a *= pow(max(0, 1 - dot(fromCenter, fromCenter)), 2);
}
