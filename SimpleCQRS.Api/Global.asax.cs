using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using SimpleCQRS.Api.App_Start;

namespace SimpleCQRS.Api
{
    public class Global : System.Web.HttpApplication
    {
        internal static FakeBus _bus = new FakeBus();
        internal static EventStore storage = new EventStore(_bus);
        internal static IRepository<InventoryItem> rep = new Repository<InventoryItem>(storage);
        internal static InventoryCommandHandlers commands = new InventoryCommandHandlers(rep);

        protected void Application_Start(object sender, EventArgs e)
        {

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            MvcConfig.RegisterRoutes(RouteTable.Routes);
            MvcConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            _bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            _bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            _bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            _bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            _bus.RegisterHandler<RenameInventoryItem>(commands.Handle);
            var detail = new InvenotryItemDetailView();
            _bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            _bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            _bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            _bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            _bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new InventoryListView();
            _bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            _bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            _bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
        }

    }
}