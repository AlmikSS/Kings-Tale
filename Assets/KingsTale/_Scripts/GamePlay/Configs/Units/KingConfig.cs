using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KingConfig", menuName = "Configs/KingUnit")]
public class KingConfigSO : UnitAttackConfigSO
{
    [Header("Abilities")] public List<AbilityConfig> Abilities;
}