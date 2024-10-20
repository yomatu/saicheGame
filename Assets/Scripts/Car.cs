using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Rigidbody rb;

    public Transform[] wheelMeshes;

    public WheelCollider[] wheelColliders;

    public int rotateSpeed; //旋转速度

    public int rotationAngle;   //旋转角度

    public int wheelRotateSpeed;

    public Transform[] grassEffects; 

    public Transform[] skidMarkPivots;

    public float grassEffectOffset;

    public Transform back;
    public float constantBackForce;

    public GameObject skidMark;

    public float skidMarkSize; 

    public float skidMarkDelay;

    public float minRotationDifference;
    
    private int targetRotation;

    private WorldGenerator generator;
    
    private float lastRotation;
    

    private bool skidMarkRoutine;

    public GameObject ragdoll;


    
    // Start is called before the first frame update
    void Start()
    {
        generator = GameObject.FindObjectOfType<WorldGenerator>();
        StartCoroutine(SkidMark());
    }

    private void FixedUpdate()
    {
        //更新车轮痕迹和粒子特效
        UpdateEffects();
    }


    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            Quaternion quat;
            Vector3 pos;
            
            wheelColliders[i].GetWorldPose(out pos ,out quat);

            wheelMeshes[i].position = pos;
            
            wheelMeshes[i].Rotate(Vector3.right*Time.deltaTime*wheelRotateSpeed);
        }

        if (Input.GetMouseButton(0)|| Input.GetAxis("Horizontal")!=0)
        {
            UpdateTargetRotation();
            
            
        }
        else if (targetRotation !=0)
        {
            targetRotation = 0;
        }

        Vector3 rotation = new Vector3(transform.localEulerAngles.x, targetRotation, transform.localEulerAngles.z);


        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation), rotateSpeed * Time.deltaTime);
    }

    //更新车的角度
    void UpdateTargetRotation()
    {
        if (Input.GetAxis("Horizontal")==0)
        {
            //鼠标坐标在屏幕右边
            if (Input.mousePosition.x>Screen.width*0.5f)
            {
                //右转
                targetRotation = rotationAngle;
            }
            else
            {
                //左转
                targetRotation = -rotationAngle;

            }
        }
        else
        {
            //如果按住ad或者左右箭头
            targetRotation = (int)(rotationAngle * Input.GetAxis("Horizontal"));

        }
        
    }
  
   
    
    
    void UpdateEffects()
    {
    
        //轮子在地面上,就不再加上力
        bool addForce = true;
        
        //判断是不是在旋转
       bool rotated = Mathf.Abs(lastRotation - transform.localEulerAngles.y)>minRotationDifference;
       
        
        
        for (int i = 0; i < 2; i++)
        {
            Transform wheelMesh = wheelMeshes[i + 2];
    
            if (Physics.Raycast(wheelMesh.position,Vector3.down,grassEffectOffset*1.5f) )
            {
                
                //粒子如果没显示,让他显示出来
                if (!grassEffects[i].gameObject.activeSelf)
                
                    grassEffects[i].gameObject.SetActive(true);
                
                
                //更新粒子的位置高度还有痕迹的位置
    
                float effectHeight = wheelMesh.position.y - grassEffectOffset;
    
                //Vector3 targetPosition = new Vector3(grassEffects[i].position.x, effectHeight, wheelMesh.position.z);
                Vector3 targetPosition = wheelMesh.position - new Vector3(0, grassEffectOffset, 0);
                grassEffects[i].position = targetPosition;
                skidMarkPivots[i].position = targetPosition;
                addForce = false;
    
    
            }
            else if (grassEffects[i].gameObject.activeSelf)
            {
                grassEffects[i].gameObject.SetActive(false);
            }
            
        }
    
        //施加一个向下的力
        if (addForce)
        {
            rb.AddForceAtPosition(back.position,Vector3.down*constantBackForce);
        }
        else
        {
            if (targetRotation !=0)
            {
               if (rotated&&!skidMarkRoutine)
               {
                   skidMarkRoutine = true;
               }
               else if(!rotated&& skidMarkRoutine)
               {
                   skidMarkRoutine = false;
               }
            }
            else
            {
                //直走不需要
                skidMarkRoutine = false;
            }
        }
    
        lastRotation = transform.localEulerAngles.y;
    }


    public void FallApart()
    {
        Instantiate(ragdoll,transform.position,transform.rotation);
        gameObject.SetActive(false);
    }
    
    IEnumerator SkidMark()
    {
        while (true)
        {
            yield return new WaitForSeconds(skidMarkDelay);

            if (skidMarkRoutine)
            {
                
                
                for (int i = 0; i < skidMarkPivots.Length; i++)
                {
                    GameObject newskidMark = Instantiate(skidMark,skidMarkPivots[i].position,skidMarkPivots[i].rotation);

                    newskidMark.transform.parent = generator.GetWorldPiece();
                    newskidMark.transform.localScale = new Vector3(1, 1, 4)*skidMarkSize;

                }
            }
            
        }
    }
}
