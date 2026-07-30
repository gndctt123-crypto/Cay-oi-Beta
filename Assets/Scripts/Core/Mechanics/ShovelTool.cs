using UnityEngine;
using UnityEngine.UI;

public class ShovelTool : MonoBehaviour
{
    private Image img;
    private Button btn;
    private MapGrid grid;

    void Start()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();
        grid = FindAnyObjectByType<MapGrid>();

        if (btn != null)
        {
            btn.onClick.AddListener(ToggleShovel);
        }
    }

    private void ToggleShovel()
    {
        if (grid != null)
        {
            grid.isDiggingMode = !grid.isDiggingMode;
            if (img != null)
            {
                img.color = grid.isDiggingMode ? Color.red : Color.yellow;
            }
        }
    }
}
