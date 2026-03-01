using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
public class PreviewSpriteDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = attribute as PreviewSpriteAttribute;
        return base.GetPropertyHeight(property, label) + attr.size + 5f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as PreviewSpriteAttribute;

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, label);

        if (property.objectReferenceValue is Sprite sprite)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(sprite);

            if (texture)
            {
                Rect previewRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2f, attr.size, attr.size);
                GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit);
            }
        }
    }
}