using System;
using System.Collections;
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
    public class NeuronNet : INeuronNet
    {
        private INeuronLayer m_inputLayer;
        private INeuronLayer m_hiddenLayer;
        private INeuronLayer m_outputLayer;
        private double m_learningRate;

        public INeuronLayer InputLayer { get => m_inputLayer; set => m_inputLayer = value; }
        public INeuronLayer HiddenLayer { get => m_hiddenLayer; set => m_hiddenLayer = value; }
        public INeuronLayer OutPut { get => m_outputLayer; set => m_outputLayer = value; }

        public void Initialize(int randomSeed, int inputNeuronCount, int hiddenNeuronCount, int outputNeuronCount)
        {
            int i;
            Random rnd = new Random(randomSeed);
            m_inputLayer = new NeuronLayer();
            m_hiddenLayer = new NeuronLayer();
            m_outputLayer = new NeuronLayer();

            for (i = 0; i < inputNeuronCount; i++)
                m_inputLayer.Add(new Neuron());
            for (i = 0; i < hiddenNeuronCount; i++)
                m_hiddenLayer.Add(new Neuron());
            for (i = 0; i < outputNeuronCount; i++)
                m_outputLayer.Add(new Neuron());

            for (i = 0; i < m_hiddenLayer.Count; i++)
                for (int j = 0; j < m_inputLayer.Count; i++)
                    m_hiddenLayer[i].Input.Add(m_inputLayer[j], new NeuronFactor(rnd.NextDouble()));
            for (i = 0; i < m_outputLayer.Count; i++)
                for (int j = 0; j < m_hiddenLayer.Count; j++)
                    m_outputLayer[i].Input.Add(m_hiddenLayer[j], new NeuronFactor(rnd.NextDouble()));
        }
        public void Train(double[] input, double[] desiredResults)
        {
            if (input.Length != m_inputLayer.Count)
                throw new ArgumentException($"В этой сети {m_inputLayer.Count} точек входа");
            for (int i = 0; i < m_inputLayer.Count; i++)
            {
                Neuron n = m_inputLayer[i] as Neuron;
                if (null != n)
                    n.Output = input[i];
            }
            Pulse(this);
            BackPropagation(desiredResults);
        }
        public void Train(double[][] inputs, double[][] expected)
        {
            for (int i = 0; i < inputs.Length; i++)
                Train(inputs[i], expected[i]);
        }
        private void BackPropagation(double[] desiredResults)
        {
            int i;
            double temp, error;
            INeuron outputNode, inputNode, hiddenNode, node, node2;

            for (i = 0; i < m_outputLayer.Count; i++)
            {
                temp = m_outputLayer[i].Output;
                m_outputLayer[i].Error = (desiredResults[i] - temp) * temp * (1.0F - temp);
            }

            for (i = 0; i < m_hiddenLayer.Count; i++)
            {
                node = m_hiddenLayer[i];
                error = 0;
                for (int j = 0; j < m_outputLayer.Count; j++)
                {
                    outputNode = m_outputLayer[j];
                    error += outputNode.Error * outputNode.Input[node].Weight * node.Output * (1.0 - node.Output);
                }
                node.Error = error;
            }

            for (i = 0; i < m_hiddenLayer.Count; i++)
            {
                node = m_hiddenLayer[i];
                for (int j = 0; j < m_outputLayer.Count; j++)
                {
                    outputNode = m_outputLayer[j];
                    outputNode.Input[node].Weight += m_learningRate * m_outputLayer[j].Error * node.Output;
                    outputNode.Bias.Delta += m_learningRate * m_outputLayer[j].Error * outputNode.Bias.Weight;
                }
            }

            for (i = 0; i < m_inputLayer.Count; i++)
            {
                inputNode = m_inputLayer[i];
                for (int j = 0; j < m_hiddenLayer.Count; j++)
                {
                    hiddenNode = m_hiddenLayer[j];
                    hiddenNode.Input[inputNode].Weight += m_learningRate * hiddenNode.Error * inputNode.Output;
                    hiddenNode.Bias.Delta += m_learningRate * hiddenNode.Error * inputNode.Bias.Weight;
                }
            }
        }
        public void ApplyLearning(INeuronNet net)
        {
            lock (this)
            {
                m_hiddenLayer.ApplyLearning(this);
                m_outputLayer.ApplyLearning(this);
            }
        }

        public void Pulse(INeuronNet net)
        {
            lock (this)
            {
                m_hiddenLayer.Pulse(this);
                m_outputLayer.Pulse(this);
            }
        }
    }
    public class NeuronLayer : INeuronLayer
    {
        private List<INeuron> m_neurons;

        public INeuron this[int index] { get => this[index]; set => this[index] = value; }
        public int Count { get; set; }
        public bool IsReadOnly { get; set; }

        public void Add(INeuron item)
        {
            if (IsReadOnly) return;
            m_neurons.Add(item);
            Count = m_neurons.Count;
        }

        public void ApplyLearning(INeuronNet net)
        {
            foreach (INeuron n in m_neurons)
                n.ApplyLearning(this);
        }

        public void Clear()
        {
            if (IsReadOnly) return;
            m_neurons.Clear();
            Count = m_neurons.Count;
        }

        public bool Contains(INeuron item)
        {
            return m_neurons.Contains(item);
        }

        public void CopyTo(INeuron[] array, int arrayIndex)
        {
            if (IsReadOnly) return;
            m_neurons.CopyTo(array, arrayIndex);
            Count = m_neurons.Count;
        }

        public IEnumerator<INeuron> GetEnumerator()
        {
            return m_neurons.GetEnumerator();
        }

        public int IndexOf(INeuron item)
        {
            return m_neurons.IndexOf(item);
        }

        public void Insert(int index, INeuron item)
        {
            if (IsReadOnly) return;
            m_neurons.Insert(index, item);
            Count = m_neurons.Count;
        }

        public void Pulse(INeuronNet net)
        {
            foreach (INeuron n in m_neurons)
                n.Pulse(this);
        }

        public bool Remove(INeuron item)
        {
            if (IsReadOnly) return false;
            Count = m_neurons.Count - 1;
            return m_neurons.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly) return;
            m_neurons.RemoveAt(index);
            Count = m_neurons.Count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_neurons.GetEnumerator();
        }
    }
    public class Neuron : INeuron
    {
        private NeuronFactor m_bias;
        private double m_biasWeight;
        private double m_error;
        private Dictionary<INeuronSignal, NeuronFactor> m_input;
        private double m_output;

        public NeuronFactor Bias { get => m_bias; set => m_bias = value; }
        public double BiasWeight {  get => m_biasWeight; set => m_biasWeight = value; }
        public double Error { get => m_error; set => m_error = value; }
        public Dictionary<INeuronSignal, NeuronFactor> Input { get => m_input; }
        public double Output { get => m_output; set => m_output = value; }
        public void Pulse(INeuronLayer layer)
        {
            lock (this)
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
            lock (this)
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
