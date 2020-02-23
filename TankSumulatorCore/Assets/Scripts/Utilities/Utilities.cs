using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public struct LineDrawer
    {
        private LineRenderer lineRenderer;
        private float lineSize;

        public LineDrawer(float lineSize)
        {
            GameObject lineObj = new GameObject("LineObj");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;

            // forCurvedLine
            points = 0;
            currentStartIndex = 0;
        }

        private void init(float lineSize = 0.5f)
        {
            if (lineRenderer == null)
            {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
                this.lineSize = lineSize;
            }
        }

        public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                init(lineSize);
            }

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;

            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        int points;
        int currentStartIndex;
        public void StartCurvedLine(int pointsAmount)
        {
            points = pointsAmount;
            currentStartIndex = 0;
        }

        public void DrawCurvedLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                init(lineSize);
            }

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;

            lineRenderer.positionCount = points;

            lineRenderer.SetPosition(currentStartIndex, start);
            currentStartIndex++;
            lineRenderer.SetPosition(currentStartIndex, end);

        }
        public void Destroy()
        {
            if (lineRenderer != null)
            {
                UnityEngine.Object.Destroy(lineRenderer.gameObject);
            }
        }
    }
}
