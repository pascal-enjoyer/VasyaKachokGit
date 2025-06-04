using UnityEngine;

public class DrunkEnemy : EnemyBase
{
/*    private float stumbleTimer;
    private bool isStumbling;
    private const string StumbleAnimation = "DrunkFall";

    protected override string MoveAnimation => "DrunkStumble";
    protected override string AttackAnimation => "DrunkAttack";
*/
    protected override void ChasePlayer()
    {
        //if (playerTarget == null) return;

        //if (isStumbling)
        //{
        //    stumbleTimer -= Time.deltaTime;
        //    if (stumbleTimer <= 0)
        //    {
        //        isStumbling = false;
        //        PlayAnimation(MoveAnimation);
        //    }
        //    return;
        //}

        //float distance = Vector3.Distance(transform.position, playerTarget.GetTransform().position);
        //if (distance > data.attackRange)
        //{
        //    navAgent.SetDestination(playerTarget.GetTransform().position);
        //    PlayAnimation(MoveAnimation);

        //    if (Random.value < 0.01f)
        //    {
        //        isStumbling = true;
        //        stumbleTimer = 1f;
        //        PlayAnimation(StumbleAnimation);
        //        navAgent.ResetPath();
        //    }
        //}
        //else
        //{
        //    navAgent.ResetPath();
        //    PlayAnimation(IdleAnimation);
        //}
    }
}