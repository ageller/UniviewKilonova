uniform sampler2D stateTexture;
uniform float transitionLength;
uniform float eventTime;
uniform vec2 timeRange;
uniform bool jump;
uniform float uv_timeSeconds;
out vec4 FragColor;

void main(){
    float epsilon = 0.01;
	vec4 currentColor = texture(stateTexture, vec2(0.5));
	float currentTime = clamp(currentColor.r,timeRange[0],timeRange[1]);
	float transitionStart = currentColor.g;
	float desiredTime = clamp(eventTime,timeRange[0],timeRange[1]);
	float yearStart = clamp(currentColor.b,timeRange[0],timeRange[1]);
	float transitionSpeed = clamp(transitionLength,0.01,1000.);
	if (abs(currentTime-desiredTime)>epsilon && !jump) {
		if (transitionStart <1.0) {
			transitionStart = uv_timeSeconds;
		}
		float newTime = mix(yearStart,desiredTime,clamp((uv_timeSeconds-transitionStart)/transitionSpeed,0,1)) ;
		newTime = clamp(newTime,timeRange[0],timeRange[1]);
		FragColor = vec4(newTime,transitionStart,yearStart,currentColor.a);
	} else {
		FragColor = vec4(desiredTime,-1.0,desiredTime,currentColor.a);
	}
}