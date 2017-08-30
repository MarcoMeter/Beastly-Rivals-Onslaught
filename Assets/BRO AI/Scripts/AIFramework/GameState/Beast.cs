using System;
using UnityEngine;
using BRO.Game;

namespace BRO.AI.Framework
{ 
    public class Beast
    {
        #region Constructor
        /// <summary>
        /// Makes an instance of the beast unsing all the relevant information for the AI Framework.
        /// </summary>
        /// <param name="speed">Speed of the beast</param>
        /// <param name="rotationSpeed">Rotation speed of the beast</param>
        /// <param name="position">Position of the beast</param>
        /// <param name="rotation">Rotation of the beast</param>
        public Beast(float speed, float rotationSpeed, Vector3 position, Quaternion rotation)
        {
            this.m_speed = speed;
            this.m_rotationSpeed = rotationSpeed;
            this.m_positon = position;
            this.m_rotation = rotation;
        }
        #endregion

        #region Member Fields
        private float m_speed;
        private float m_rotationSpeed;
        private Vector3 m_positon;
        private Quaternion m_rotation;
        #endregion

        #region Member Properties
        /// <summary>
        /// Returns the beast's speed
        /// </summary>
        public float Speed
        {
            get { return m_speed; }
        }

        /// <summary>
        /// Returns the beast's rotation speed
        /// </summary>
        public float RotationSpeed
        {
            get { return m_rotationSpeed; }
        }

        /// <summary>
        /// Returns the beast's position
        /// </summary>
        public Vector3 Position
        {
            get { return m_positon; }
        }

        /// <summary>
        /// Returns the beast's rotation
        /// </summary>
        public Quaternion Rotation
        {
            get { return m_rotation; }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Info about the beast</returns>
        public override string ToString()
        {
            return  "Speed: " + m_speed + Environment.NewLine +
                    "Rotation Speed: " + m_rotationSpeed + Environment.NewLine +
                    "Postion: " + m_positon + Environment.NewLine +
                    "Rotation: " + m_rotation.eulerAngles.ToString();
        }
        #endregion
    }
}