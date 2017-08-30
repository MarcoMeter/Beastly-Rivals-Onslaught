using BRO.AI;
using UnityEngine;
using System;

[System.Serializable]
public class BlankAIBase : AIBase
{
    public override void AddAIComponents(GameObject go)
    {
        go.AddComponent<BlankAI>();
    }

    public override AIInformation GetAIInformation()
    {
        return new AIInformation("Blank AI", "Marco Pleines", "HSRW", "Just for testing purposes.", AITag.Experimental, "1.0", "x", "none.png");
    }
}
