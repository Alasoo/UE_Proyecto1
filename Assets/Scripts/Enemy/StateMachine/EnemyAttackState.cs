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



        public override void OnDestroy()
        {
            ctsAttack?.ClearCts();
            ctsAttack = null;
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
            if (!inRange)
                stateMachine.SwitchState(new EnemyFollowPlayerState(stateMachine));
        }

        public override void Exit()
        {
            ctsAttack?.ClearCts();
            ctsAttack = null;
        }



        private async UniTask Attack()
        {
            try
            {
                while (true)
                {
                    Vector3 directionToPlayer = (PlayerStateMachine.Instance.transform.position - stateMachine.transform.position).normalized;
                    Vector3 spawnPosition = stateMachine.transform.position + (directionToPlayer * 0.5f);

                    stateMachine.attackController.Attack(
                            stateMachine.enemyScriptable,
                            directionToPlayer,
                            spawnPosition
                        );

                    //var buble = stateMachine.enemyScriptable.TakeBulletScriptable(); //.TakeBulletPrefab();
                    //BulletPool.Instance.Get(stateMachine.enemyScriptable.bulletData.bullet, direction, position);
                    await UniTask.WaitForSeconds(stateMachine.enemyScriptable.attackCooldown, cancellationToken: ctsAttack.Token);    //esperando en el punto de 2 a 5 segs
                    await UniTask.Yield(ctsAttack.Token);
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