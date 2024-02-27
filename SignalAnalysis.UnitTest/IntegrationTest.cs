using System.Diagnostics;

namespace SignalAnalysis.UnitTest;

[TestClass]
public class IntegrationTest
{
    private readonly double[] sin1Hz = [0, 0.06279052, 0.125333234, 0.187381315, 0.248689887, 0.309016994, 0.368124553, 0.425779292, 0.481753674, 0.535826795, 0.587785252, 0.63742399,
        0.684547106, 0.728968627, 0.770513243, 0.809016994, 0.844327926, 0.87630668, 0.904827052, 0.929776486, 0.951056516, 0.968583161, 0.982287251, 0.992114701, 0.998026728,
        1, 0.998026728, 0.992114701, 0.982287251, 0.968583161, 0.951056516, 0.929776486, 0.904827052, 0.87630668, 0.844327926, 0.809016994, 0.770513243, 0.728968627, 0.684547106,
        0.63742399, 0.587785252, 0.535826795, 0.481753674, 0.425779292, 0.368124553, 0.309016994, 0.248689887, 0.187381315, 0.125333234, 0.06279052, 0, -0.06279052, -0.125333234,
        -0.187381315, -0.248689887, -0.309016994, -0.368124553, -0.425779292, -0.481753674, -0.535826795, -0.587785252, -0.63742399, -0.684547106, -0.728968627, -0.770513243,
        -0.809016994, -0.844327926, -0.87630668, -0.904827052, -0.929776486, -0.951056516, -0.968583161, -0.982287251, -0.992114701, -0.998026728, -1, -0.998026728, -0.992114701,
        -0.982287251, -0.968583161, -0.951056516, -0.929776486, -0.904827052, -0.87630668, -0.844327926, -0.809016994, -0.770513243, -0.728968627, -0.684547106, -0.63742399,
        -0.587785252, -0.535826795, -0.481753674, -0.425779292, -0.368124553, -0.309016994, -0.248689887, -0.187381315, -0.125333234, -0.06279052, 0];

    private readonly double[] sin2Hz = [0, 0.062666617, 0.124344944, 0.184062276, 0.240876837, 0.293892626, 0.342273553, 0.385256621, 0.422163963, 0.452413526, 0.475528258,
        0.491143625, 0.499013364, 0.499013364, 0.491143625, 0.475528258, 0.452413526, 0.422163963, 0.385256621, 0.342273553, 0.293892626, 0.240876837, 0.184062276, 0.124344944,
        0.062666617, 0, -0.062666617, -0.124344944, -0.184062276, -0.240876837, -0.293892626, -0.342273553, -0.385256621, -0.422163963, -0.452413526, -0.475528258, -0.491143625,
        -0.499013364, -0.499013364, -0.491143625, -0.475528258, -0.452413526, -0.422163963, -0.385256621, -0.342273553, -0.293892626, -0.240876837, -0.184062276, -0.124344944,
        -0.062666617, 0, 0.062666617, 0.124344944, 0.184062276, 0.240876837, 0.293892626, 0.342273553, 0.385256621, 0.422163963, 0.452413526, 0.475528258, 0.491143625,
        0.499013364, 0.499013364, 0.491143625, 0.475528258, 0.452413526, 0.422163963, 0.385256621, 0.342273553, 0.293892626, 0.240876837, 0.184062276, 0.124344944, 0.062666617,
        0, -0.062666617, -0.124344944, -0.184062276, -0.240876837, -0.293892626, -0.342273553, -0.385256621, -0.422163963, -0.452413526, -0.475528258, -0.491143625,
        -0.499013364, -0.499013364, -0.491143625, -0.475528258, -0.452413526, -0.422163963, -0.385256621, -0.342273553, -0.293892626, -0.240876837, -0.184062276, -0.124344944,
        -0.062666617, 0];

    private readonly double[] sinSum = [0, 0.125457136, 0.249678177, 0.371443591, 0.489566724, 0.602909621, 0.710398106, 0.811035913, 0.903917637, 0.988240321, 1.06331351,
        1.128567615, 1.18356047, 1.227981992, 1.261656868, 1.284545253, 1.296741452, 1.298470643, 1.290083674, 1.272050039, 1.244949142, 1.209459998, 1.166349527, 1.116459645,
        1.060693345, 1, 0.935360112, 0.867769758, 0.798224974, 0.727706324, 0.65716389, 0.587502933, 0.519570431, 0.454142717, 0.391914399, 0.333488736, 0.279369617, 0.229955263,
        0.185533742, 0.146280364, 0.112256994, 0.083413269, 0.059589711, 0.04052267, 0.025851, 0.015124368, 0.00781305, 0.003319038, 0.00098829, 0.000123903, 0, -0.000123903,
        -0.00098829, -0.003319038, -0.00781305, -0.015124368, -0.025851, -0.04052267, -0.059589711, -0.083413269, -0.112256994, -0.146280364, -0.185533742, -0.229955263, -0.279369617,
        -0.333488736, -0.391914399, -0.454142717, -0.519570431, -0.587502933, -0.65716389, -0.727706324, -0.798224974, -0.867769758, -0.935360112, -1, -1.060693345, -1.116459645,
        -1.166349527, -1.209459998, -1.244949142, -1.272050039, -1.290083674, -1.298470643, -1.296741452, -1.284545253, -1.261656868, -1.227981992, -1.18356047, -1.128567615,
        -1.06331351, -0.988240321, -0.903917637, -0.811035913, -0.710398106, -0.602909621, -0.489566724, -0.371443591, -0.249678177, -0.125457136, 0];

    [TestMethod]
    public void Test_Integration_LeftPoint()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz,IntegrationMethod.LeftPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031908, Integration.Integrate(sin1Hz, IntegrationMethod.LeftPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.LeftPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.31789089679, Integration.Integrate(sin2Hz, IntegrationMethod.LeftPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.LeftPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031904, Integration.Integrate(sinSum, IntegrationMethod.LeftPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, true, false), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547874, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, true), 1e-15);
        
        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.317890896877306, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, true), 1e-15);
        
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547874, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.LeftPointRule, 0, 1, 100, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_RightPoint()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.RightPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031908, Integration.Integrate(sin1Hz, IntegrationMethod.RightPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.RightPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.31789089679, Integration.Integrate(sin2Hz, IntegrationMethod.RightPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.RightPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031904, Integration.Integrate(sinSum, IntegrationMethod.RightPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, true, false), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.RightPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547874, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.RightPointRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.RightPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.317890896877306, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.RightPointRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.RightPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547874, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.RightPointRule, 0, 1, 100, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_MidPoint()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.MidPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031908, Integration.Integrate(sin1Hz, IntegrationMethod.MidPointRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.MidPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.31789089679, Integration.Integrate(sin2Hz, IntegrationMethod.MidPointRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.MidPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031904, Integration.Integrate(sinSum, IntegrationMethod.MidPointRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, true, false), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.MidPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.636724504181952, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.MidPointRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.MidPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.31851942219817281, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.MidPointRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.MidPointRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63672450418195237, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.MidPointRule, 0, 1, 100, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_Trapezoid()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.TrapezoidRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031908, Integration.Integrate(sin1Hz, IntegrationMethod.TrapezoidRule, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.TrapezoidRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.31789089679, Integration.Integrate(sin2Hz, IntegrationMethod.TrapezoidRule, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.TrapezoidRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63641031904, Integration.Integrate(sinSum, IntegrationMethod.TrapezoidRule, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, true, false), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547874, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.317890896877306, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63641031907547885, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.TrapezoidRule, 0, 1, 100, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_Simpson3()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule3, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.6366198275199999, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule3, sin1Hz.GetLowerBound(0), sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule3, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.31789089679999988, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule3, sin2Hz.GetLowerBound(0), sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule3, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, false, false), 1e-10);
        Assert.AreEqual(0.63661982749333346, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule3, sinSum.GetLowerBound(0), sinSum.GetUpperBound(0), 100, true, false), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63661982751576784, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.317890896877306, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, false), 1e-15);
        Assert.AreEqual(0.63661982751576784, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule3, 0, 1, 100, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_Simpson8()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule8, 0, sin1Hz.GetUpperBound(0), 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule8, 0, sin1Hz.GetUpperBound(0), 100, false, true), 1e-4);
        Assert.AreEqual(0.63646261004750027, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule8, 0, sin1Hz.GetUpperBound(0), 100, true, false), 1e-10);
        Assert.AreEqual(0.63638412189750015, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonRule8, 0, sin1Hz.GetUpperBound(0), 100, true, true), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule8, 0, sin2Hz.GetUpperBound(0), 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule8, 0, sin2Hz.GetUpperBound(0), 100, false, true), 1e-4);
        Assert.AreEqual(0.31799547915125009, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule8, 0, sin2Hz.GetUpperBound(0), 100, true, false), 1e-10);
        Assert.AreEqual(0.31791714588000003, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonRule8, 0, sin2Hz.GetUpperBound(0), 100, true, true), 1e-10);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule8, 0, sinSum.GetUpperBound(0), 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule8, 0, sinSum.GetUpperBound(0), 100, false, true), 1e-3);
        Assert.AreEqual(0.63661969052000034, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule8, 0, sinSum.GetUpperBound(0), 100, true, false), 1e-10);
        Assert.AreEqual(0.63646286910000038, Integration.Integrate(sinSum, IntegrationMethod.SimpsonRule8, 0, sinSum.GetUpperBound(0), 100, true, true), 1e-10);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, true), 1e-14);
        Assert.AreEqual(0.63646261004565408, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, false), 1e-15);
        Assert.AreEqual(0.63661988705121764, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, true), 1e-15);
        Assert.AreEqual(0.3179954792264908, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, false), 1e-15);
        Assert.AreEqual(0.31830994352560887, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, true), 1e-15);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, false), 1e-6);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, false, true), 1e-14);
        Assert.AreEqual(0.636619690551253, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, false), 1e-15);
        Assert.AreEqual(0.63661988705121808, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonRule8, 0, 1, 100, true, true), 1e-15);
    }

    [TestMethod]
    public void Test_Integration_SimpsonComposite()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonComposite, 0, sin1Hz.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0.6365150736070837, Integration.Integrate(sin1Hz, IntegrationMethod.SimpsonComposite, 0, sin1Hz.GetUpperBound(0), 100, true, false), 1e-16);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonComposite, 0, sin2Hz.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0.31799575944291686, Integration.Integrate(sin2Hz, IntegrationMethod.SimpsonComposite, 0, sin2Hz.GetUpperBound(0), 100, true, false), 1e-16);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.SimpsonComposite, 0, sinSum.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0.63661993620750035, Integration.Integrate(sinSum, IntegrationMethod.SimpsonComposite, 0, sinSum.GetUpperBound(0), 100, true, false), 1e-16);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, false, false), 1e-15);
        Assert.AreEqual(0.63651507360114012, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, true, false), 1e-16);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, false, false), 1e-16);
        Assert.AreEqual(0.31799575952023684, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, true, false), 1e-16);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, false, false), 1e-15);
        Assert.AreEqual(0.63661993624407109, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.SimpsonComposite, 0, 1, 100, true, false), 1e-16);
    }

    [TestMethod]
    public void Test_Integration_Romberg()
    {
        // Test data array input
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.Romberg, 0, sin1Hz.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.Romberg, 0, sin1Hz.GetUpperBound(0), 100, false, true), 1e-5);
        Assert.AreEqual(0, Integration.Integrate(sin1Hz, IntegrationMethod.Romberg, 0, sin1Hz.GetUpperBound(0), 100, true, false), 1e-16);
        Assert.AreEqual(0.63669457505048688, Integration.Integrate(sin1Hz, IntegrationMethod.Romberg, 0, sin1Hz.GetUpperBound(0), 100, true, true), 1e-16);

        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.Romberg, 0, sin2Hz.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.Romberg, 0, sin2Hz.GetUpperBound(0), 100, false, true), 1e-5);
        Assert.AreEqual(0, Integration.Integrate(sin2Hz, IntegrationMethod.Romberg, 0, sin2Hz.GetUpperBound(0), 100, true, false), 1e-16);
        Assert.AreEqual(0.31789089707873663, Integration.Integrate(sin2Hz, IntegrationMethod.Romberg, 0, sin2Hz.GetUpperBound(0), 100, true, true), 1e-16);

        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.Romberg, 0, sinSum.GetUpperBound(0), 100, false, false), 1e-16);
        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.Romberg, 0, sinSum.GetUpperBound(0), 100, false, true), 1e-5);
        Assert.AreEqual(0, Integration.Integrate(sinSum, IntegrationMethod.Romberg, 0, sinSum.GetUpperBound(0), 100, true, false), 1e-16);
        Assert.AreEqual(0.636614345880711, Integration.Integrate(sinSum, IntegrationMethod.Romberg, 0, sinSum.GetUpperBound(0), 100, true, true), 1e-16);

        // Test function input
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.Romberg, 0, 1, 100, false, false), 1e-15);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.Romberg, 0, 1, 100, false, true), 1e-16);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.Romberg, 0, 1, 100, true, false), 1e-16);
        Assert.AreEqual(0.63661977236758138, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x), IntegrationMethod.Romberg, 0, 1, 100, true, true), 1e-16);

        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, false, false), 1e-16);
        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, false, true), 1e-16);
        Assert.AreEqual(0, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, true, false), 1e-16);
        Assert.AreEqual(0.31830988618556694, Integration.Integrate((x) => 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, true, true), 1e-16);

        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, false, false), 1e-16);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, false, true), 1e-16);
        Assert.AreEqual(0, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, true, false), 1e-16);
        Assert.AreEqual(0.63661977236758083, Integration.Integrate((x) => Math.Sin(2 * Math.PI * 1 * x) + 0.5 * Math.Sin(2 * Math.PI * 2 * x), IntegrationMethod.Romberg, 0, 1, 100, true, true), 1e-16);
    }
}
