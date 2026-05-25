using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MyExtensions;
using Cysharp.Threading.Tasks;
using System;
using Controller.Player;


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
            _ = Attack();
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
        }



        private async UniTask Attack()
        {
            try
            {
                while (true)
                {
                    await UniTask.WaitForSeconds(stateMachine.enemyScriptable.attackCooldown, cancellationToken: ctsAttack.Token);    //esperando en el punto de 2 a 5 segs
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
                    //var buble = stateMachine.enemyScriptable.TakeBulletScriptable(); //.TakeBulletPrefab();
                    //BulletPool.Instance.Get(stateMachine.enemyScriptable.bulletData.bullet, direction, position);
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