using UnityEngine;
using MyBox;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSelect : MonoBehaviour
{
    public Button button { get; private set; }
    [SerializeField] Image border;

    private void Awake()
    {
        button = GetComponent<Button>();
        SetBorder(false);
    }
    public void SetBorder(bool borderStatus) => SetBorder(borderStatus, Color.white);
    public void SetBorder(bool borderStatus, Color color)
    {
        if (border != null)
        {
            this.border.gameObject.SetActive(borderStatus);
            this.border.color = color;
        }
    }
    private void FixedUpdate()
    {
        if (border != null && CreateGame.inst != null)
            this.border.SetAlpha(CreateGame.inst.opacity);
    }
}
