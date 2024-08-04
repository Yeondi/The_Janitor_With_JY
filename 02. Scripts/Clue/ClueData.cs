[System.Serializable]
public class ClueData
{
    public int Id;
    public string Type;
    public string Content;
    public int Page;

    public ClueData(int id, string type, string content, int page)
    {
        Id = id;
        Type = type;
        Content = content;
        Page = page;
    }

    public int GetUniqueKey()
    {
        return Id * 1000 + Page;
    }
}
