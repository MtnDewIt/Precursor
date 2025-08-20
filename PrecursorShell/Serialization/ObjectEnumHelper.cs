using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PrecursorShell.Serialization
{
    public static class ObjectEnumHelper
    {
        private static readonly ConcurrentDictionary<Type, EnumInfo> EnumCache = new();

        private readonly struct EnumInfo
        {
            public readonly bool IsFlags;
            public readonly ulong ValidMask;
            public readonly ulong[] ValidValues;

            public EnumInfo(bool isFlags, ulong validMask, ulong[] validValues)
            {
                IsFlags = isFlags;
                ValidMask = validMask;
                ValidValues = validValues;
            }
        }

        public static bool IsEnumDefined(Type type, object value)
        {
            var enumInfo = GetEnumInfo(type);
            var ulongValue = ValueToUInt64(value);

            if (enumInfo.IsFlags)
            {
                return (ulongValue & ~enumInfo.ValidMask) == 0;
            }

            return ContainsValue(enumInfo.ValidValues, ulongValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsValue(ulong[] values, ulong target)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == target) 
                {
                    return true;
                }
            }

            return false;
        }

        private static EnumInfo GetEnumInfo(Type enumType)
        {
            return EnumCache.GetOrAdd(enumType, static type =>
            {
                var isFlags = type.GetCustomAttribute<FlagsAttribute>() != null;
                var values = Enum.GetValues(type);
                var ulongValues = new ulong[values.Length];
                ulong mask = 0UL;

                for (int i = 0; i < values.Length; i++)
                {
                    var ulongValue = ValueToUInt64Unsafe(values.GetValue(i));
                    ulongValues[i] = ulongValue;
                    mask |= ulongValue;
                }

                return new EnumInfo(isFlags, mask, ulongValues);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong ValueToUInt64(object value)
        {
            if (value is int intVal)
                return (uint)intVal;
            if (value is uint uintVal)
                return uintVal;
            if (value is byte byteVal)
                return byteVal;
            if (value is long longVal)
                return (ulong)longVal;
            if (value is ulong ulongVal)
                return ulongVal;
            if (value is short shortVal)
                return (ushort)shortVal;
            if (value is ushort ushortVal)
                return ushortVal;
            if (value is sbyte sbyteVal)
                return (byte)sbyteVal;

            return ValueToUInt64Slow(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong ValueToUInt64Unsafe(object value)
        {
            var type = value.GetType();

            if (type == typeof(int))
                return (uint)Unsafe.Unbox<int>(value);
            if (type == typeof(uint))
                return Unsafe.Unbox<uint>(value);
            if (type == typeof(byte))
                return Unsafe.Unbox<byte>(value);
            if (type == typeof(long))
                return (ulong)Unsafe.Unbox<long>(value);
            if (type == typeof(ulong))
                return Unsafe.Unbox<ulong>(value);
            if (type == typeof(short))
                return (ushort)Unsafe.Unbox<short>(value);
            if (type == typeof(ushort))
                return Unsafe.Unbox<ushort>(value);
            if (type == typeof(sbyte))
                return (byte)Unsafe.Unbox<sbyte>(value);

            return ValueToUInt64Slow(value);
        }

        private static ulong ValueToUInt64Slow(object value)
        {
            return Type.GetTypeCode(value.GetType()) switch
            {
                TypeCode.SByte => (byte)(sbyte)value,
                TypeCode.Int16 => (ushort)(short)value,
                TypeCode.Int32 => (uint)(int)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.Byte => (byte)value,
                TypeCode.UInt16 => (ushort)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }
    }
}
