namespace RealtimeCv.Core.Interfaces;

public interface IStreamInferenceService
{
    void HandleStream(string source, string targetUrl);
    
    void Dispose();
}
