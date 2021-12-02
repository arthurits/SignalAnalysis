namespace SignalAnalysis
{
    // This class implements the computation of the fractal dimension of a discrete curve according to 
    // Carlos Sevcik's "A procedure to estimate the fractal dimension of waveforms" (https://arxiv.org/abs/1003.5266)
    public class FractalDimension
    {
        private int _n;
        private double _xMax;
        private double _xMin;
        private double _yMax;
        private double _yMin;
        private double _length;
        private double _dimension;
        public double Dimension => _dimension;
        

        public FractalDimension()
        {

        }

        public FractalDimension(double[] xValues, double[] yValues)
            :this()
        {
            _dimension = ComputeDim(xValues, yValues);
        }

        public FractalDimension(double samplingFreq, double[] yValues)
            : this()
        {
            _dimension = ComputeDim(samplingFreq, yValues);
        }

        /// <summary>
        /// Computes the fractal dimension of a discrete curve.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <returns>The fractal dimension of the curve</returns>
        public double ComputeDim(double[] xValues, double[] yValues)
        {
            _n = yValues.Length;
            (_xMax, _xMin) = GetMaxMin(xValues);
            (_yMax, _yMin) = GetMaxMin(yValues);
            
            _length = NormalizedLength(xValues, yValues, _n, _xMax, _xMin, _yMax, _yMin);
            _dimension = 1 + System.Math.Log10(_length) / System.Math.Log10(2 * (_n - 1));

            return _dimension;
        }

        /// <summary>
        /// Computes the fractal dimension of a discrete curve with a constant sampling rate starting at time 0 seconds.
        /// </summary>
        /// <param name="samplingFreq">The sampling frequency</param>
        /// <param name="yValues">Array containing the ordinate points</param>
        /// <returns>The fractal dimension of the curve</returns>
        public double ComputeDim(double samplingFreq, double[] yValues)
        {
            _n = yValues.Length;
            (_yMax, _yMin) = GetMaxMin(yValues);

            _length = NormalizedLength(yValues, _n, _yMax, _yMin);
            _dimension = 1 + System.Math.Log10(_length) / System.Math.Log10(2 * (_n - 1));

            return _dimension;
        }

        /// <summary>
        /// Computes the normalized length of a discrete curve.
        /// To do so, the passed values are normalized to be in the range [0, 1] both in the abscissa and the ordinate axes.
        /// </summary>
        /// <param name="xValues">Array containing the abscissa points</param>
        /// <param name="yValues">Array containing the ordinate points</param>
        /// <param name="n">Number of points</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <returns>The normalized length of the curve</returns>
        private double NormalizedLength(double [] xValues, double[] yValues, int n, double xMax, double xMin, double yMax, double yMin)
        {
            double length = 0.0;
            double xNorm1 = 0.0;
            double xNorm2 = 0.0;
            double yNorm1 = 0.0;
            double yNorm2 = 0.0;

            if (n <= 1) return 0.0;

            for (int i=0; i < n; i++)
            {
                xNorm1 = (xValues[i] - xMin) / (xMax - xMin);
                yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
                if (i > 0)
                    length += System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(xNorm1 - xNorm2, 2));
                xNorm2 = xNorm1;
                yNorm2 = yNorm1;
            }
            return length;
        }

        /// <summary>
        /// Computes the normalized length of a discrete curve with a constant sampling rate starting at time 0 seconds.
        /// The passed values are normalized to be in the range [0, 1] both in the abscissa and the ordinate axes.
        /// </summary>
        /// <param name="yValues">Array containing the ordinate points</param>
        /// <param name="n">Number of points</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <returns>The normalized length of the curve</returns>
        private double NormalizedLength(double[] yValues, int n, double yMax, double yMin)
        {
            double length = 0.0;
            double yNorm1 = 0.0;
            double yNorm2 = 0.0;

            if (n <= 1) return 0.0;

            for (int i = 0; i < n; i++)
            {
                yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
                if (i > 0)
                    length += System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(1 / (n - 1), 2));
                yNorm2 = yNorm1;
            }
            return length;
        }

        private (double max, double min) GetMaxMin(double[] values)
        {
            double highest = values[0];
            double lowest = values[0];

            foreach (dynamic i in values)
            {
                if (i > highest)
                    highest = i;
                if (i < lowest)
                    lowest = i;
            }

            return (highest, lowest);
        }
    }
}
