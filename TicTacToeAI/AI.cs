using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeAI
{
    #region Interfaces
    public interface INeuronSignal
    {
        double Output { get; set; }
    }
    public interface INeuronNet
    {
        INeuronLayer InputLayer { get; set; }
        INeuronLayer HiddenLayer { get; set; }
        INeuronLayer OutPut { get; set; }

        void Pulse(INeuronNet net);
        void ApplyLearning(INeuronNet net);
    }
    public interface INeuronLayer : IList<INeuron>
    {
        void Pulse(INeuronNet net);
        void ApplyLearning(INeuronNet net);
    }
    public interface INeuron : INeuronReceptor, INeuronSignal
    {
        void Pulse(INeuronLayer layer);
        void ApplyLearning(INeuronLayer layer);
        NeuronFactor Bias { get; set; }
        double BiasWeight { get; set; }
        double Error { get; set; }
    }
    public interface INeuronReceptor
    {
        Dictionary<INeuronSignal,NeuronFactor> Input { get; }
    }
    #endregion
    #region Classes
    public class Neuron : INeuron
    {
        private NeuronFactor m_bias;
        private double m_biasWeight;
        private double m_error;
        private Dictionary<INeuronSignal, NeuronFactor> m_input;
        private double m_output;

        public NeuronFactor Bias
        {
            get { return m_bias; }
            set { m_bias = value; }
        }
        public double BiasWeight
        {
            get { return m_biasWeight; }
            set { m_biasWeight = value; }
        }
        public double Error
        {
            get { return m_error; }
            set { m_error = value; }
        }
        public Dictionary<INeuronSignal, NeuronFactor> Input
        {
            get { return m_input; }
        }
        public double Output
        {
            get { return m_output; }
            set { m_output = value; }
        }
        public void Pulse(INeuronLayer layer)
        {
            lock (layer)
            {
                m_output = 0;
                foreach (KeyValuePair<INeuronSignal, NeuronFactor> item in m_input)
                    m_output += item.Key.Output * item.Value.Weight;
                m_output += m_bias.Weight * BiasWeight;
                m_output = Sigmoid(m_output);
            }
        }
        public void ApplyLearning(INeuronLayer layer)
        {
            lock (layer)
            {
                m_output += m_biasWeight;
                m_biasWeight = 0;
            }
        }
        private double Sigmoid(double value)
        {
            return value / (1 + Math.Abs(value));
        }
    }
    public class NeuronFactor
    {
        private double m_weight;
        private double m_delta;
        public double Weight
        {
            get { return m_weight; }
            set { m_weight = value; }
        }
        public double Delta
        {
            get { return m_delta; }
            set { m_delta = value; }
        }
        public NeuronFactor(double Weight)
        {
            m_weight = Weight;
            m_delta = 0;
        }

        public void ApplyDelta()
        {
            m_weight += m_delta;
            m_delta = 0;
        }
    }
    #endregion
    public class AI
    {
        protected int[,] Field { get; private set; }
        public AI(int[,] field)
        {
            Field = field;
        }

        static AI()
        {
            Console.WriteLine("AI was been created");
        }

        public override string ToString()
        {
            return "Information:\n Version: v0.0.2";
        }
    }
}
