using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace JsonSrcGen
{
    public partial class JsonUtf8Builder : IJsonBuilder
    {
        byte[] _buffer = new byte[5];
        int _index = 0;

        public JsonUtf8Builder()
        {
            InitializeEscapingLookups();
        }

        void ResizeBuffer(int added)
        {
            var newSize = Math.Max(_buffer.Length * 2, _buffer.Length + added);
            var newArray = new byte[newSize];
            Array.Copy(_buffer, newArray, _buffer.Length);
            _buffer = newArray;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IJsonBuilder Append(string value)
        {
            if (_index + value.Length > _buffer.Length)
            {
                ResizeBuffer(value.Length);
            }

            _index += System.Text.Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_index));

            return this;
        }

        public IJsonBuilder Append(ReadOnlySpan<char> value)
        {
            if (_index + value.Length > _buffer.Length)
            {
                ResizeBuffer(value.Length);
            }

            _index += System.Text.Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_index));

            return this;
        }

        public IJsonBuilder AppendAscii(char value)
        {
            if (_index + 1 > _buffer.Length)
            {
                ResizeBuffer(1);
            }
            _buffer[_index] = (byte)value;
            _index += 1;
            return this;
        }

        public IJsonBuilder Append(byte value)
        {
            if (_index + 3 > _buffer.Length)
            {
                ResizeBuffer(3);
            }
            int intValue = value;
            
            if(intValue > 99) goto hundreds;
            if(intValue > 9) goto tens;
            else goto ones;

            hundreds:
            _buffer[_index++] = (byte)((intValue / 100) + '0');
            intValue = (intValue % 100);

            tens:
            _buffer[_index++] = (byte)((intValue / 10)  + '0');
            intValue = intValue % 10;

            ones:
            _buffer[_index++] = (byte)(intValue + '0');
            return this;
        }

        static byte[] _decimalPairs = new byte[]
        {
            (byte)'0',(byte)'0',(byte)'0',(byte)'1',(byte)'0',(byte)'2',(byte)'0',(byte)'3',(byte)'0',(byte)'4',(byte)'0',(byte)'5',(byte)'0',(byte)'6',(byte)'0',(byte)'7',(byte)'0',(byte)'8',(byte)'0',(byte)'9',
            (byte)'1',(byte)'0',(byte)'1',(byte)'1',(byte)'1',(byte)'2',(byte)'1',(byte)'3',(byte)'1',(byte)'4',(byte)'1',(byte)'5',(byte)'1',(byte)'6',(byte)'1',(byte)'7',(byte)'1',(byte)'8',(byte)'1',(byte)'9',
            (byte)'2',(byte)'0',(byte)'2',(byte)'1',(byte)'2',(byte)'2',(byte)'2',(byte)'3',(byte)'2',(byte)'4',(byte)'2',(byte)'5',(byte)'2',(byte)'6',(byte)'2',(byte)'7',(byte)'2',(byte)'8',(byte)'2',(byte)'9',
            (byte)'3',(byte)'0',(byte)'3',(byte)'1',(byte)'3',(byte)'2',(byte)'3',(byte)'3',(byte)'3',(byte)'4',(byte)'3',(byte)'5',(byte)'3',(byte)'6',(byte)'3',(byte)'7',(byte)'3',(byte)'8',(byte)'3',(byte)'9',
            (byte)'4',(byte)'0',(byte)'4',(byte)'1',(byte)'4',(byte)'2',(byte)'4',(byte)'3',(byte)'4',(byte)'4',(byte)'4',(byte)'5',(byte)'4',(byte)'6',(byte)'4',(byte)'7',(byte)'4',(byte)'8',(byte)'4',(byte)'9',
            (byte)'5',(byte)'0',(byte)'5',(byte)'1',(byte)'5',(byte)'2',(byte)'5',(byte)'3',(byte)'5',(byte)'4',(byte)'5',(byte)'5',(byte)'5',(byte)'6',(byte)'5',(byte)'7',(byte)'5',(byte)'8',(byte)'5',(byte)'9',
            (byte)'6',(byte)'0',(byte)'6',(byte)'1',(byte)'6',(byte)'2',(byte)'6',(byte)'3',(byte)'6',(byte)'4',(byte)'6',(byte)'5',(byte)'6',(byte)'6',(byte)'6',(byte)'7',(byte)'6',(byte)'8',(byte)'6',(byte)'9',
            (byte)'7',(byte)'0',(byte)'7',(byte)'1',(byte)'7',(byte)'2',(byte)'7',(byte)'3',(byte)'7',(byte)'4',(byte)'7',(byte)'5',(byte)'7',(byte)'6',(byte)'7',(byte)'7',(byte)'7',(byte)'8',(byte)'7',(byte)'9',
            (byte)'8',(byte)'0',(byte)'8',(byte)'1',(byte)'8',(byte)'2',(byte)'8',(byte)'3',(byte)'8',(byte)'4',(byte)'8',(byte)'5',(byte)'8',(byte)'6',(byte)'8',(byte)'7',(byte)'8',(byte)'8',(byte)'8',(byte)'9',
            (byte)'9',(byte)'0',(byte)'9',(byte)'1',(byte)'9',(byte)'2',(byte)'9',(byte)'3',(byte)'9',(byte)'4',(byte)'9',(byte)'5',(byte)'9',(byte)'6',(byte)'9',(byte)'7',(byte)'9',(byte)'8',(byte)'9',(byte)'9'
        };

        public IJsonBuilder Append(short value)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 6 > buffer.Length)
            {
                ResizeBuffer(6);
                buffer = _buffer;
            }

            int intValue = value;

            if(intValue < 0)
            {
                buffer[index++] = (byte)'-';
                intValue = intValue * -1;
            }

            int decimalIndex = 0;

            if(intValue > 9999) 
            {
                buffer[index++] = (byte)((intValue / 10000) + '0');
                intValue = (intValue % 10000);
                goto thousands;
            }
            if(intValue > 999) goto thousands;
            if(intValue > 99) 
            {
                buffer[index++] = (byte)((intValue / 100) + '0');
                intValue = (intValue % 100);
                goto tens;
            }
            if(intValue > 9) goto tens;

            buffer[index++] = (byte)(intValue + '0');
            goto end;

            thousands:
            int pair = intValue / 100;
            decimalIndex = pair*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100);

            tens:
            decimalIndex = intValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }

        public IJsonBuilder Append(ushort value)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 5 > buffer.Length)
            {
                ResizeBuffer(5);
                buffer = _buffer;
            }

            int intValue = value;
            int decimalIndex = 0;

            if(intValue > 9999) 
            {
                buffer[index++] = (byte)((intValue / 10000) + '0');
                intValue = (intValue % 10000);
                goto thousands;
            }
            if(intValue > 999) goto thousands;
            if(intValue > 99) 
            {
                buffer[index++] = (byte)((intValue / 100) + '0');
                intValue = (intValue % 100);
                goto tens;
            }
            if(intValue > 9) goto tens;

            buffer[index++] = (byte)(intValue + '0');
            goto end;

            thousands:
            int pair = intValue / 100;
            decimalIndex = pair*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100);

            tens:
            decimalIndex = intValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }
        public IJsonBuilder Append(int value)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 11 > buffer.Length)
            {
                ResizeBuffer(11);
                buffer = _buffer;
            }

            uint intValue = 0;

            if(value < 0)
            {
                buffer[index++] = (byte)'-';
                intValue = (uint)(value * -1);
            }
            else
            {
                intValue = (uint)value;
            }

            uint decimalIndex = 0;
            if(intValue > 999_999_999) goto billions;
            if(intValue > 99_999_999) 
            {
                buffer[index++] = (byte)((intValue / 100_000_000) + '0');
                intValue = (intValue % 100_000_000);
                goto tenMillions;
            }
            if(intValue > 99_99_999) goto tenMillions;
            if(intValue > 999_999) 
            {
                buffer[index++] = (byte)((intValue / 1_000_000) + '0');
                intValue = (intValue % 1_000_000);
                goto hundredThousands;
            }
            if(intValue > 99_999) goto hundredThousands;
            if(intValue > 9_999) 
            {
                buffer[index++] = (byte)((intValue / 10_000) + '0');
                intValue = (intValue % 10_000);
                goto thousands;
            }
            if(intValue > 999) goto thousands;
            if(intValue > 99) 
            {
                buffer[index++] = (byte)((intValue / 100) + '0');
                intValue = (intValue % 100);
                goto tens;
            }
            if(intValue > 9) goto tens;

            buffer[index++] = (byte)(intValue + '0');
            goto end;

            billions:
            decimalIndex = (intValue / 100_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100_000_000);

            tenMillions:
            decimalIndex = (intValue / 1_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 1_000_000);

            hundredThousands:
            decimalIndex = (intValue / 10_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 10_000);

            thousands:
            decimalIndex = (intValue / 100)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100);

            tens:
            decimalIndex = intValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }

        public IJsonBuilder Append(uint value)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 10 > buffer.Length)
            {
                ResizeBuffer(10);
                buffer = _buffer;
            }

            uint intValue = value;

            uint decimalIndex = 0;
            if(intValue > 999_999_999) goto billions;
            if(intValue > 99_999_999) 
            {
                buffer[index++] = (byte)((intValue / 100_000_000) + '0');
                intValue = (intValue % 100_000_000);
                goto tenMillions;
            }
            if(intValue > 99_99_999) goto tenMillions;
            if(intValue > 999_999) 
            {
                buffer[index++] = (byte)((intValue / 1_000_000) + '0');
                intValue = (intValue % 1_000_000);
                goto hundredThousands;
            }
            if(intValue > 99_999) goto hundredThousands;
            if(intValue > 9_999) 
            {
                buffer[index++] = (byte)((intValue / 10_000) + '0');
                intValue = (intValue % 10_000);
                goto thousands;
            }
            if(intValue > 999) goto thousands;
            if(intValue > 99) 
            {
                buffer[index++] = (byte)((intValue / 100) + '0');
                intValue = (intValue % 100);
                goto tens;
            }
            if(intValue > 9) goto tens;

            buffer[index++] = (byte)(intValue + '0');
            goto end;

            billions:
            decimalIndex = (intValue / 100_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100_000_000);

            tenMillions:
            decimalIndex = (intValue / 1_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 1_000_000);

            hundredThousands:
            decimalIndex = (intValue / 10_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 10_000);

            thousands:
            decimalIndex = (intValue / 100)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            intValue = (intValue % 100);

            tens:
            decimalIndex = intValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }

        public IJsonBuilder Append(long value)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 26 > buffer.Length)
            {
                ResizeBuffer(26);
                buffer = _buffer;
            }

            ulong longValue = 0;

            if(value < 0)
            {
                buffer[index++] = (byte)'-';
                longValue = (ulong)(value * -1);
            }
            else
            {
                longValue = (ulong)value;
            }

            ulong decimalIndex = 0;
            if(longValue > 999_999_999_999_999_999)
            {
                buffer[index++] = (byte)((longValue / 1_000_000_000_000_000_000) + '0');
                longValue = longValue % 1_000_000_000_000_000_000;
                goto tenQuadtrillions;
            }
            if(longValue > 99_999_999_999_999_999) goto tenQuadtrillions;
            if(longValue > 9_999_999_999_999_999)
            {
                buffer[index++] = (byte)((longValue / 10_000_000_000_000_000) + '0');
                longValue = longValue % 10_000_000_000_000_000;
                goto quadtrillions;
            }
            if(longValue > 999_999_999_999_999) goto quadtrillions;
            if(longValue > 99_999_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 100_000_000_000_000) + '0');
                longValue = longValue % 100_000_000_000_000;
                goto tenTrillions;
            }
            if(longValue > 9_999_999_999_999) goto tenTrillions;
            if(longValue > 999_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 1_000_000_000_000) + '0');
                longValue = longValue % 1_000_000_000_000;
                goto hundredBillions;
            }
            if(longValue > 99_999_999_999) goto hundredBillions;
            if(longValue > 9_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 10_000_000_000) + '0');
                longValue = longValue % 10_000_000_000;
                goto billions;
            }
            if(longValue > 999_999_999) goto billions;
            if(longValue > 99_999_999)
            {
                buffer[index++] = (byte)((longValue / 100_000_000) + '0');
                longValue = (longValue % 100_000_000);
                goto tenMillions;
            }
            if(longValue > 9_999_999) goto tenMillions;
            if(longValue > 999_999) 
            {
                buffer[index++] = (byte)((longValue / 1_000_000) + '0');
                longValue = (longValue % 1_000_000);
                goto hundredThousands;
            }
            if(longValue > 99_999) goto hundredThousands;
            if(longValue > 9_999) 
            {
                buffer[index++] = (byte)((longValue / 10_000) + '0');
                longValue = (longValue % 10_000);
                goto thousands;
            }
            if(longValue > 999) goto thousands;
            if(longValue > 99) 
            {
                buffer[index++] = (byte)((longValue / 100) + '0');
                longValue = (longValue % 100);
                goto tens;
            }
            if(longValue > 9) goto tens;

            buffer[index++] = (byte)(longValue + '0');
            goto end;

            tenQuadtrillions:
            decimalIndex = (longValue / 10_000_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 10_000_000_000_000_000;

            quadtrillions:
            decimalIndex = (longValue / 100_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 100_000_000_000_000;

            tenTrillions:
            decimalIndex = (longValue / 1_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 1_000_000_000_000;

            hundredBillions:
            decimalIndex = (longValue / 10_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 10_000_000_000;

            billions:
            decimalIndex = (longValue / 100_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 100_000_000);

            tenMillions:
            decimalIndex = (longValue / 1_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 1_000_000);

            hundredThousands:
            decimalIndex = (longValue / 10_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 10_000);

            thousands:
            decimalIndex = (longValue / 100)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 100);

            tens:
            decimalIndex = longValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }

        public IJsonBuilder Append(ulong longValue)
        {
            int index = _index;
            var buffer = _buffer;
            if (index + 26 > buffer.Length)
            {
                ResizeBuffer(26);
                buffer = _buffer;
            }
            // 9_223_372_036_854_775_807
            //18_446_744_073_709_551_615

            ulong decimalIndex = 0;
            if(longValue > 9_999_999_999_999_999_999) goto quintrillion;
            if(longValue > 999_999_999_999_999_999)
            {
                buffer[index++] = (byte)((longValue / 1_000_000_000_000_000_000) + '0');
                longValue = longValue % 1_000_000_000_000_000_000;
                goto tenQuadtrillions;
            }
            if(longValue > 99_999_999_999_999_999) goto tenQuadtrillions;
            if(longValue > 9_999_999_999_999_999)
            {
                buffer[index++] = (byte)((longValue / 10_000_000_000_000_000) + '0');
                longValue = longValue % 10_000_000_000_000_000;
                goto quadtrillions;
            }
            if(longValue > 999_999_999_999_999) goto quadtrillions;
            if(longValue > 99_999_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 100_000_000_000_000) + '0');
                longValue = longValue % 100_000_000_000_000;
                goto tenTrillions;
            }
            if(longValue > 9_999_999_999_999) goto tenTrillions;
            if(longValue > 999_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 1_000_000_000_000) + '0');
                longValue = longValue % 1_000_000_000_000;
                goto hundredBillions;
            }
            if(longValue > 99_999_999_999) goto hundredBillions;
            if(longValue > 9_999_999_999) 
            {
                buffer[index++] = (byte)((longValue / 10_000_000_000) + '0');
                longValue = longValue % 10_000_000_000;
                goto billions;
            }
            if(longValue > 999_999_999) goto billions;
            if(longValue > 99_999_999)
            {
                buffer[index++] = (byte)((longValue / 100_000_000) + '0');
                longValue = (longValue % 100_000_000);
                goto tenMillions;
            }
            if(longValue > 9_999_999) goto tenMillions;
            if(longValue > 999_999) 
            {
                buffer[index++] = (byte)((longValue / 1_000_000) + '0');
                longValue = (longValue % 1_000_000);
                goto hundredThousands;
            }
            if(longValue > 99_999) goto hundredThousands;
            if(longValue > 9_999) 
            {
                buffer[index++] = (byte)((longValue / 10_000) + '0');
                longValue = (longValue % 10_000);
                goto thousands;
            }
            if(longValue > 999) goto thousands;
            if(longValue > 99) 
            {
                buffer[index++] = (byte)((longValue / 100) + '0');
                longValue = (longValue % 100);
                goto tens;
            }
            if(longValue > 9) goto tens;

            buffer[index++] = (byte)(longValue + '0');
            goto end;

            quintrillion:
            decimalIndex = (longValue / 1_000_000_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 1_000_000_000_000_000_000;

            tenQuadtrillions:
            decimalIndex = (longValue / 10_000_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 10_000_000_000_000_000;

            quadtrillions:
            decimalIndex = (longValue / 100_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 100_000_000_000_000;

            tenTrillions:
            decimalIndex = (longValue / 1_000_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 1_000_000_000_000;

            hundredBillions:
            decimalIndex = (longValue / 10_000_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = longValue % 10_000_000_000;

            billions:
            decimalIndex = (longValue / 100_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 100_000_000);

            tenMillions:
            decimalIndex = (longValue / 1_000_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 1_000_000);

            hundredThousands:
            decimalIndex = (longValue / 10_000)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 10_000);

            thousands:
            decimalIndex = (longValue / 100)*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            longValue = (longValue % 100);

            tens:
            decimalIndex = longValue*2;
            buffer[index++] = _decimalPairs[decimalIndex++];
            buffer[index++] = _decimalPairs[decimalIndex];
            _index = index;
            return this;

            end:
            _index = index;
            
            return this;
        }

        public IJsonBuilder Append(float value)
        {
            if (_index + 19 > _buffer.Length)
            {
                ResizeBuffer(19);
            }

            Utf8Formatter.TryFormat(value, _buffer.AsSpan(_index), out int bytesWritten);
            _index += bytesWritten;
            return this;
        }

        public IJsonBuilder Append(double value)
        {
            if (_index + 19 > _buffer.Length)
            {
                ResizeBuffer(19);
            }
            Utf8Formatter.TryFormat(value, _buffer.AsSpan(_index), out int bytesWritten);
            _index += bytesWritten;
            return this;
        }

        public IJsonBuilder Append(decimal value)
        {
            if (_index + 30 > _buffer.Length)
            {
                ResizeBuffer(30);
            }
            Utf8Formatter.TryFormat(value, _buffer.AsSpan(_index), out int bytesWritten);
            _index += bytesWritten;
            return this;
        }

        const byte Dash = (byte)'-';

        const byte Zero = (byte)'0';
        const byte One = (byte)'1';
        const byte Two = (byte)'2';
        const byte Three = (byte)'3';
        const byte Four = (byte)'4';
        const byte Five = (byte)'5';
        const byte Six = (byte)'6';
        const byte Seven = (byte)'7';
        const byte Eight = (byte)'8';
        const byte Nine = (byte)'9';
        const byte A = (byte)'a';
        const byte B = (byte)'b';
        const byte C = (byte)'c';
        const byte D = (byte)'d';
        const byte E = (byte)'e';
        const byte F = (byte)'f';

        static readonly byte[] _lowerHexLookup = new byte[256]
        {
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F,
            Zero,One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A,B,C,D,E,F
        };
        static readonly byte[] _uperHexLookup = new byte[256]
        {
            Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,Zero,
            One,One,Zero,One,Zero,One,Zero,One,Zero,One,One,One,One,One,One,One,
            Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,Two,
            Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,Three,
            Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,Four,
            Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,Five,
            Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,Six,
            Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,Seven,
            Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,Eight,
            Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,Nine,
            A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,
            B,B,B,B,B,B,B,B,B,B,B,B,B,B,B,B,
            C,C,C,C,C,C,C,C,C,C,C,C,C,C,C,C,
            D,D,D,D,D,D,D,D,D,D,D,D,D,D,D,D,
            E,E,E,E,E,E,E,E,E,E,E,E,E,E,E,E,
            F,F,F,F,F,F,F,F,F,F,F,F,F,F,F,F
        };

        public IJsonBuilder Append(Guid value)
        {
            if (_index + 36 > _buffer.Length)
            {
                ResizeBuffer(36);
            }

            DecomposedGuid guidAsBytes = default;
            guidAsBytes.Guid = value;

            var destination = _buffer.AsSpan(_index);

            ///nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
            
            // this forces a jit bounds check (lookup get optimzed away) which means
            // the rest of the lookups don't need to be bounds checked
            { _ = destination[35]; }
            if (BitConverter.IsLittleEndian)
            {
                destination[0] = _uperHexLookup[guidAsBytes.Byte03];
                destination[1] = _lowerHexLookup[guidAsBytes.Byte03];
                destination[2] = _uperHexLookup[guidAsBytes.Byte02];
                destination[3] = _lowerHexLookup[guidAsBytes.Byte02];
                destination[4] = _uperHexLookup[guidAsBytes.Byte01];
                destination[5] = _lowerHexLookup[guidAsBytes.Byte01];
                destination[6] = _uperHexLookup[guidAsBytes.Byte00];
                destination[7] = _lowerHexLookup[guidAsBytes.Byte00];
            }
            else
            {
                destination[0] = _uperHexLookup[guidAsBytes.Byte00];
                destination[1] = _lowerHexLookup[guidAsBytes.Byte00];
                destination[2] = _uperHexLookup[guidAsBytes.Byte01];
                destination[3] = _lowerHexLookup[guidAsBytes.Byte01];
                destination[4] = _uperHexLookup[guidAsBytes.Byte02];
                destination[5] = _lowerHexLookup[guidAsBytes.Byte02];
                destination[6] = _uperHexLookup[guidAsBytes.Byte03];
                destination[7] = _lowerHexLookup[guidAsBytes.Byte03];
            }

            destination[8] = Dash;

            if (BitConverter.IsLittleEndian)
            {
                destination[9] = _uperHexLookup[guidAsBytes.Byte05];
                destination[10] = _lowerHexLookup[guidAsBytes.Byte05];
                destination[11] = _uperHexLookup[guidAsBytes.Byte04];
                destination[12] = _lowerHexLookup[guidAsBytes.Byte04];
            }
            else
            {
                destination[9] = _uperHexLookup[guidAsBytes.Byte04];
                destination[10] = _lowerHexLookup[guidAsBytes.Byte04];
                destination[11] = _uperHexLookup[guidAsBytes.Byte05];
                destination[12] = _lowerHexLookup[guidAsBytes.Byte05];
            }

            destination[13] = Dash;

            if (BitConverter.IsLittleEndian)
            {
                destination[14] = _uperHexLookup[guidAsBytes.Byte07];
                destination[15] = _lowerHexLookup[guidAsBytes.Byte07];
                destination[16] = _uperHexLookup[guidAsBytes.Byte06];
                destination[17] = _lowerHexLookup[guidAsBytes.Byte06];
            }
            else
            {
                destination[14] = _uperHexLookup[guidAsBytes.Byte06];
                destination[15] = _lowerHexLookup[guidAsBytes.Byte06];
                destination[16] = _uperHexLookup[guidAsBytes.Byte07];
                destination[17] = _lowerHexLookup[guidAsBytes.Byte07];
            }

            destination[18] = Dash;


            destination[19] = _uperHexLookup[guidAsBytes.Byte08];
            destination[20] = _lowerHexLookup[guidAsBytes.Byte08];
            destination[21] = _uperHexLookup[guidAsBytes.Byte09];
            destination[22] = _lowerHexLookup[guidAsBytes.Byte09];

            destination[23] = Dash;

            destination[24] = _uperHexLookup[guidAsBytes.Byte10];
            destination[25] = _lowerHexLookup[guidAsBytes.Byte10];
            destination[26] = _uperHexLookup[guidAsBytes.Byte11];
            destination[27] = _lowerHexLookup[guidAsBytes.Byte11];
            destination[28] = _uperHexLookup[guidAsBytes.Byte12];
            destination[29] = _lowerHexLookup[guidAsBytes.Byte12];
            destination[30] = _uperHexLookup[guidAsBytes.Byte13];
            destination[31] = _lowerHexLookup[guidAsBytes.Byte13];
            destination[32] = _uperHexLookup[guidAsBytes.Byte14];
            destination[33] = _lowerHexLookup[guidAsBytes.Byte14];
            destination[34] = _uperHexLookup[guidAsBytes.Byte15];
            destination[35] = _lowerHexLookup[guidAsBytes.Byte15];

            _index += 36;
            return this;
        }

                /// <summary>
        /// Used to provide access to the individual bytes of a GUID.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct DecomposedGuid
        {
            [FieldOffset(00)] public Guid Guid;
            [FieldOffset(00)] public byte Byte00;
            [FieldOffset(01)] public byte Byte01;
            [FieldOffset(02)] public byte Byte02;
            [FieldOffset(03)] public byte Byte03;
            [FieldOffset(04)] public byte Byte04;
            [FieldOffset(05)] public byte Byte05;
            [FieldOffset(06)] public byte Byte06;
            [FieldOffset(07)] public byte Byte07;
            [FieldOffset(08)] public byte Byte08;
            [FieldOffset(09)] public byte Byte09;
            [FieldOffset(10)] public byte Byte10;
            [FieldOffset(11)] public byte Byte11;
            [FieldOffset(12)] public byte Byte12;
            [FieldOffset(13)] public byte Byte13;
            [FieldOffset(14)] public byte Byte14;
            [FieldOffset(15)] public byte Byte15;
        }

        public void Clear()
        {
            _index = 0;
        }

        public override string ToString()
        {
            // return new string(_buffer.AsSpan(0, _index));
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> AsSpan()
        {
            return _buffer.AsSpan(0, _index);
        }

        bool[] _needsEscaping = new bool[128];
        string[] _escapeLookup = new string[128];

        void InitializeEscapingLookups()
        {
            for(int index = 0; index < 32; index++)
            {
                _needsEscaping[index] = true;
            }
            _needsEscaping['\"'] = true;

            _needsEscaping['\\'] = true;
            _needsEscaping['/'] = true;
            _needsEscaping['\b'] = true;
            _needsEscaping['\f'] = true;
            _needsEscaping['\n'] = true;
            _needsEscaping['\r'] = true;
            _needsEscaping['\t'] = true;

            for(int index = 0; index < 32; index++)
            {
                var hex = index.ToString("X4");
                _escapeLookup[index] = "\\u" + hex;
            }
            _escapeLookup['\"'] = "\\\"";
            _escapeLookup ['\\'] = "\\\\";
            _escapeLookup['/'] = "\\/";
            _escapeLookup['\b'] = "\\b";
            _escapeLookup['\f'] = "\\f";
            _escapeLookup['\n'] = "\\n";
            _escapeLookup['\r'] = "\\r";
            _escapeLookup['\t'] = "\\t";
        }

        public IJsonBuilder AppendEscaped(char input)
        {
            if(input < 93 && _needsEscaping[input])
            {
                return Append(_escapeLookup[input]);
            }
            return Append(input.ToString());
        }

        public IJsonBuilder AppendEscaped(string input)
        {
            int start = 0;
            for(int index = 0; index < input.Length; index++)
            {
                char character = input[index];

                if(character < 93 && _needsEscaping[character])
                {
                    this.Append(input.AsSpan(start, index - start));
                    this.Append(_escapeLookup[character]);
                    start = index + 1;
                }
            }
            return this
                .Append(input.AsSpan(start, input.Length - start));
        }

        byte[] _offset = new byte[6];
        int _offsetCacheTime = 0;

        byte[] GetOffset()
        {
            int tickCount = Environment.TickCount;
            if(_offsetCacheTime + 1000 < tickCount)
            {
                _offsetCacheTime = tickCount;
                var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                var builder = new JsonUtf8Builder();
                if(offset.TotalMinutes > 0) builder.AppendAscii('+');
                else builder.AppendAscii('-');
                builder.AppendIntTwo(Math.Abs(offset.Hours));
                builder.AppendAscii(':');
                builder.AppendIntTwo(offset.Minutes);
                var offsetSpan = builder.AsSpan();
                for(int index = 0; index < 6; index++)
                {
                    _offset[index] = offsetSpan[index];
                }
            }
            return _offset;
        }

        IJsonBuilder AppendOffset(TimeSpan offset)
        {
            if (_index + 6 > _buffer.Length)
            {
                ResizeBuffer(6);
            }
            if(offset.TotalMinutes >= 0) _buffer[_index++] = (byte)'+';
            else _buffer[_index++] = (byte)'-';
            AppendIntTwo(Math.Abs(offset.Hours));
            _buffer[_index++] = (byte)':';
            AppendIntTwo(Math.Abs(offset.Minutes));
            return this;
        }

        public IJsonBuilder AppendDate(DateTime date)
        {
            if (_index + 38 > _buffer.Length)
            {
                ResizeBuffer(38);
            }
            var buffer = _buffer;
            buffer[_index++] = (byte)'\"'; 
            AppendIntFour(date.Year);
            buffer[_index++] = (byte)'-'; 
            AppendIntTwo(date.Month);
            buffer[_index++] = (byte)'-'; 
            AppendIntTwo(date.Day);
            buffer[_index++] = (byte)'T'; 
            AppendIntTwo(date.Hour);
            buffer[_index++] = (byte)':';
            AppendIntTwo(date.Minute);
            buffer[_index++] = (byte)':';
            AppendIntTwo(date.Second);
            var fractions = date.Ticks % TimeSpan.TicksPerSecond * TimeSpan.TicksPerMillisecond;
            if(fractions != 0)
            {
                buffer[_index++] = (byte)'.';
                AppendDateTimeFraction(fractions);
            }

            //offset
            if(date.Kind == DateTimeKind.Utc)
            {
                return Append("Z\"");
            }
            else if(date.Kind == DateTimeKind.Unspecified)
            {
                buffer[_index++] = (byte)'\"';
                return this;
            }
            var offset = GetOffset();
            for(int index = 0; index < offset.Length; index++)
            {
                buffer[_index++] = offset[index];
            }
            buffer[_index++] = (byte)'\"';
            
            return this;
        }

        public IJsonBuilder AppendDateTimeOffset(DateTimeOffset date)
        {
            if (_index + 38 > _buffer.Length)
            {
                ResizeBuffer(38);
            }
            var buffer = _buffer;
            buffer[_index++] = (byte)'\"'; 
            AppendIntFour(date.Year);
            buffer[_index++] = (byte)'-'; 
            AppendIntTwo(date.Month);
            buffer[_index++] = (byte)'-'; 
            AppendIntTwo(date.Day);
            buffer[_index++] = (byte)'T'; 
            AppendIntTwo(date.Hour);
            buffer[_index++] = (byte)':';
            AppendIntTwo(date.Minute);
            buffer[_index++] = (byte)':';
            AppendIntTwo(date.Second);
            var fractions = date.Ticks % TimeSpan.TicksPerSecond * TimeSpan.TicksPerMillisecond;
            if(fractions != 0)
            {
                buffer[_index++] = (byte)'.';
                AppendDateTimeFraction(fractions);
            }

            //offset
            AppendOffset(date.Offset);
            buffer[_index++] = (byte)'\"';
            
            return this;
        }

        JsonUtf8Builder AppendIntTwo(int number)
        {
            if (_index + 2 > _buffer.Length)
            {
                ResizeBuffer(2);
            }
            int tens = number/10;
            int soFar = tens*10;
            _buffer[_index] = (byte)('0' + tens);
            _index++;

            int ones = number - soFar;
            _buffer[_index] = (byte)('0' + ones);
            _index++;
            return this;
        }

        JsonUtf8Builder AppendIntFour(int number)
        {
            if (_index + 4 > _buffer.Length)
            {
                ResizeBuffer(4);
            }
            int thousands = (number)/1000;
            int soFar = thousands*1000;
            _buffer[_index] = (byte)('0' + thousands);
            _index++;

            int hundreds = (number-soFar)/100;
            soFar += hundreds*100;
            _buffer[_index] = (byte)('0' + hundreds);
            _index++;
            
            int tens = (number - soFar)/10;
            soFar += tens*10;
            _buffer[_index] = (byte)('0' + tens);
            _index++;

            int ones = number - soFar;
            _buffer[_index] = (byte)('0' + ones);
            _index++;
            return this;
        }

        JsonUtf8Builder AppendDateTimeFraction(long number)
        {
            if (_index + 11 > _buffer.Length)
            {
                ResizeBuffer(11);
            }

            //24 973 300 000
            long soFar = 0;

            long tenBillions = (number)/10000000000;
            soFar += tenBillions*10000000000;
            _buffer[_index] = (byte)('0' + tenBillions);
            _index++;
            long leftLong = number-soFar;
            if(leftLong == 0) return this;

            long billions = leftLong/1000000000;
            _buffer[_index] = (byte)('0' + billions);
            _index++;
            int left = (int)(leftLong-(billions*1000000000));
            if(left == 0) return this;

            int hundredMillions = left/100000000;
            _buffer[_index] = (byte)('0' + hundredMillions);
            _index++;
            left = left-(hundredMillions*100000000);
            if(left == 0) return this;

            int tenMillions = left/10000000;
            _buffer[_index] = (byte)('0' + tenMillions);
            _index++;
            left = left-(tenMillions*10000000);
            if(left == 0) return this;

            int millions = left/1000000;
            _buffer[_index] = (byte)('0' + millions);
            _index++;
            left = left-(millions*1000000);
            if(left == 0) return this;

            int hundredThousands = left/100000;
            _buffer[_index] = (byte)('0' + hundredThousands);
            _index++;
            left = left-(hundredThousands*100000);
            if(left == 0) return this;

            int tenThousands = left/10000;
            _buffer[_index] = (byte)('0' + tenThousands);
            _index++;
            left = left-(tenThousands*10000);
            if(left == 0) return this;

            int thousands = left/1000;
            _buffer[_index] = (byte)('0' + thousands);
            _index++;
            left = left-(thousands*1000);
            if(left == 0) return this;

            int hundreds = left/100;
            _buffer[_index] = (byte)('0' + hundreds);
            _index++;
            left = left-(hundreds*100);
            if(left == 0) return this;

            int tens = left/10;
            _buffer[_index] = (byte)('0' + tens);
            _index++;
            left = left-(tens*10);
            if(left == 0) return this;

            int ones = left;
            _buffer[_index] = (byte)('0' + ones);
            _index++;
            return this;
        }

        public IJsonBuilder AppendBytes(ReadOnlySpan<byte> input)
        {
            if (_index + input.Length > _buffer.Length)
            {
                ResizeBuffer(input.Length );
            }

            for(int index = 0; index < input.Length; index++)
            {
                _buffer[index + _index] = input[index];
            }

            _index += input.Length;
            return this;
        }

        public IJsonBuilder AppendPreset()
        {
            if (_index + 10 > _buffer.Length)
            {
                ResizeBuffer(10);
            }

            var span = _buffer.AsSpan();

            { _ = span[9]; }

            span[0] = 0;
            span[1] = 1;
            span[2] = 2;
            span[3] = 3;
            span[4] = 4;
            span[5] = 5;
            span[6] = 6;
            span[7] = 7;
            span[8] = 8;
            span[9] = 9;

            _index += 10;
            return this;
        }
    }
}