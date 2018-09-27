in vec3 uv_vertexAttrib;

uniform mat4 uv_modelViewProjectionMatrix;
uniform float uv_simulationtimeSeconds;
uniform int uv_simulationtimeDays;
uniform vec4 uv_cameraPos;

uniform float gridScale;
uniform float shadingFactor;
uniform float A;
uniform float killFunctionDecay; 

uniform float eventTime;
uniform vec2 pfit;
uniform float GWAmpClamp;

out float falloffFactor;
out float colorHighlight;
out float fAlpha;

const float PI = 3.141592653589793;

float periodFunc(float t)
{
//returns the period in seconds (from a fit to the LIGO frequency vs. time plot)
	return pow(-1.*pfit[0]*t, pfit[1]);
}

void calcDistortion (inout vec3 pos, float w){
	
    //The equation to calculate the Ricci Scalar Curvature of a binary system is courtesy of Shane L. Larson
	//float t = uv_simulationtimeDays+uv_simulationtimeSeconds/(3600.*24.);

    float t = eventTime*2.5e4;//factor decided by eye to match NS locations
    float x = pos.x;
    float y = pos.y;
    float r = sqrt(x*x + y*y);
    float x2 = x * x;
    float y2 = y * y;
    float w2 = w*w;
    float cosmag = -3.*x2 + 4.*w2*x2*x2 + 3.*y2 - 4.*w2*y2*y2 + 12.*w*x*y*r;
    float sinmag = 2*(4*w2*x2*x*y - 3*w*x2*r + 3*w*y2*r + x*y*(-3+4*w2*y2));
    float trigarg = 2.*w*(-t + r)*0.5; //added factor to spread out the waves
    float kfarg = killFunctionDecay/r;
    float killfactor = exp(-kfarg*kfarg);
	//float amp = clamp((cosmag*cos(trigarg)+sinmag*sin(trigarg)) *pow(r, -5.), -1, 1);
	float amp = (cosmag*cos(trigarg)+ sinmag*sin(trigarg)) *pow(r, -5.);

    //pos.z = -A* amp * killfactor;
    pos.z = clamp(-A* amp, -GWAmpClamp, GWAmpClamp)*killfactor;

}

void main(){
	//this calculate the appropreate coorodinates of points on the grid and makes sure the edges of the membrane remain flat.
	vec3 vertexPosition = uv_vertexAttrib*gridScale;
	float dist = length(uv_vertexAttrib.xy);
	float pMin = 100.; // min value so we don't get a zero period
	float period = pMin;
	float offset = -0.2;
	float tval = offset * dist + eventTime;
	//float tval = eventTime*(1. + pow(dist, 2.));
	period = max(periodFunc(tval)*86400., pMin); //changing with radius from center (by eye) 
	fAlpha = 1.;
	if (period <= pMin){ //trying to get a flat plane after merger
		period = 1e10;
		fAlpha = 0.;
	}
	float w = 2.*PI/period * 5.; //angular frequency (rad/day) of the binary system, factor of 5 to try to match the NSs (by eye) 
	calcDistortion(vertexPosition, w);
	falloffFactor = min(1.0-(length(uv_vertexAttrib.xy)-0.8)/0.15,1.);
	vertexPosition.z *= falloffFactor;
	
	gl_Position = uv_modelViewProjectionMatrix * vec4(vertexPosition, 1.0);
	
	//Calculates a psuedonormailzed height to create contrast between positive and negative curvature values
	colorHighlight = max((vertexPosition.z/A)*shadingFactor+1,0);
	
	//Scale the grid up slightly and calculate the distance from the center (for a fade toward the edge)
	vec3 pos = uv_vertexAttrib*1.2;
	float r = length(pos.xy);
	
	//Throw away points that are too close to or too far from the center and fade the others to create a halo effect
	//fAlpha = min((.95-r)/.2+1,1.);
	fAlpha *= clamp(0.5 - r, 0., 1.) ;

	
}