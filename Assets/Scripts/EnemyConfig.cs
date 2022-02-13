using System.Collections;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

[CreateAssetMenu]
public class EnemyConfig : ScriptableObject
{
	public List<GameObject> CustomZombiePrefabs;
	public List<SosigEnemyTemplate> ZosigPrefabs;

	[Tooltip("If round is not special, the curve affects spawning rates in normal rounds")]
	public AnimationCurve AppearanceQuantity;

	public bool SpecialRoundOnly;

	public int SpecialRoundStart;
	public int SpecialRoundInterval;
}
