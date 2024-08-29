using System;

namespace FlyffUAutoFSPro._Script
{
    public class RandomService
    {
        private static RandomService instance = null;
        Random _random = new Random();

        private RandomService()
        {
        }

        public static RandomService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RandomService();
                }
                return instance;
            }
        }

        public int GetRandom(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
