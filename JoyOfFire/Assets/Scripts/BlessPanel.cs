using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BlessPanel : MonoBehaviour
{
    public GameObject blessPanel;
    public Button blessButton;
    public Button closeButton;
    public GameObject layoutGroup;
    public GameObject blessTextPrefab;
    
    public void Start()
    {
        blessButton.onClick.AddListener(OnBlessButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    
    private void OnBlessButtonClicked()
    {
        blessPanel.SetActive(true);
        Time.timeScale = 0f;
        foreach(Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(var bless in EventManager.instance.Blessings)
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
                    if (bless.Value > 0)
                    {
                        blessText.GetComponent<Text>().text = $"最大生命值    +{bless.Value*100}% <b><color=#FFD700>BLESS</color></b>";

                    }
                    else
                    {
                        blessText.GetComponent<Text>().text = $"最大生命值    {bless.Value*100}% <b><color=#FFD700>BLESS</color></b>";

                    }
                    break;
                case "Strength":
                    if (bless.Value > 0)
                    {
                        blessText.GetComponent<Text>().text = $"力量    +{bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
                    else
                    {
                        blessText.GetComponent<Text>().text = $"力量    {bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
                    break;
                case "Agility":
                    if (bless.Value > 0)
                    {
                        blessText.GetComponent<Text>().text = $"敏捷    +{bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
                    else
                    {
                        blessText.GetComponent<Text>().text = $"敏捷    {bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
                    break;
                case "Wisdom":
                    if (bless.Value > 0)
                    {
                        blessText.GetComponent<Text>().text = $"智力    +{bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
                    else
                    {
                        blessText.GetComponent<Text>().text = $"智力    {bless.Value} <b><color=#FFD700>BLESS</color></b>";

                    }
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
    
    private void OnCloseButtonClicked()
    {
        blessPanel.SetActive(false);
        Time.timeScale = 1f;
        NavMeshAgent agent = GameObject.Find("Player").GetComponent<NavMeshAgent>();
        agent.ResetPath();
    }
}
