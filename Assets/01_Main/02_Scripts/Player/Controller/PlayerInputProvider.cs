using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IInputProvider
    {
        // 현재 플레이어의 위치를 받아, 이동해야 할 '목표 위치(World 좌표)'를 반환합니다.
        Vector2 GetTargetPosition(Vector2 currentPosition);

        // 센서의 영점을 현재 기기 상태로 맞춥니다.
        void Calibrate();
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

        public void Calibrate()
        {
            // 마우스 입력은 영점 조절이 불필요합니다.
        }
    }

    public class GyroInput : MonoBehaviour, IInputProvider
    {
        private Vector3 _baselineOffset;

        private void Start()
        {
            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
            }
        }

        public void Calibrate()
        {
            if (Accelerometer.current != null)
            {
                // 폰을 눕혀놓은 현재 상태를 기준점(영점)으로 기록합니다.
                _baselineOffset = Accelerometer.current.acceleration.ReadValue();
            }
        }

        public Vector2 GetTargetPosition(Vector2 currentPosition)
        {
            if (Accelerometer.current == null)
                return currentPosition;

            // 현재 가속도 값에서 게임 시작 시 저장한 영점(baseline) 값을 빼서 순수 변화량만 사용합니다.
            Vector3 tRawAcceleration = Accelerometer.current.acceleration.ReadValue();
            Vector3 tAdjustedAcceleration = tRawAcceleration - _baselineOffset;

            // 민감도 증폭 후 1로 클램핑
            Vector2 tDir = new Vector2(tAdjustedAcceleration.x, tAdjustedAcceleration.y);
            tDir = Vector2.ClampMagnitude(tDir * 2f, 1f);

            // 자이로 방식: 현재 위치에서 기울어진 방향으로 일정 거리만큼 앞선 곳을 목표 지점으로 반환
            // 이렇게 하면 Lerp를 통과할 때 자연스럽게 해당 방향으로 이동하게 됩니다.
            return currentPosition + tDir;
        }
    }
}