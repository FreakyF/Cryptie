using Docker.DotNet;
using Docker.DotNet.Models;

namespace Cryptie.Server.API;

public class DockerStarter
{
    public async Task StartPostgresAsync()
    {
        var client = new DockerClientConfiguration().CreateClient();

        var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        var existingContainer = containers.FirstOrDefault(c => c.Names.Any(n => n.TrimStart('/') == "panel-db"));

        if (existingContainer != null)
        {
            if (!existingContainer.State.Equals("running", StringComparison.OrdinalIgnoreCase))
                await client.Containers.StartContainerAsync(existingContainer.ID, new ContainerStartParameters());

            return;
        }

        await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "postgres",
            Name = "panel-db",
            Env = new List<string> { "POSTGRES_PASSWORD=admin" },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["5432/tcp"] = new List<PortBinding> { new() { HostPort = "55123" } }
                }
            }
        });

        await client.Containers.StartContainerAsync("panel-db", new ContainerStartParameters());
    }
}