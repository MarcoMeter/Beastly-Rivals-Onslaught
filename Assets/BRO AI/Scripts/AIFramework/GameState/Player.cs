using BRO.Game;
using System;
using UnityEngine;

namespace BRO.AI.Framework
{
    public class Player
    {
        #region Constructor
        /// <summary>
        /// Makes an instance of a player unsing all the relevant information for the AI Framework.
        /// </summary>
        /// <param name="id">Id of the player</param>
        /// <param name="name">Name of the player</param>
        /// <param name="state">State of the player</param>
        /// <param name="lifes">Remaining lifes of the player</param>
        /// <param name="kills">Scored kills of the player</param>
        /// <param name="hasBall">Determines if the players is in possesion of the ball</param>
        /// <param name="blinkOnCooldown">Is the player's blink on cooldown?</param>
        /// <param name="remainingBlinkCooldown">What's the remaining blink cooldown?</param>
        /// <param name="position">Position of the player</param>
        /// <param name="rotation">Rotation of the player</param>
        public Player(int id, string name, int state, int lifes, int kills, bool hasBall, bool blinkOnCooldown, float remainingBlinkCooldown, Vector3 position, Quaternion rotation)
        {
            this.m_id = id;
            this.m_name = name;
            this.m_state = state;
            this.m_lifes = lifes;
            this.m_kills = kills;
            this.m_hasBall = hasBall;
            this.m_blinkOnCooldown = blinkOnCooldown;
            this.m_reaminingBlinkCoolDown = remainingBlinkCooldown;
            this.m_position = position;
            this.m_rotation = rotation;
        }
        #endregion

        #region Member Fields
        private int m_id;
        private string m_name;
        private int m_state;
        private int m_lifes;
        private int m_kills;
        private bool m_hasBall;
        private bool m_blinkOnCooldown;
        private float m_reaminingBlinkCoolDown;
        private Vector3 m_position;
        private Quaternion m_rotation;
        #endregion

        #region Member Properties
        /// <summary>
        /// Returns the player's id
        /// </summary>
        public int Id
        {
            get { return m_id; }
        }

        /// <summary>
        /// Returns the player's name
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Returns the player's state
        /// </summary>
        public PlayerState State
        {
            get { return (PlayerState)m_state; }
        }

        /// <summary>
        /// Returns the player's remaining lifes
        /// </summary>
        public int Lives
        {
            get { return m_lifes; }
        }

        /// <summary>
        /// Returns the player's scored kills
        /// </summary>
        public int Kills
        {
            get { return m_kills; }
        }

        /// <summary>
        /// Tells if the player is in possession of the ball
        /// </summary>
        public bool HasBall
        {
            get { return m_hasBall; }
        }

        /// <summary>
        /// States if the blink ability is on cooldown
        /// </summary>
        public bool BlinkOnCooldown
        {
            get { return m_blinkOnCooldown; }
        }

        /// <summary>
        /// Returns the remaining cooldown of the blink ability
        /// </summary>
        public float RemainingBlinkCooldown
        {
            get { return m_reaminingBlinkCoolDown; }
        }

        /// <summary>
        /// Returns the player's position
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
        }

        /// <summary>
        /// Returns the player's rotation
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
        /// <returns>Info about the player</returns>
        public override string ToString()
        {
            return  "Player ID: " + m_id + Environment.NewLine +
                    "Name: " + m_name + Environment.NewLine +
                    "State: " + State.ToString() + Environment.NewLine +
                    "Lifes: " + m_lifes + Environment.NewLine +
                    "Kills: " + m_kills + Environment.NewLine +
                    "Has Ball: " + m_hasBall + Environment.NewLine +
                    "Blink CD?: " + m_blinkOnCooldown + Environment.NewLine +
                    "Blink CD: " + m_reaminingBlinkCoolDown + Environment.NewLine +
                    "Position: " + m_position.ToString() + Environment.NewLine +
                    "Rotation: " + m_rotation.eulerAngles.ToString();
        }
        #endregion
    }
}