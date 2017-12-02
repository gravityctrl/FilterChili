using System.IO;
using System.Reflection;

namespace GravityCTRL.FilterChili.Tests.Utils
{
    public static class ResourceHelper
    {
        private const string RESOURCES_NAMESPACE = "GravityCTRL.FilterChili.Tests.Resources";

        public static string Load(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = $"{RESOURCES_NAMESPACE}.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
