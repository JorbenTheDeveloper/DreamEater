using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class BunnyBoss : MonoBehaviour
{
    public string Name = "Queen Bunny";
    public int HP = 100;
    public int Damage = 20;

    public bool StartAttacking = false;

    public float HopCoolDown = 5f;
    private bool CanHop = true;
    public float FallingTime = 2;
    public float FallIndicatorTime = 1;

    public FallingShadow FallingShadow;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool CanAttack = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Invoke(nameof(StartAttack), 1);
    }

    void StartAttack()
    {
        StartAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartAttacking) return;

        if (CanHop)
        {
            CanHop = false;
            StartCoroutine(Hop());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CanAttack)
        {
            CanAttack = false;
            Player.Instance.TakeDamage(Damage);
        }
    }

    IEnumerator Hop()
    {
        animator.SetBool("Hop", true);

        yield return new WaitForSeconds(FallingTime);

        var targtPos = Player.Instance.transform.position;
        // show UI 
        FallingShadow.gameObject.SetActive(true);
        FallingShadow.StartFilling(FallingTime, targtPos);

        yield return new WaitForSeconds(FallIndicatorTime);

        transform.position = targtPos;
        animator.SetBool("Hop", false);
        animator.SetBool("Fall", true);

        yield return new WaitForSeconds(0.5f);

        FallingShadow.gameObject.SetActive(false);
        CanAttack = true;

        yield return new WaitForSeconds(HopCoolDown);

        animator.SetBool("Fall", false);

        CanHop = true;
    }
}
