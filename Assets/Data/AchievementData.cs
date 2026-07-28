using UnityEngine;

public enum AchievementType { Tiered, Single }

[System.Serializable]
public class AchievementTier
{
    public string label;       // "I", "II", "III", "IV", "V"
    public int requirement;    // cantidad necesaria para completar este tier
    public int reward;         // placeholder para recompensa futura
}

[CreateAssetMenu(fileName = "AchievementData", menuName = "SystemBreach/Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string achievementID;   // ID único para PlayerPrefs, ej: "virus_slayer"
    public string achievementName;
    [TextArea] public string description;
    public Sprite icon;
    public AchievementType type;

    // Solo para Tiered
    public AchievementTier[] tiers;
}
