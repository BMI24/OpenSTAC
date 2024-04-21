using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class TeapotSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "teapotSDF";
        public GlobalIdentifier SMinName { get; } = "teapot_smin";
        public GlobalIdentifier SMaxName { get; } = "teapot_smin";
        public GlobalIdentifier SquareName { get; } = "sq";
        public GlobalIdentifier TorusName { get; } = "torus";
        public GlobalIdentifier LidName { get; } = "lid";
        public GlobalIdentifier NoseName { get; } = "nose";
        public GlobalIdentifier TeapotName { get; } = "teapot";
        public Vector3 Center { get; set; }
        public float Scale { get; set; } = 1f;

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""

                // <Begin Teapot Function>
                // licensed under CC BY-NC-SA 3.0 by NVIDIA CORPORATION
                // Accessible at https://github.com/tovacinni/sdf-explorer/blob/master/data-files/sdf/Manufactured/Teapot.glsl

                // Smooth combine functions from IQ

                float {{SMinName}}(float a, float b, float k)
                {
                    float h=clamp(0.5+0.5*(b-a)/k, 0.0, 1.0);
                    return mix(b, a, h)-k*h*(1.0-h);
                }

                float {{SMaxName}}( float a, float b, float k)
                {
                    return -{{SMinName}}(-a,-b,k);
                }

                float {{SMinName}}( float a, float b)
                {
                    return {{SMinName}}(a,b,0.1);
                }

                float {{SMaxName}}( float a, float b)
                {
                    return {{SMaxName}}(a,b,0.1);
                }

                float {{SquareName}}(float x){return x*x;}

                float {{TorusName}}(float x, float y, float z, float R, float r)
                {
                    vec2 xz = vec2(x, z); 
                    vec2 q = vec2(length(xz)-R,y); 
                    return length(q)-r;
                }

                float {{TorusName}}(vec3 p, float R, float r)
                {
                    vec2 q = vec2(length(p.xz)-R,p.y);
                    return length(q)-r;
                }


                float {{LidName}}(float x, float y, float z)
                {
                    float v=sqrt({{SquareName}}(x)+{{SquareName}}(y-0.55)+{{SquareName}}(z))-1.4;
                    v={{SMinName}}(v,{{TorusName}}(y-2.,x,z,.2,.08),.1);
                    v={{SMaxName}}(v,-sqrt({{SquareName}}(x)+{{SquareName}}(y-0.55)+{{SquareName}}(z))+1.3);
                    v={{SMaxName}}(v,sqrt({{SquareName}}(x)+{{SquareName}}(y-2.5)+{{SquareName}}(z))-1.3);

                    v={{SMaxName}}(v,-sqrt({{SquareName}}(x-.25)+{{SquareName}}(z-.35))+0.05,.05);
                    v={{SMinName}}(v,{{TorusName}}(x,(y-1.45)*.75,z,.72,.065),.2);
                    return v;
                }

                float {{NoseName}}(float x, float y, float z)
                {
                    z-=sin((y+0.8)*3.6)*.15;

                    float v=sqrt({{SquareName}}(x)+{{SquareName}}(z));

                    v=abs(v-.3+sin(y*1.6+.5)*0.18)-.05;
                    v={{SMaxName}}(v,-y-1.);
                    v={{SMaxName}}(v,y-0.85,.075);

                    return v;
                }

                float {{TeapotName}}(vec3 p)
                {
                    float x=p.x;
                    float y=p.y;
                    float z=p.z;

                    float v=0.0;
                    v=sqrt(x*x+z*z)-1.2-sin(y*1.5+2.0)*.4;
                    v={{SMaxName}}(v,abs(y)-1.,0.3);

                    float v1=sqrt(x*x*4.+{{SquareName}}(y+z*.1)*1.6+{{SquareName}}(z+1.2))-1.0;
                    v1={{SMaxName}}(v1,-sqrt({{SquareName}}(z+1.2)+{{SquareName}}(y+z*.12+.015)*1.8)+.8,.3);

                    v={{SMinName}}(v,{{TorusName}}(y*1.2+.2+z*.3,x*.75,z+1.25+y*.2,.8,.1),.25);
                    v={{SMinName}}(v,sqrt({{SquareName}}(x)+{{SquareName}}(y-1.1)+{{SquareName}}(z+1.8))-.05,.32);

                    float v3={{NoseName}}(x,(y+z)*sqrt(.5)-1.6,(z-y)*sqrt(.5)-1.1);

                    v={{SMinName}}(v,v3,0.2);

                    v={{SMaxName}}(v,{{SMinName}}(sin(y*1.4+2.0)*0.5+.95-sqrt(x*x+z*z),y+.8, .2));
                    v={{SMaxName}}(v,-sqrt({{SquareName}}(x)+{{SquareName}}(y+.15)+{{SquareName}}(z-1.5))+.12);

                    v={{SMinName}}(v,{{TorusName}}(x,y-0.95,z,0.9,.075));
                    v={{SMinName}}(v,{{TorusName}}(x,y+1.05,z,1.15,.05),0.15);


                    float v2={{LidName}}(x,y+.5,z);
                    v=min(v,v2);

                    return v;
                }

                float {{SDFName}}(vec3 p)
                {
                    const float scale = {{Scale}};
                    p = p - vec3{{Center}};
                    p *= 1. / scale;
                    return {{TeapotName}}(p) * scale * 0.8;
                }

                // <End Teapot Function>
                """;
        }
    }
}
