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
        public Button button;          // ����ť���
        public Image glowImage;        // ��Ч��ͼ
        public Image backgroundImage;  // ����ť��ͼ
    }

    [Header("����ť")]
    public List<BreathButton> mainButtons;

    [Header("��ť�����б��Զ�ʶ���Ӱ�ť��")]
    public List<Transform> buttonRoots = new List<Transform>();

    [Header("������Ч����")]
    public float pulseScale = 1.1f;
    public float pulseAlpha = 0.6f;
    public float pulseSpeed = 2f;

    [Header("����ťѡ����ɫ")]
    public Color selectedColor = new Color(0.4f, 1f, 0.9f); // ����ɫ

    [Header("����ťĬ����ɫ")]
    public Color normalColor = Color.white;

    private BreathButton currentMain;
    private Vector3 originalScale;
    private Color originalGlowColor;

    private HashSet<Button> mainButtonSet;
    private List<Button> subButtons = new List<Button>();

    void Start()
    {
        // ��¼����ť
        mainButtonSet = new HashSet<Button>(mainButtons.Select(b => b.button));

        // �Զ�ʶ���Ӱ�ť
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

        // ��ʼ������ť
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

        // ��ʼ���Ӱ�ť
        foreach (var btn in subButtons)
        {
            AddSelectListener(btn, OnSubButtonSelected);
        }

        // Ĭ��ѡ�е�һ������ť
        if (mainButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(mainButtons[0].button.gameObject);
            OnMainButtonSelected(mainButtons[0]);
        }
    }

    void Update()
    {
        // ��������
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
        // �ر���һ������ťЧ��
        if (currentMain != null && currentMain != btn)
        {
            if (currentMain.glowImage != null)
                currentMain.glowImage.enabled = false;

            if (currentMain.backgroundImage != null)
                currentMain.backgroundImage.color = normalColor;
        }

        currentMain = btn;

        // ������ǰ����ťЧ��
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
        // �Ӱ�ť����ı�����ť״̬������ǿ�ƻ�ѡ��ǰ����ť
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