using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcRendererMP : MonoBehaviour
{
    public GameObject arrowPrefab;//A arrow head em si
    public GameObject dotPrefab;//O ponto em si
    public int poolSize = 50;//O tanto de pontos que da pra criar
    private List<GameObject> dotPool = new List<GameObject>();//A criação dos pontos em si
    private GameObject arrowInstance;//Referencia para usar a arrow head

    public float spacing = 50f;//O espaço entre os pontos
    public float arrowHeadAdjustment = 0;//Correção angular da arrow head
    public int dotsToSkip = 1;//Quantos pontos deve pula pra colocar a arrow head
    public Vector3 arrowDirection;//Guarda a posição que a arrow head deve apontar
    
    // Start is called before the first frame update
    void Start()
    {
        arrowInstance = Instantiate(arrowPrefab, transform);
        arrowInstance.transform.localPosition = Vector3.zero;
        InitializeDotPool(poolSize);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 0;

        Vector3 startPos = transform.position;
        Vector3 midPos = CalculateMidPoint(startPos, mousePos);

        UpdateArc(startPos, midPos, mousePos);
        PositionAndRotateArrow(mousePos);
    }

    void PositionAndRotateArrow(Vector3 mousePos)
    {
        arrowInstance.transform.position = mousePos + new Vector3(0, 3, 0);
        Vector3 direction = arrowDirection - mousePos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = angle + arrowHeadAdjustment;
        arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void UpdateArc(Vector3 startPos, Vector3 midPos, Vector3 endPos)
    {
        int numDots = Mathf.CeilToInt(Vector3.Distance(startPos,endPos)/spacing);

        for (int i=0; i < numDots && i < dotPool.Count; i++)
        {
            float t = i/ (float)numDots;
            t = Mathf.Clamp(t, 0f, 1f);//Prende o valor de t entre 0 e 1

            Vector3 position = QuadraticBezierPoint(startPos, midPos, endPos, t);

            if(i != numDots - dotsToSkip)
            {
                dotPool[i].transform.position = position;
                dotPool[i].SetActive(true);
            }
            if (i == numDots - (dotsToSkip+1) && i - dotsToSkip + 1 >= 0)
            {
                arrowDirection = dotPool[i].transform.position;
            }

        }

        for (int i= numDots - dotsToSkip; i < dotPool.Count; i++)
        {
            if(i > 0)
            {
                dotPool[i].SetActive(false);
            }
        }
    }

    Vector3 QuadraticBezierPoint(Vector3 startPos, Vector3 midPos, Vector3 endPos, float t)
    {
        float u = 1 - t;
        float tSqr = t * t;
        float uSqr = u * u;

        Vector3 point = uSqr * startPos;
        point = point + 2 * u * t * midPos;
        point = point + tSqr * endPos;
        
        return point;
    }

    Vector3 CalculateMidPoint(Vector3 startPos, Vector3 endPos)
    {
        Vector3 midPoint = (startPos + endPos) / 2;
        float arcHeight = Vector3.Distance(startPos, endPos) / 3f;
        midPoint.y = midPoint.y + arcHeight;
        return midPoint;
    }

    void InitializeDotPool(int counter)
    {
        for(int i=0; i < counter; i++)
        {
            GameObject dot = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, transform);
            dot.SetActive(false);
            dotPool.Add(dot);
        }
    }
}
