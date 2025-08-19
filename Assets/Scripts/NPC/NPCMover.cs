using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public float moveSpeed;         // 移动速度
    public float range;             // 活动范围半径
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // 如果快到目标点了，就换一个新目标
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);
        targetPos = startPos + new Vector3(x, 0, z);
    }
}
