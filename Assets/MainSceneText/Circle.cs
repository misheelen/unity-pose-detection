using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private LineRenderer l_lineRenderer; // 円を描画するための LineRenderer
    [SerializeField] private LineRenderer r_lineRenderer; // 円を描画するための LineRenderer
    [SerializeField] private LineRenderer f_lineRenderer; // 円を描画するための LineRenderer
    [SerializeField] private LineRenderer m_lineRenderer; // 円を描画するための LineRenderer

    [SerializeField] private float m_radius = 0;    // 円の半径
    [SerializeField] private float m_lineWidth = 0;    // 円の線の太さ

    public int flag = 0;

    private void Reset()
    {
        // Debug.Log(PointsChangeText.instance.LHMarker.GetComponent<LineRenderer>());
    }

    private void Awake()
    {
        InitLineRenderer();
    }

    private void Update()
    {
        l_lineRenderer = PointsChangeText.instance.LHMarker.GetComponent<LineRenderer>();
        if (PointsChangeText.instance.LHColor == 1) {
            l_lineRenderer.startColor = Color.green;
            l_lineRenderer.endColor = Color.green;
        }
        else 
        {
            l_lineRenderer.startColor = Color.blue;
            l_lineRenderer.endColor = Color.blue;
        }

        r_lineRenderer = PointsChangeText.instance.RHMarker.GetComponent<LineRenderer>();
        if (PointsChangeText.instance.RHColor == 1) {
            r_lineRenderer.startColor = Color.green;
            r_lineRenderer.endColor = Color.green;
        }
        else 
        {
            r_lineRenderer.startColor = Color.blue;
            r_lineRenderer.endColor = Color.blue;
        }

        f_lineRenderer = PointsChangeText.instance.FMarker.GetComponent<LineRenderer>();
        if (PointsChangeText.instance.FColor == 1) {
            f_lineRenderer.startColor = Color.green;
            f_lineRenderer.endColor = Color.green;
        }
        else 
        {
            f_lineRenderer.startColor = Color.red;
            f_lineRenderer.endColor = Color.red;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            if (flag == 0)
            {
                m_lineRenderer.startColor = Color.red;
                m_lineRenderer.endColor = Color.red;
                flag = 1;
            }
            else if (flag == 1)
            {
                m_lineRenderer.startColor = Color.white;
                m_lineRenderer.endColor = Color.white;
                flag = 2;
            }
            else if (flag == 2)
            {
                m_lineRenderer.startColor = Color.blue;
                m_lineRenderer.endColor = Color.blue;
                flag = 3;
            }
            else if (flag == 3)
            {
                m_lineRenderer.startColor = Color.green;
                m_lineRenderer.endColor = Color.green;
                flag = 0;
            }
        }
    }

    private void InitLineRenderer()
    {
        if (PointsChangeText.instance != null)
        {
            if (PointsChangeText.instance.LHMarker != null)
            {
                l_lineRenderer = PointsChangeText.instance.LHMarker.GetComponent<LineRenderer>();
            }

            if (PointsChangeText.instance.RHMarker != null)
            {
                r_lineRenderer = PointsChangeText.instance.RHMarker.GetComponent<LineRenderer>();
            }

            if (PointsChangeText.instance.FMarker != null)
            {
                f_lineRenderer = PointsChangeText.instance.FMarker.GetComponent<LineRenderer>();
            }

            // i dont know what is m_line, so i assign the fmarker..
            if (PointsChangeText.instance.FMarker != null)
            {
                m_lineRenderer = PointsChangeText.instance.FMarker.GetComponent<LineRenderer>();
            }
        }

        if (l_lineRenderer != null)
        {
            l_lineRenderer.startColor = Color.blue;
            l_lineRenderer.endColor = Color.blue;
        }

        if (r_lineRenderer != null)
        {
            r_lineRenderer.startColor = Color.blue;
            r_lineRenderer.endColor = Color.blue;
        }

        if (f_lineRenderer != null)
        {
            f_lineRenderer.startColor = Color.blue;
            f_lineRenderer.endColor = Color.blue;
        }

        var segments = 360;
        if (m_lineRenderer != null)
        {
            m_lineRenderer.startWidth = m_lineWidth;
            m_lineRenderer.endWidth = m_lineWidth;
            m_lineRenderer.positionCount = segments;
            m_lineRenderer.loop = true;
            m_lineRenderer.useWorldSpace = false; // transform.localScale を適用するため

            m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_lineRenderer.startColor = Color.green;
            m_lineRenderer.endColor = Color.green;

            m_lineRenderer.SetPositions(GetVector3(segments));
        }
    }

    private Vector3[] GetVector3(int segments)
    {
        var points = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            var x = Mathf.Sin(rad) * m_radius;
            var y = Mathf.Cos(rad) * m_radius;

            points[i] = new Vector3(x, y, 0);
        }

        return points;
    }
}

