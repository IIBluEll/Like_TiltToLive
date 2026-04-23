using UnityEngine;

namespace HM.UI.InGame
{
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

