using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MyExtensions;
using Cysharp.Threading.Tasks;
using System;
using Controller.Player;
using HealthSystem;


namespace Controller.Enemy
{
    public class EnemyAttackState : EnemyBaseState
    {
        public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        private CancellationTokenSource ctsAttack;

        private bool playerInRange = true;



        public override void OnDestroy()
        {
            Extensions.ClearCts(ref ctsAttack);
        }

        public override void Enter()
        {
            ctsAttack?.Cancel();
            ctsAttack = new();
            _ = Attack(ctsAttack.Token);
            stateMachine.health.OnDie += OnDie;
            stateMachine.OnStun += OnStun;
        }

        public override void Tick(float deltaTime)
        {
            /*
            if (Vector3.Distance(PlayerStateMachine.Instance.transform.position, stateMachine.transform.position) > stateMachine.enemyScriptable.rangeAttack)
            {
                stateMachine.SwitchState(new EnemyFollowPlayerState(stateMachine));
            }
            */
        }



        public override void LateTick(float deltaTime)
        {
            RotateBodyTowardsPlayer(deltaTime);
        }


        public override void PlayerOnRange(bool inRange)
        {
            playerInRange = inRange;
        }

        public override void Exit()
        {
            Extensions.ClearCts(ref ctsAttack);
            stateMachine.health.OnDie -= OnDie;
            stateMachine.OnStun -= OnStun;
        }

        private void OnDie(Health health)
        {
            Extensions.ClearCts(ref ctsAttack);
        }

        private void OnStun(float stunTime)
        {
            _ = Stun(stunTime);
        }

        private async UniTask Stun(float time)
        {
            if (time <= 0.001f) return;
            ctsAttack?.Cancel();
            ctsAttack = new();
            try
            {
                await UniTask.WaitForSeconds(time, cancellationToken: ctsAttack.Token);
                _ = Attack(ctsAttack.Token);
            }
            catch (OperationCanceledException)
            {
                //Debug.LogError($"Cancelado patrolling!!!");
            }
        }


        private async UniTask Attack(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.WaitForSeconds(stateMachine.enemyScriptable.attackCooldown, cancellationToken: token);    //esperando en el punto de 2 a 5 segs
                    token.ThrowIfCancellationRequested();
                    Vector3 directionToPlayer = (PlayerStateMachine.Instance.transform.position - stateMachine.transform.position).normalized;
                    Vector3 spawnPosition = stateMachine.transform.position + (directionToPlayer * 0.5f);

                    stateMachine.attackController.Attack(
                            stateMachine.enemyScriptable,
                            directionToPlayer,
                            spawnPosition
                        );

                    if (!playerInRange)
                    {
                        stateMachine.SwitchState(new EnemyFollowPlayerState(stateMachine));
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //Debug.LogError($"Cancelado patrolling!!!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
                return;
            }
            finally
            {
                ctsAttack?.ClearCts();
                ctsAttack = null;
            }
        }


    }
}