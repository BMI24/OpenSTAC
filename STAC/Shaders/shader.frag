#version 450 core
uniform vec2 ScreenSize;

const int MAX_MARCHING_STEPS = 255;
const float MIN_DIST = 0.0;
const float MAX_DIST = 100.0;
const float EPSILON = 0.0001;

float sphereSDF(vec3 p, vec3 center, float radius){
    return length(p-center) - radius;
}

float sceneSDF(vec3 p){
    return sphereSDF(p, vec3(0,0,0), 1.0);
}

vec3 rayDirection(float fov, vec2 fragCoord) {
    vec2 xy = fragCoord - ScreenSize / 2.0;
    float z = ScreenSize.y / tan(radians(fov) / 2.0);
    return normalize(vec3(xy, -z));
}

float march(vec3 origin, vec3 dir, float near, float far){
    float dist = near;
    for (int i = 0; i < MAX_MARCHING_STEPS; i++){
        float distAtSample = sceneSDF(origin + dist * dir);
        if (distAtSample < EPSILON){
            return dist;
        }
        dist += distAtSample;
        if (dist >= far){
            return far;
        }
    }
    return far;
}

out vec4 outputColor;
in vec2 texCoordV;

void main() {
    vec3 dir = rayDirection(45.0, texCoordV * ScreenSize);
    vec3 cam = vec3(0.0, 0.0, 5.0);

    float dist = march(cam, dir, MIN_DIST, MAX_DIST);
    
    
    if (dist > MAX_DIST - EPSILON){
        outputColor = vec4(0);
        return;
    }

    vec2 uv = texCoordV.xy;
    outputColor = vec4(uv.x, uv.y, 0, 0);
}

