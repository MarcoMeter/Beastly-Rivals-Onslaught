using UnityEngine;

/// <summary>
/// Override the child's (animated model) transform, because it will receive an offset overtime due to animations. This would result in ougly movements made by the NavMeshAgent.
/// </summary>
public class OverrideChildTransform : MonoBehaviour 
{
    #region Unity Lifecycle
    void Update () 
    {
        transform.GetChild(0).position = transform.position;
        transform.GetChild(0).rotation = transform.rotation;
	}
    #endregion
}
