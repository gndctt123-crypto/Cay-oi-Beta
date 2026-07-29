using UnityEngine;

public class LevelState : BaseGameState
{
    private float sunTimer;

    private void Start()
    {
        // Đăng ký state này vào hệ thống quản lý
        GameStateManager.Instance?.RegisterState(GameConstants.StateLevel, this);
    }

    public override void EnterState()
    {
        Debug.Log("Bắt đầu màn chơi mới!");
        sunTimer = 0f;
        // Thực hiện các setup như sinh xe cắt cỏ (Lawnmower), khởi tạo UI, nhạc nền...
    }

    public override void UpdateState()
    {
        // Xử lý logic chung của level như sinh mặt trời rơi từ trên trời xuống
        sunTimer += Time.deltaTime;
        if (sunTimer >= GameConstants.ProduceSunInterval)
        {
            SpawnSkySun();
            sunTimer = 0f;
        }

        // Kiểm tra điều kiện thắng thua (nếu zombie vào nhà -> chuyển qua GameLose)
    }

    public override void ExitState()
    {
        Debug.Log("Kết thúc màn chơi!");
        // Dọn dẹp level (xóa zombie, cây cối hiện tại để sang màn khác)
    }

    private void SpawnSkySun()
    {
        // Logic Instantiate Mặt trời rơi ngẫu nhiên trên bản đồ
        // (Sẽ cần truy cập vào MapGrid để lấy toạ độ hợp lý)
    }
}
