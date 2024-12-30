using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToScreenFollow : MonoBehaviour
{
    public float offset;
    Camera cam;
    RectTransform rectTransform;
    public bool follow;
    
    
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
        //offset = 50;
     
    }

    public Vector3 pos_offset;
    public Transform target;

    private void Update()
    {
        //if (Time.frameCount % 15 == 0)
        //{
        if (follow)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward) * Quaternion.AngleAxis(offset, Vector3.right);
            if (follow && target)
            {
                transform.position = target.position + pos_offset;
            }    
        }
        
        //}
    }
}