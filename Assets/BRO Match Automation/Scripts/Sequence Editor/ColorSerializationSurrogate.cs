using System.Runtime.Serialization;
using UnityEngine;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// A serialization surrogate selector that allows a Color object to be serialized and deserialized.
    /// </summary>
    sealed class ColorSerializationSurrogate : ISerializationSurrogate
    {
        /// <summary>
        /// Method called to serialize a Color object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {

            Color color = (Color)obj;
            info.AddValue("r", color.r);
            info.AddValue("g", color.g);
            info.AddValue("b", color.b);
            info.AddValue("a", color.a);
            //Debug.Log(color);
        }

        // Method called to deserialize a Color object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {

            Color color = (Color)obj;
            color.r = (float)info.GetValue("r", typeof(float));
            color.g = (float)info.GetValue("g", typeof(float));
            color.b = (float)info.GetValue("b", typeof(float));
            color.a = (float)info.GetValue("a", typeof(float));
            obj = color;
            return obj;   // Formatters ignore this return value
        }
    }
}