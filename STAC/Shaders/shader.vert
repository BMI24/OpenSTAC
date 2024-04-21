#version 330 core

// from https://stackoverflow.com/a/59739538/8512719 (screen-filling triangle)
out vec2 texCoordV; // texcoords are in the normalized [0,1] range for the viewport-filling quad part of the triangle
void main() {
        vec2 vertices[3]=vec2[3](vec2(-1,-1), vec2(3,-1), vec2(-1, 3));
        gl_Position = vec4(vertices[gl_VertexID],0,1);
        texCoordV = 0.5 * gl_Position.xy + vec2(0.5);
}