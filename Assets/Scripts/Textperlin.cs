using UnityEngine;

public class Textperlin : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private float _a=0.06f; //间隙平滑度

    public bool UsePerLin;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Vector3[] posArr = new Vector3[100];

        float ranX = Random.Range(1, 1000);
        
        float ranY = Random.Range(1, 1000); 
        
        for (int i = 0; i < posArr.Length; i++)
        {
            if (UsePerLin)
            {
                posArr[i] = new Vector3(i * 0.1f, Mathf.PerlinNoise(i*_a+ranX,i*+ranY), 0);
            }
            else
            {
                posArr[i] = new Vector3(i * 0.1f, Random.value, 0);

            }
        }

        _lineRenderer.SetPositions(posArr);
    }

    // Update is called once per frame
    void Update()
    {
    }
}