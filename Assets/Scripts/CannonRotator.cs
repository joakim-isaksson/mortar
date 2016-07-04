using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour
{
    float angle;
    bool colliding;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (colliding) transform.Rotate(new Vector3(0, 0, Time.deltaTime * 90));
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.tag);
        if (other.tag == "Controller") colliding = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.tag);
        if (other.tag == "Controller") colliding = true;
    }
}
