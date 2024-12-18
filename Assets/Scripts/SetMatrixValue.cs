using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SetMatrixValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText; // The displayed value
    [SerializeField] private Button Plus;
    [SerializeField] private Button Minus;

    private float currentValue = 0; // or int, depending on your needs

    private void Start()
    {
        // Assign listeners to the plus and minus buttons
        Plus.onClick.AddListener(IncrementValue);
        Minus.onClick.AddListener(DecrementValue);

    }

    public void IncrementValue()
    {
        Debug.Log("Plus Button Pressed!");
        currentValue += 1;
        UpdateDisplay();
    }

    public void DecrementValue()
    {
        Debug.Log("Minus Button Pressed!");
        currentValue -= 1;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        valueText.text = currentValue.ToString();
        valueText.text = "5";

    }

    public float GetValue()
    {
        return currentValue;
    }
}
