using System.Reflection;

namespace DispatchR.Configuration
{
    public class ConfigurationOptions
    {
        public bool RegisterPipelines { get; set; } = true;
        public bool RegisterNotifications { get; set; } = true;
        public List<Assembly> Assemblies { get; } = new();
        public List<Type>? PipelineOrder { get; set; }
    }
}