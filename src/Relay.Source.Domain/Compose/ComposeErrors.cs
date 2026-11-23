using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Compose;

public static class ComposeErrors
{
    public static Error Unreadable(string path) =>
        Error.Validation("compose.unreadable", $"{path} is not a valid compose file, so it was ignored.");

    public static Error Privileged(string service) =>
        Error.Validation("compose.privileged_rejected", $"Service '{service}' sets 'privileged', which relay does not allow.");

    public static Error HostNetwork(string service) =>
        Error.Validation("compose.host_network_rejected", $"Service '{service}' sets 'network_mode: host', which relay does not allow.");

    public static Error Devices(string service) =>
        Error.Validation("compose.devices_rejected", $"Service '{service}' sets 'devices', which relay does not allow.");

    public static Error DockerSocket(string service, string mount) =>
        Error.Validation("compose.docker_socket_rejected", $"Service '{service}' mounts the Docker socket in 'volumes' ('{mount}'), which relay does not allow.");

    public static Error HostPath(string service, string mount) =>
        Error.Validation("compose.host_path_rejected", $"Service '{service}' mounts a host path in 'volumes' ('{mount}'), which relay does not allow.");

    public static Error BuildContextMissing(string service) =>
        Error.Validation("compose.build_context_missing", $"Service '{service}' has 'build' without a 'context'.");

    public static Error BuildContextOutsideRepository(string service, string context) =>
        Error.Validation("compose.build_context_outside_repository", $"Service '{service}' builds from '{context}', which is not a directory inside the repository.");

    public static Error SourceMissing(string service) =>
        Error.Validation("compose.service_without_source", $"Service '{service}' has neither 'image' nor 'build'.");
}
