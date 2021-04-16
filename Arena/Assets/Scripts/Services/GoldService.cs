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
        return 0;
    }

    public void SpawnGold(int amount)
    {
        int entityId = _entityService.GetNewEntityId();

        GoldData goldData = new GoldData();
        goldData.Amount = amount;
        goldData.EntityId = entityId;
        _gold.Add(goldData);
        
        var message = new SpawnGoldMessage();
        message.Amount = amount;
        message.EntityId = entityId;
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

public class DestroyGoldMessage
{
    public int EntityId { get; set; }
}

public class SpawnGoldMessage
{
    public int EntityId { get; set; }
    public int Amount { get; set; }
}