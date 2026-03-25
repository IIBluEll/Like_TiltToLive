using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IInputProvider
    {
        // 현재 플레이어의 위치를 받아, 이동해야 할 '목표 위치(World 좌표)'를 반환합니다.
        Vector2 GetTargetPosition(Vector2 currentPosition);
    }

    public class MouseInput : MonoBehaviour, IInputProvider
    {
        private Camera _mainCam;

        private void Awake()
        {
            _mainCam = Camera.main;
        }

        public Vector2 GetTargetPosition(Vector2 currentPosition)
        {
            Vector3 tMouseScreenPos = Mouse.current.position.ReadValue();
            return _mainCam.ScreenToWorldPoint(tMouseScreenPos);
        }
    }

    public class GyroInput : MonoBehaviour, IInputProvider
    {
        private void Start()
        {
            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
            }
        }

        public Vector2 GetTargetPosition(Vector2 currentPosition)
        {
            if (Accelerometer.current == null)
                return currentPosition;

            // x -> 좌우 기울기 , y -> 위아래 기울기
            Vector3 tAcceleration = Accelerometer.current.acceleration.ReadValue();

            // 민감도 증폭 후 1로 클램핑
            Vector2 tDir = new Vector2(tAcceleration.x, tAcceleration.y);
            tDir = Vector2.ClampMagnitude(tDir * 2f, 1f);

            // 자이로 방식: 현재 위치에서 기울어진 방향으로 일정 거리만큼 앞선 곳을 목표 지점으로 반환
            // 이렇게 하면 Lerp를 통과할 때 자연스럽게 해당 방향으로 이동하게 됩니다.
            return currentPosition + tDir;
        }
    }
}