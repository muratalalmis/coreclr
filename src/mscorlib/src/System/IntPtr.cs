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
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct IntPtr : IComparable, IFormattable, ISerializable
            , IComparable<int>, IComparable<IntPtr>, IEquatable<int>, IEquatable<IntPtr>
    {
        unsafe private void* _value; // The compiler treats void* closest to uint hence explicit casts are required to preserve int behavior

        public static readonly IntPtr Zero = new IntPtr(0);

#if BIT64
        public static readonly IntPtr MaxValue = new IntPtr(long.MaxValue);
#else // !BIT64 (32)
        public static readonly IntPtr MaxValue = new IntPtr(int.MaxValue);
#endif

#if BIT64
        public static readonly IntPtr MinValue = new IntPtr(long.MinValue);
#else // !BIT64 (32)
        public static readonly IntPtr MinValue = new IntPtr(int.MinValue);
#endif

        // fast way to compare IntPtr to (IntPtr)0 while IntPtr.Zero doesn't work due to slow statics access
        [Pure]
        internal unsafe bool IsNull()
        {
            return (_value == null);
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe IntPtr(int value)
        {
#if BIT64
            _value = (void*)(long)value;
#else // !BIT64 (32)
            _value = (void*)value;
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe IntPtr(long value)
        {
#if BIT64
            _value = (void*)value;
#else // !BIT64 (32)
            _value = (void*)checked((int)value);
#endif
        }

        [CLSCompliant(false)]
        [System.Runtime.Versioning.NonVersionable]
        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        private unsafe IntPtr(SerializationInfo info, StreamingContext context)
        {
            long l = info.GetInt64("value");

#if BIT64
            Debug.Assert(Size == 8);
#else // !BIT64 (32)
            Debug.Assert(Size == 4);

            if (l > int.MaxValue || l < int.MinValue)
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

            info.AddValue("value", ToInt64());
        }

        // Compares this object to another object, returning an integer that indicates the relationship. 
        // null is considered to be less than any instance.
        // If object is not of type int or IntPtr, this method throws an ArgumentException.
        int IComparable.CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            else if (value is int i)
            {
                return CompareTo(i);
            }
            else if (value is IntPtr n)
            {
                return CompareTo(n);
            }
            else
            {
                throw new ArgumentException(SR.Arg_MustBeInt32OrIntPtr);
            }
        }

        public int CompareTo(int value)
        {
            return CompareTo((IntPtr)value);
        }

        public unsafe int CompareTo(IntPtr value)
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
            return (obj is int i) && Equals(i)
                || (obj is IntPtr n) && Equals(n);
        }

        public bool Equals(int other)
        {
            return (this == other);
        }

        public bool Equals(IntPtr other)
        {
            return (this == other);
        }

        public override int GetHashCode()
        {
#if BIT64
            long l = ToInt64();
            return unchecked((int)l ^ (int)(l >> 32));
#else // !BIT64 (32)
            return ToInt32();
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe int ToInt32()
        {
#if BIT64
            long value = (long)_value;
            return checked((int)value);
#else // !BIT64 (32)
            return (int)_value;
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe long ToInt64()
        {
#if BIT64
            return (long)_value;
#else // !BIT64 (32)
            return (long)(int)_value;
#endif
        }

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToInt64().ToString(CultureInfo.InvariantCulture);
#else // !BIT64 (32)
            return ToInt32().ToString(CultureInfo.InvariantCulture);
#endif
        }

        public string ToString(string format)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToInt64().ToString(format, CultureInfo.InvariantCulture);
#else // !BIT64 (32)
            return ToInt32().ToString(format, CultureInfo.InvariantCulture);
#endif
        }

        public string ToString(IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToInt64().ToString(provider);
#else // !BIT64 (32)
            return ToInt32().ToString(provider);
#endif
        }

        public string ToString(string format, IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);

#if BIT64
            return ToInt64().ToString(format, provider);
#else // !BIT64 (32)
            return ToInt32().ToString(format, provider);
#endif
        }

        public static IntPtr Parse(string s)
        {
#if BIT64
            return new IntPtr(long.Parse(s));
#else // !BIT64 (32)
            return new IntPtr(int.Parse(s));
#endif
        }

        public static IntPtr Parse(string s, NumberStyles style)
        {
#if BIT64
            return new IntPtr(long.Parse(s, style));
#else // !BIT64 (32)
            return new IntPtr(int.Parse(s, style));
#endif
        }

        public static IntPtr Parse(string s, IFormatProvider provider)
        {
#if BIT64
            return new IntPtr(long.Parse(s, provider));
#else // !BIT64 (32)
            return new IntPtr(int.Parse(s, provider));
#endif
        }

        public static IntPtr Parse(string s, NumberStyles style, IFormatProvider provider)
        {
#if BIT64
            return new IntPtr(long.Parse(s, style, provider));
#else // !BIT64 (32)
            return new IntPtr(int.Parse(s, style, provider));
#endif
        }

        public static bool TryParse(string s, out IntPtr result)
        {
#if BIT64
            bool succeeded = long.TryParse(s, out long l);
            result = new IntPtr(l);
            return succeeded;
#else // !BIT64 (32)
            bool succeeded = int.TryParse(s, out int i);
            result = new IntPtr(i);
            return succeeded;
#endif
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out IntPtr result)
        {
#if BIT64
            bool succeeded = long.TryParse(s, style, provider, out long l);
            result = new IntPtr(l);
            return succeeded;
#else // !BIT64 (32)
            bool succeeded = int.TryParse(s, style, provider, out int i);
            result = new IntPtr(i);
            return succeeded;
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public static explicit operator IntPtr(int value)
        {
            return new IntPtr(value);
        }

        [System.Runtime.Versioning.NonVersionable]
        public static explicit operator IntPtr(long value)
        {
            return new IntPtr(value);
        }

        [CLSCompliant(false), ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        [System.Runtime.Versioning.NonVersionable]
        public static unsafe explicit operator IntPtr(void* value)
        {
            return new IntPtr(value);
        }

        [CLSCompliant(false)]
        [System.Runtime.Versioning.NonVersionable]
        public static unsafe explicit operator void*(IntPtr value)
        {
            return value._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static explicit operator int(IntPtr value)
        {
            return value.ToInt32();
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static explicit operator long(IntPtr value)
        {
            return value.ToInt64();
        }

        [System.Runtime.Versioning.NonVersionable]
        public static bool operator ==(IntPtr left, int right)
        {
            return (left == (IntPtr)right);
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator ==(IntPtr left, IntPtr right)
        {
            return left._value == right._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator !=(IntPtr left, int right)
        {
            return (left != (IntPtr)right);
        }

        [System.Runtime.Versioning.NonVersionable]
        public unsafe static bool operator !=(IntPtr left, IntPtr right)
        {
            return left._value != right._value;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static IntPtr Add(IntPtr pointer, int offset)
        {
            return pointer + offset;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static IntPtr operator +(IntPtr pointer, int offset)
        {
#if BIT64
            return new IntPtr(pointer.ToInt64() + offset);
#else // !BIT64 (32)
            return new IntPtr(pointer.ToInt32() + offset);
#endif
        }

        [System.Runtime.Versioning.NonVersionable]
        public static IntPtr Subtract(IntPtr pointer, int offset)
        {
            return pointer - offset;
        }

        [System.Runtime.Versioning.NonVersionable]
        public static IntPtr operator -(IntPtr pointer, int offset)
        {
#if BIT64
            return new IntPtr(pointer.ToInt64() - offset);
#else // !BIT64 (32)
            return new IntPtr(pointer.ToInt32() - offset);
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


