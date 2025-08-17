using System;
using System.Reflection;

namespace Precursor.Serialization
{
    public static class ObjectEnumHelper
    {
        public static bool IsEnumDefined(Type type, object value)
        {
            if (type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                return (ValueToUInt64(value) & ~GetEnumBitMask(type)) == 0;
            }

            return Enum.IsDefined(type, value);
        }

        public static ulong GetEnumBitMask(Type enumType)
        {
            ulong mask = 0UL;

            foreach (var v in Enum.GetValues(enumType)) 
            {
                mask |= ValueToUInt64(v);
            }

            return mask;
        }

        public static ulong ValueToUInt64(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte: 
                    return (byte)(sbyte)value;
                case TypeCode.Int16: 
                    return (ushort)(short)value;
                case TypeCode.Int32: 
                    return (uint)(int)value;
                case TypeCode.Int64: 
                    return (ulong)(long)value;
                case TypeCode.Byte: 
                    return (byte)value;
                case TypeCode.UInt16: 
                    return (ushort)value;
                case TypeCode.UInt32: 
                    return (uint)value;
                case TypeCode.UInt64: 
                    return (ulong)value;
                default: throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
