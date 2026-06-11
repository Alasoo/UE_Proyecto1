using Controller.Player;
using UnityEngine;

namespace GameSystem
{
    public class Experience : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer spriteRenderer;



        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            int exp = UnityEngine.Random.Range(150, 350);
            PlayerStateMachine.Instance.playerStats.AddExperience(exp);
            ExperiencePool.Instance.Return(this);
        }
    }
}
