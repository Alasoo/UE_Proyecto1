using UnityEngine;
using UnityEngine.AI;

namespace EnemySystem
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected float waitTime = 2f;



        protected EnemyScriptable enemyScriptable;
        private float timer;




        void Start()
        {
            agent.autoBraking = false;
            timer = waitTime; // Iniciamos el temporizador para que busque un punto de inmediato
        }

        public void Init(EnemyScriptable enemyScriptable)
        {
            this.enemyScriptable = enemyScriptable;
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Sumamos el tiempo que pasa
                timer += Time.deltaTime;

                // Si ha pasado el tiempo de espera, buscamos un nuevo destino
                if (timer >= waitTime)
                {
                    SetNewRandomDestination();
                    timer = 0f; // Reiniciamos el contador
                }
            }
        }

        void SetNewRandomDestination()
        {
            // 1. Generamos un punto aleatorio en una esfera multiplicada por nuestro radio
            Vector3 randomDirection = Random.insideUnitSphere * enemyScriptable.patrolRadius;

            // 2. Le sumamos la posición actual del enemigo para que busque alrededor de él
            randomDirection += transform.position;

            NavMeshHit hit;
            // 3. Comprobamos cuál es el punto válido más cercano en el NavMesh
            // El "1" al final es el NavMeshArea (por defecto todo es 1, "Walkable")
            if (NavMesh.SamplePosition(randomDirection, out hit, enemyScriptable.patrolRadius, 1))
            {
                // 4. Si encuentra un punto válido, le decimos al agente que vaya allí
                agent.SetDestination(hit.position);
            }
        }

    }
}
