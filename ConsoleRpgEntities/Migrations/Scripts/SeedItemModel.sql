-- Establish PlayerId
DECLARE @PlayerId INT = 1;

-- Insert into Inventory table
INSERT INTO Inventory (PlayerId)
VALUES (@PlayerId);

-- Establish InventoryId
DECLARE @InventoryId INT = SCOPE_IDENTITY();

-- Update Items
UPDATE Items
SET InventoryId = @InventoryId
WHERE Id = 1;

-- Update Sword from SeedEquipment to include weight & value
UPDATE Items
SET Value = 50, Weight = 4
WHERE Id = 1;