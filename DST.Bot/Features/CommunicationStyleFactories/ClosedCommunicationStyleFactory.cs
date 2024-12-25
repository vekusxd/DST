namespace DST.Bot.Features.CommunicationStyleFactories;

public class ClosedCommunicationStyleFactory : ICommunicationStyleFactory
{
    public string GetMainMenuMessage()
    {
        return "Сообщение поакуратнее о главном меню";
    }

    public string FrontPageGenerationMessage()
    {
        return "Сообщение поакуратнее о генерации титульного листа";
    }

    public string SourceFinderMessage()
    {
        return "Сообщения поакуратнее о поиске статей";
    }
}