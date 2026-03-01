using UnityEngine;

public class PreviewSpriteAttribute : PropertyAttribute
{
    public float size;

    public PreviewSpriteAttribute(float size = 64f)
    {
        this.size = size;
    }
}