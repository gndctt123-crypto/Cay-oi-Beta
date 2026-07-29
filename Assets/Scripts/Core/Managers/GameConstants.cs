using UnityEngine;

public static class GameConstants
{
    // Cấu hình Lưới (Grid) của sân vườn
    public const int GridColumns = 9;
    public const int GridRows = 5;
    
    // Cấu hình Game State
    public const string StateMainMenu = "MainMenu";
    public const string StateLevel = "Level";
    public const string StateVictory = "GameVictory";
    public const string StateLose = "GameLose";

    // Tags & Layers
    public const string TagZombie = "Zombie";
    public const string TagPlant = "Plant";
    public const string TagBullet = "Bullet";
    public const string TagSun = "Sun";

    // Giá trị chuẩn
    public const int SunValue = 25;
    public const float SunLiveTime = 7.0f;
    public const float ProduceSunInterval = 7.0f;
}
