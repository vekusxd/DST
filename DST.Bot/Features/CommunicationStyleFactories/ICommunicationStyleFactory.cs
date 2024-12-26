namespace DST.Bot.Features.CommunicationStyleFactories;

public interface ICommunicationStyleFactory
{
    string GetMainMenuMessage();
    string FrontPageGenerationMessage();
    string SourceFinderMessage();

    public static ICommunicationStyleFactory CreateFactory(int testResult) => testResult switch
    {
        > -1 and < 60 => new UnsocializedCommunicationStyleFactory(),
        >= 60 and < 120 => new ClosedCommunicationStyleFactory(),
        >= 120 and < 180 => new BalancedCommunicationStyleFactory(),
        >= 180 and < 211 => new OpenedCommunicationStyleFactory(),
        _ => throw new ArgumentException("Unsupported test result")
    };
}