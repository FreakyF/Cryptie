using Docker.DotNet;
using Docker.DotNet.Models;

namespace Cryptie.Server;

public class DockerStarter
{
    /// <summary>
    /// Ensures that a PostgreSQL container is running for the application.
    /// If an existing container named <c>cryptie-db</c> is found it will be started
    /// if necessary, otherwise a new one will be created and started.
    /// </summary>
    /// <returns>A task representing the asynchronous start operation.</returns>
    public async Task StartPostgresAsync()
    {
        var client = new DockerClientConfiguration().CreateClient();

        var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        var existingContainer = containers.FirstOrDefault(c => c.Names.Any(n => n.TrimStart('/') == "cryptie-db"));

        if (existingContainer != null)
        {
            if (!existingContainer.State.Equals("running", StringComparison.OrdinalIgnoreCase))
                await client.Containers.StartContainerAsync(existingContainer.ID, new ContainerStartParameters());

            return;
        }

        await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "postgres",
            Name = "cryptie-db",
            Env = new List<string> { "POSTGRES_PASSWORD=admin" },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["5432/tcp"] = new List<PortBinding> { new() { HostPort = "55123" } }
                }
            }
        });

        await client.Containers.StartContainerAsync("cryptie-db", new ContainerStartParameters());
    }
}