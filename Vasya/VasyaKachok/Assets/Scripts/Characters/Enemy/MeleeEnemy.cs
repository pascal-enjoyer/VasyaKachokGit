using System;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public void DieEnemy()
    {
        Die();
    }

    protected override void ChasePlayer()
    {
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.transform.position/*.GetTransform().position*/);
        if (distance > data.attackRange)
        {
            navAgent.SetDestination(playerTarget.transform.position/*.GetTransform().position*/);
        }
        else
        {
            navAgent.ResetPath();
        }
    }
}