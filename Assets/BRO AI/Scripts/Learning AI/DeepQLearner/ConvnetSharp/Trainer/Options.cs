using System;

namespace ConvnetSharp
{
    [Serializable]
    public class Options
    {
        public string method = "sgd";
        public int batchSize = 1;

        public double learningRate = 0.001;
        public double l1_decay = 0;
        public double l2_decay = 0;
        public double momentum = 0.1;
        public double beta1 = 0.9;
        public double beta2 = 0.999;
        public double ro = 0.95;
        public double eps = 1e-8;
    }
}
