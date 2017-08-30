using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BRO.AI.Loader
{
    public class AILoader
    {
        #region Member Fields
        private const string AI_FOLDER = "AILibs";
        public String _pathToLibFolder;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of this class
        /// Tries to set the AILibs folder path
        /// </summary>
        /// <param name="pathToLibFolder"></param>
        public AILoader()
        {
            // Get the path where the AI Dlls are stored
            this._pathToLibFolder = GetAIDirPath();           
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// This method should return the path to the AIs
        /// Name of the folder is defined in a constant at the top of the class
        /// if the directory does not exist, it will be created!
        /// </summary>
        /// <returns></returns>
        public String GetAIDirPath()
        {
            // Application dataPath should always be the right path (Unity Editor vs. Standalone)
            DirectoryInfo parentDir = Directory.GetParent(Application.dataPath);
            String aiDir = Path.Combine(parentDir.FullName, AI_FOLDER);
            // Create the AILibs folder if it doesn't exist.
            FileUtilities.CheckIfDirExistsIfNotCreate(aiDir);
            return aiDir;
        }

        /// <summary>
        /// This method should return a list of AIs that have been found
        ///     -Dll from AILibs folder
        ///     -Currently loaded assemblys by Unity Editor
        /// </summary>
        /// <returns>List of all available AI instances in the scope of this program and external assemblies.</returns>
        public List<AIInstance> GetAIInstances()
        {
            List<AIInstance> aiInstanceList = new List<AIInstance>();

            // This method will load the dlls in the AILibs folder into memory!
            LoadFolderAssemblysIntoMemory();
            // This method will look through all currently loaded assemblys for our AIs.
            // This includes the loaded dlls from the folder as well as all scripts that are currently loaded by the UnityEditor.
            List<AIAssemblyInfo> allAIS = FindAIAssemblies();

            foreach(AIAssemblyInfo info in allAIS)
            {
                AIInstance aiInstance = new AIInstance();

                //Instantiate the AI class
                AIBase aiBase = CreateAIInstance(info);
                aiInstance.Instance = aiBase;

                aiInstance.AvatarSprite = GetPictureOfAI(aiBase.GetAIInformation().GetPictureFileName());
                
                aiInstanceList.Add(aiInstance);
            }
            return aiInstanceList;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// This method will load all possible assemblies located inside a folder into memory.
        /// </summary>
        private void LoadFolderAssemblysIntoMemory()
        {
            // Get a list of all Dlls in the folder
            String[] possibleAiPaths = FileUtilities.GetDllPathsFromDirectory(_pathToLibFolder);
            // Loop through each file and try to load the assembly
            foreach (String possibleAIDllPath in possibleAiPaths)
            {
                try
                {
                    Assembly.LoadFile(possibleAIDllPath);
                }
                catch (BadImageFormatException)
                {
                    Debug.Log("Error while loading dll: " + possibleAIDllPath + " seems not to be a dll");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// This method looks for classes that inherit from AIBaseClass (AIs)
        /// It loops through every assembly and every class in it
        /// </summary>
        /// <returns></returns>
        private List<AIAssemblyInfo> FindAIAssemblies()
        {
            List<AIAssemblyInfo> aiAssemblys = new List<AIAssemblyInfo>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Loop through each class
                foreach (Type type in assembly.GetTypes())
                {
                    // We found an AI class that inherits from AIBase
                    if (type.BaseType == typeof(AIBase))
                    {
                        AIAssemblyInfo aiAssInfo = new AIAssemblyInfo();
                        // Name of the class (type) that inherits from AIBaseClass (needed to instantiate it later)
                        aiAssInfo.ClassName = type.FullName;
                        // Name of the assembly where the class was found
                        aiAssInfo.Assembly = assembly;
                        aiAssemblys.Add(aiAssInfo);
                    }
                }
            }
            return aiAssemblys;
        }

        /// <summary>
        /// This method tries to instantiate an AI class
        /// </summary>
        /// <param name="dllFilePath"></param>
        private AIBase CreateAIInstance(AIAssemblyInfo aiToLoad)
        {
            AIBase newAI = null;

            try
            {
                // This command tries to create an instance of the class that inherits from AIBase
                newAI = (AIBase) aiToLoad.Assembly.CreateInstance(aiToLoad.ClassName);
            }
            catch (Exception)
            {
                throw;
            }   
            return newAI;
        }

        private Sprite GetPictureOfAI(String pictureName)
        {
            // check if the picture exists in the ailibs folder
            String possiblePicturePath = Path.Combine(_pathToLibFolder, pictureName);
            if (!String.IsNullOrEmpty(pictureName) && File.Exists(possiblePicturePath))
            {
                byte[] fileData = File.ReadAllBytes(possiblePicturePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            // picture does not exist - there is no picture for the ai
            else
            {
                return null;
            }

        }
        #endregion
    }
}