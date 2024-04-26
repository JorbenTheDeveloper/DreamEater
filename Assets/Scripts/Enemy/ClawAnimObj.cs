using UnityEngine;

public class ClawAnimObj : MonoBehaviour
{
    Animator Animator;
    public float AnimationDuration = 2f;
    public int DamageAmount = 10;

    private void OnEnable()
    {
        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }

        Animator.SetBool("StartClawAttack", true);
        Invoke(nameof(Disable), AnimationDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(DamageAmount);
        }
    }

    private void Disable()
    {
        Animator.SetBool("StartClawAttack", false);
        gameObject.SetActive(false);
    }
}
