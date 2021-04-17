public class EntityService
{
    private int _currentId = 0;
    
    public int GetNewEntityId()
    {
        //Assigns and returns an ID value and increments currentID
        var newEntityId = _currentId;
        _currentId++;
        return newEntityId;
    }
}
