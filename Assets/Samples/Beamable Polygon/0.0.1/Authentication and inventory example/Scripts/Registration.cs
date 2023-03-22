using Beamable;
using Beamable.Common.Dependencies;
using UnityEngine;

namespace PolygonExamples.Scripts
{
    [BeamContextSystem]
    public class Registration
    {
        [RegisterBeamableDependencies()]
        public static void Register(IDependencyBuilder builder)
        {
            builder.AddSingleton<Data>(() => Object.FindObjectOfType<Data>());
        }
    }

    public static class RegistrationExtensions
    {
        public static Data GetExampleData(this BeamContext ctx) => ctx.ServiceProvider.GetService<Data>();
    }
}