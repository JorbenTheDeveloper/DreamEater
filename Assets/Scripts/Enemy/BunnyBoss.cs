using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BunnyBoss : MonoBehaviour
{
    public string Name = "Queen Bunny";
    public int CurrentHP = 100;
    public int MaxHP = 100;
    public int Damage = 20;

    public bool StartAttacking = false;

    public float HopCoolDown = 5f;
    private bool CanHop = true;
    public float FallingTime = 2;
    public float FallIndicatorTime = 1;

    public FallingShadow FallingShadow;
    public GameObject DustParticlePrefab;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool CanAttack = false;
    private int HopCount = 0;
    public int MaxHopBeforeVulnerable = 3;
    public float HopCoolDownWhenVulnerable = 8f;
    public float DamagebleCoolDown = 2;
    public float DamagebleTimer = 0;

    [Header("Spawning")]
    public int[] EnemyCountForRound;
    public GameObject SmallBunnyPrefab;
    private int currentSpawnRound = 0;
    private bool IsSpawnPhase = false;

    private bool IsVulnerable => HopCount >= MaxHopBeforeVulnerable;

    // for testing
    private Color originalColor;
    private Color vulnerableColor = new Color(255f / 255, 119f / 255, 119f / 255);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        originalColor = spriteRenderer.color;
        DamagebleTimer = DamagebleCoolDown;
        CurrentHP = MaxHP;

        Invoke(nameof(StartAttack), 2);
    }

    void StartAttack()
    {
        StartAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartAttacking) return;
        DamagebleTimer += Time.deltaTime;

        if (IsSpawnPhase)
        {
            
            return;
        }
        if (CanHop)
        {
            HopCount++;
            CanHop = false;
            StartCoroutine(Hop());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (CanAttack)
            {
                CanAttack = false;
                Player.Instance.TakeDamage(Damage);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsVulnerable && DamagebleTimer >= DamagebleCoolDown)
            {
                DamagebleTimer = 0;
                CurrentHP -= 5;

                if (CurrentHP <= MaxHP / 2)
                {
                    IsSpawnPhase = true;
                }

                if (CurrentHP <= 0)
                {
                    SceneManager.LoadScene("Win");
                }
            }
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

        yield return new WaitForSeconds(0.4f);

        animator.SetBool("Fall", false);
        SpawnParticleEffect();
        FallingShadow.gameObject.SetActive(false);
        CanAttack = true;

        yield return new WaitForSeconds(0.2f);
        CanAttack = false;

        if (IsVulnerable)
        {
            spriteRenderer.color = vulnerableColor;

            yield return new WaitForSeconds(HopCoolDownWhenVulnerable);

            HopCount = 0;
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(HopCoolDown);
        }

        CanHop = true;
    }

    void SpawnParticleEffect()
    {
        GameObject effect = Instantiate(DustParticlePrefab, transform.position, Quaternion.identity);

        // Set the particle effect's scale to match the player's scale
        effect.transform.localScale = transform.localScale;

        Destroy(effect, 2f);
    }
}
