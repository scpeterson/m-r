using System;
namespace SimpleCQRS
{
	public class Command : Message
	{
	}
	
	public class DeactivateInventoryItem : Command {

		public readonly Guid InventoryItemId;
	    public readonly int? OriginalVersion;

	    public DeactivateInventoryItem(Guid inventoryItemId, int? originalVersion)
	    {
	        InventoryItemId = inventoryItemId;
	        OriginalVersion = originalVersion;
	    }
	}
	
	public class CreateInventoryItem : Command {
		public readonly Guid InventoryItemId;
		public readonly string Name;
	    
	    public CreateInventoryItem(Guid inventoryItemId, string name)
        {
			InventoryItemId = inventoryItemId;
			Name = name;
        }
	}
	
	public class RenameInventoryItem : Command {
		public readonly Guid InventoryItemId;
		public readonly string NewName;
	    public readonly int? OriginalVersion;

	    public RenameInventoryItem(Guid inventoryItemId, string newName, int? originalVersion = null)
        {
			InventoryItemId = inventoryItemId;
			NewName = newName;
            OriginalVersion = originalVersion;
        }
	}

	public class CheckInItemsToInventory : Command {
		public Guid InventoryItemId;
		public readonly int Count;

	    public CheckInItemsToInventory(Guid inventoryItemId, int count) {
			InventoryItemId = inventoryItemId;
			Count = count;
		}
	}
	
	public class RemoveItemsFromInventory : Command {
		public Guid InventoryItemId;
		public readonly int Count;

	    public RemoveItemsFromInventory(Guid inventoryItemId, int count)
        {
			InventoryItemId = inventoryItemId;
			Count = count;
        }
	}
}
