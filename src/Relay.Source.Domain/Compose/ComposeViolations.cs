using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Compose;

public static class ComposeViolations
{
    public static IEnumerable<ComposeViolation> Of(ComposeService service)
    {
        if (service.Privileged)
        {
            yield return new ComposeViolation("privileged", ComposeErrors.Privileged(service.Name));
        }

        if (service.NetworkMode == "host")
        {
            yield return new ComposeViolation("network_mode", ComposeErrors.HostNetwork(service.Name));
        }

        if (service.Devices.Count > 0)
        {
            yield return new ComposeViolation("devices", ComposeErrors.Devices(service.Name));
        }

        foreach (var volume in service.Volumes)
        {
            var violation = VolumeViolation(service, volume);
            if (violation is not null)
            {
                yield return violation;
            }
        }

        if (service.Build is not null && BuildViolation(service) is { } build)
        {
            yield return build;
        }

        if (service.Image is null && service.Build is null)
        {
            yield return new ComposeViolation("image", ComposeErrors.SourceMissing(service.Name));
        }
    }

    public static string? BuildContextOf(ComposeService service)
    {
        var context = service.Build?.Context;
        return context is null || IsRemote(context) ? null : service.File.DirectoryOf().Join(context);
    }

    private static ComposeViolation? VolumeViolation(ComposeService service, ComposeVolume volume) => volume.Kind() switch
    {
        ComposeVolumeKind.DockerSocket => new ComposeViolation("volumes", ComposeErrors.DockerSocket(service.Name, volume.ToString())),
        ComposeVolumeKind.HostPath => new ComposeViolation("volumes", ComposeErrors.HostPath(service.Name, volume.ToString())),
        _ => null,
    };

    private static ComposeViolation? BuildViolation(ComposeService service)
    {
        var context = service.Build!.Context;
        if (string.IsNullOrWhiteSpace(context))
        {
            return new ComposeViolation("build", ComposeErrors.BuildContextMissing(service.Name));
        }

        return BuildContextOf(service) is null
            ? new ComposeViolation("build", ComposeErrors.BuildContextOutsideRepository(service.Name, context))
            : null;
    }

    private static bool IsRemote(string context) =>
        context.Contains("://", StringComparison.Ordinal) || context.StartsWith("git@", StringComparison.Ordinal);
}
