public class EntityService
{
    private int _currentId = 0;
    
    public int GetNewEntityId()
    {
        var newEntityId = _currentId;
        _currentId++;
        return newEntityId;
    }
}
