using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A/D 或 ←/→
        float v = Input.GetAxis("Vertical");   // W/S 或 ↑/↓

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
