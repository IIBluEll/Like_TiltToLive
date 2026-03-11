using UnityEngine;

namespace ProtoType.Enemy
{
    public enum PATTERN_TYPE
    {
        CIRCLE,
        CROSS,
    }

    public interface IEnemyPattern
    {
        Vector3[] CalculateOffsets(int count , float spacing);
    }

    public class CirclePattern : IEnemyPattern
    {
        public Vector3[] CalculateOffsets(int count , float spacing)
        {
            Vector3[] tOffsets = new Vector3[count];
            float tAngleStep = 360f / count;

            for ( int tIndex = 0; tIndex < count; tIndex++ )
            {
                float tAngle = tIndex * tAngleStep * Mathf.Deg2Rad;
                tOffsets[ tIndex ] = new Vector3(Mathf.Cos(tAngle) , Mathf.Sin(tAngle) , 0f) * spacing;
            }

            return tOffsets;
        }
    }

    public class CrossPattern : IEnemyPattern
    {
        public Vector3[] CalculateOffsets(int count , float spacing)
        {
            Vector3[] tOffsets = new Vector3[count];
            int tArmLength = count / 4; // 4방향으로 나눔

            for ( int tIndex = 0; tIndex < count; tIndex++ )
            {
                int tDirection = tIndex % 4; // 0:상, 1:하, 2:좌, 3:우
                int tStep = (tIndex / 4) + 1; // 중심으로부터의 거리

                switch ( tDirection )
                {
                    case 0: tOffsets[ tIndex ] = new Vector3(0 , tStep * spacing , 0); break;
                    case 1: tOffsets[ tIndex ] = new Vector3(0 , -tStep * spacing , 0); break;
                    case 2: tOffsets[ tIndex ] = new Vector3(-tStep * spacing , 0 , 0); break;
                    case 3: tOffsets[ tIndex ] = new Vector3(tStep * spacing , 0 , 0); break;
                }
            }
            return tOffsets;
        }
    }
}

