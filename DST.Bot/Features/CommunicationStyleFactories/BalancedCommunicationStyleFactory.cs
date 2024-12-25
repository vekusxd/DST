namespace DST.Bot.Features.CommunicationStyleFactories;

public class BalancedCommunicationStyleFactory : ICommunicationStyleFactory
{
    public string GetMainMenuMessage()
    {
        return "Сообщение о главном меню для дефолтного чела";
    }

    public string FrontPageGenerationMessage()
    {
        return "Сообщения о генерации титульного листа для дефолтного чела";
    }

    public string SourceFinderMessage()
    {
        return "Сообщение о поиске статей для дефолтного чела";
    }
}