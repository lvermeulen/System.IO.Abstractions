#if NETSTANDARD1_6 || NETCOREAPP1_0

// ReSharper disable once CheckNamespace
namespace System
{
    public class SerializableAttribute : Attribute
    { }

    public class NonSerializedAttribute : Attribute
    { }
}

#endif