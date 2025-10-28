using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class TiltleButtonUI : BaseUI, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image fillImage;
    
    public float speed = 3f;

    private float target = 0f;

    void Awake()
    {
        fillImage.type = Image.Type.Filled;
        fillImage.fillAmount = 0f;
        fillImage.raycastTarget = false;
    }
    void Update()
    {
        ImageMoveTowards();
    }
    private void ImageMoveTowards()
    {
        fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, target, speed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        target = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target = 0f;
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}




