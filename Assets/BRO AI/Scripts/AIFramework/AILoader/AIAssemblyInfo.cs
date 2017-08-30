using System.Reflection;

namespace BRO.AI.Loader
{
    /// <summary>
    /// This class holds information about the AI assembly.
    /// </summary>
    internal class AIAssemblyInfo
    {
        /// <summary>
        /// Bame of the class.
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Reference to the Assembly.
        /// </summary>
        public Assembly Assembly { get; set; }
    }
}