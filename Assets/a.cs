using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class a : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        if (Input.GetKey(KeyCode.DownArrow)) transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float vertical = Input.GetAxis("Vertical");     // W/S 或 上/下箭头


        Vector3 f = new(transform.forward.x, 0, transform.forward.z);
        Vector3 r = new(transform.right.x, 0, transform.right.z);
        Vector3 v = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            v -= new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            v += new Vector3(0, 1, 0);
        }

        Vector3 move = (vertical * f.normalized + horizontal * r.normalized + v) * Time.deltaTime;
        transform.position += move;
    }
}
