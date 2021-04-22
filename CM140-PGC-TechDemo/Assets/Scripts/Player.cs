using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;

    void Update ()
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * moveSpeed * Time.deltaTime;
        transform.position += moveDir;
    }
}
