using System;

namespace Application
{
    public class GameTime
    {
        private float _deltaTime = 0f;
        private float _timeScale = 1f;

        private float TimeScale
        {
            get { return _timeScale; }
            set { _timeScale = value; }
        }

        public float DeltaTime
        {
            get { return _deltaTime * _timeScale; }
            set { _deltaTime = value; }
        }

        public float DeltaTimeUnscaled
        {
            get { return _deltaTime; }
        }

        public float TotalTimeElapsed
        { get; private set; }

        public void Update(float deltaTime, float totalTimeElapsed)
        {
            _deltaTime = deltaTime;
            TotalTimeElapsed = totalTimeElapsed;
        }
    }
}
