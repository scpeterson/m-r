using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public HttpResponseMessage Post(CreateInventoryItemCommand createInventoryItem)
        {
            if (!createInventoryItem.Id.HasValue)
                createInventoryItem.Id = Guid.NewGuid();

            _bus.Send(new CreateInventoryItem(createInventoryItem.Id.Value, createInventoryItem.Name));
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri(
                new Uri(Request.RequestUri.ToString().TrimEnd('/') + "/"), 
                createInventoryItem.Id.ToString());
            return response;
        }

        public HttpResponseMessage Delete(Guid id, DeactivateInventoryItemCommand deactivateInventoryItem)
        {

            int versionNumber = 0;
            int? ver = int.TryParse(deactivateInventoryItem.ConcurrencyVersion, out versionNumber)
                           ? (int?) versionNumber
                           : null;

            deactivateInventoryItem.Id = id;
            _bus.Send(new DeactivateInventoryItem(deactivateInventoryItem.Id,
                ver));

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        public HttpResponseMessage Put(Guid id, RenameInventoryItemCommnad renameInventoryItemCommnad)
        {
            int versionNumber = 0;
            int? ver = int.TryParse(renameInventoryItemCommnad.ConcurrencyVersion, out versionNumber)
                           ? (int?) versionNumber
                           : null;

            _bus.Send(new RenameInventoryItem(id,
                renameInventoryItemCommnad.NewName, ver));

            return Request.CreateResponse(HttpStatusCode.Accepted);

        }


        public HttpResponseMessage Post(Guid id, CheckInItemsToInventoryCommand checkInItemsToInventory)
        {
            _bus.Send(new CheckInItemsToInventory(id, 
                checkInItemsToInventory.Count
                ));

            return Request.CreateResponse(HttpStatusCode.Accepted);

        }

        public HttpResponseMessage Post(Guid id, RemoveItemsFromInventoryCommand removeItemsFromInventory)
        {

            _bus.Send(new RemoveItemsFromInventory(id,
                removeItemsFromInventory.Count));

            return Request.CreateResponse(HttpStatusCode.Accepted);

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