// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
**
** Purpose: Platform independent integer
**
** 
===========================================================*/

namespace System
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security;

    [Serializable]
    [CLSCompliant(false), System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct UIntPtr : IComparable, IFormattable, ISerializable
            , IComparable<uint>, IComparable<UIntPtr>, IEquatable<uint>, IEquatable<UIntPtr>
    {
        unsafe private void* _value;

        public static readonly UIntPtr Zero = new UIntPtr(0);

#if BIT64
        public static readonly UIntPtr MaxValue = new IntPtr(ulong.MaxValue);
#else // !BIT64 (32)
        public static readonly UIntPtr MaxValue = new IntPtr(uint.MaxValue);
#endif

#if BIT64
        public static readonly UIntPtr MinValue = new IntPtr(ulong.MinValue);
#else // !BIT64 (32)
        public static readonly UIntPtr MinValue = new IntPtr(uint.MinValue);
#endif

        [System.Runtime.Versioning.NonVersionable]
        public unsafe UIntPtr(uint value)
        {
            _value = (void*)value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe UIntPtr(ulong value)
        {
#if BIT64
            _value = (void*)value;
#else // !BIT64 (32)
            _value = (void*)checked((uint)value);
#endif
        }

        [CLSCompliant(false)]
        [System.Runtime.Versioning.NonVersionable]
        public unsafe UIntPtr(void* value)
        {
            _value = value;
        }

        private unsafe UIntPtr(SerializationInfo info, StreamingContext context)
        {
            ulong l = info.GetUInt64("value");

#if BIT64
            Debug.Assert(Size == 8);
#else // !BIT64 (32)
            Debug.Assert(Size == 4);

            if (l > uint.MaxValue)
            {
                throw new ArgumentException(SR.Serialization_InvalidPtrValue);
            }
#endif

            _value = (void*)l;
        }

        unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            Contract.EndContractBlock();

            info.AddValue("value", ToUInt64());
        }

        // Compares this object to another object, returning an integer that indicates the relationship. 
        // null is considered to be less than any instance.
        // If object is not of type uint or UIntPtr, this method throws an ArgumentException.
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            else if (value is uint i)
            {
                return CompareTo(i);
            }
            else if (value is UIntPtr n)
            {
                return CompareTo(n);
            }
            else
            {
                throw new ArgumentException(SR.Arg_MustBeUInt32OrUIntPtr);
            }
        }

        public int CompareTo(uint value)
        {
            return CompareTo((UIntPtr)value);
        }

        public unsafe int CompareTo(UIntPtr value)
        {
            // Need to use compare because subtraction will wrap to positive for very large neg numbers, etc.
            if (_value < value._value)
            {
                return -1;
            }
            else if (_value > value._value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is uint i) && Equals(i)
                || (obj is UIntPtr n) && Equals(n);
        }

        public bool Equals(uint other)
        {
            return (this == other);
        }

        public bool Equals(UIntPtr other)
        {
            return (this == other);
        }

        public override int GetHashCode()
        {
#if BIT64
            ulong l = ToUInt64();
            return unchecked((int)l ^ (int)(l >> 32));
#else // !BIT64 (32)
            return ToInt32();
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe uint ToUInt32()
        {
#if BIT64
            return checked((uint)_value);
#else // !BIT64 (32)
            return (uint)_value;
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe ulong ToUInt64()
        {
            return (ulong)_value;
        }

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToUInt64().ToString(CultureInfo.InvariantCulture);
#else // !BIT64 (32)
            return ToUInt32().ToString(CultureInfo.InvariantCulture);
#endif
        }

        public string ToString(string format)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToUInt64().ToString(format, CultureInfo.InvariantCulture);
#else // !BIT64 (32)
            return ToUInt32().ToString(format, CultureInfo.InvariantCulture);
#endif
        }

        public string ToString(IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToUInt64().ToString(provider);
#else // !BIT64 (32)
            return ToUInt32().ToString(provider);
#endif
        }

        public string ToString(string format, IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToUInt64().ToString(format, provider);
#else // !BIT64 (32)
            return ToUInt32().ToString(format, provider);
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public static explicit operator UIntPtr(uint value)
        {
            return new UIntPtr(value);
        }

        [System.Runtime.Versioning.NonVersionable]
        public static explicit operator UIntPtr(ulong value)
        {
            return new UIntPtr(value);
        }

        [CLSCompliant(false), ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        [System.Runtime.Versioning.NonVersionable]
        public static unsafe explicit operator UIntPtr(void* value)
        {
            return new UIntPtr(value);
        }

        [CLSCompliant(false)]
        [System.Runtime.Versioning.NonVersionable]
        public static unsafe explicit operator void*(UIntPtr value)
        {
            return value._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static explicit operator uint(UIntPtr value)
        {
            return value.ToUInt32();
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static explicit operator ulong(UIntPtr value)
        {
            return value.ToUInt64();
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator ==(UIntPtr left, uint right)
        {
            return (left == (UIntPtr)right);
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator ==(UIntPtr left, UIntPtr right)
        {
            return left._value == right._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator !=(UIntPtr left, uint right)
        {
            return (left != (UIntPtr)right);
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator !=(UIntPtr left, UIntPtr right)
        {
            return left._value != right._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static UIntPtr Add(UIntPtr pointer, int offset)
        {
            return pointer + offset;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static UIntPtr operator +(UIntPtr pointer, int offset)
        {
#if BIT64
            return new UIntPtr(pointer.ToUInt64() + (ulong)offset);
#else // !BIT64 (32)
            return new UIntPtr(pointer.ToUInt32() + (uint)offset);
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public static UIntPtr Subtract(UIntPtr pointer, int offset)
        {
            return pointer - offset;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static UIntPtr operator -(UIntPtr pointer, int offset)
        {
#if BIT64
            return new UIntPtr(pointer.ToUInt64() - (ulong)offset);
#else // !BIT64 (32)
            return new UIntPtr(pointer.ToUInt32() - (uint)offset);
#endif
        }

        public static int Size
        {
            [Pure]
            [System.Runtime.Versioning.NonVersionable]
            get
            {
#if BIT64
                return 8;
#else // !BIT64 (32)
                return 4;
#endif
            }
        }

        [CLSCompliant(false)]
        [System.Runtime.Versioning.NonVersionable]
        public unsafe void* ToPointer()
        {
            return _value;
        }
    }
}


