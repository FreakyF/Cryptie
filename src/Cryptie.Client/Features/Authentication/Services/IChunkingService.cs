namespace Cryptie.Client.Features.Authentication.Services;

public interface IChunkingService
{
    string[] Split(string value, int maxChunkSize);
    string Join(string[] chunks);
}