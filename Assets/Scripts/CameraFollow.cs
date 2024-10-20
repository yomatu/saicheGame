using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform camTarget;

    public float distance = 6f;

    public float height = 5f;
     
    public float heightDanping = 0.5f;

    public float rotationDanping = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SkidMark());
    }



    private void LateUpdate()
    {
        if (camTarget == null)
        {
            return;
        }
        
        //取一些值,将要旋转和定位的值
        float wantedRotationAngle = camTarget.eulerAngles.y;

        float wantedHeight = camTarget.position.y + height;
        
        float currentRotationAngle = transform.eulerAngles.y;

        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle,wantedRotationAngle,rotationDanping*Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight,wantedHeight,rotationDanping*Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        
        
        //第一步把摄像机位置移动到观察者位置
        transform.position = camTarget.position;
        
        //第二步在被观察者的基础上,往后偏移
        transform.position -= currentRotation * Vector3.forward * distance;
        //重置一下相机的高度
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        
        
        //摄像机看向被观察物体
        transform.LookAt(camTarget);
    }
    
     IEnumerator SkidMark()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
        }
    }

    
}
