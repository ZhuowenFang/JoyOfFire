using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Button infoButton; 
    public GameObject popupPanel;
    public Text popupText;
    [TextArea(3, 10)]
    public string detailedInfo = "<color=red>  </color>\n<color=black>  </color>\n<color=green>  </color>";
    private bool isPopupVisible = false;

    void Start()
    {
        popupPanel.SetActive(false);
        infoButton.onClick.AddListener(TogglePopup);
    }

    void TogglePopup()
    {
        isPopupVisible = !isPopupVisible;

        if (isPopupVisible)
        {
            popupPanel.SetActive(true);
            popupText.text = detailedInfo;

            // 计算按钮中心位置
            Vector3 buttonPosition = infoButton.transform.position;
            RectTransform panelRect = popupPanel.GetComponent<RectTransform>();
            panelRect.position = new Vector3(buttonPosition.x, buttonPosition.y - panelRect.rect.height, buttonPosition.z);
        }
        else
        {
            popupPanel.SetActive(false);
        }
    }
}
