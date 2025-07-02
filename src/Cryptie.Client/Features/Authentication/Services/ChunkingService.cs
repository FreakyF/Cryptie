using System;

namespace Cryptie.Client.Features.Authentication.Services;

public class ChunkingService : IChunkingService
{
    public string[] Split(string value, int maxChunkSize)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxChunkSize);

        var totalChunks = (value.Length + maxChunkSize - 1) / maxChunkSize;
        var chunks = new string[totalChunks];

        for (var i = 0; i < totalChunks; i++)
        {
            var offset = i * maxChunkSize;
            var length = Math.Min(maxChunkSize, value.Length - offset);
            chunks[i] = value.Substring(offset, length);
        }

        return chunks;
    }

    public string Join(string[] chunks)
    {
        ArgumentNullException.ThrowIfNull(chunks);
        return string.Concat(chunks);
    }
}