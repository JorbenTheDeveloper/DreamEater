using System;
using UnityEngine;
using UnityEngine.AI;

public interface IWolfPhase
{
    Animator Animator { get; set; }
    NavMeshAgent NavMeshAgent { get; set; }
    WolfBoss WolfBoss { get; set; }

    void Enter();
    void Update();
    void Exit();
    bool Stop();
    bool IsTired();
}

public class WolfBoss : MonoBehaviour
{
    public int curHP = 100;
    public int MaxHP = 100;
    public int PlayerDamage = 5;

    [Header("Phase 1")]
    public float ClawAnimIndicatorDuration = 0.5f;

	[Header("Phase Lunge")]
	public float LungeIndicatorTimer = 3f;
	public int LungeSpeed = 35;
    public int LungeDamage = 10;
	public GameObject LungeIndicator;

	[Header("Shared")]
	public int StrikeDistance = 3;
	public GameObject ClawAnim;
	public int SlowSpeed = 3;
	public int FastSpeed = 10;

    private bool _canTakeDamage = true;

	NavMeshAgent NavMeshAgent;
    Animator Animator;
    IWolfPhase Phase1;
    IWolfPhase PhaseLunge;
    IWolfPhase CurPhase;

	// Start is called before the first frame update
	void Start()
    {
		NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;

        Animator = GetComponent<Animator>();

		curHP = MaxHP;

		Phase1 = new WolfPhase1
        {
            WolfBoss = this,
            Animator = Animator,
            NavMeshAgent = NavMeshAgent
        };

		PhaseLunge = new WolfPhaseLunge
		{
			WolfBoss = this,
			Animator = Animator,
			NavMeshAgent = NavMeshAgent
		};

        CurPhase = Phase1;
        CurPhase.Enter();
	}

    // Update is called once per frame
    void Update()
    {
		CurPhase.Update();

        if (curHP <= 75 && !CurPhase.IsTired())
        {
            CurPhase = PhaseLunge;
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
        if (!collision.gameObject.CompareTag("Player")) return;
		if (CurPhase.IsTired() && _canTakeDamage)
        {
			_canTakeDamage = false;
			curHP -= PlayerDamage;
            Invoke(nameof(ToggleTakeDamage), 1);
        }
		else if (CurPhase is WolfPhaseLunge)
        {
            (CurPhase as WolfPhaseLunge).AttackPlayer();
		}
	}

    private void ToggleTakeDamage()
    {
        _canTakeDamage = true;
	}

	public bool Stop()
    {
		if (CurPhase is WolfPhaseLunge)
			return CurPhase.Stop();
        return false;
	}
}
