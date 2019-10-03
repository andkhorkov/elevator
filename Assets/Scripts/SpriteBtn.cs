using UnityEngine;

public interface IClickable
{
    void OnClick();
}

public class SpriteBtn : MonoBehaviour
{
    private Collider2D col;
    private IClickable context;
    private SpriteRenderer sprRenderer;

    public SpriteRenderer SprRenderer => sprRenderer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        context = GetComponent<IClickable>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector3 clickPos = Vector3.zero;
        bool isClicked = false;

        if (Input.GetMouseButtonDown(0))
        {
            clickPos = Input.mousePosition;
            isClicked = true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            clickPos = Input.GetTouch(0).position;
            isClicked = true;
        }

        if (isClicked)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(clickPos);
            var worldClickPos = new Vector2(worldPos.x, worldPos.y);

            if (col == Physics2D.OverlapPoint(worldClickPos))
            {
                context.OnClick();
            }
        }
    }
}
