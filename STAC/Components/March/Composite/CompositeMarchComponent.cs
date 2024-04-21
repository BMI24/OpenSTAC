using STAC.Components.Color;
using STAC.Components.March.Composite.Interfaces;
using STAC.Components.March.Composite.Parts;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March.Composite
{
    public class CompositeMarchComponent : ComponentBase, IFullMarchingComponent
    {
        public MarchingOutputType OutputType { get; set; }
        public GlobalIdentifier MarchFN { get; protected init; } = "compositeMarch";
        public GlobalIdentifier ColorOutputName { get; } = "color";
        private ISDFComponent? _sdfComponent;
        public required ISDFComponent SDFComponent
        {
            get => _sdfComponent!;
            set
            {
                _sdfComponent = value;
                ColorMarchingUpdated();
                MaterialMarchingUpdated();
            }
        }
        private bool _isColorMarching;
        public bool IsColorMarching
        {
            get => _isColorMarching;
            set
            {
                _isColorMarching = value;
                ColorMarchingUpdated();
            }
        }
        public List<ICompositePart> Parts { get; init; } = new();
        public bool IsMaterialIdMarching
        {
            get => _isMaterialIdMarching;
            set
            {
                _isMaterialIdMarching = value;
                MaterialMarchingUpdated();
            }
        }

        private void MaterialMarchingUpdated()
        {
            //if (SDFComponent is IMaterialSDFComponent marchingSDF)
            //    marchingSDF.MaterialIdOutputName = IsColorMarching;
        }

        public GlobalIdentifier MaterialIdOutputName { get; } = "materialId";

        private void ColorMarchingUpdated()
        {
            if (SDFComponent is IColorSDFComponent colorSDF)
                colorSDF.IsColorSDF = IsColorMarching;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var part in Parts)
            {
                part.CompositeMarch = this;
            }
        }
        public string ReturnStatement = "";
        private bool _isMaterialIdMarching;

        public override string Generate()
        {
            if (OutputType != MarchingOutputType.Distance && OutputType != MarchingOutputType.Iterations)
                throw new NotImplementedException("OutputType is not implemented");

            if (IsColorMarching && SDFComponent is not IColorSDFComponent)
                throw new ArgumentException("If the march should supply color, then the SDF must also return color (SDFComponent is not IColorSDFComponent).");

            // Needed:
            // 1 StepLengthPart
            // 1 IterationExceededPart
            var stepLegthPart = Parts.OfType<IStepLengthPart>().Single();
            var iterationExceededPart = Parts.OfType<IIterationExceededPart>().Single();

            // May have:
            // any number of PreIterationPart
            // any number of PostDistChangePart
            var preIterationParts = Parts.OfType<IPreIterationPart>().ToList();
            var preDistChangeParts = Parts.OfType<IPreDistChangePart>().ToList();
            var sampleValidateParts = Parts.OfType<IValidateSamplePart>().ToList();
            var notifySDF = SDFComponent as IMarchBeginNotifySDF;

            ReturnStatement = OutputType switch
            {
                MarchingOutputType.Distance => "return dist;",
                MarchingOutputType.Iterations => "return i;",
                _ => throw new NotImplementedException(),
            };
            
            return $$""""
                {{(IsColorMarching ? $"vec3 {ColorOutputName};" : "")}}
                {{(IsMaterialIdMarching ? $"int {MaterialIdOutputName};" : "")}}
                float {{MarchFN}}(vec3 origin, vec3 dir, float near, float far) {
                    float dist = near;
                    float stepLength = 0;
                    
                    // <PRE ITERATION (multiple)>
                    {{string.Join(Environment.NewLine, preIterationParts.Select(p => p.GeneratePreIterationDefinitions()))}}
                    // </PRE ITERATION (multiple)>
                    {{(notifySDF != null ? notifySDF.MarchBeginName + "();" : "")}}
                
                    int i;
                    for (i = 0; i < {{GenerationManager!.MAX_MARCHING_STEPS}}; i++){
                        float signedRadius = {{SDFComponent.SDFName}}(origin + dist * dir);
                        float radius = abs(signedRadius);

                        // <VALIDATE COMPONENT (multiple)>
                        {{string.Join(Environment.NewLine, sampleValidateParts.Select(p => p.GenerateValidateSample()))}}
                        // </VALIDATE COMPONENT (multiple)>
                        
                
                        {{(IsColorMarching ? $"{ColorOutputName} = {((IColorSDFComponent)SDFComponent).ColorOutputName};" : "")}}
                        {{(IsMaterialIdMarching ? $"{MaterialIdOutputName} = {((IMaterialSDFComponent)SDFComponent).MaterialIdOutputName};" : "")}}

                        if (dist >= far){
                            dist = far;
                            {{(notifySDF != null ? notifySDF.MarchEndName + "();" : "")}}
                            {{ReturnStatement}}
                        }
                        if (radius < {{GenerationManager.EPSILON_NAME}}){
                            {{(notifySDF != null ? notifySDF.MarchEndName + "();" : "")}}
                            {{ReturnStatement}}
                        }

                        // <STEP LENGTH COMPONENT (1)>
                        {{stepLegthPart}}
                        // </STEP LENGTH COMPONENT (1)>

                        
                        // <POST DIST CHANGE (multiple)>
                        {{string.Join(Environment.NewLine, preDistChangeParts.Select(p => p.GeneratePreDistChange()))}}
                        // </POST DIST CHANGE (multiple)>
                        
                        dist += stepLength;
                    }

                    {{(notifySDF != null ? notifySDF.MarchEndName + "();" : "")}}
                    
                    // <ITERATION EXCEEDED (1)>
                    {{iterationExceededPart}}
                    // </ITERATION EXCEEDED (1)>
                
                }
                """";
        }
    }
}
