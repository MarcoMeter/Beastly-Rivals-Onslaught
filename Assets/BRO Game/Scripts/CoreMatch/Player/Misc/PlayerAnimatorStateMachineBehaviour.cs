using UnityEngine;
using BRO.Game;

/// <summary>
/// This class is supposed to let the PlayerAliveState know when the taunt animation is done playing.
/// </summary>
public class PlayerAnimatorStateMachineBehaviour : StateMachineBehaviour
{
    #region Member Fields
    private const string m_NO_MOTION_STATE = "NoMotion";
    #endregion

    #region Member Properties
    /// <summary>
    /// Stores a reference to the PlayerAliveState, which needs to be notified upon the end of a taunt animation.
    /// </summary>
    public virtual PlayerAliveState Script { get; set; }
    #endregion

    #region Public Functions
    /// <summary>
    /// Notify the PlayerAliveState if the taunt animaiton is done playing.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInfo.IsName(m_NO_MOTION_STATE))
        {
            Script.TauntAnimationIsDone();
            base.OnStateExit(animator, stateInfo, layerIndex);
        }
    }
    #endregion
}