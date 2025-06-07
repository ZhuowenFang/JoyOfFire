using UnityEngine;

public class CloudBehavior : MonoBehaviour
{
    public float detectRadius = 3f;       // ��Ӧ�뾶
    public float moveSpeed = 1f;          // ��Ʈ�ٶ�
    public float fadeSpeed = 1f;          // �Ƶ����ٶ�
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
            // ��ʼ�ƶ������� = �ӽ�ɫָ���Ƶķ���
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
                Destroy(gameObject); // ����ն����
        }
    }
}