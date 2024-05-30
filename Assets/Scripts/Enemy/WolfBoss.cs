using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public interface IWolfPhase
{
    Animator Animator { get; set; }
    NavMeshAgent NavMeshAgent { get; set; }
    WolfBoss WolfBoss { get; set; }
	bool IsRunning { get; set; }

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
	public int TakeDamageAmountFromProjectile = 10;

	[Header("Phase Lunge")]
	public GameObject LungeIndicator;

	[Header("Shared")]
	public GameObject ClawAnim;

    private bool _canTakeDamage = true;

	NavMeshAgent NavMeshAgent;
    Animator Animator;
    IWolfPhase Phase1;
    IWolfPhase PhaseLunge;
    IWolfPhase Phase3;
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
            NavMeshAgent = NavMeshAgent,
			StrikeDistance = 10,
			ClawAnimIndicatorDuration = 1,
			SlowSpeed = 3,
			FastSpeed = 8
		};

		PhaseLunge = new WolfPhaseLunge
		{
			WolfBoss = this,
			Animator = Animator,
			NavMeshAgent = NavMeshAgent,

			StrikeDistance = 10,
			ClawAnimIndicatorDuration = 1,
			SlowSpeed = 3,
			FastSpeed = 8,
			LungeIndicatorTimer = 3,
			LungeSpeed = 35,
			LungeDamage = 10
		};

		Phase3 = new WolfPhase3
		{
			WolfBoss = this,
			Animator = Animator,
			NavMeshAgent = NavMeshAgent,

			StrikeDistance = 12,
			ClawAnimIndicatorDuration = 0.5f,
			SlowSpeed = 6,
			FastSpeed = 9,
			LungeIndicatorTimer = 1,
			LungeSpeed = 35,
			LungeDamage = 10
		};

		CurPhase = Phase1;
		Phase1.Enter();
	}

    // Update is called once per frame
    void Update()
    {
		CurPhase.Update();

		if (!CurPhase.IsTired()) return;

		if (!Phase3.IsRunning && curHP <= 25)
		{
			CurPhase = Phase3;
			PhaseLunge.Exit();
			Phase3.Enter();
		}
		else if (!PhaseLunge.IsRunning && curHP <= 75)
		{
			CurPhase = PhaseLunge;
			Phase1.Exit();
			PhaseLunge.Enter();
		}

		else if (curHP <= 0)
		{
            SceneManager.LoadScene("EndingCutscene");
        }
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
        if (!collision.gameObject.CompareTag("Player")) return;
		if (CurPhase.IsTired() && _canTakeDamage)
        {
			TakeDamage(false);

		}
		else if (CurPhase is WolfPhaseLunge)
        {
            (CurPhase as WolfPhaseLunge).AttackPlayer();
		}
	}

	public void TakeDamage(bool isProjectile)
	{
		if (CurPhase.IsTired() && _canTakeDamage)
		{
			_canTakeDamage = false;
			curHP -= isProjectile ? TakeDamageAmountFromProjectile :PlayerDamage;
			Invoke(nameof(ToggleTakeDamage), 1);
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
