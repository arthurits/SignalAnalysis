namespace SignalAnalysis;

partial class FrmMain
{
    private class Stats
    {
        public Stats(double Max = 0, double Min = 0, double Avg = 0, double FractalDim = 0, double FractalVar = 0, double AppEn = 0, double SampEn = 0, double ShannEn = 0, double BitEn = 0, double IdealEn = 0)
        {
            Maximum = Max;
            Minimum = Min;
            Average = Avg;
            FractalDimension = FractalDim;
            FractalVariance = FractalVar;
            ApproximateEntropy = AppEn;
            SampleEntropy = SampEn;
            ShannonEntropy = ShannEn;
            EntropyBit = BitEn;
            IdealEntropy = IdealEn;
        }

        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public double Average { get; set; }
        public double FractalDimension { get; set; }
        public double FractalVariance { get; set; }
        public double ApproximateEntropy { get; set; }
        public double SampleEntropy { get; set; }
        public double ShannonEntropy { get; set; }      // The Shannon entropy value of a message
        public double EntropyBit { get; set; }          // The entropy bit value of a message
        public double IdealEntropy { get; set; }        // The ideal entropy value of a message

        public override string ToString() =>
            $"Average illuminance: {Average:0.######}" + Environment.NewLine +
            $"Maximum illuminance: {Maximum:0.##}" + Environment.NewLine +
            $"Minimum illuminance: {Minimum:0.##}" + Environment.NewLine +
            $"Fractal dimension: {FractalDimension:0.########}" + Environment.NewLine +
            $"Fractal variance: {FractalVariance:0.########}" + Environment.NewLine +
            $"Approximate entropy: {ApproximateEntropy:0.########}" + Environment.NewLine +
            $"Sample entropy: {SampleEntropy:0.########}";

        public string ToString(System.Resources.ResourceManager stringsRM, System.Globalization.CultureInfo culture) =>
            (stringsRM.GetString("strHeader07", culture) ?? "Average illuminance") + $": {Average:0.######}" + Environment.NewLine +
            (stringsRM.GetString("strHeader08", culture) ?? "Maximum illuminance") + $": {Maximum:0.##}" + Environment.NewLine +
            (stringsRM.GetString("strHeader09", culture) ?? "Minimum illuminance") + $": {Minimum:0.##}" + Environment.NewLine +
            (stringsRM.GetString("strHeader10", culture) ?? "Fractal dimension") + $": {FractalDimension:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader11", culture) ?? "Fractal variance") + $": {FractalVariance:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader12", culture) ?? "Approximate entropy") + $": {ApproximateEntropy:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader13", culture) ?? "Sample entropy") + $": {SampleEntropy:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader14", culture) ?? "Shannon entropy") + $": {ShannonEntropy:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader15", culture) ?? "Entropy bit") + $": {EntropyBit:0.########}" + Environment.NewLine +
            (stringsRM.GetString("strHeader16", culture) ?? "Ideal entropy") + $": {IdealEntropy:0.########}";
    }
}
