# STAC - Sphere Tracing Algorithm Combiner

STACs modularizes the steps needed to create GLSL renderers which apply Sphere Tracing to SDFs. Developed as part of my masters thesis.


## Getting Started

### Dependencies

* OS: tested with Windows 10 (should be cross-platform, except STACNodes)
* IDE: Visual Studio Community 2023

### Installing

* Clone the project
* Open STAC.sln using VS Community 2023
* Maybe [restore nuget packages](https://stackoverflow.com/questions/27895504/how-do-i-enable-nuget-package-restore-in-visual-studio) when the execution below fails

### Executing program

* Use STAC as a startup project
* Use the Debug or Release build to start normal execution. Modify the code inside GenerationManager.GetMainComponentFromSource(), try out different setups offered by our PipelineProvider! Try typing PipelineProvider.MC and see what intellisense suggest you! (MC stands for MainComponent - full programs)
* Use the Benchmark Build let the execution be controlled by the BenchmarkWindow.

## Authors

Contributors names and contact info

[Maximilian Enderling](https://github.com/BMI24)

## License

This project is licensed under the [NAME HERE] License - see the LICENSE.md file for details
