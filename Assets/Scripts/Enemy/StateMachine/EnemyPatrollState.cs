using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MyExtensions;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using Controller.Player;


namespace Controller.Enemy
{
    public class EnemyPatrollState : EnemyBaseState
    {
        public EnemyPatrollState(EnemyStateMachine stateMachine) : base(stateMachine) { }


        private CancellationTokenSource ctsPatroll;





        public override void OnDestroy()
        {
            ctsPatroll?.ClearCts();
            ctsPatroll = null;
        }

        public override void Enter()
        {
            ctsPatroll?.Cancel();
            ctsPatroll = new();
            _ = SetNewRandomDestination();
        }

        public override void LateTick(float deltaTime)
        {
            RotateBodyTowardsMovement(deltaTime);
        }

        public override void Exit()
        {
            ctsPatroll?.ClearCts();
            ctsPatroll = null;
            stateMachine.agent.ResetPath();
        }


        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject != PlayerStateMachine.Instance.gameObject) return;
            stateMachine.SwitchState(new EnemyFollowPlayerState(stateMachine));
        }


        private async UniTask SetNewRandomDestination()
        {
            try
            {
                while (true)
                {
                    await UniTask.WaitForSeconds(UnityEngine.Random.Range(2f, 5f), cancellationToken: ctsPatroll.Token);    //esperando en el punto de 2 a 5 segs

                    Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * stateMachine.enemyScriptable.patrolRadius;
                    Vector3 randomDirection = new Vector3(randomPoint.x, randomPoint.y, 0) + stateMachine.transform.position;
                    NavMeshHit hit;

                    if (NavMesh.SamplePosition(randomDirection, out hit, stateMachine.enemyScriptable.patrolRadius, NavMesh.AllAreas))
                    {
                        stateMachine.agent.SetDestination(hit.position);
                    }
                    await UniTask.WaitUntil(() => !stateMachine.agent.pathPending && stateMachine.agent.remainingDistance <= stateMachine.agent.stoppingDistance, cancellationToken: ctsPatroll.Token); //esperando a llegar al destino
                    await UniTask.Yield(ctsPatroll.Token);
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
                ctsPatroll?.ClearCts();
                ctsPatroll = null;
            }

        }


    }
}