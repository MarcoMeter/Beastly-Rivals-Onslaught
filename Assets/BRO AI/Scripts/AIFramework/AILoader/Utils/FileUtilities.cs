using System;
using System.IO;

namespace BRO.AI.Loader
{
    public static class FileUtilities
    {

        /// <summary>
        /// Creates a directory if it not exists yet.
        /// If it already exists, nothing happens
        /// </summary>
        /// <param name="aiDir"></param>
        public static void CheckIfDirExistsIfNotCreate(string aiDir)
        {
            System.IO.Directory.CreateDirectory(aiDir);
        }

        /// <summary>
        /// This method returns all files in a given directory that have a ".dll" extension
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static String[] GetDllPathsFromDirectory(String directory)
        {
            String[] fileArray = Directory.GetFiles(directory, "*.dll");
            return fileArray;
        }

        //TODO: add *.png?
        /// <summary>
        /// Return all pictures files in a given directory
        /// currently only *.jpg
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static String[] GetPicturesFromDirectory(String directory)
        {
            String[] fileArray = Directory.GetFiles(directory, "*jpg");
            return fileArray;
        }

    }
}