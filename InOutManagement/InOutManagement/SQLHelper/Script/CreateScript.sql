create table "Input"("Identity" integer primary key autoincrement,
                     "InputDate" text not null,
                     "Material" integer not null,
                     "Count" real not null,
                     "Price" real not null,
                     "Warehouse" integer not null,
                     "BillArchive" text,
                     "Supplier" text not null,
                     "IsDeleated" Boolean );

create table "Output"("Identity" integer primary key autoincrement,
                      "Input" integer not null,
                      "Count" real not null,
                      "OutputDate" text not null,  
                      "Warehouse" integer not null,
                      "Pickup" text not null,                       
                      "BillArchive" text ,
                      "IsDeleated" Boolean
                      );

create table "Material"("Identity" integer primary key autoincrement,
                        "Name" text not null,
                        "Model" text not null,
                        "Unit" text not null);

create table "Warehouse"("Identity" integer primary key autoincrement,
                        "Name" text not null,
                        "IsDeleated" Boolean);

create view Query as
select input.Identity, input.InputDate as 'Date' ,input.Supplier as 'Supplier',warehouse.Name as 'Warehouse', input.Price as 'Price',input.Count as 'Count', "" as 'Pickup', material.Name as 'Name',material.Model as 'Model',material.Unit as 'Unit','入库' as 'Status'
from input as input,material as material, warehouse as warehouse
where input.Material = material.Identity and input.IsDeleated = 'False' and input.Warehouse = warehouse.Identity
union All
select output.Identity, output.OutputDate  as 'Date' ,input.Supplier  as 'Supplier',warehouse.Name as 'Warehouse', input.Price  as 'Price' , output.Count  as 'Count',output.Pickup  as 'Pickup',material.Name  as 'Name',material.Model  as 'Model',material.Unit  as 'Unit','出库' as 'Status'
from input as input,material as material,output as output , warehouse as warehouse
where output.Input = input.Identity and input.Material = material.Identity and  input.IsDeleated = 'False' and  output.IsDeleated = 'False' and output.Warehouse = warehouse.Identity
