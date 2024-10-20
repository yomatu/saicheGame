using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float movespeed = 30;

    public float rotatespeed = 30f;

    public bool lamp;
    
    private WorldGenerator generator;

    private Car car;

    private Transform carTransform;

    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.FindObjectOfType<Car>();
        generator = GameObject.FindObjectOfType<WorldGenerator>();

        if (car!=null)
        {
            carTransform = car.gameObject.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * movespeed * Time.deltaTime);

        if (car!=null)
        {
            CheckRotation();
        }
    }

    void CheckRotation()
    {
        Vector3 direction = (lamp) ? Vector3.right : Vector3.forward;

        float carRotation = carTransform.localEulerAngles.y;

        if (carRotation > car.rotationAngle * 2f)
        {
            carRotation = (360 - carRotation) * -1f;
        }
        
        
        transform.Rotate( direction * - rotatespeed * (carRotation/(float) car.rotationAngle)*(36f/generator.dimensions.x)*Time.deltaTime);
        
    }
    
}