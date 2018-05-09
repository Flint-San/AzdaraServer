namespace Azdara.Metadata.Interfaces
{
    public interface IDbSchemaConnection
    {
        void OpenConnection();
        void Dispose(bool disposing);

        void ReadTables();
        void ReadColumns();
        void ReadPrimaryKeys();
        void ReadForeignKeys();

        void ReadAll();
    }
}
