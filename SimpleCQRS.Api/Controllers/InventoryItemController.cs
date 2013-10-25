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
            _bus = new FakeBus();
            _readmodel = new ReadModelFacade();
        }

        public void Post(CreateInventoryItem createInventoryItem)
        {
            _bus.Send(createInventoryItem);
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