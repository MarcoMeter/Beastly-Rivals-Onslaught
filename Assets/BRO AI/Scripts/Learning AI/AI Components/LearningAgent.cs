using BRO.AI.Framework;
using BRO.AI.Framework.Events;
using BRO.Game;
using ConvnetSharp;
using DeepQLearning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BRO.AI.Learning
{
    /// <summary>
    /// This component is in charge of training and executing the learning AI.
    /// </summary>
    public class LearningAgent : AISolutionScopeBehaviour
    {
        #region Member Fields
        private const float m_AI_ENGINE_UPDATE_TIME_STEP = 0.025f; // time intervals concerning the invocation of the function AIEngineUpdate
        
        // Brain
        private DeepQLearn m_brain;
        [SerializeField]
        private bool m_isLearning = true;
        private const int m_NUM_INPUTS = 12;
        private const int m_MOVEMENT_DISTANCE = 10;
        private bool m_initialForwardPass = true;
        private DeepQCanvas m_brainCanvas;

        // Reward signals
        private float m_gatheredReward = 0;
        private const float m_CONTINUOUS_ALIVE_REWARD = 0.2f;
        private const float m_CHARGE_POWER_SHOT_REWARD = m_CONTINUOUS_ALIVE_REWARD * 3f;
        private const float m_FINAL_ALIVE_REWARD = 5;
        private const float m_DEAD_REWARD = -20;
        private const float m_KILLED_PLAYER_REWARD = 5;
        #endregion

        #region Member Properties
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Initialize brain
            if (m_brain == null)
            {
                // Define input and output size
                int numInputs = m_NUM_INPUTS; // all inputs, which were individually selected and prepared
                int numActions = 4; // the agent can execute 3 actions (do nothing, dodge beast, pass ball easiest target)
                int temporalWindow = 1; // the temporal window adds n states of the past states to the inputs, 0 means that the agent does not use information of the past
                int totalInputCount = numInputs * temporalWindow + numActions * temporalWindow + numInputs;

                // Define layers of the neural network
                List<LayerDefinition> layerDefinitions = new List<LayerDefinition>();
                layerDefinitions.Add(new LayerDefinition { type = "input", out_sx = 1, out_sy = 1, out_depth = totalInputCount }); // Input layer
                layerDefinitions.Add(new LayerDefinition { type = "fc", num_neurons = 36, activation = "relu" }); // First hidden layer using the activation function rectified linear units (fc = fully connected)
                layerDefinitions.Add(new LayerDefinition { type = "fc", num_neurons = 24, activation = "relu" });
                layerDefinitions.Add(new LayerDefinition { type = "regression", num_neurons = numActions }); // Output layer for regression

                // Define training options
                // Neural net training parameters
                Options opt = new Options();
                opt.method = "sgd";
                opt.learningRate = 0.001;
                opt.momentum = 0.05;
                opt.batchSize = 64;
                opt.l1_decay = 0.001;
                opt.l2_decay = 0.001;

                // Q-Learning training parameters
                TrainingOptions trainingOptions = new TrainingOptions();
                trainingOptions.temporalWindow = temporalWindow;
                trainingOptions.experienceSize = 10000;
                trainingOptions.startLearnThreshold = 500;
                trainingOptions.gamma = 0.9;
                trainingOptions.learningStepsTotal = 50000;
                trainingOptions.learningStepsBurnin = 500;
                trainingOptions.epsilonMin = 0.05;
                trainingOptions.epsilonTestTime = 0;
                trainingOptions.layerDefinitions = layerDefinitions;
                trainingOptions.options = opt;
                trainingOptions.randomActionDistribution = new List<double>();
                trainingOptions.randomActionDistribution.AddRange(new double[] { 0.15, 0.4, 0.4, 0.05});

                m_brain = new DeepQLearn(numInputs, numActions, trainingOptions);

                m_brainCanvas = Instantiate(Resources.Load("CanvasDeepQ") as GameObject).GetComponent<DeepQCanvas>();
            }
        }

        private void Update()
        {
            if (m_brain != null)
            {
                m_brain.IsLearning = m_isLearning;
            }
        }
        #endregion

        #region AI Engine
        private void AIEngineUpdate()
        {
            if (m_isLearning)
            {
                // Don't learn if player is dead
                if (MyPlayer.State == PlayerState.DeadState || MyPlayer.State == PlayerState.ReviveState)
                {
                    m_brain.IsLearning = false;
                }
                else
                {
                    m_brain.IsLearning = true;
                }
            }
            else
            {
                m_brain.IsLearning = false;
            }

            // The Backward processing has to be done after executing an action, so it is called before the next action is being set.
            // Initially there was no action taken, so check for that.
            if (!m_initialForwardPass)
            {
                StartCoroutine(Backward());
            }
            // Make decision based on input information
            Volume inputVolume = new Volume(m_NUM_INPUTS, 1, 1);
            inputVolume.w = GetSimplifiedInputData(true);
            var chosenAction = (OutputAction) m_brain.Forward(inputVolume);
            ExecuteDecision(chosenAction);
            m_brainCanvas.SetValue(chosenAction);
            m_initialForwardPass = false;
        }
        #endregion

        #region AI Events
        /// <summary>
        /// Starts the decision making logics.
        /// </summary>
        /// <param name="e">Event data</param>
        public override void OnEvent(MatchStartEvent e)
        {
            InvokeRepeating("AIEngineUpdate", 0, m_AI_ENGINE_UPDATE_TIME_STEP);
            m_ballSequenceStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the decision making logics and finally triggers the training process of the gathered training examples.
        /// </summary>
        /// <param name="e">Event data</param>
        public override void OnEvent(MatchDoneEvent e)
        {
            CancelInvoke(); // Stops invoking the AI Engine Update function
        }

        public override void OnEvent(BallPassedEvent e)
        {
            if (e.InitalBall)
            {
                m_ballSequenceStartTime = DateTime.Now; // Track time for the agent's input usage
            }
        }

        /// <summary>
        /// Evaluates the gathered data in order to come up with training examples.
        /// </summary>
        /// <param name="e">Event data</param>
        public override void OnEvent(PlayerKilledEvent e)
        {
            // Gather rewards
            if(MyPlayer.Id == e.Victim)
            {
                m_gatheredReward += m_DEAD_REWARD;
            }
            else
            {
                m_gatheredReward += m_FINAL_ALIVE_REWARD;
            }

            if(MyPlayer.Id == e.Killer)
            {
                m_gatheredReward += m_KILLED_PLAYER_REWARD;
            }
        }
        #endregion

        #region Public Functions
        #endregion

        #region Local Functions
        /// <summary>
        /// Triggers the training process.
        /// </summary>
        /// <returns></returns>
        IEnumerator Backward()
        {
            if (MyPlayer.State == PlayerState.AliveState)
            {

                if (!GameState.IsPowerShot)
                {
                    m_gatheredReward += m_CONTINUOUS_ALIVE_REWARD;
                }
                else if(GameState.IsPowerShot && MyPlayer.HasBall)
                {
                    m_gatheredReward += m_CHARGE_POWER_SHOT_REWARD;
                }
            }
            m_brain.Backward(m_gatheredReward); // Triggers training process, which is affected by the gathered reward.
            m_gatheredReward = 0; // The reward is stored as member, so it has to be set to 0 again after forwarding the reward to the brain
            m_brainCanvas.SetValues(m_brain.Age, m_brain.ExperienceReplaySize, m_brain.ExplorationEpsilon, m_brain.AverageQLoss, m_brain.SmoothishReward);
            yield return null;
        }

        /// <summary>
        /// Executes the action, which was decided on.
        /// </summary>
        /// <param name="action">The action to carry out</param>
        private void ExecuteDecision(OutputAction action)
        {
            switch (action)
            {
                case OutputAction.DoNothing:
                    DoNothing();
                    break;
                case OutputAction.DodgeBeast:
                    MoveToDodgeBeast();
                    break;
                case OutputAction.GainDistance:
                    GainDistanceToBeast();
                    break;
                case OutputAction.PassBallRandomly:
                    PassBallRandomly();
                    break;
            }
        }
        #endregion
    }
}