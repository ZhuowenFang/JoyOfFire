using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GenderButtonGroup : MonoBehaviour
{
    public List<Button> buttons;
    public Color selectedColor = Color.blue;
    public Color defaultColor = Color.white;

    private Button selectedButton;
    
    public bool transparence = false;
    public Button ConfirmButton = null;
    
    void Start()
    {
        foreach (Button button in buttons)
        {
            if (!transparence)
            {
                button.onClick.AddListener(() => OnButtonClicked(button));
            }
            else
            {
                button.onClick.AddListener(() => OnButtonTransparenceClicked(button));

            }
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        if (selectedButton == clickedButton)
            return;

        if (selectedButton != null)
        {
            SetButtonColor(selectedButton, defaultColor);
        }

        selectedButton = clickedButton;
        SetButtonColor(selectedButton, selectedColor);
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                SetButtonColor(button, defaultColor);
            }
        }
    }

    void SetButtonColor(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;

        button.GetComponent<Image>().color = color;
        button.OnDeselect(null);
        button.OnSelect(null);
    }
    
    void OnButtonTransparenceClicked(Button clickedButton)
    {
        if (ConfirmButton != null)
        {
            ConfirmButton.gameObject.SetActive(true);
        }
        if (selectedButton == clickedButton)
            return;

        if (selectedButton != null)
        {
            SetButtonTransparence(selectedButton, 0.5f);
        }

        selectedButton = clickedButton;
        SetButtonTransparence(selectedButton, 1f);
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                SetButtonTransparence(button, 0.5f);
            }
        }
    }
    
    void SetButtonTransparence(Button button, float alpha)
    {
        var image = button.GetComponent<Image>();
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}