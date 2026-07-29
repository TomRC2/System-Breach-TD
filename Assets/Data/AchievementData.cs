using UnityEngine;

public enum AchievementType { Tiered, Single }

[System.Serializable]
public class AchievementTier
{
    public string label;
    public int requirement;
    public int reward;
}

[CreateAssetMenu(fileName = "AchievementData", menuName = "SystemBreach/Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string achievementID;
    public string achievementName;
    [TextArea] public string description;
    public Sprite icon;
    public AchievementType type;

    // Solo para Tiered
    public AchievementTier[] tiers;
}
