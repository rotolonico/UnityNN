using UnityEngine;

public static class InterfaceStyle
{
    private static Texture2D inputCell;
    private static GUIStyle inputCellStyle;

    private static Texture2D hiddenCell;
    private static GUIStyle hiddenCellStyle;

    private static Texture2D hiddenRecurrentCell;
    private static GUIStyle hiddenRecurrentCellStyle;

    private static Texture2D hiddenMemoryCell;
    private static GUIStyle hiddenMemoryCellStyle;

    private static Texture2D hiddenMemoryRecurrentCell;
    private static GUIStyle hiddenMemoryRecurrentCellStyle;

    private static Texture2D outputCell;
    private static GUIStyle outputCellStyle;

    private static Texture2D outputMemoryCell;
    private static GUIStyle outputMemoryCellStyle;
    public static GUIStyle InputCell()
    {
        if (inputCell == null)
            inputCell = CellRender(new Color(1, 0.8F, 0, 1F), false);
        if (inputCellStyle == null)
            inputCellStyle = MakeStyle(inputCell);
        return inputCellStyle;
    }

    public static GUIStyle HiddenCell()
    {
        if (hiddenCell == null)
            hiddenCell = CellRender(new Color(0.4F, 0.8F, 0, 1F), false);
        if (hiddenCellStyle == null)
            hiddenCellStyle = MakeStyle(hiddenCell);
        return hiddenCellStyle;
    }

    public static GUIStyle HiddenMemoryCell()
    {
        if (hiddenMemoryCell == null)
            hiddenMemoryCell = CellRender(new Color(0.4F, 0.8F, 0, 1F), true);
        if (hiddenMemoryCellStyle == null)
            hiddenMemoryCellStyle = MakeStyle(hiddenMemoryCell);
        return hiddenMemoryCellStyle;
    }

    public static GUIStyle OutputCell()
    {
        if (outputCell == null)
            outputCell = CellRender(new Color(1F, 0.4F, 0, 1F), false);
        if (outputCellStyle == null)
            outputCellStyle = MakeStyle(outputCell);
        return outputCellStyle;
    }
    public static GUIStyle OutputMemoryCell()
    {
        if (outputMemoryCell == null)
            outputMemoryCell = CellRender(new Color(1F, 0.4F, 0, 1F), true);
        if (outputMemoryCellStyle == null)
            outputMemoryCellStyle = MakeStyle(outputMemoryCell);
        return outputMemoryCellStyle;
    }

    private static Texture2D CellRender(Color color, bool circle)
    {
        Texture2D cell = new Texture2D(40, 40);
        int x = 0;
        while (x < cell.width)
        {
            int y = 0;
            while (y < cell.height)
            {
                float D = Vector2.Distance(new Vector2(x, y), new Vector2(19.5F, 19.5F));
                if (circle && D <= 17 && D >= 15)
                {
                    D = Mathf.Abs(16F - D);
                    cell.SetPixel(x, y, Color.Lerp(Color.black, color, D));
                }
                else if (D < 20.5F)
                {
                    if (D <= 19.5F)
                        cell.SetPixel(x, y, color);
                    else
                    {
                        D = 20.5F - D;
                        cell.SetPixel(x, y, color * D);
                    }
                }
                else
                    cell.SetPixel(x, y, Color.clear);

                y++;
            }
            x++;
        }
        cell.Apply();
        return cell;
    }

    private static GUIStyle MakeStyle(Texture2D t2d)
    {
        GUIStyle style = new GUIStyle();
        style.normal.background = t2d;
        style.normal.textColor = Color.black;
        style.alignment = TextAnchor.MiddleCenter;
        return style;
    }
}