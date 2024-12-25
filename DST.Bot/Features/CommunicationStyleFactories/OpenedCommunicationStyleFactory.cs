namespace DST.Bot.Features.CommunicationStyleFactories;

public class OpenedCommunicationStyleFactory : ICommunicationStyleFactory
{
    public string GetMainMenuMessage()
    {
        return "Сообщениe о главном меню для легенды";
    }

    public string FrontPageGenerationMessage()
    {
        return "Сообщение о генерации титульного листа для легенды";
    }

    public string SourceFinderMessage()
    {
        return "Сообщения о поиске статей для легенды";
    }
}