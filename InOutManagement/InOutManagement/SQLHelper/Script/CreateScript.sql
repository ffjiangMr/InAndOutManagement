create table "Input"("Identity" integer primary key autoincrement,
                     "InputDate" text not null,
                     "Material" integer not null,
                     "Count" integer not null,
                     "Price" real not null,
                     "BillArchive" text,
                     "Supplier" text not null );

create table "Output"("Identity" integer primary key autoincrement,
                      "OutputDate" text not null,
                      "Material" integer not null,
                      "Count" integer not null,
                      "BillArchive" text,
                      "Input" integer not null);

create table "Material"("Identity" integer primary key autoincrement,
                       "Name" text not null,
                       "Model" text not null,
                       "Unit" text not null);

create view Surplus as
select input.InputDate,input.Supplier,input.price,input.Count, input.BillArchive, material.Name,material.Model,material.Unit,'入库' as "状态"
from input as input,material as material
where input.Material = material.Identity
union All
select output.OutputDate,input.Supplier,input.price,output.Count,output.BillArchive,material.Name,material.Model,material.Unit,'出库' as "状态"
from input as input,material as material,output as output
where output.Input = input.Identity and input.Material = material.Identity


--create table "Supplier"("Identity" integer primary key autoincrement,
--                       "SupplierType" text not null);

--create table "Surplus"("Identity" integer primary key autoincrement,
--                       "Material" integer not null,
--                       "Count" integer not null,
--                       "Amount" real not null);