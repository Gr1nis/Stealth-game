using TMPro;
using UnityEngine;

public class UIInteractionPrompt : MonoBehaviour
{
    public static UIInteractionPrompt Instance;

    public TextMeshProUGUI interactionText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Hide();
    }

    public void Show(string text)
    {
        interactionText.text = text;
        interactionText.enabled = true;
    }

    public void Hide()
    {
        interactionText.text = "";
        interactionText.enabled = false;
    }
}
