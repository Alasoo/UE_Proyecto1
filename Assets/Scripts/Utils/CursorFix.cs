using UnityEngine;

public class CursorFix : MonoBehaviour
{
    public Texture2D cursorNormal;
    public Texture2D cursorAltaDensidad;

    void Start()
    {
        if (Screen.dpi > 120)
        {
            Cursor.SetCursor(cursorAltaDensidad, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
        }
    }
}
