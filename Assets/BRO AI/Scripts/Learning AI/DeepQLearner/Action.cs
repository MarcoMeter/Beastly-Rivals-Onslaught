using System;

namespace DeepQLearning
{
    [Serializable]
    public struct Action
    {
        public int action;
        public double value;
    };
}
