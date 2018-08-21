using UnityEngine;
using UnityEngine.UI;
using System.Text;

public static class SpriteExtension
{
    public static string vectorString(this Vector2[] uv)
    {
        var buffer = new StringBuilder();
        buffer.Append('[');
        for (var i = 0; i < uv.Length; i++)
        {
            var item = uv[i];
            buffer.Append(string.Format("[{0}]{{x={1:F3}, y={2:F3}}}", i, item.x, item.y));
            if (i < uv.Length - 1)
            {
                buffer.Append(',');
            }
        }
        buffer.Append(']');
        return buffer.ToString();
    }

    public static string triangleString(this ushort[] triangles)
    {
        var buffer = new StringBuilder();
        buffer.Append('[');
        for (var i = 0; i < triangles.Length; i += 3)
        {
            buffer.Append(string.Format("[{3}]{{{0},{1},{2}}}", triangles[i], triangles[i + 1], triangles[i + 2], i / 3));
            if (i < triangles.Length - 3)
            {
                buffer.Append(',');
            }
        }
        buffer.Append(']');
        return buffer.ToString();
    }
}

public class SpriteInspector : MonoBehaviour
{
    [Space]
    [SerializeField] public SpriteRenderer spriteRenderer;

    [Space]
    [SerializeField] public Image image;

    [Space]
    [SerializeField] public Color color = Color.green;

    private Sprite sprite;

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer != null)
        {
            sprite = spriteRenderer.sprite;
            if (sprite == null) { return; }

            for (var i = 0; i < sprite.triangles.Length; i += 3)
            {
                var v1 = transform.TransformPoint(sprite.vertices[sprite.triangles[i]]);
                var v2 = transform.TransformPoint(sprite.vertices[sprite.triangles[i + 1]]);
                var v3 = transform.TransformPoint(sprite.vertices[sprite.triangles[i + 2]]);

                Debug.DrawLine(v1, v2, color);
                Debug.DrawLine(v1, v3, color);
                Debug.DrawLine(v2, v3, color);
            }
        }
        else if (image != null)
        {
            sprite = image.sprite;
            if (sprite == null) { return; }

            // 图片实际渲染容器
            var tfm = image.rectTransform;
            //image.rectTransform.pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
            //image.SetNativeSize();

            var pixelsPerUnit = sprite.pixelsPerUnit;

            // 图片实际显示缩放比例
            var scale = new Vector2(tfm.rect.width / sprite.rect.width, tfm.rect.height / sprite.rect.height);
            // 考虑图片实际显示后缩放变形
            var pivot = Vector2.Scale(sprite.pivot, scale);
            // 考虑缩放因素后变形点归位偏移
            var offset = new Vector2(pivot.x - tfm.rect.width * tfm.pivot.x, pivot.y - tfm.rect.height * tfm.pivot.y);
            for (var i = 0; i < sprite.triangles.Length; i += 3)
            {
                // 标准单位转像素单位
                var v1 = sprite.vertices[sprite.triangles[i]] * pixelsPerUnit;
                var v2 = sprite.vertices[sprite.triangles[i + 1]] * pixelsPerUnit;
                var v3 = sprite.vertices[sprite.triangles[i + 2]] * pixelsPerUnit;
                // 缩放+变形点偏移+局部坐标转世界坐标
                v1 = tfm.TransformPoint(Vector2.Scale(v1, scale) + offset);
                v2 = tfm.TransformPoint(Vector2.Scale(v2, scale) + offset);
                v3 = tfm.TransformPoint(Vector2.Scale(v3, scale) + offset);
                // 绘制三角形
                Debug.DrawLine(v1, v2, color);
                Debug.DrawLine(v1, v3, color);
                Debug.DrawLine(v2, v3, color);
            }
        }
    }
}
