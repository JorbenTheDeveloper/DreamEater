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

    public Sprite Injured1;
    public Sprite Injured2;
    public Sprite Injured3;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool CanAttack = false;
    private int HopCount = 0;
    public int MaxHopBeforeVulnerable = 3;
    public float HopCoolDownWhenVulnerable = 8f;
    public float DamagebleCoolDown = 2;
    public float DamagebleTimer = 0;
    public int TakeDamageAmount = 5;
    public int TakeDamageAmountFromProjectile = 10;

    [Header("Spawning")]
    public GameObject RetreatObj;
    public int[] EnemyCountForRound;
    public GameObject SmallBunnyPrefab;
    public GameObject[] SpawnPositions;
    private List<GameObject> SpawnedObjects;
    private int currentSpawnRound = -1;
    private bool IsSpawnPhase = false;
    private bool CanSpawn = false;
    private bool hasBeenInSpawningPhase = false;

    private bool IsVulnerable => HopCount >= MaxHopBeforeVulnerable;
    private bool isDazed = false;

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

        /*IsSpawnPhase = true;
        CanSpawn = true;
        CanHop = false;
        Invoke(nameof(StartAttack), 2);*/
    }

    public void StartAttack()
    {
        StartAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartAttacking) return;
        DamagebleTimer += Time.deltaTime;

        if (!IsVulnerable && !IsSpawnPhase) LookAtPlayer();

        if (IsSpawnPhase)
        {
            if (CanSpawn)
            {
                currentSpawnRound++;
                CanSpawn = false;
                StartCoroutine(StartSpawning());
            }
            else
            {
                if (SpawnedObjects != null && SpawnedObjects.Count != 0)
                {
                    bool allDead = true;
                    for (int i = 0; i < SpawnedObjects.Count; i++)
                    {
                        if (SpawnedObjects[i] != null)
                        {
                            allDead = false;
                            break;
                        }
                    }

                    if (allDead)
                    {
                        SpawnedObjects.Clear();
                        SpawnedObjects = null;
                        CanSpawn = true;
                    }
                }
            }
        }
        else if (CanHop)
        {
            HopCount++;
            CanHop = false;
            StartCoroutine(Hop());
        }
    }

    private void LateUpdate()
    {
        if (isDazed) return;
        

        if (CurrentHP >= 75)
        {
            //spriteRenderer.sprite = spriteRenderer.sprite;
        }
        else if (CurrentHP >= 50)
        {
            spriteRenderer.sprite = Injured1;
        }
        else if (CurrentHP >= 25)
        {
            spriteRenderer.sprite = Injured2;
        }
        else if (CurrentHP > 0)
        {
            spriteRenderer.sprite = Injured3;
        }
    }

    IEnumerator StartSpawning()
    {
        hasBeenInSpawningPhase = true;
        if (currentSpawnRound == 0) StartCoroutine(HopToRetreat());

        yield return new WaitForSeconds(3);

        // TODO: this phase is done, move to next phase
        if (currentSpawnRound >= EnemyCountForRound.Length)
        {
            IsSpawnPhase = false;
            CanSpawn = false;
            CanHop = true;
        }
        else
        {
            SpawnedObjects = new List<GameObject>();
            int objectCountToSpawn = EnemyCountForRound[currentSpawnRound];
            int spawnPosIndex = 0;
            for (int i = 0; i < objectCountToSpawn; i++)
            {
                var spawnPos = SpawnPositions[spawnPosIndex].transform.position;
                var bunny = Instantiate(SmallBunnyPrefab, spawnPos, SmallBunnyPrefab.transform.rotation);
                SpawnedObjects.Add(bunny);

                spawnPosIndex++;
                if (spawnPosIndex >= SpawnPositions.Length)
                    spawnPosIndex = 0;
            }
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
            TakeDamage(false);
        }
    }

    public void TakeDamage(bool isProjectile)
    {
        if (IsVulnerable && DamagebleTimer >= DamagebleCoolDown)
        {
            DamagebleTimer = 0;
            CurrentHP -= isProjectile ? TakeDamageAmountFromProjectile : TakeDamageAmount;

            if (CurrentHP <= 0)
            {
                SceneManager.LoadScene("LevelTwo");
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

        LookAtPlayer();
        yield return new WaitForSeconds(0.4f);

        AudioManager.Instance.Play("Slam");
        animator.SetBool("Fall", false);
        SpawnParticleEffect();
        FallingShadow.gameObject.SetActive(false);
        CanAttack = true;

        yield return new WaitForSeconds(0.2f);
        CanAttack = false;

        if (IsVulnerable)
        {
            string dazedAnimName = "DazedInjured3";
            if (CurrentHP >= 75) dazedAnimName = "Dazed";
            else if (CurrentHP >= 50) dazedAnimName = "DazedInjured1";
            else if(CurrentHP >= 25) dazedAnimName = "DazedInjured2";

            isDazed = true;
            animator.SetBool(dazedAnimName, true);

            yield return new WaitForSeconds(HopCoolDownWhenVulnerable);
            animator.SetBool(dazedAnimName, false);
            HopCount = 0;
            isDazed = false;


            if (!hasBeenInSpawningPhase && CurrentHP <= MaxHP / 2)
            {
                IsSpawnPhase = true;
                CanSpawn = true;

                // half all the cooldowns
                HopCoolDown /= 2;
                FallingTime /= 2;
                FallIndicatorTime /= 2;
                HopCoolDownWhenVulnerable /= 2;
                MaxHopBeforeVulnerable += 1;

                yield break;
            }
        }
        else
        {
            yield return new WaitForSeconds(HopCoolDown);
        }

        CanHop = true;
    }

    IEnumerator HopToRetreat()
    {

        animator.SetBool("Hop", true);
        yield return new WaitForSeconds(1);

        // show UI 
        FallingShadow.gameObject.SetActive(true);
        FallingShadow.StartFilling(FallingTime, RetreatObj.transform.position);

        yield return new WaitForSeconds(1);

        transform.position = RetreatObj.transform.position;
        animator.SetBool("Hop", false);
        animator.SetBool("Fall", true);

        transform.rotation = RetreatObj.transform.rotation;
        yield return new WaitForSeconds(0.4f);

        animator.SetBool("Fall", false);
        SpawnParticleEffect();
        FallingShadow.gameObject.SetActive(false);
    }

    void LookAtPlayer()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - Player.Instance.transform.position);
    }

    void SpawnParticleEffect()
    {
        GameObject effect = Instantiate(DustParticlePrefab, transform.position, Quaternion.identity);

        // Set the particle effect's scale to match the player's scale
        effect.transform.localScale = transform.localScale;

        Destroy(effect, 2f);
    }
}
