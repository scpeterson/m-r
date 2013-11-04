using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public void Delete(Guid id, DeactivateInventoryItemCommand deactivateInventoryItem)
        {

            int versionNumber = 0;
            int? ver = int.TryParse(deactivateInventoryItem.ConcurrencyVersion, out versionNumber)
                           ? (int?) versionNumber
                           : null;

            deactivateInventoryItem.Id = id;
            _bus.Send(new DeactivateInventoryItem(deactivateInventoryItem.Id,
                ver));
        }

        public void Put(Guid id, RenameInventoryItemCommnad renameInventoryItemCommnad)
        {
            int versionNumber = 0;
            int? ver = int.TryParse(renameInventoryItemCommnad.ConcurrencyVersion, out versionNumber)
                           ? (int?) versionNumber
                           : null;

            _bus.Send(new RenameInventoryItem(id,
                renameInventoryItemCommnad.NewName, ver));
        }


        public void Post(Guid id, CheckInItemsToInventoryCommand checkInItemsToInventory)
        {
            _bus.Send(new CheckInItemsToInventory(id, 
                checkInItemsToInventory.Count
                ));
        }

        public void Post(Guid id, RemoveItemsFromInventoryCommand removeItemsFromInventory)
        {

            _bus.Send(new RemoveItemsFromInventory(id,
                removeItemsFromInventory.Count));
        }


        public IEnumerable<InventoryItemListDto> GetInventoryItems()
        {
            return _readmodel.GetInventoryItems();
        }

  
        public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
        {
            var detailsDto = _readmodel.GetInventoryItemDetails(id);
            if (detailsDto == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return detailsDto;
        }
    }
}