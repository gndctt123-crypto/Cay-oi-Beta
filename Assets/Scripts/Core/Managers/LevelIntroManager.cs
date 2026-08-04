using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelIntroManager : MonoBehaviour
{
    public Image introImage;
    public Sprite[] introSprites; // Mảng chứa các ảnh "Ready", "Set", "Plant"
    
    public float panStartX = 5f;
    public float panEndX = 0f;
    public float viewTime = 2f;
    public float panDuration = 2f;
    
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            StartCoroutine(IntroSequence());
        }
    }

    private IEnumerator IntroSequence()
    {
        // 1. Đặt camera ở vị trí Start (bên phải để nhìn thấy khu vực zombie)
        Vector3 camPos = mainCamera.transform.position;
        camPos.x = panStartX;
        mainCamera.transform.position = camPos;
        
        // Đảm bảo Image rỗng lúc đầu
        if (introImage != null) 
        {
            introImage.sprite = null;
            introImage.color = new Color(1, 1, 1, 0); // Trong suốt
        }

        // 2. Chờ 2 giây để người chơi nhìn
        yield return new WaitForSeconds(viewTime);

        // 3. Pan từ phải sang trái
        float elapsed = 0f;
        while (elapsed < panDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / panDuration;
            // Dùng hàm SmoothStep để chuyển động mượt mà hơn
            t = Mathf.SmoothStep(0f, 1f, t);
            
            camPos.x = Mathf.Lerp(panStartX, panEndX, t);
            mainCamera.transform.position = camPos;
            yield return null;
        }
        
        // Đảm bảo chính xác vị trí cuối cùng
        camPos.x = panEndX;
        mainCamera.transform.position = camPos;

        // 4. Hiển thị chữ ""Ready... Set... PLANT!"" bằng hình ảnh
        if (introImage != null && introSprites != null && introSprites.Length > 0)
        {
            introImage.color = Color.white; // Hiện lên
            
            foreach (Sprite spr in introSprites)
            {
                if (spr != null)
                {
                    introImage.sprite = spr;
                    yield return StartCoroutine(AnimateTextScale(introImage.transform));
                }
            }
            
            // Xóa chữ
            introImage.color = new Color(1, 1, 1, 0);
            introImage.sprite = null;
        }
    }

    private IEnumerator AnimateTextScale(Transform textTransform)
    {
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 initialScale = Vector3.one * 0.5f;
        Vector3 targetScale = Vector3.one * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Tạo hiệu ứng phóng to và đàn hồi nhẹ
            float scaleMultiplier = Mathf.Sin(t * Mathf.PI); // Từ 0 -> 1 -> 0
            
            textTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }
        
        textTransform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.2f);
    }
}
