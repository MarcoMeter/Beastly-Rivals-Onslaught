using BRO.AI.Framework;
using BRO.AI.Framework.Events;
using UnityEngine;

public class BlankAI : AIEventListener
{
    public override void OnEvent(BallCaughtEvent e)
    {
        if(MyPlayer.HasBall)
        {
            // Make the AI pass the ball to the host
            PassBall(GameState.GetRemainingEnemies(MyId)[0].Id);
        }
    }

    public override void OnEvent(PlayerKilledEvent e)
    {
        if(MyId == e.Killer)
        {
            Taunt((int)Random.Range(1, 8));
        }
    }
}
