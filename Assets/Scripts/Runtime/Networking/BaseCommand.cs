
using System;
using System.IO;


public abstract class BaseCommand
{
    public BaseCommand()
    {}

    public BaseCommand(BinaryReader binaryReader)
    {
        try
        {
            this.DeserializeCommand(binaryReader);
            this.ValidPackage = true;
        }
        catch (Exception ex)
        {
            this.ValidPackage = false;
            this.deserializeException = ex;
            Console.WriteLine(string.Format("Failed to deserialize package of type {0}, exception: {1}", base.GetType().Name, ex));
        }
    }

    public abstract void SerializeCommand();

    public abstract void DeserializeCommand(BinaryReader binaryReader);

    public bool ValidPackage;

    public Exception deserializeException;

    public byte[] Buffer;
}
