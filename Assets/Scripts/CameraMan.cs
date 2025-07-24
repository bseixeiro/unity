using UnityEngine;

public class CameraMan : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = new Vector3();
        if (Input.GetAxis("Mouse X") < 0)
        {
            rotation.x += 1;
        }

        if (Input.GetAxis("Mouse X") > 0)
        {
            rotation.x -= 1;
        }
        //if (Input.GetAxis("Mouse Y") < 0)
        //{
        //    rotation.y += 1;
        //}
        //if (Input.GetAxis("Mouse Y") > 0)
        //{
        //    rotation.y -= 1;
        //}
        transform.Rotate(rotation);
    }
}
