namespace SignalAnalysis.Template.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
