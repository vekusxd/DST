namespace DST.Bot.Features.CommunicationStyleFactories;

public class UnsocializedCommunicationStyleFactory : ICommunicationStyleFactory
{
    public string GetMainMenuMessage()
    {
        return "Сообщения о главном меню для тех, кто не трогал траву всю жизнь";
    }

    public string FrontPageGenerationMessage()
    {
        return "Сообщениие про генерацию титульного листа для тех, кто не трогал траву всю жизнь";
    }

    public string SourceFinderMessage()
    {
        return "Сообщениие про поиск статей для тех, кто не трогал траву всю жизнь";
    }
}