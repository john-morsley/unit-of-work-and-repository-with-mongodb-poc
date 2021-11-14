namespace IntegrationTests.TestUtilities;

internal class MongoDBInDocker : IDisposable
{
    public const string MONGODB_IMAGE = "mongo";
    public const string MONGODB_IMAGE_TAG = "5.0.3-focal";

    //public const string MONGODB_ROOT_USERNAME = "root";
    //public const string MONGODB_ROOT_PASSWORD = "password";

    public const string MONGODB_CONTAINER_NAME = "IntegrationTesting_MongoDB";
    //public const string MONGODB_VOLUME_NAME = "IntegrationTesting_MongoDB";

    public const string MONGODB_AUTHENTICATION_MECHANISM = "SCRAM-SHA-1";
    public const int MONGODB_PORT = 27017;

    public static async Task<(string containerId, int port)> EnsureDockerStartedAndGetContainerIdAndPortAsync(MongoSettings settings)
    {
        await CleanupRunningContainers();
        //await CleanupRunningVolumes();

        var dockerClient = GetDockerClient();
        var freePort = GetFreePort();

        // This call ensures that the latest Docker image is pulled
        var imagesCreateParameters = new ImagesCreateParameters() { FromImage = $"{MONGODB_IMAGE}:{MONGODB_IMAGE_TAG}" };
        await dockerClient.Images.CreateImageAsync(imagesCreateParameters, null, new Progress<JSONMessage>());



        // Create a volume, if one doesn't already exist
        //var volumeList = await dockerClient.Volumes.ListAsync();
        //var volumeCount = volumeList.Volumes.Where(v => v.Name == MONGODB_VOLUME_NAME).Count();
        //if (volumeCount <= 0)
        //{
        //var parameters = new VolumesCreateParameters { Name = MONGODB_VOLUME_NAME };
        //await dockerClient.Volumes.CreateAsync(parameters);
        //}

        // create container, if one doesn't already exist
        //var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
        //var existingContainer = containers.Where(c => c.Names.Any(n => n.Contains(MONGODB_CONTAINER_NAME))).FirstOrDefault();

        //if (existingContainer == null)
        //{
            var container = await dockerClient
                .Containers
                .CreateContainerAsync(new CreateContainerParameters
                {
                    Name = MONGODB_CONTAINER_NAME,
                    Image = $"{MONGODB_IMAGE}:{MONGODB_IMAGE_TAG}",
                    Env = new List<string>
                    {
                        $"MONGO_INITDB_ROOT_USERNAME={settings.Username}",
                        $"MONGO_INITDB_ROOT_PASSWORD={settings.Password}"
                    },
                    //Volumes = { $"{MONGODB_VOLUME_NAME}", "" },
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            {
                                $"{MONGODB_PORT}/tcp",
                                new PortBinding[]
                                {
                                    new PortBinding
                                    {
                                        HostPort = freePort.ToString(),
                                    }
                                }
                            }
                        }//,
                        //Binds = new List<string>
                        //{
                        //    $"{MONGODB_VOLUME_NAME}:/data/db"
                        //}
                    },
                });

            await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            await WaitUntilDatabaseAvailableAsync(settings, freePort);

            return (container.ID, freePort);
        //}

        //return (existingContainer.ID, existingContainer.Ports.FirstOrDefault().PublicPort.ToString());
    }

    internal static async Task<string> GetPort()
    {
        var dockerClient = GetDockerClient();
        var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
        //var existingContainer = await containers.Where(c => c.Names.Any(n => n.Contains(MONGODB_CONTAINER_NAME))).FirstOrDefault;
        //return existingContainer.Port;

        throw new NotImplementedException();
    }

    private static bool IsRunningOnWindows()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    private static DockerClient GetDockerClient()
    {
        var dockerUri = IsRunningOnWindows() ? "npipe://./pipe/docker_engine" : "unix:///var/run/docker.sock";
        return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
    }

    private static async Task CleanupRunningContainers(int hoursTillExpiration = -24)
    {
        var dockerClient = GetDockerClient();
        
        var runningContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true } );

        foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(MONGODB_CONTAINER_NAME))))
        {
            // Stopping all test containers that are older than 24 hours
            //var expiration = hoursTillExpiration > 0 ? hoursTillExpiration * -1 : hoursTillExpiration;
            //if (runningContainer.Created < DateTime.UtcNow.AddHours(expiration))
            //{
                try
                {
                    await EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            //}
        }
    }

    //private static async Task CleanupRunningVolumes(int hoursTillExpiration = -24)
    //{
    //    var dockerClient = GetDockerClient();

    //    var runningVolumes = await dockerClient.Volumes.ListAsync();

    //    foreach (var runningVolume in runningVolumes.Volumes.Where(v => v.Name == MONGODB_VOLUME_NAME))
    //    {
    //        // Stopping all test volumes that are older than 24 hours
    //        var expiration = hoursTillExpiration > 0 ? hoursTillExpiration * -1 : hoursTillExpiration;
    //        if (DateTime.Parse(runningVolume.CreatedAt) < DateTime.UtcNow.AddHours(expiration))
    //        {
    //            try
    //            {
    //                await EnsureDockerVolumesRemovedAsync(runningVolume.Name);
    //            }
    //            catch
    //            {
    //                // Ignoring failures to stop running containers
    //            }
    //        }
    //    }
    //}

    public static async Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Containers.StopContainerAsync(dockerContainerId, new ContainerStopParameters());
        await dockerClient.Containers.RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
        //await dockerClient.Containers.PruneContainersAsync(new ContainersPruneParameters());
        //await dockerClient.Volumes.PruneAsync(new VolumesPruneParameters());
    }

    public static async Task EnsureDockerVolumesRemovedAsync(string volumeName)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Volumes.RemoveAsync(volumeName);
    }

    private static async Task WaitUntilDatabaseAvailableAsync(MongoSettings settings, int databasePort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                //var mongoDbConnectionString = GetMongoDbConnectionString(databasePort);
                //using var sqlConnection = new SqlConnection(sqlConnectionString);
                //await sqlConnection.OpenAsync();

                var internalIdentity = new MongoInternalIdentity("admin", settings.Username);
                var passwordEvidence = new PasswordEvidence(settings.Password);
                var mongoCredential = new MongoCredential(MONGODB_AUTHENTICATION_MECHANISM, internalIdentity, passwordEvidence);

                var mongoClientSettings = new MongoClientSettings
                {
                    Credential = mongoCredential,
                    Server = new MongoServerAddress("localhost", databasePort)
                };

                var client = new MongoClient(mongoClientSettings);
                var instance = client.GetDatabase(MONGODB_CONTAINER_NAME);

                connectionEstablished = true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                await Task.Delay(500);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to the SQL docker database could not be established within {maxWaitTimeSeconds} seconds.");
        }

        return;
    }

    private static int GetFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }

    public void Dispose()
    {
        
    }

    internal static string ConnectionString(MongoSettings settings, int port)
    {
        return $"mongodb://{settings.Username}:{settings.Password}@localhost:{port}";
    }
}
