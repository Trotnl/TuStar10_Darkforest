using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform targetPlayer;
    public GameObject box;
    private float size;

    // Start is called before the first frame update
    void Start()
    {
        size = transform.GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer)
        {
            transform.position = targetPlayer.localPosition + new Vector3(0, 0, -10);
        }
        if (box)
        {
            Vector2 boxSize = box.GetComponent<BoxCollider2D>().size;
            Vector2 boxOffset = box.GetComponent<BoxCollider2D>().offset;
            Vector2 min = new Vector2(box.transform.position.x, box.transform.position.y) + boxOffset - boxSize * 0.5f;
            Vector2 max = new Vector2(box.transform.position.x, box.transform.position.y) + boxOffset + boxSize * 0.5f;

            Vector3 cameraP = transform.position;

            if (cameraP.x < min.x + 2 * size)
            {
                cameraP.x = min.x + 2 * size;
            }
            else if (cameraP.x > max.x - 2 * size)
            {
                cameraP.x = max.x - 2 * size;
            }

            if (cameraP.y < min.y + size)
            {
                cameraP.y = min.y + size;
            }
            else if (cameraP.y > max.y - size)
            {
                cameraP.y = max.y - size;
            }

            transform.position = cameraP;
        }
    }
}
