using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupCanvasObject;
    [SerializeField] private RectTransform popupObject;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Vector3 offset = new Vector3(0, 50, 0);
    [SerializeField] private float padding = 25;

    private Canvas popupCanvas;

    private void Awake()
    {
        popupCanvas = popupCanvasObject.GetComponent<Canvas>();
    }

    private void Update()
    {
        FollowCursor();
    }

    private void FollowCursor()
    {
        if (!popupCanvasObject.activeSelf) return;

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0;
        // right handling
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if(rightEdgeToScreenEdgeDistance < 0) newPos.x += rightEdgeToScreenEdgeDistance;
        
        // left handling
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 2) + padding;
        if(leftEdgeToScreenEdgeDistance > 0) newPos.x += leftEdgeToScreenEdgeDistance;
        
        // top handling
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if(topEdgeToScreenEdgeDistance < 0) newPos.y += topEdgeToScreenEdgeDistance;
        
        popupObject.transform.position = newPos;
    }

    public void DisplayInfo(Collectible collectible)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=35>").Append(collectible.ColoredName).Append("</size>").AppendLine();
        builder.Append(collectible.GetInfoDisplayText());

        infoText.text = builder.ToString();

        popupCanvasObject.SetActive(true);

        // Fixes not resizing correctly
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    public void HideInfo()
    {
        popupCanvasObject.SetActive(false);
    }
}
