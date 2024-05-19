using UnityEngine;

public class WolfPhase3 : WolfPhaseLunge
{
	public override void Enter()
	{
		base.Enter();
		WolfBoss.ClawAnim.transform.localScale = new Vector3(4, 4, WolfBoss.ClawAnim.transform.localScale.z);
	}
}
