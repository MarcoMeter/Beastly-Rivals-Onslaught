using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
//using System.Windows.Forms;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// Save and Load the match sequence on the match automation tool.
    /// </summary>
    public static class SaveLoad
    {

        /// <summary>
        /// Save the match sequence shown on the match automation tool using Binary Formatter.
        /// </summary>
        /// <param name="saveMatchSequence"></param>
        public static void Save(MatchSequenceData saveMatchSequence)
        {
            throw new System.NotImplementedException();
            //// Check if the directory "Saves" already exists. If not, create one.
            //if (!Directory.Exists("Saves"))
            //{
            //    Directory.CreateDirectory("Saves");
            //}
            //
            //// Save File Dialog
            //// The initial directory is set to "Saves" and filters ".binary" file
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.InitialDirectory = "Saves";
            //sfd.Filter = "Binary Files(*.binary)|*.binary";
            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            //    BinaryFormatter bf = new BinaryFormatter();
            //
            //    //Construct a SurrogateSelector object.
            //    SurrogateSelector ss = new SurrogateSelector();
            //
            //    ColorSerializationSurrogate colorSS = new ColorSerializationSurrogate();
            //    ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSS);
            //
            //    // Have the formatter use the surrogate selector.
            //    bf.SurrogateSelector = ss;
            //
            //    // Create the file named "saveSequence.binary" within the directory "Saves".
            //    FileStream file = File.Create(sfd.FileName);
            //    bf.Serialize(file, saveMatchSequence);
            //    file.Close();
            //    Debug.Log("Sequence saved at: " + sfd.FileName);
            //}
            //// If no sequence is saved.
            //else
            //{
            //    Debug.Log("No sequence is saved.");
            //}
        }

        /// <summary>
        /// Load the saved match sequence.
        /// </summary>
        /// <returns></returns>
        public static MatchSequenceData Load()
        {
            throw new System.NotImplementedException();
            //// Open File Dialog
            //// The initial directory is set to "Saves" and filters ".binary" file
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = "Saves";
            //ofd.Filter = "Binary Files(*.binary)|*.binary";
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    BinaryFormatter bf = new BinaryFormatter();
            //
            //    //Construct a SurrogateSelector object.
            //    SurrogateSelector ss = new SurrogateSelector();
            //
            //    ColorSerializationSurrogate colorSS = new ColorSerializationSurrogate();
            //    ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSS);
            //
            //    // Have the formatter use the surrogate selector.
            //    bf.SurrogateSelector = ss;
            //
            //    // Open the file named and return the match sequence.
            //    FileStream file = File.Open(ofd.FileName, FileMode.Open);
            //    MatchSequenceData loadedSequence = (MatchSequenceData)bf.Deserialize(file);
            //    file.Close();
            //    Debug.Log("Sequence loaded from: " + ofd.FileName);
            //    return loadedSequence;
            //}
            //
            //// If no file is selected, return null.
            //else
            //{
            //    Debug.Log("No sequence is selected.");
            //    return null;
            //}
        }
    }
}