using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

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

        public void Post(SimpleCQRS.Api.PublicDomain.CreateInventoryItem createInventoryItem)
        {
            if (!createInventoryItem.Id.HasValue)
                createInventoryItem.Id = Guid.NewGuid();

            _bus.Send(new CreateInventoryItem(createInventoryItem.Id.Value, createInventoryItem.Name));
        }

        public void Put(DeactivateInventoryItem deactivateInventoryItem)
        {
            _bus.Send(deactivateInventoryItem);
        }

        public void Put(CheckInItemsToInventory checkInItemsToInventory)
        {
            _bus.Send(checkInItemsToInventory);
        }

        public void Put(RemoveItemsFromInventory removeItemsFromInventory)
        {
            _bus.Send(removeItemsFromInventory);
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