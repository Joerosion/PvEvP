using System.Collections.Generic;
using Frictionless;

public class GoldService
{
    private List<GoldData> _gold = new List<GoldData>();
    private MessageRouter _messageRouter;
    private EntityService _entityService;

    public GoldService()
    {
        _messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        _entityService = ServiceFactory.Instance.GetService<EntityService>();
    }
    
    public int GetGoldAmount(int entityId)
    {
        for (int i = 0; i < _gold.Count; ++i)
        {
            if (_gold[i].EntityId == entityId)
            {
                return _gold[i].Amount;
            }
        }

        //What do we do about this case
        return 0;
    }

    public void SpawnGold(int amount)
    {

        //Calling on enityService to create a new ID
        int entityId = _entityService.GetNewEntityId();

        //Creating a new piece of GoldData and assigning it an amount and ID
        GoldData goldData = new GoldData();
        goldData.Amount = amount;
        goldData.EntityId = entityId;
        //adding this new gold to the List of gold in current Model
        _gold.Add(goldData);
        
        //creating a new spawn gold message and assgining the same amount and ID
        var message = new SpawnGoldMessage();
        message.Amount = amount;                //do we need to pass the amount in the message
        message.EntityId = entityId;
        //sending out messsage
        _messageRouter.RaiseMessage(message);
    }

    public void DestroyGold(int entityId)
    {
        for (int i = 0; i < _gold.Count; ++i)
        {
            if (_gold[i].EntityId == entityId)
            {
                var message = new DestroyGoldMessage();
                message.EntityId = _gold[i].EntityId;
                _messageRouter.RaiseMessage(message);
                _gold.RemoveAt(i);
                break;
            }
        }
    }
}
