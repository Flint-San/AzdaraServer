namespace Azdara.CodeGenerator.Interfaces
{
    public interface IDbSchemaClassesWriter
    {
        void WriteColumns();
        void WritePrimaryKeys();
        void WriteForeignKeys();
    }
}
