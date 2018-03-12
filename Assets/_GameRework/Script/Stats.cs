using System;

namespace _Game.ScriptRework {
    [System.Serializable]
    public struct Stats{
        public int atk;
        public int hp;
        public int def;
        public int awareness;
        public int speed;

        public const int NumberOfAttributes = 5;

        public int this[int key]
        {
            get
            {
                switch (key){ // quick and dirty to not break current implementations
                    case 0: return atk;
                    case 1: return hp;
                    case 2: return def;
                    case 3: return awareness;
                    case 4: return speed;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}