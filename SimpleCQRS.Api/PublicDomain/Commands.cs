using System;
using SimpleCQRS.Api.Concurrency;

namespace SimpleCQRS.Api.PublicDomain
{
    public class DeactivateInventoryItemCommand : IConcurrencyAware
    {
        public Guid Id { get; set; }
        public string ConcurrencyVersion { get; set; }
    }

    public class CreateInventoryItemCommand
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RenameInventoryItemCommand : IConcurrencyAware
    {
        public Guid Id { get; set; }
        public string ConcurrencyVersion { get; set; }
        public string NewName { get; set; }        
    }

    public class CheckInItemsToInventoryCommand
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }

    public class RemoveItemsFromInventoryCommand
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }
}