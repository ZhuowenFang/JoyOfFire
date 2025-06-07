using UnityEngine;

public class CloudBehavior : MonoBehaviour
{
    public float detectRadius = 3f;       // 感应半径
    public float moveSpeed = 1f;          // 云飘速度
    public float fadeSpeed = 1f;          // 云淡出速度
    public Transform player;

    private bool isMoving = false;
    private Vector3 moveDirection;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (!isMoving && dist < detectRadius)
        {
            // 开始移动，方向 = 从角色指向云的方向
            moveDirection = (transform.position - player.position).normalized;
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            Color c = sr.color;
            c.a = Mathf.MoveTowards(c.a, 0, fadeSpeed * Time.deltaTime);
            sr.color = c;

            if (c.a <= 0f)
                Destroy(gameObject); // 或回收对象池
        }
    }
}