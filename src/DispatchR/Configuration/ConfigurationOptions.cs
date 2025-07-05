using System.Reflection;

namespace DispatchR.Configuration
{
    public class ConfigurationOptions
    {
        public bool RegisterPipelines { get; set; } = true;
        public bool RegisterNotifications { get; set; } = true;
        public List<Assembly> Assemblies { get; } = new();
        public List<Type>? PipelineOrder { get; set; }
        /// <summary>
        /// If null, this List is ignored. 
        /// If set, only the specified handlers will be included.
        /// </summary>
        public List<Type>? IncludeHandlers { get; set; }
        /// <summary>
        /// If null, this List is ignored. 
        /// If set, only the specified handlers will be NOT included.
        /// </summary>
        public List<Type>? ExcludeHandlers  { get; set; }

        public bool IsHandlerIncluded(Type handlerType)
        {
            var included = IncludeHandlers?.Contains(handlerType) ?? true;
            var excluded = ExcludeHandlers?.Contains(handlerType) ?? false;

            return included && !excluded;
        }
        
    }
}