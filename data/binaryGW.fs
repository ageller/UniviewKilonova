uniform float uv_fade;

uniform vec4 fillColor;

in float falloffFactor;
in float colorHighlight;
in float fAlpha;

out vec4 fragColor;

void main(){
	//Throw away fragments that are too far from the center
	if (falloffFactor < 0)
		discard;
	//Calculate the fragment color based on the set membrane color and the value of the curvature
	vec4 color = fillColor;
	color.rgb = color.rgb * colorHighlight;
	fragColor = color * uv_fade;
	fragColor.a = fAlpha;
}