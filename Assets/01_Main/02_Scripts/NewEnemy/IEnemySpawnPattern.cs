using HM.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace HM.NewEnemy
{
    public interface IEnemySpawnPattern
    {
        void CalculatePosition(int count , Vector3 center , float spacing , List<Vector3> results);
    }

    public class CirclePattern : IEnemySpawnPattern
    {
        public void CalculatePosition(int count , Vector3 center , float spacing , List<Vector3> results)
        {
            results.Clear();

            float tRadius = spacing * 1f;
            float tAngleStep = 360f/count;

            for(int i = 0; i < count; i++ )
            {
                float tAngle = i * tAngleStep * Mathf.Deg2Rad;
                float tX = center.x + Mathf.Cos(tAngle) * tRadius;
                float tY = center.y + Mathf.Sin(tAngle) * tRadius;

                tX = Mathf.Clamp(tX , ScreenBoundary.PlayableArea.xMin , ScreenBoundary.PlayableArea.xMax);
                tY = Mathf.Clamp(tY , ScreenBoundary.PlayableArea.yMin , ScreenBoundary.PlayableArea.yMax);

                results.Add(new Vector3(tX , tY , 0f));
            }
        }
    }
    public class HorizontalLinePattern : IEnemySpawnPattern
    {
        public void CalculatePosition(int count , Vector3 center , float spacing , List<Vector3> results)
        {
            results.Clear();
            float tHalfLength = (count - 1) * spacing * 0.5f;

            for ( int i = 0; i < count; i++ )
            {
                float tOffsetX = (i * spacing) - tHalfLength;
                float tFinalX = Mathf.Clamp(center.x + tOffsetX, ScreenBoundary.PlayableArea.xMin, ScreenBoundary.PlayableArea.xMax);
                results.Add(new Vector3(tFinalX , center.y , 0f));
            }
        }
    }

    public class VerticalLinePattern : IEnemySpawnPattern
    {
        public void CalculatePosition(int count , Vector3 center , float spacing , List<Vector3> results)
        {
            results.Clear();
            float tHalfLength = (count - 1) * spacing * 0.5f;

            for ( int i = 0; i < count; i++ )
            {
                float tOffsetY = (i * spacing) - tHalfLength;
                float tFinalY = Mathf.Clamp(center.y + tOffsetY, ScreenBoundary.PlayableArea.yMin, ScreenBoundary.PlayableArea.yMax);
                results.Add(new Vector3(center.x , tFinalY , 0f));
            }
        }
    }

    public class RandomPattern : IEnemySpawnPattern
    {
        public void CalculatePosition(int count , Vector3 center , float spacing , List<Vector3> results)
        {
            results.Clear();

            Rect tBounds = ScreenBoundary.PlayableArea;

            for ( int i = 0; i < count; i++ )
            {
                // 경계 영역 내부의 무작위 X, Y 좌표 추출
                float tRandomX = Random.Range(tBounds.xMin, tBounds.xMax);
                float tRandomY = Random.Range(tBounds.yMin, tBounds.yMax);

                results.Add(new Vector3(tRandomX , tRandomY , 0f));
            }
        }
    }
}

