using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    [Header("Referencias")]
    public Image iconImage;
    public Image backgroundImage;
    public TMP_Text nameText;
    public TMP_Text progressText;
    public Slider progressBar;
    public TMP_Text tierText;
    public Button claimButton;

    [Header("Colores")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedColor = Color.white;

    private AchievementData data;

    public void Setup(AchievementData achievementData)
    {
        data = achievementData;
        claimButton.onClick.AddListener(OnClaim);
        Refresh();
    }

    public void Refresh()
    {
        if (data == null) return;

        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.achievementName;

        if (data.type == AchievementType.Single)
            RefreshSingle();
        else
            RefreshTiered();
    }

    void RefreshSingle()
    {
        bool completed = AchievementManager.Instance.IsSingleCompleted(data.achievementID);

        if (backgroundImage != null)
            backgroundImage.color = completed ? unlockedColor : lockedColor;

        if (progressText != null)
            progressText.text = completed ? "Completed!" : "Not yet...";

        if (progressBar != null)
            progressBar.value = completed ? 1f : 0f;

        if (tierText != null)
            tierText.gameObject.SetActive(false);

        if (claimButton != null)
            claimButton.gameObject.SetActive(false);
    }

    void RefreshTiered()
    {
        int currentTier = AchievementManager.Instance.GetCurrentTier(data.achievementID);
        int progress = AchievementManager.Instance.GetProgress(data.achievementID);
        bool maxed = currentTier >= data.tiers.Length;
        bool canClaim = AchievementManager.Instance.CanClaim(data.achievementID);

        // Background
        if (backgroundImage != null)
            backgroundImage.color = currentTier > 0 ? unlockedColor : lockedColor;

        // Tier label
        if (tierText != null)
        {
            tierText.gameObject.SetActive(true);
            tierText.text = maxed ? "MAX" : data.tiers[currentTier].label;
        }

        // Barra y contador
        if (!maxed)
        {
            int requirement = data.tiers[currentTier].requirement;
            float fill = Mathf.Clamp01((float)progress / requirement);

            if (progressBar != null) progressBar.value = fill;
            if (progressText != null) progressText.text = $"{progress} / {requirement}";
        }
        else
        {
            if (progressBar != null) progressBar.value = 1f;
            if (progressText != null) progressText.text = "Completed!";
        }

        // Botón de claim
        if (claimButton != null)
            claimButton.gameObject.SetActive(canClaim);
    }

    void OnClaim()
    {
        AchievementManager.Instance.Claim(data.achievementID);
        Refresh();
    }
}