using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelSlider : MonoBehaviour
{
    public RectTransform mainPanel;
    public RectTransform[] pages;
    public float slideSpeed = 10f;
    public Button[] buttons;
    public RectTransform[] buttonIcons;
    public CanvasScaler canvasScaler;

    private int currentPage = 0;
    private Coroutine slideCoroutine;
    private Vector2 startTouchPosition, endTouchPosition;
    private Vector2 lastScreenSize;

    void Start()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        AdjustPanels(); 
        AdjustPanelPositions();
    }

    void Update()
    {
        DetectSwipe();

        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            AdjustPanels();
            AdjustPanelPositions();
        }
    }

    public void SlideToPage(int pageIndex)
    {
        if (pageIndex < -2 || pageIndex > 2) return;

        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        currentPage = pageIndex;

        float panelWidth = GetScreenWidth();
        Vector2 targetPos = new Vector2(-pageIndex * panelWidth, mainPanel.anchoredPosition.y);
        slideCoroutine = StartCoroutine(SmoothSlide(targetPos));

        UpdateButtonSize();
    }

    private IEnumerator SmoothSlide(Vector2 targetPos)
    {
        while (Vector2.Distance(mainPanel.anchoredPosition, targetPos) > 0.1f)
        {
            mainPanel.anchoredPosition = Vector2.Lerp(mainPanel.anchoredPosition, targetPos, slideSpeed * Time.deltaTime);
            yield return null;
        }
        mainPanel.anchoredPosition = targetPos;
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
            startTouchPosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            float swipeDistance = endTouchPosition.x - startTouchPosition.x;

            if (Mathf.Abs(swipeDistance) > 50)
            {
                if (swipeDistance < 0 && currentPage < 2)
                    SlideToPage(currentPage + 1);
                else if (swipeDistance > 0 && currentPage > -2)
                    SlideToPage(currentPage - 1);
            }
        }
    }

    private void UpdateButtonSize()
    {
        for (int i = 0; i < buttonIcons.Length; i++)
        {
            int buttonIndex = i - 2;
            buttonIcons[i].localScale = (buttonIndex == currentPage) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private void AdjustPanels()
    {
        float screenWidth = GetScreenWidth();

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].sizeDelta = new Vector2(screenWidth, pages[i].sizeDelta.y);
            pages[i].anchoredPosition = new Vector2((i - 2) * screenWidth, pages[i].anchoredPosition.y);
        }

        mainPanel.sizeDelta = new Vector2(screenWidth * pages.Length, mainPanel.sizeDelta.y);
    }

    private void AdjustPanelPositions()
    {
        float screenWidth = GetScreenWidth();
        Vector2 newPosition = new Vector2(-currentPage * screenWidth, mainPanel.anchoredPosition.y);
        mainPanel.anchoredPosition = newPosition;
    }

    private float GetScreenWidth()
    {
        return Screen.width;
    }
}
