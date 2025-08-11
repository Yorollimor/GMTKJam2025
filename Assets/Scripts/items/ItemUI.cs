using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Image currencyIcon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI price;
    public TextMeshProUGUI description;

    private void Awake()
    {
        description.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        description.enabled = true;
        description.transform.GetComponent<Animator>().SetTrigger("Enabled");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        description.transform.GetComponent<Animator>().SetTrigger("Enabled");
    }
}
