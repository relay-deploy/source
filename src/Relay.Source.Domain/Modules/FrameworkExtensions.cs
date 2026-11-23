namespace Relay.Source.Domain.Modules;

public static class FrameworkExtensions
{
    public static BuildStack Stack(this Framework framework) => framework switch
    {
        Framework.React or Framework.Vue or Framework.Angular or Framework.StaticPlain => BuildStack.Static,
        Framework.NextJs or Framework.Express => BuildStack.Node,
        Framework.SpringBoot => BuildStack.Java,
        Framework.Django or Framework.FastApi or Framework.Flask => BuildStack.Python,
        Framework.Dotnet => BuildStack.Dotnet,
        Framework.Rust => BuildStack.Rust,
        _ => BuildStack.Go,
    };

    public static int DefaultPort(this Framework framework) => framework switch
    {
        Framework.NextJs or Framework.Express => 3000,
        Framework.Django or Framework.FastApi => 8000,
        Framework.Flask => 5000,
        _ => 8080,
    };
}
