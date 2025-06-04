using UnityEngine;

public class RangedEnemy : EnemyBase
{/*
    protected override string MoveAnimation => "RunRanged";
    protected override string AttackAnimation => "Shoot";*/

    protected override void ChasePlayer()
    {
        //if (playerTarget == null) return;

        //float distance = Vector3.Distance(transform.position, playerTarget.GetTransform().position);
        //Vector3 direction = (playerTarget.GetTransform().position - transform.position).normalized;

        //if (distance > data.preferredDistance)
        //{
        //    navAgent.SetDestination(playerTarget.GetTransform().position);
        //    PlayAnimation(MoveAnimation);
        //}
        //else if (distance < data.preferredDistance - 0.5f)
        //{
        //    navAgent.SetDestination(transform.position - direction * data.preferredDistance);
        //    PlayAnimation(MoveAnimation);
        //}
        //else
        //{
        //    navAgent.ResetPath();
        //    PlayAnimation(IdleAnimation);
        //}
    }
}