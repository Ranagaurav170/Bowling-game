using UnityEngine;

public class BounceMarker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Pitch Limits")]
    public float minX = -4f;
    public float maxX = 4f;
    public float minZ = 2f;
    public float maxZ = 16f;

    [Header("Fixed Height")]
    public float fixedY = 0.05f;

    private void Update()
    {
        MoveMarker();
    }

    private void MoveMarker()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A))
            x = -1f;
        else if (Input.GetKey(KeyCode.D))
            x = 1f;

        if (Input.GetKey(KeyCode.W))
            z = 1f;
        else if (Input.GetKey(KeyCode.S))
            z = -1f;

        Vector3 movement = new Vector3(x, 0f, z).normalized;

        transform.position += movement * moveSpeed * Time.deltaTime;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.y = fixedY;

        transform.position = pos;
    }

    public Vector3 GetBouncePoint()
    {
        return transform.position;
    }
}