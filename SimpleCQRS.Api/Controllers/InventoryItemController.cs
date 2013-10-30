using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SimpleCQRS.Api.PublicDomain;

namespace SimpleCQRS.Api.Controllers
{
    public class InventoryItemController : ApiController, IReadModelFacade
    {
        private FakeBus _bus;
        private ReadModelFacade _readmodel;

        public InventoryItemController()
        {
            _bus = Global._bus;
            _readmodel = new ReadModelFacade();
        }

        public void Post(CreateInventoryItemCommand createInventoryItem)
        {
            if (!createInventoryItem.Id.HasValue)
                createInventoryItem.Id = Guid.NewGuid();

            _bus.Send(new CreateInventoryItem(createInventoryItem.Id.Value, createInventoryItem.Name));
        }

        public void Put(DeactivateInventoryItemCommand deactivateInventoryItem)
        {
            _bus.Send(new DeactivateInventoryItem(deactivateInventoryItem.Id, 
                Convert.ToInt32(deactivateInventoryItem.ConcurrencyVersion)));
        }

        public void Put(CheckInItemsToInventoryCommand checkInItemsToInventory)
        {
            _bus.Send(new CheckInItemsToInventory(checkInItemsToInventory.Id, 
                checkInItemsToInventory.Count
                ));
        }

        public void Put(RemoveItemsFromInventoryCommand removeItemsFromInventory)
        {

            _bus.Send(new RemoveItemsFromInventory(removeItemsFromInventory.Id,
                removeItemsFromInventory.Count));
        }


        public IEnumerable<InventoryItemListDto> GetInventoryItems()
        {
            return _readmodel.GetInventoryItems();
        }

        public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
        {
            return _readmodel.GetInventoryItemDetails(id);
        }
    }
}