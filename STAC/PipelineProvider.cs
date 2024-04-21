using LearnOpenTK.Common;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using STAC.BoundingShapeGeneration;
using STAC.Components;
using STAC.Components.CameraRay;
using STAC.Components.Color;
using STAC.Components.March;
using STAC.Components.March.Composite;
using STAC.Components.March.Composite.Interfaces;
using STAC.Components.March.Composite.Parts;
using STAC.Components.Normals;
using STAC.Components.SDF;
using STAC.Components.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static STAC.Components.Color.IterationCountColorComponent;
using static STAC.PipelineProvider;

namespace STAC
{
    internal static class PipelineProvider
    {
        public interface IPipelineSetup
        {
            public MainComponent Generate();
            public ICameraRayComponent? Camera { get;set; }
        }
        public record struct PipelineSetup<MarchT>(
            ICameraRayComponent? Camera,
            Func<ISDFComponent, MarchT>? MarchCtor,
            ISDFComponent? SDF,
            Func<ISDFComponent, INormalComponent>? NormalCtor,
            Func<MarchT, INormalComponent, ISDFComponent, IColorComponent>? ColorCtor,
            List<IComponent>? LooseComponents = null
            )
             : IPipelineSetup
            where MarchT : IMarchingComponent
        {
            public MainComponent Generate()
            {
                ArgumentNullException.ThrowIfNull(SDF);
                ArgumentNullException.ThrowIfNull(NormalCtor);
                ArgumentNullException.ThrowIfNull(Camera);
                ArgumentNullException.ThrowIfNull(MarchCtor);
                ArgumentNullException.ThrowIfNull(ColorCtor);

                INormalComponent normal = NormalCtor(SDF);
                MarchT march = MarchCtor(SDF);
                IColorComponent color = ColorCtor(march, normal, SDF);

                return new()
                {
                    SDFComponent = SDF,
                    CameraRayComponent = Camera,
                    ColorComponent = color,
                    MarchingComponent = march,
                    NormalComponent = normal,
                    LooseComponents = this.LooseComponents ?? new List<IComponent>()
                };
            }

            public static implicit operator MainComponent(PipelineSetup<MarchT> setup)
            {
                return setup.Generate();
            }
        }

        public readonly static StaticCameraRayComponent CameraStatic = new()
        {
            CameraPosition = new(0, 5, 0),
            CameraCenter = new(0, 0, 0),
            CameraUp = new(1, 0, 0),
        };
        public readonly static OpenTKMoveRayComponent CameraDynamic = new()
        {
            CameraPosition = new(7.167197f, -0.33089286f, 3.5869665f),
            Yaw = 43.99419f,
        };
        public readonly static AtomSDFComponent SDFAtom = new()
        {
            AtomCount = 10,
            UsedAtoms = 10,
            RProbe = 1.6f
        };
        public readonly static ISDFComponent SDFTwoSpheres = new SceneSDFComponent()
        {
            SDFComponents = new() {
                new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f },
                new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f },
            }
        };

        public readonly static ISDFComponent SDFTwoSpheresEarlyExit = new SceneSDFComponent()
        {
            SDFComponents = new() {
                new ConvexExitSDF { WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f } },
                new ConvexExitSDF { WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f } },
            }
        };


        public static readonly List<ISDFComponent> SDFDemoComponents = new() {
                new BoxSDFComponent() { Center = new Vector3(-4, 0, 0), Scale = new Vector3(2,2,2) },
                new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f },
                new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f },
                new PyramidSDFComponent() { Center = new Vector3(8, 0, 0), Height = 2f },
                BoundingGenerator.CreateSphereWrapper(new TorusSDFComponent() { Center = new Vector3(12, 0, 0), Radius = 2f, Thickness = 1f }),
                new VerticalCapsuleSDFComponent() {Center = new Vector3(16, 0, 0), Height = 2f, Radius = 1f },
                BoundingGenerator.CreateSphereWrapper(new TeapotSDFComponent() {Center = new Vector3(20, 0, 0), Scale = 1f}),
                new GridSDFComponent() { Center = new Vector3(24, 0, 0) },
                new HorizontalPlaneSDFComponent() { Height = -3 },
        };

        public static readonly List<ISDFComponent> SDFDemoTeapotWrappedComponents = new() {
                new BoxSDFComponent() { Center = new Vector3(-4, 0, 0), Scale = new Vector3(2,2,2) },
                new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f },
                new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f },
                new PyramidSDFComponent() { Center = new Vector3(8, 0, 0), Height = 2f },
                BoundingGenerator.CreateSphereWrapper(new TorusSDFComponent() { Center = new Vector3(12, 0, 0), Radius = 2f, Thickness = 1f }),
                new VerticalCapsuleSDFComponent() {Center = new Vector3(16, 0, 0), Height = 2f, Radius = 1f },
                new ConvexExitSDF { WrappedSDFComponent = BoundingGenerator.CreateSphereWrapper(new TeapotSDFComponent() {Center = new Vector3(20, 0, 0), Scale = 1f}) },
                new GridSDFComponent() { Center = new Vector3(24, 0, 0) },
                new HorizontalPlaneSDFComponent() { Height = -3 },
        };

        public static readonly List<ISDFComponent> SDFDemoComplexWrappedComponents = new() {
                new ConvexExitSDF { WrappedSDFComponent = new BoxSDFComponent() { Center = new Vector3(-4, 0, 0), Scale = new Vector3(2,2,2) } },
                new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f },
                new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f },
                new ConvexExitSDF { WrappedSDFComponent = new PyramidSDFComponent() { Center = new Vector3(8, 0, 0), Height = 2f } },
                BoundingGenerator.CreateSphereWrapper(new TorusSDFComponent() { Center = new Vector3(12, 0, 0), Radius = 2f, Thickness = 1f }),
                new ConvexExitSDF { WrappedSDFComponent = new VerticalCapsuleSDFComponent() {Center = new Vector3(16, 0, 0), Height = 2f, Radius = 1f }},
                new ConvexExitSDF { WrappedSDFComponent = BoundingGenerator.CreateSphereWrapper(new TeapotSDFComponent() {Center = new Vector3(20, 0, 0), Scale = 1f}) },
                new GridSDFComponent() { Center = new Vector3(24, 0, 0) },
                new HorizontalPlaneSDFComponent() { Height = -3 },
        };

        public static readonly List<ISDFComponent> SDFDemoAllWrappedComponents = new() {
                new ConvexExitSDF { WrappedSDFComponent = new BoxSDFComponent() { Center = new Vector3(-4, 0, 0), Scale = new Vector3(2,2,2) } },
                new ConvexExitSDF { WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f } },
                new ConvexExitSDF { WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f } },
                new ConvexExitSDF { WrappedSDFComponent = new PyramidSDFComponent() { Center = new Vector3(8, 0, 0), Height = 2f } },
                new ConvexExitSDF { WrappedSDFComponent = BoundingGenerator.CreateSphereWrapper(new TorusSDFComponent() { Center = new Vector3(12, 0, 0), Radius = 2f, Thickness = 1f }) },
                new ConvexExitSDF { WrappedSDFComponent = new VerticalCapsuleSDFComponent() {Center = new Vector3(16, 0, 0), Height = 2f, Radius = 1f }},
                new ConvexExitSDF { WrappedSDFComponent = BoundingGenerator.CreateSphereWrapper(new TeapotSDFComponent() {Center = new Vector3(20, 0, 0), Scale = 1f}) },
                new GridSDFComponent() { Center = new Vector3(24, 0, 0) },
                new ConvexExitSDF { WrappedSDFComponent = new HorizontalPlaneSDFComponent() { Height = -3 } },
        };

        public static ISDFComponent ToSceneSDF(List<ISDFComponent> sdfs) => new SceneSDFComponent() { SDFComponents = sdfs };

        public readonly static ISDFComponent SDFDemoScene = new SceneSDFComponent()
        {
            SDFComponents = SDFDemoComponents
        };

        public readonly static ISDFComponent MaterialSDFDemoScene = new MaterialSceneSDFComponent()
        {
            SDFComponents = SDFDemoComponents.Select((sdf, i) => (sdf, i)).ToList()
        };

        public readonly static ISDFComponent SDFTwoSpheresWrapped = new SceneSDFComponent()
        {
            SDFComponents = new() {
                new ConvexExitSDF{ WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(0, 0, 0), Radius = 2f } },
                new ConvexExitSDF{ WrappedSDFComponent = new SphereSDFComponent() { Center = new Vector3(4, 0, 0), Radius = 2f } },
            }
        };
        public static NaiveMarchComponent MarchNaive(ISDFComponent sdf) => new() { SDFComponent = sdf };
        public static EnhancedKeinertMarchComponent MarchKeinert(ISDFComponent sdf) => new() { SDFComponent = sdf };
        public static CompositeMarchComponent MarchBalint(ISDFComponent sdf) => MarchComposite<StepLengthBalintPart, IterationExceededKeinertPart>(sdf);
        public static CompositeMarchComponent MarchComposite<T, U>(ISDFComponent sdf)
            where T : IStepLengthPart, new()
            where U : IIterationExceededPart, new()
        {
            return MarchComposite(new T(), new U())(sdf);
        }
        public static Func<ISDFComponent, CompositeMarchComponent> MarchComposite<T, U>(T stepLengthPart, U iterationExceededPart)
            where T : IStepLengthPart
            where U : IIterationExceededPart
        {
            return (sdf) =>
            {
                var val = new CompositeMarchComponent() { SDFComponent = sdf };
                val.Parts.Add(stepLengthPart);
                val.Parts.Add(iterationExceededPart);
                return val;
            };
        }

        public static Func<ISDFComponent, MarchErrorComponent> MarchError(Func<ISDFComponent, IFlexibleMarchingComponent> march1, Func<ISDFComponent, IFlexibleMarchingComponent> march2)
        {
            return (sdf) => new MarchErrorComponent { MarchComponent1 = march1(sdf), MarchComponent2 = march2(sdf) };
        }

        public static NaiveNormalComponent NormalNaive(ISDFComponent sdf) => new() { SDFComponent = sdf };

        public static HitMissColorComponent ColorFromMarch<MarchT>(MarchT march, INormalComponent normal, ISDFComponent sdf)
            where MarchT : IMarchingComponent, IColorMarchingComponent
        {
            return new HitMissColorComponent
            {
                MissColorComponent = new ConstantColorComponent { Color = new(0, 0, 0, 0) },
                HitColorComponent = new MarchProvidedColorComponent { ColorMarchingComponent = march }
            };
        }
        public static Func<MarchT, INormalComponent, ISDFComponent, IterationCountColorComponent> ColorIteration<MarchT>(ColorMap colorMap = ColorMap.Gray)
            where MarchT : IMarchingComponent, IFlexibleMarchingComponent
         => (MarchT march, INormalComponent normal, ISDFComponent sdf) => new IterationCountColorComponent
         {
             MarchingComponent = march,
             UsedColorMap = colorMap
         };

        public static Func<MarchT, INormalComponent, ISDFComponent, HitMissColorComponent> ColorPhongShadedMaterials<MarchT>(Vector3 lightPosition)
            where MarchT : IMaterialIdMarchingComponent
        {
            return (MarchT march, INormalComponent normal, ISDFComponent sdf) =>
            new HitMissColorComponent
            {
                MissColorComponent = new ConstantColorComponent { Color = new(0, 0, 0, 0) },
                HitColorComponent = new AmbientOcclusionColorComponent
                {
                    NormalComponent = normal,
                    SDFComponent = sdf,
                    ColorComponent = new PhongComponent
                    {
                        NormalComponent = normal,
                        Shininess = 10f,
                        AmbientLight = new(1f, 1f, 1f),
                        AmbientColorComponent = new ConstantColorComponent { Color = new(0f, 0f, 0f, 0f) },
                        DiffuseColorComponent = new MaterialIDColorComponent
                        {
                            MaterialMarchingComponent = march,
                            MaterialIDToComponent = new()
                            {
                                (4, new TriplanarMappingColorComponent() {NormalComponent = normal}),
                                (6, new UVColorComponent())
                            }
                        },
                        SpecularColorComponent = new ConstantColorComponent { Color = new(0f, 0f, 0f, 0f) },
                        LightVisibilityMarchingComponent = march,
                        LightVisibilityFactorUsed = PhongComponent.LightVisibilityFactor.Continous,
                        SDFComponent = sdf,
                        Lights = new()
                        {
                            new() { Position = lightPosition, Intensity = new(1f, 1f, 1f) },
                        }
                    }
                }
            };
        }
        public static HitMissColorComponent ColorPhongShadedMaterials<MarchT>(MarchT march, INormalComponent normal, ISDFComponent sdf)
            where MarchT : IMaterialIdMarchingComponent
        {
            return ColorPhongShadedMaterials<MarchT>(new Vector3(10, 0, 0))(march, normal, sdf);
        }

        public static HitMissColorComponent ColorPhongShaded<MarchT>(MarchT march, INormalComponent normal, ISDFComponent sdf)
            where MarchT : IMarchingComponent
        {
            return ColorPhongShaded<MarchT>(new Vector3(10, 0, 0))(march, normal, sdf);
        }

        public static HitMissColorComponent ColorBinaryShadow<MarchT>(MarchT march, INormalComponent normal, ISDFComponent sdf)
            where MarchT : IMarchingComponent, IColorMarchingComponent
        {
            return new HitMissColorComponent
            {
                MissColorComponent = new ConstantColorComponent { Color = new(0, 0, 0, 0) },
                HitColorComponent = new BinaryShadowComponent
                {
                    NormalComponent = normal,
                    ColorComponent = new ConstantColorComponent { Color = new(1f, 0f, 0f, 0f) },
                    LightVisibilityMarchingComponent = march,
                    Lights = new()
                    {
                        new() { Position = new(10.0f, 0, 0), Intensity = new(1f, 1f, 1f) },
                    }
                }
            };
        }

        public static HitMissColorComponent ColorFlat<MarchT>(MarchT march, INormalComponent normal, ISDFComponent sdf)
           where MarchT : IMarchingComponent
        {
            return new HitMissColorComponent
            {
                MissColorComponent = new ConstantColorComponent { Color = new(0, 0, 0, 0) },
                HitColorComponent = new ConstantColorComponent { Color = new(1f, 0f, 0f, 0f) }
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCShadedAtoms()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFAtom,
                Camera = CameraDynamic,
                ColorCtor = ColorFromMarch,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }
        
        public static PipelineSetup<IFlexibleMarchingComponent> MCDiffAtomsNaiveKeinert()
        {
            return new PipelineSetup<IFlexibleMarchingComponent>
            {
                SDF = SDFAtom,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Seismic },
                MarchCtor = MarchError(MarchNaive, MarchKeinert),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFlexibleMarchingComponent> MCDiffAtomsNaiveBalint()
        {
            return new PipelineSetup<IFlexibleMarchingComponent>
            {
                SDF = SDFAtom,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Seismic },
                MarchCtor = MarchError(MarchNaive, MarchBalint),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCIterationsSpheresKeinert()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Gray },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart()),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCSpheresKeinertFlat()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = ColorFlat,
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchKeinert,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCIterationsSpheresNaive()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Gray },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCIterationsSpheresBalint()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Gray },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchComposite(new StepLengthBalintPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart()),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFlexibleMarchingComponent> MCDiffSpheresKeinertBalint()
        {
            return new PipelineSetup<IFlexibleMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Seismic },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchError(
                        MarchComposite(new StepLengthBalintPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart()),
                        MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart())
                    ),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFlexibleMarchingComponent> MCDiffSpheresNaiveKeinert()
        {
            return new PipelineSetup<IFlexibleMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Seismic },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchError(
                    MarchNaive,
                    MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart())
                    ),
                NormalCtor = NormalNaive
            };
        }

        public static MainComponent MCDiffSpheresWrapped()
        {
            var twoSpheres = SDFTwoSpheres;
            var twoSpheresWrapped = SDFTwoSpheresWrapped;
            var marcher = new MarchErrorComponent
            {
                MarchComponent1 = MarchNaive(twoSpheres),
                MarchComponent2 = MarchNaive(twoSpheresWrapped)
            };

            return new MainComponent
            {
                CameraRayComponent = CameraDynamic,
                ColorComponent = new IterationCountColorComponent { MarchingComponent = marcher, UsedColorMap = ColorMap.Seismic },
                MarchingComponent = marcher,
                NormalComponent = NormalNaive(twoSpheres),
                SDFComponent = twoSpheres,
                LooseComponents = new() { twoSpheresWrapped }
            };
        }

        public static PipelineSetup<IFlexibleMarchingComponent> MCDiffSpheresKeinertRelaxationFallback()
        {
            return new PipelineSetup<IFlexibleMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = (m, v, s) => new IterationCountColorComponent { MarchingComponent = m, UsedColorMap = ColorMap.Seismic },
                // when using seismic color map
                // Red = first Marcher is better; blue = second Marcher is better
                // better = returns after less iterations
                MarchCtor = MarchError(
                    MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.StopRelexation }, new IterationExceededKeinertPart()),
                    MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation }, new IterationExceededKeinertPart())
                    ),
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCIterationAtoms()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFAtom,
                Camera = CameraDynamic,
                ColorCtor = ColorIteration<IFlexibleMarchingComponent>(ColorMap.Gray),
                MarchCtor = MarchComposite<StepLengthBalintPart, IterationExceededKeinertPart>,
                NormalCtor = NormalNaive
            };
        }



        public static PipelineSetup<IFullMarchingComponent> MCBinaryShadowsAtoms()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = ColorBinaryShadow,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCGridPhong()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = new GridSDFComponent(),
                Camera = CameraDynamic,
                ColorCtor = ColorPhongShaded,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }
        public static PipelineSetup<IFullMarchingComponent> MCGridIterations()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = new GridSDFComponent(),
                Camera = CameraDynamic,
                ColorCtor = ColorIteration<IFlexibleMarchingComponent>(ColorMap.Gray),
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }
        public static PipelineSetup<IFullMarchingComponent> MCSpheresShaded()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = ColorPhongShaded,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }
        public static PipelineSetup<IFullMarchingComponent> MCTest()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = ColorFlat,
                MarchCtor = MarchComposite(new StepLengthKeinertPart(), new IterationExceededNaivePart()),
                NormalCtor = NormalNaive
            };
        }

        public static Func<PipelineSetup<IFullMarchingComponent>> MCFlat(ISDFComponent sdf)
        {
            return () => new PipelineSetup<IFullMarchingComponent>
            {
                SDF = sdf,
                Camera = CameraDynamic,
                ColorCtor = ColorFlat,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static Func<PipelineSetup<IFullMarchingComponent>> MCPhong(ISDFComponent sdf, Vector3? lightPos = null)
        {
            return () => new PipelineSetup<IFullMarchingComponent>
            {
                SDF = sdf,
                Camera = CameraDynamic,
                ColorCtor = ColorPhongShaded<IColorMarchingComponent>(lightPos ?? new Vector3(10, 0, 0)),
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCSpheresFlat()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFTwoSpheres,
                Camera = CameraDynamic,
                ColorCtor = ColorFlat,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCDemoShaded()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFDemoScene,
                Camera = CameraDynamic,
                ColorCtor = ColorPhongShaded<IColorMarchingComponent>(new Vector3(10, 10, 0)),
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCDemoFlat()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = SDFDemoScene,
                Camera = CameraDynamic,
                ColorCtor = ColorFlat<IMarchingComponent>,
                MarchCtor = MarchNaive,
                NormalCtor = NormalNaive
            };
        }

        public static PipelineSetup<IFullMarchingComponent> MCDemoMaterialsShaded()
        {
            return new PipelineSetup<IFullMarchingComponent>
            {
                SDF = MaterialSDFDemoScene,
                Camera = CameraDynamic,
                ColorCtor = ColorPhongShadedMaterials<IMaterialIdMarchingComponent>(new Vector3(10, 10, 0)),
                MarchCtor = MarchComposite<StepLengthNaivePart, IterationExceededNaivePart>,
                NormalCtor = NormalNaive
            };
        }
    }
}
