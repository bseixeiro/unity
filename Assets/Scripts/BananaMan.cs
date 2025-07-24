using UnityEngine;

public class BananaMan : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Mouvement();
        Rotation();
    }

    void Mouvement()
    {
        Vector3 mouv = new Vector3();
        float speed = 5f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            mouv.z += 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            mouv.z -= 1;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            mouv.x += 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            mouv.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            mouv.y += 100;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = 10f;



        transform.position += mouv * Time.deltaTime * speed;
    }

    void Rotation()
    {
        Vector3 rotation = new Vector3();
        if (Input.GetAxis("Mouse X") < 0)
        { 
            rotation.y += 1;
        }

        if (Input.GetAxis("Mouse X") > 0)
        {
            rotation.y -= 1;
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
