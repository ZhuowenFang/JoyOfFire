using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class ButtonBreathManager : MonoBehaviour
{
    [System.Serializable]
    public class BreathButton
    {
        public Button button;          // 主按钮组件
        public Image glowImage;        // 光效贴图
        public Image backgroundImage;  // 主按钮底图
    }

    [Header("主按钮")]
    public List<BreathButton> mainButtons;

    [Header("按钮容器列表（自动识别子按钮）")]
    public List<Transform> buttonRoots = new List<Transform>();

    [Header("呼吸光效参数")]
    public float pulseScale = 1.1f;
    public float pulseAlpha = 0.6f;
    public float pulseSpeed = 2f;

    [Header("主按钮选中颜色")]
    public Color selectedColor = new Color(0.4f, 1f, 0.9f); // 青绿色

    [Header("主按钮默认颜色")]
    public Color normalColor = Color.white;

    private BreathButton currentMain;
    private Vector3 originalScale;
    private Color originalGlowColor;

    private HashSet<Button> mainButtonSet;
    private List<Button> subButtons = new List<Button>();

    void Start()
    {
        mainButtonSet = new HashSet<Button>(mainButtons.Select(b => b.button));

        foreach (var root in buttonRoots)
        {
            if (root == null) continue;

            var allButtons = root.GetComponentsInChildren<Button>(includeInactive: true);
            foreach (var btn in allButtons)
            {
                if (!mainButtonSet.Contains(btn) && !subButtons.Contains(btn))
                {
                    subButtons.Add(btn);
                }
            }
        }

        foreach (var btn in mainButtons)
        {
            if (btn.glowImage != null)
            {
                btn.glowImage.enabled = false;
                originalScale = btn.glowImage.rectTransform.localScale;
                originalGlowColor = btn.glowImage.color;
            }

            if (btn.backgroundImage != null)
                btn.backgroundImage.color = normalColor;

            AddButtonListeners(btn.button, () => OnMainButtonSelected(btn));
        }

        foreach (var btn in subButtons)
        {
            AddButtonListeners(btn, OnSubButtonSelected);
        }

        if (mainButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(mainButtons[0].button.gameObject);
            OnMainButtonSelected(mainButtons[0]);
        }
    }

    void Update()
    {
        if (currentMain != null && currentMain.glowImage != null && currentMain.glowImage.enabled)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float scale = Mathf.Lerp(1f, pulseScale, t);
            float alpha = Mathf.Lerp(0.3f, pulseAlpha, t);

            currentMain.glowImage.rectTransform.localScale = originalScale * scale;

            Color c = originalGlowColor;
            c.a = alpha;
            currentMain.glowImage.color = c;
        }
    }

    /// <summary>
    /// 每次外部刷新呼吸光效
    /// </summary>
    public void RefreshBreathEffect()
    {
        if (currentMain == null && mainButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(mainButtons[0].button.gameObject);
            OnMainButtonSelected(mainButtons[0]);
        }
        else if (currentMain != null)
        {
            if (currentMain.glowImage != null)
            {
                currentMain.glowImage.enabled = true;
                currentMain.glowImage.rectTransform.localScale = originalScale;
                currentMain.glowImage.color = originalGlowColor;
            }

            if (currentMain.backgroundImage != null)
                currentMain.backgroundImage.color = selectedColor;
        }
    }

    void OnMainButtonSelected(BreathButton btn)
    {
        if (currentMain != null && currentMain != btn)
        {
            if (currentMain.glowImage != null)
                currentMain.glowImage.enabled = false;

            if (currentMain.backgroundImage != null)
                currentMain.backgroundImage.color = normalColor;
        }

        currentMain = btn;

        if (btn.glowImage != null)
        {
            btn.glowImage.enabled = true;
            btn.glowImage.rectTransform.localScale = originalScale;
            btn.glowImage.color = originalGlowColor;
        }

        if (btn.backgroundImage != null)
            btn.backgroundImage.color = selectedColor;
    }

    void OnSubButtonSelected()
    {
        if (currentMain != null)
            EventSystem.current.SetSelectedGameObject(currentMain.button.gameObject);
    }

    void AddButtonListeners(Button button, UnityEngine.Events.UnityAction onSelectOrClick)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;
        selectEntry.callback.AddListener((_) => onSelectOrClick());
        trigger.triggers.Add(selectEntry);

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((_) => onSelectOrClick());
        trigger.triggers.Add(clickEntry);
    }
}
