namespace SignalAnalysis.Enumerations;

/// <summary>
/// Differentiation algorithms supported by the library.
/// Each method corresponds to a specific numerical differentiation strategy.
/// </summary>
public enum DerivativeMethod
{
    BackwardOnePoint,
    ForwardOnePoint,
    CenteredThreePoint,
    CenteredFivePoint,
    CenteredSevenPoint,
    CenteredNinePoint,
    SGLinearThreePoint,
    SGLinearFivePoint,
    SGLinearSevenPoint,
    SGLinearNinePoint,
    SGCubicFivePoint,
    SGCubicSevenPoint,
    SGCubicNinePoint
}
