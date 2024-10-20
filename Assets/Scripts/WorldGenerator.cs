using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    public Material meshMaterial;

    public float scale;

    public float perlinScale;
    
    public float offset;

    public Vector2 dimensions;
    
    public float waveHeight;
    
    public float globalSpeed;
    
    public float randomness;

    private Vector3[] beginPoints;
    
    private GameObject[] pieces = new GameObject[2];

    public BasicMovement lampMovement;
    
    public  int startObstaceleChance;

    public int startTransitionLength;

    public int gateChance;

    
    public GameObject[] obstacles;

    public GameObject gate;


    private GameObject currentCyclinder;
    
    // Start is called before the first frame update
    void Start()
    {
        beginPoints = new Vector3[(int)dimensions.x + 1];
        
        //0-1-2-1-2循环过程

        for (int i = 0; i < 2; i++)
        {
            GenerateWorldPiece(i);
        }
        
        //CreateCylinder();
    }

    private void LateUpdate()
    {
        if (pieces[1]&& pieces[1].transform.position.z <=-15f)
        {
            StartCoroutine(UpdateWorldPieces());

        } 
    }
    
    IEnumerator UpdateWorldPieces()
    {
        //这个时候第一段已经过去了没用了所以删除掉
        Destroy(pieces[0]);
        
        //把当前的往前串一位
        pieces[0] = pieces[1];
        
        //第二个块再新生成一个
        pieces[1] = CreateCylinder();

        
        //重新设置位置
        pieces[1].transform.position =
            pieces[0].transform.position + Vector3.forward * (dimensions.y * scale * Mathf.PI);

        pieces[1].transform.rotation = pieces[0].transform.rotation;
        
        UpdateSinglePiece(pieces[1]);
        
        yield return 0;


    }

    void GenerateWorldPiece(int i)
    { 
        //生成圆柱体,保存进数组
        pieces[i] = CreateCylinder();
        //根据他的索引,去摆正圆柱体的位置
        pieces[i].transform.Translate(Vector3.forward*(dimensions.y*scale*Mathf.PI*i));
        
        //再写一个函数,用于标记尾部的位置,将来移动用
        UpdateSinglePiece(pieces[i]);
    }

    void UpdateSinglePiece(GameObject piece)
    {
                
        //增加移动
        BasicMovement movement = piece.AddComponent<BasicMovement>();

        movement.movespeed = -globalSpeed;
        
        //创建结束点,并且设置他的位置
        GameObject endPoint = new GameObject();

        endPoint.transform.position = piece.transform.position + Vector3.forward * (dimensions.y * scale * Mathf.PI);

        endPoint.transform.parent = piece.transform;
        endPoint.name = "End point";

        offset += randomness;


    }
    
    
    public GameObject CreateCylinder()
    {
        //Mesh通过网格绘制的
        //MeshFilter持有mesh的引用
        //MeshRenderer
        
        //创建game object并且命名
        GameObject newCylinder = new GameObject();

        newCylinder.name = "World piece";

        currentCyclinder = newCylinder;
        
        //添加网格MeshFilter 和MeshRenderer组件

        MeshFilter meshFilter = newCylinder.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = newCylinder.AddComponent<MeshRenderer>();
        
        
        //什么是材质,包含纹理 ,shader
        meshRenderer.material = meshMaterial;
        meshFilter.mesh = Generate();
        
        //创建网格以后,添加碰撞,适配新的mesh
        newCylinder.AddComponent<MeshCollider>();

        return newCylinder;
    }

    Mesh Generate()
    {
        //创建一个mesh
        Mesh mesh = new Mesh();
        mesh.name = "MESH";
        
        //需要uv .顶点 ,三角形等数据

        Vector3[] vertices = null;
        Vector2[] uvs = null;
        int[] triangles = null;
        
        //创建形状
        CreateShape(ref vertices, ref uvs, ref triangles);

        //再去赋值
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        
        return mesh;

    }

    
    //创建形状
    void CreateShape(ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles)
    {
        //向z轴里面延时.x是横截面,

        int xCount = (int)dimensions.x ;
        int zCount =(int)dimensions.y;
        
        //初始化节点和uv数组.通过定义的尺寸
        vertices = new Vector3[(xCount + 1) * (zCount + 1)];

        uvs = new Vector2[(xCount + 1) * (zCount + 1)];

        int index = 0;
        
        //半径计算
        float radius = xCount * scale * 0.5f;

        //通过一个双循环,设置顶点和uv
        for (int x = 0; x <= xCount; x++)
        {
            for (int z = 0; z <= zCount; z++)
            {
                //首先获取圆柱体的角度,根据x的位置
                float angle = x * Mathf.PI * 2f / xCount;
                
                //通过角度计算了顶点的值
                vertices[index] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius,
                    z * scale * Mathf.PI);
                
                //接下来可以计算出uv的值
                uvs[index] = new Vector2(x * scale, z * scale);
                
                //现在我们可以用之前的柏林噪声了

                float pX = (vertices[index].x*perlinScale) + offset;
                float pZ = (vertices[index].z*perlinScale) + offset;

                
                //需要一个中心点和当前的顶点做剑法然后归一化,再去计算柏林噪声
                Vector3 center = new Vector3(0, 0, vertices[index].z);

                vertices[index] += (center - vertices[index]).normalized * Mathf.PerlinNoise(pX, pZ) * waveHeight;

                if (z< startTransitionLength && beginPoints[0]!=Vector3.zero)
                {
                    float perlinPercentage = z * (1f / startTransitionLength);
                    Vector3 beginPoint = new Vector3(beginPoints[x].x,beginPoints[x].y,vertices[index].z);
                    
                    
                    vertices[index] = (perlinPercentage * vertices[index]) + ((1 - perlinPercentage) * beginPoint);
                }
                else if(z == zCount)
                {
                    beginPoints[x] = vertices[index];
                }

                if (Random.Range(0,startObstaceleChance)== 0 && !(gate == null && obstacles.Length == 0))
                {
                    CreateItem(vertices[index], x);
                }
                
                index++;

            }
            
        }
        
        //初始化三角形数组,x乘z这样一个总数,1个矩形2个三角形,1个三角形3个顶点那么一个正方形就是6个顶点
        triangles = new int [xCount * zCount * 6];

        
        //创建一个数值,存6个三角形顶点,方便调用
        int[] boxBase = new int[6];
        int current = 0;
        
        
        for (int x = 0; x < xCount; x++)
        {
                //每次重新赋值,根据x的变化
                
                boxBase = new int[]{ 
                    x * (zCount + 1), 
                    x * (zCount + 1) + 1,
                    (x + 1) * (zCount + 1),
                    x * (zCount + 1) + 1,
                    (x + 1) * (zCount + 1) + 1,
                    (x + 1) * (zCount + 1),
                };
            for (int z = 0; z < zCount; z++)
            {
                //增长boxBase的索引,方便计算下一个正方形
                for (int i = 0; i < 6; i++)
                {
                    boxBase[i] = boxBase[i] + 1;
                }

                
                //把这6个顶点填充到具体的三角形去
                
                for (int j = 0; j < 6; j++)
                {
                    triangles[current + j] = boxBase[j] - 1;
                }

            //current = current + 6;
                current += 6;
            }
            
        }

    }

    void CreateItem(Vector3 vert, int x)
    {
        Vector3 zCenter = new Vector3(0,0,vert.z);
        

        GameObject newItem = 
            Instantiate(Random.Range(0,
            gateChance) == 0 ? gate : obstacles[Random.Range(0, obstacles.Length)]);

        newItem.transform.rotation = Quaternion.LookRotation(zCenter - vert, Vector3.up);

        newItem.transform.position = vert;
        
        
        
        newItem.transform.SetParent(currentCyclinder.transform,false);
        
        

    }
    
    public Transform GetWorldPiece()
    {
        return pieces[0].transform;
    }

}
