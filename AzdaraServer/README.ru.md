# AzdaraServer
>>>Посвящается моей горячо любимой дочери.
Выдающемуся интеллектуалу и философу опередившей своё время. Ей было только 18...
Светлой памяти `☦️` Дианы Александровны Столповских `☦️` (25 октября 1998 - 2 июля 2017), также известной под псевдонимом - "Azdara".
Эта программа(приложение) в честь неё была названо её псевдонимом - "Azdara".
>>>

Данное приложение является веб-сервисом предоставляющим информацию из любых таблиц баз данных MS SQL по протоколу [OData](http://odata.org) [версии 4](http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part1-protocol.html).
Автоматически доступны операции чтения, фильтрации, записи, обновления и многое другое!

## Настройка
В `Web.config` в секции `connectionStrings` добавьте свою строку подключения к вашей базе данных и в `defaultConnectionString`
укажите данное имя строки подключения по умолчанию.
```xml
<connectionStrings>
	<add name="Chinook" connectionString="data source=.\SQLEXPRESS;initial catalog=Chinook;integrated security=True;MultipleActiveResultSets=True" providerName="System.Data.SqlClient" />
</connectionStrings>  
<appSettings>
	<add key="defaultConnectionString" value="Chinook" />
</appSettings>
```
Схемы таблиц будут автоматически прочитаны, после первого(любого) запроса odata.
Сервис автоматически сгенерирует строготипизированных c# классы в виде `*.cs` файлов размещенных в подкаталоге Edm.
А также автоматически построет из них библиотеку в виде `dll`.
В результате таблицы из схемы базы данных станут доступны для использовании их с odata запросами.

**Важно!** К каталогу веб-приложения должен быть обеспечен полный доступ от имени IIS пользователей на чтение/запись.
>>>Примечание: Первый запрос к сервису будет медленным, зависит от скорости подъёма веб-пула и считываемой схемы базы данных.

## Иницилизация
`AzdaraServer` должен быть проиницилизирован в следующем порядке:
```c#
//1. read structure of the mssql database
string prefix = "";
string defaultDbSchema = "dbo";
string connectionStringName = "your ConnectionString name";
AzdaraPOCO poco = new AzdaraPOCO(new CodeConfig() { prefixCSharp = prefix, defaultSchema = defaultDbSchema });
poco.GetDbStructure(new DBSchemaSettings() { connectionStringName = connectionStringName, providerName = "System.Data.SqlClient" });
```

```c#
//2. generate classes *.cs 
poco.GenerateAndBuildCSharpCode(new SchemaWriterSettings() { IsRegionProperties = true });
```

```c#
//3. build .dll
poco.BuildAssembly();
```
