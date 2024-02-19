namespace SignalAnalysis.UnitTest;

[TestClass]
public class SignalAnalysisTest
{
    readonly double[] dataRandom = [0.635332247, 0.870181136, 0.678631034, 1.078938819, 1.105371046, 1.160009639, 1.091093042, 1.01799636, 0.992046413, 1.40562247, 0.74047614, 1.01297892,
            0.930566431, 0.829267703, 1.268018434, 1.2646188, 0.943980255, 0.53140922, 1.89863075, 0.806829855, 0.428950107, 2.020562594, 0.787877134, 1.195852193, 1.963551608,
            0.345232064, 1.036935295, 0.373574234, 0.691577798, 0.750008462, 1.475358373, 0.857603129, 0.060878366, 0.259649207, 1.302601821, 1.000431607, 0.546063164, 1.192633612,
            0.610321354, 0.428376566, 1.132490096, 1.493178088, 1.088099726, 0.718872968, 0.661457021, 1.172447167, 0.512788372, 1.722244192, 1.309734253, 1.38219508];
    
    readonly int[] dataPrimes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
        73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
        179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
        283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
        419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541];

    [TestMethod]
    public void Test_Sum()
    {
        Assert.AreEqual(48.783544365, Statistics.Descriptive.Sum(dataRandom), 1e-9);
        Assert.AreEqual(24133, Statistics.Descriptive.Sum(dataPrimes), 1e-1);
    }

    [TestMethod]
    public void Test_Mean()
    {
        Assert.AreEqual(0.9756708873, Statistics.Descriptive.Mean(dataRandom), 1e-10);
        Assert.AreEqual(241.33, Statistics.Descriptive.Mean(dataPrimes), 1e-2);
    }

    [TestMethod]
    public void Test_Variance()
    {
        // Sample variance
        Assert.AreEqual(0.185565777171397, Statistics.Descriptive.Variance(dataRandom, true), 1e-15);
        Assert.AreEqual(0.185565777171397, Statistics.Descriptive.VarianceParallel(dataRandom, true), 1e-15);

        Assert.AreEqual(25865.7586868687, Statistics.Descriptive.Variance(dataPrimes, true), 1e-10);
        Assert.AreEqual(25865.7586868687, Statistics.Descriptive.VarianceParallel(dataPrimes, true), 1e-4);

        // Population variance
        Assert.AreEqual(0.181854461627969, Statistics.Descriptive.Variance(dataRandom, false), 1e-15);
        Assert.AreEqual(0.181854461627969, Statistics.Descriptive.VarianceParallel(dataRandom, false), 1e-15);

        Assert.AreEqual(25607.1011, Statistics.Descriptive.Variance(dataPrimes, false), 1e-10);
        Assert.AreEqual(25607.1011, Statistics.Descriptive.VarianceParallel(dataPrimes, false), 1e-4);
    }
    
    [TestMethod]
    public void Test_SST()
    {
        Assert.AreEqual(9.092723081398477, Statistics.Descriptive.SST(dataRandom), 1e-15);
        Assert.AreEqual(9.092723081398475, Statistics.Descriptive.SSTParallel(dataRandom), 1e-15);

        Assert.AreEqual(2560710.11, Statistics.Descriptive.SST(dataPrimes), 1e-2);
        Assert.AreEqual(2560710.11, Statistics.Descriptive.SSTParallel(dataPrimes), 1e-2);

    }
}