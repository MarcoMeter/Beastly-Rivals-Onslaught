using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Text;
using System.IO;
using System.Linq;
using UnityEditor;
using BRO.AI.Loader;

namespace BRO.AI.Compiler
{
    /// <summary>
    /// The TinyCompilerWindow adds a MenuItem to compile the source files of a selected folder.
    /// </summary>
    public class TinyCompilerMenuItem : EditorWindow
    {
        [MenuItem("Assets/Compile Folder")]
        private static void CompileSourceFiles()
        {
            string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string[] sourceFiles = GetSourceFiles(filePath);
            //check if the folder contains source files
            //if not, display an error message
            if (sourceFiles.Length > 0)
            {
                AILoader aiLoader = new AILoader();
                string destinationPath = EditorUtility.SaveFilePanel("Compile Assembly to", aiLoader._pathToLibFolder, ".dll", "dll");
                CompileAssembly(destinationPath, sourceFiles);
            } else
            {
                //Displays a dialog in the unity editor - only ok field, no cancel button
                EditorUtility.DisplayDialog("Error", "It seems that there are no C# source files in the folder", "Ok");
            }

        }

        /// <summary>
        /// Validation for the MenuItem "Compile Folder" in order to check if the selected asset is a directory.
        /// </summary>
        /// <returns></returns>
        [MenuItem("Assets/Compile Folder", true)]
        private static bool CompileSourceFilesValidation()
        {
            string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);

            // check if filePath is empty or null to avoid ArgumentException
            if (filePath == null || filePath == "")
            {
                return false;
            }
            FileAttributes attr = File.GetAttributes(filePath);
            return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
        }

        /// <summary>
        /// Based on a directory's path, all CSharp source files are getting collected within all subdirectories.
        /// </summary>
        /// <param name="path">Path to the folder with the source files</param>
        /// <returns>Returns the paths of all files</returns>
        private static string[] GetSourceFiles(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories);
            return files.Select(file => file.FullName).ToArray();
        }

        /// <summary>
        /// This function compiles source files to a class library assembly.
        /// </summary>
        /// <param name="destinationPath">Path to the assembly output</param>
        /// <param name="sourceFiles">Paths to the source files</param>
        private static void CompileAssembly(string destinationPath, string[] sourceFiles)
        {
            // Configure parameters for compilation
            CompilerParameters parameters = new CompilerParameters();
            // Generate a class library instead of an executable
            parameters.GenerateExecutable = false;
            // Set the assembly file name to generate
            parameters.OutputAssembly = destinationPath;
            // Generate debug information
            parameters.IncludeDebugInformation = true;
            // Add ALL of the assembly references
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                parameters.ReferencedAssemblies.Add(assembly.Location);
            }
            // Add specific assembly references
            //parameters.ReferencedAssemblies.Add("System.dll");
            //parameters.ReferencedAssemblies.Add("UnityEngine.dll");
            // Save the assembly as a physical file
            parameters.GenerateInMemory = false;
            // set wether to treat all warnings as errors
            parameters.TreatWarningsAsErrors = false;

            // Compile source
            CSharpCodeProvider provider = new CSharpCodeProvider();
            var result = provider.CompileAssemblyFromFile(parameters, sourceFiles);

            // Log exceptions
            if (result.Errors.Count > 0)
            {
                var msg = new StringBuilder();
                foreach (CompilerError error in result.Errors)
                {
                    msg.AppendFormat("Error ({0}): {1}\n",
                        error.ErrorNumber, error.ErrorText);
                }
                throw new Exception(msg.ToString());
            }
        }
    }
}