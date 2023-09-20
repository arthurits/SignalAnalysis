namespace SignalAnalysis;

// https://github.com/Samson-Mano/Fast_Fourier_Transform/blob/main/Fast_fourier_transform/Fast_fourier_transform/process_data.cs
public static class FFT
{
    public static double[] FFTpower(double[] input_signal)
    {
        int i;
        int N = input_signal.Length;
        if (N == 1)
            return input_signal;

        // Even array
        double[] evenList = new double[N / 2];
        for (i = 0; i < (N / 2); i++)
            evenList[i] = input_signal[2 * i];
        evenList = FFTpower(evenList);

        // Odd array
        double[] oddList = new double[N / 2];
        for (i = 0; i < (N / 2); i++)
        {
            oddList[i] = input_signal[(2 * i) + 1];
        }
        oddList = FFTpower(oddList);

        // Result
        double[] result = new double[N];

        for (i = 0; i < (N / 2); i++)
        {
            double w = (-2.0 * i * Math.PI) / N;
            Complex wk = new(Math.Cos(w), Math.Sin(w));
            Complex even = new(evenList[i], 0);
            Complex odd = new(oddList[i], 0);

            result[i] = (even + (wk * odd)).Real;
            result[i + (N / 2)] = (even - (wk * odd)).Real;
        }
        return result;
    }

    public struct Complex
    {
        public double Real;
        public double Imaginary;
        public double MagnitudeSquared { get { return Real * Real + Imaginary * Imaginary; } }
        public double Magnitude { get { return Math.Sqrt(MagnitudeSquared); } }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public override string ToString()
        {
            if (Imaginary < 0)
                return $"{Real}-{-Imaginary}j";
            else
                return $"{Real}+{Imaginary}j";
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(
                real: (a.Real * b.Real) - (a.Imaginary * b.Imaginary),
                imaginary: (a.Real * b.Imaginary) + (a.Imaginary * b.Real));
        }

        public static Complex operator *(Complex a, double b)
        {
            return new Complex(a.Real * b, a.Imaginary * b);
        }

        public static Complex[] FromReal(double[] real)
        {
            Complex[] complex = new Complex[real.Length];
            for (int i = 0; i < real.Length; i++)
                complex[i].Real = real[i];
            return complex;
        }

        public static double[] GetMagnitudes(Complex[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = input[i].Magnitude;
            return output;
        }
    }

}
