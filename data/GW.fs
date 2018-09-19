uniform float uv_fade;

in vec2 texcoord;
in vec4 color;

out vec4 fragColor;

void main()
{
    fragColor = color;
    fragColor.a *= uv_fade;

	//fragColor.a *= exp(-0.5*dist/0.1);
    //fragColor.a *= smoothstep(-1.5, -0.5, -length(fwidth(texcoord.xy)));
    //fragColor.a *= pow(max(0, 1 - dot(fromCenter, fromCenter)), 2);
}
