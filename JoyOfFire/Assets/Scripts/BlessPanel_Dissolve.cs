using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class BlessPanel_Dissolve : MonoBehaviour
{
    public GameObject blessPanel;
    public Button blessButton;
    public Button closeButton;
    public GameObject layoutGroup;
    public GameObject blessTextPrefab;

    private Material dissolveMaterial;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        dissolveMaterial = blessPanel.GetComponent<Image>().material;
        canvasGroup = blessPanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        blessButton.onClick.AddListener(OnBlessButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnBlessButtonClicked()
    {
        StartCoroutine(ShowBlessPanelWithDissolve());
    }

    private void OnCloseButtonClicked()
    {
        StartCoroutine(HideBlessPanelWithDissolve());
    }

    private IEnumerator ShowBlessPanelWithDissolve()
    {
        blessPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        dissolveMaterial.SetFloat("_Cutoff", 1f);

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            dissolveMaterial.SetFloat("_Cutoff", 1f - t);
            canvasGroup.alpha = t;
            yield return null;
        }

        Time.timeScale = 0f;

        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var bless in EventManager.instance.Blessings)
        {
            if (bless.Key != "HP_max" &&
                bless.Key != "Strength" &&
                bless.Key != "Agility" &&
                bless.Key != "Wisdom" &&
                bless.Key != "Dream_vision" &&
                bless.Key != "Cave_vision" &&
                bless.Key != "Garnett_vision")
            {
                continue;
            }

            var blessText = Instantiate(blessTextPrefab, layoutGroup.transform);
            switch (bless.Key)
            {
                case "HP_max":
                    blessText.GetComponent<Text>().text = bless.Value > 0 ?
                        $"最大生命值    +{bless.Value * 100}% <b><color=#FFD700>BLESS</color></b>" :
                        $"最大生命值    {bless.Value * 100}% <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Strength":
                    blessText.GetComponent<Text>().text = bless.Value > 0 ?
                        $"力量    +{bless.Value} <b><color=#FFD700>BLESS</color></b>" :
                        $"力量    {bless.Value} <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Agility":
                    blessText.GetComponent<Text>().text = bless.Value > 0 ?
                        $"敏捷    +{bless.Value} <b><color=#FFD700>BLESS</color></b>" :
                        $"敏捷    {bless.Value} <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Wisdom":
                    blessText.GetComponent<Text>().text = bless.Value > 0 ?
                        $"智力    +{bless.Value} <b><color=#FFD700>BLESS</color></b>" :
                        $"智力    {bless.Value} <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Dream_vision":
                    blessText.GetComponent<Text>().text = $"获得1-11视野 <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Cave_vision":
                    blessText.GetComponent<Text>().text = $"获得1-6视野 <b><color=#FFD700>BLESS</color></b>";
                    break;
                case "Garnett_vision":
                    blessText.GetComponent<Text>().text = $"获得1-11视野 <b><color=#FFD700>BLESS</color></b>";
                    break;
            }
        }
    }

    private IEnumerator HideBlessPanelWithDissolve()
    {
        Time.timeScale = 1f;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            dissolveMaterial.SetFloat("_Cutoff", t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        blessPanel.SetActive(false);
        NavMeshAgent agent = GameObject.Find("Player").GetComponent<NavMeshAgent>();
        agent.ResetPath();
    }
}
