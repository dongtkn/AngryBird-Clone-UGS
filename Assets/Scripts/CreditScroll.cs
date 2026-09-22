using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform creditsContent;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private CanvasGroup letterboxTop;
    [SerializeField] private CanvasGroup letterboxBottom;
    [SerializeField] private CanvasGroup titleCardGroup;
    [SerializeField] private AudioSource musicSource;

    [Header("Positioning")]
    [Tooltip("Vị trí Y ban đầu của cụm chữ (đặt -1200f để không bị che chữ trên cùng)")]
    [SerializeField] private float startPositionY = -1200f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeScroll = 0.5f; // Giảm xuống 0.5s để mượt và liền mạch hơn
    [SerializeField] private float scrollSpeed = 40f;
    [SerializeField] private float fastForwardMultiplier = 3f;
    [SerializeField] private float letterboxFadeDuration = 1f;
    [SerializeField] private float titleFadeDuration = 1.2f;
    [SerializeField] private float titleHoldDuration = 1.5f;

    [Header("Chuyển scene")]
    [SerializeField] private string nextSceneName = "LoginScene";
    [SerializeField] private bool autoReturnToMenu = true;

    private bool isScrolling;

    private void Awake()
    {
        // 1. Đặt vị trí ban đầu NGAY TỪ AWAKE để Unity dựng UI chuẩn từ frame 0, không bị khựng mid-game
        if (creditsContent != null)
        {
            creditsContent.anchoredPosition = new Vector2(0, startPositionY);
        }

        // 2. Chuẩn bị ẩn TitleCard mờ sẵn
        if (titleCardGroup != null)
        {
            titleCardGroup.alpha = 0f;
            titleCardGroup.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(PlayCinematicSequence());
        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopBGM();
        }

        StartCoroutine(PlayCinematicSequence());
    }

    private IEnumerator PlayCinematicSequence()
    {
        // Hiện 2 thanh đen điện ảnh cùng lúc (chạy song song, không bắt chờ nhau)
        StartCoroutine(FadeCanvasGroup(letterboxTop, 0f, 1f, letterboxFadeDuration));
        StartCoroutine(FadeCanvasGroup(letterboxBottom, 0f, 1f, letterboxFadeDuration));

        if (musicSource != null) musicSource.Play();

        // Hiện TitleCard -> Giữ -> Mờ dần -> Tắt hẳn (tránh đè chữ)
        if (titleCardGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(titleCardGroup, 0f, 1f, titleFadeDuration));
            yield return new WaitForSeconds(titleHoldDuration);
            yield return StartCoroutine(FadeCanvasGroup(titleCardGroup, 1f, 0f, titleFadeDuration));

            titleCardGroup.gameObject.SetActive(false); // Tắt Object hoàn toàn
        }

        // Chờ 1 nhịp ngắn rồi chuyển trạng thái cuộn mượt mà
        yield return new WaitForSeconds(delayBeforeScroll);
        isScrolling = true;
    }

    private void Update()
    {
        if (!isScrolling || creditsContent == null || viewport == null) return;

        // Tua nhanh khi giữ Space hoặc Chuột trái
        float currentSpeed = scrollSpeed;
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            currentSpeed *= fastForwardMultiplier;
        }

        creditsContent.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;

        float viewportHeight = viewport.rect.height;

        // Với Pivot Y = 0 (đáy), chỉ cần vị trí Y lớn hơn nửa chiều cao Viewport (+100px lề an toàn)
        // là toàn bộ chữ đã hoàn toàn trôi khỏi mép trên màn hình.
        if (creditsContent.anchoredPosition.y >= (viewportHeight * 0.5f) + 100f)
        {
            isScrolling = false;
            OnCreditsFinished();
        }
    }

    private void OnCreditsFinished()
    {
        if (autoReturnToMenu) StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }

    public void SkipCredits()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) yield break;
        float t = 0f;
        group.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        group.alpha = to;
    }
}