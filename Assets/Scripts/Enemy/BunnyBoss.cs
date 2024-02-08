using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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

    public FallingShadow FallingShadow;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    IEnumerator Hop()
    {
        // TODO: play animation hopping
        // disappear
        spriteRenderer.enabled = false;
        // go to player pos
        transform.position = Player.Instance.transform.position;
        // show UI 
        FallingShadow.gameObject.SetActive(true);
        FallingShadow.StartFilling(FallingTime, transform.position);

        yield return new WaitForSeconds(FallingTime);

        // appear
        spriteRenderer.enabled = true;
        // TODO: play animation falling

        yield return new WaitForSeconds(0.5f);
        FallingShadow.gameObject.SetActive(false);
        Player.Instance.TakeDamage(Damage);

        yield return new WaitForSeconds(HopCoolDown);

        CanHop = true;
    }
}
