using UnityEngine;

public class SkyMove : MonoBehaviour
{
    Transform player;
    public float sensitive = 1f;
    //private float startingX = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //startingX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3((player.position.x*sensitive), transform.position.y, transform.position.z);
    }
}
