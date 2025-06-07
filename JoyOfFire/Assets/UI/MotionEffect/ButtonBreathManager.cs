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
        // 记录主按钮
        mainButtonSet = new HashSet<Button>(mainButtons.Select(b => b.button));

        // 自动识别子按钮
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

        // 初始化主按钮
        foreach (var btn in mainButtons)
        {
            if (btn.glowImage != null)
            {
                btn.glowImage.enabled = false;
                originalScale = btn.glowImage.rectTransform.localScale;
                originalGlowColor = btn.glowImage.color;
            }

            if (btn.backgroundImage != null)
            {
                btn.backgroundImage.color = normalColor;
            }

            AddSelectListener(btn.button, () => OnMainButtonSelected(btn));
        }

        // 初始化子按钮
        foreach (var btn in subButtons)
        {
            AddSelectListener(btn, OnSubButtonSelected);
        }

        // 默认选中第一个主按钮
        if (mainButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(mainButtons[0].button.gameObject);
            OnMainButtonSelected(mainButtons[0]);
        }
    }

    void Update()
    {
        // 呼吸动画
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

    void OnMainButtonSelected(BreathButton btn)
    {
        // 关闭上一个主按钮效果
        if (currentMain != null && currentMain != btn)
        {
            if (currentMain.glowImage != null)
                currentMain.glowImage.enabled = false;

            if (currentMain.backgroundImage != null)
                currentMain.backgroundImage.color = normalColor;
        }

        currentMain = btn;

        // 开启当前主按钮效果
        if (btn.glowImage != null)
        {
            btn.glowImage.enabled = true;
            btn.glowImage.rectTransform.localScale = originalScale;
            btn.glowImage.color = originalGlowColor;
        }

        if (btn.backgroundImage != null)
        {
            btn.backgroundImage.color = selectedColor;
        }
    }

    void OnSubButtonSelected()
    {
        // 子按钮不会改变主按钮状态，但可强制回选当前主按钮
        if (currentMain != null)
        {
            EventSystem.current.SetSelectedGameObject(currentMain.button.gameObject);
        }
    }

    void AddSelectListener(Button button, UnityEngine.Events.UnityAction onSelect)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((_) => onSelect());
        trigger.triggers.Add(entry);
    }
}