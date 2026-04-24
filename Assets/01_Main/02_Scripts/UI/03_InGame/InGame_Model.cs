using UnityEngine;

namespace HM.UI.InGame
{
    /// <summary>
    /// 인게임 진행 중의 생존 시간 등 순수한 데이터 상태를 저장하고 관리하는 모델
    /// </summary>
    public class InGame_Model
    {
        public float SurviveTime { get; private set; }

        public void ResetData()
        {
            SurviveTime = 0f;
        }

        public void AddTime(float deltaTime)
        {
            SurviveTime += deltaTime;
        }
    }
}

