# reset the db
dotnet ef database drop; dotnet ef database update

# http command from HTTPie

# Add some buyable items
echo '{
    "name": "AMD Ryzen Threadripper PRO 7995WX",
    "manufacturer": "AMD",
    "price": 164.00,
    "category": "Cpu",
    "details": {
        "Cores": "96",
        "IntegratedGraphics": "false",
        "Socket": "sTR5",
        "Series": "AMD Ryzen PRO Processors"
    }
}' | http -v POST ":5148/api/Item"

echo '{
    "name": "Intel Core i5-7600K",
    "manufacturer": "Intel",
    "price": 164.00,
    "category": "Cpu",
    "details": {
        "Cores": "4",
        "IntegratedGraphics": "true",
        "Socket": "LGA1151",
        "Series": "Intel Core i5"
    }
}' | http -v POST ":5148/api/Item"

echo '{
    "Name": "CRYORIG H5 Universal 65 CFM CPU Cooler",
    "Manufacturer": "CRYORIG",
    "Price": 19.99,
    "Category": "CpuCooler",
    "details": {
        "Socket": "LGA1151",
        "IsWaterCooled": "false",
        "Size": "160 mm"
    }
}' | http -v POST ":5148/api/Item"

echo '{
    "Name": "Gigabyte GA-Z270X-Gaming K7 ATX LGA1151 Motherboard",
    "Manufacturer": "Gigabyte",
    "Price": 348.68,
    "Category": "Motherboard",
    "details": {
        "Socket": "LGA1151",
        "Chipset": "Intel Z270",
        "MemoryType": "DDR4",
        "FormFactor": "ATX"
    }
}' | http -v POST ":5148/api/Item"

# Get all items
http -v GET ":5148/api/Item/GetAllItems"

# Filter for a specific item
http -v GET ":5148/api/Item/Filter/cpu?searchTerm=intel"

# Get a specific items details
http -v GET ":5148/api/Item/3"

# 1 Cpu
http -v POST ":5148/api/AddItemToCart/1" Id:=2 Quantity:=1
# 2 Coolers
http -v POST ":5148/api/AddItemToCart/1" Id:=3 Quantity:=2
# 1 Motherboard
http -v POST ":5148/api/AddItemToCart/1" Id:=4 Quantity:=1

# Get the cart
http -v GET ":5148/api/GetCart/1"

# Get just the totals
http -v GET ":5148/api/GetTotals/1"

# Pay
echo '{
    "CartId": 1,
    "CardNumber": 3782822463100053,
    "Exp": "11/24",
    "CardHolderName": "John Smith",
    "Cvv": 241
}' | http -v POST ":5148/api/ProcessPayment"

# Delete an item from the inventory
http -v DELETE ":5148/api/Item/2"

# Test
dotnet test