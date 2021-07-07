using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player player;
    private Vector2 origin;

    [SerializeField]
    private float ceiling;

    [SerializeField]
    private float end;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        origin = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player.transform.position.x > origin.x && player.transform.position.x < end)
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        else if (player.transform.position.x > origin.x && player.transform.position.x >= end)
            transform.position = new Vector3(end, transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(origin.x, transform.position.y, transform.position.z);

        if (player.transform.position.y > origin.y && player.transform.position.y < ceiling)
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        else if (player.transform.position.y >= ceiling)
            transform.position = new Vector3(transform.position.x, ceiling, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, origin.y, transform.position.z);
    }
}
