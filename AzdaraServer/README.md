# AzdaraServer.

>>>Dedicated to my beloved daughter.
She was great intellectual and philosopher for me, ahead of her time.
She was 18 years old.
Blessed memory of `☦️` Diana Aleksandrovna Stolpovskikh (October 25, 1998 - July 1, 2017) `☦️` aka "Azdara".
This application in honor of her was called her pseudonym - "Azdara".
>>>

This application is web service which provides access to the database through the protocol [OData](http://odata.org) [ver. 4](http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part1-protocol.html).
Web service can of the reading, inserting, deleting and updating your data through odata protocol.
You can use odata operations: `$select`, `$filter`, `$expand`, `$orderby`, `$skip`, '$top' and more.

## Settings Web.config

Add your connection string in section `connectionStrings`. 
Set value `nameConnection` for parameter `defaultConnectionString` in section `appSettings`. 

```xml
<connectionStrings>
	<add name="Chinook" connectionString="data source=.\SQLEXPRESS;initial catalog=Chinook;integrated security=True;MultipleActiveResultSets=True" providerName="System.Data.SqlClient" />
</connectionStrings>  
<appSettings>
	<add key="defaultConnectionString" value="Chinook" />
</appSettings>
```

Metadata will be auto generate after first(any) odata query to the database.

## Initialization

`AzdaraServer` must be initialization in next order:
```c#
//1. read structure of the mssql database
string prefix = "";
string defaultDbSchema = "dbo";
string connectionStringName = "your ConnectionString name";
AzdaraPOCO poco = new AzdaraPOCO(new CodeConfig() { prefixCSharp = prefix, defaultSchema = defaultDbSchema, folderName = connectionStringName});
poco.GetDbStructure(new DBSchemaSettings() { connectionStringName = connectionStringName, providerName = "System.Data.SqlClient" });
```

```c#
//2. generate classes *.cs 
poco.GenerateCSharpCode(new SchemaWriterSettings() { IsRegionProperties = true });
```

```c#
//3. build .dll
poco.BuildAssembly();
```