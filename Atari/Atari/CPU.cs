using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atari
{
    class CPU
    {

        //REGISTERS
        byte _regA; // Accumulator
        byte _regX; // Index Register X
        byte _regY; // Index Register Y
        short _regPC; // Program Counter
        byte _regS; // Stack Pointer 
        byte _regP; // Processor Status Register

        //REGISTER FLAGS
        bool _flagC; // Carry
        bool _flagZ; // Zero
        bool _flagI; // IRQ Disable
        bool _flagD; // Decimal Mode
        bool _flagB; // Break Flag
        bool _flagV; // Overflow
        bool _flagN; // Negative (sign)
        bool _flagU = true; // Unused Flag

        byte[] _memory; // Addressable memory

        public CPU()
        {
            _regS = 0;
            _memory = new byte[65535];
        }

        public bool ProcessInstruction (byte[] instruction)
        {
            var pText = (_flagN ? "1" : "0") + (_flagV ? "1" : "0") + (_flagU ? "1" : "0") + (_flagB ? "1" : "0") + (_flagD ? "1" : "0") + (_flagI ? "1" : "0") + (_flagZ ? "1" : "0") + (_flagC ? "1" : "0");
            _regP = Convert.ToByte(pText, 2);
            switch (BitConverter.ToString(new[] { instruction[0] }).Replace("-", " "))
            {
                #region Register Immeditate To Register Transfer
                case "A8":
                    Console.WriteLine("MOV Y,A");
                    _regY = _regA;
                    processFlags(_regY, true, true);
                    break;
                case "AA":
                    Console.WriteLine("MOV X,A");
                    _regX = _regA;
                    processFlags(_regX, true, true);
                    break;
                case "BA":
                    Console.WriteLine("MOV X,S");
                    _regX = _regS;
                    processFlags(_regX, true, true);
                    break;
                case "98":
                    Console.WriteLine("MOV A,Y");
                    _regA = _regY;
                    processFlags(_regA, true, true);
                    break;
                case "8A":
                    Console.WriteLine("MOV A,X");
                    _regA = _regX;
                    processFlags(_regA, true, true);
                    break;
                case "9A":
                    Console.WriteLine("MOV S,X");
                    _regS = (byte)(_regX);
                    break;
                case "A9":
                    Console.WriteLine("MOV A,"+instruction[1]);
                    _regA = instruction[1];
                    processFlags(_regA, true, true);
                    break;
                case "A2":
                    Console.WriteLine("MOV X," + instruction[1]);
                    _regX = instruction[1];
                    processFlags(_regX, true, true);
                    break;
                case "A0":
                    Console.WriteLine("MOV Y," + instruction[1]);
                    _regY = instruction[1];
                    processFlags(_regY, true, true);
                    break;
                #endregion
                #region Load Register from Memory

                case "A5":
                    Console.WriteLine("MOV A,[" + instruction[1] + "]");
                    _regA = _memory[instruction[1]];
                    processFlags(_regA, true, true);
                    break;
                case "B5":
                    Console.WriteLine("MOV A,[" + instruction[1] + "+X]");
                    _regA = _memory[instruction[1]+_regX];
                    processFlags(_regA, true, true);
                    break;
                case "AD":
                    Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = _memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regA, true, true);
                    break;
                case "BD":
                    Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+X]");
                    _regA = _memory[(instruction[1] << 8 | instruction[2])+_regX];
                    processFlags(_regA, true, true);
                    break;
                case "B9":
                    Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+Y]");
                    _regA = _memory[(instruction[1] << 8 | instruction[2]) + _regY];
                    processFlags(_regA, true, true);
                    break;
                case "A1":
                    Console.WriteLine("MOV A,[[" + instruction[1] + "+X]]");
                    _regA = _memory[_memory[instruction[1] + _regX]];
                    processFlags(_regA, true, true);
                    break;
                case "B1":
                    Console.WriteLine("MOV A,[[" + instruction[1] + "]+X]");
                    _regA = _memory[_memory[instruction[1]]+_regX];
                    processFlags(_regA, true, true);
                    break;
                case "A6":
                    Console.WriteLine("MOV X,[" + instruction[1] + "]");
                    _regX = _memory[instruction[1]];
                    processFlags(_regX, true, true);
                    break;
                case "B6":
                    Console.WriteLine("MOV X,[" + instruction[1] + "+Y]");
                    _regX = _memory[instruction[1] + _regY];
                    processFlags(_regX, true, true);
                    break;
                case "AE":
                    Console.WriteLine("MOV X,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regX = _memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regX, true, true);
                    break;
                case "BE":
                    Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+Y]");
                    _regX = _memory[(instruction[1] << 8 | instruction[2]) + _regY];
                    processFlags(_regX, true, true);
                    break;
                case "A4":
                    Console.WriteLine("MOV Y,[" + instruction[1] + "]");
                    _regY = _memory[instruction[1]];
                    processFlags(_regY, true, true);
                    break;
                case "B4":
                    Console.WriteLine("MOV Y,[" + instruction[1] + "+X]");
                    _regY = _memory[instruction[1] + _regX];
                    processFlags(_regY, true, true);
                    break;
                case "AC":
                    Console.WriteLine("MOV Y,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regY = _memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regY, true, true);
                    break;
                case "BC":
                    Console.WriteLine("MOV Y,[" + instruction[1] + "+" + instruction[2] + "+X]");
                    _regY = _memory[(instruction[1] << 8 | instruction[2]) + _regX];
                    processFlags(_regY, true, true);
                    break;
                #endregion
                #region Store Register in Memory
                case "85":
                    Console.WriteLine("MOV [" + instruction[1] + "], A");
                    _memory[instruction[1]] = _regA;
                    break;
                case "95":
                    Console.WriteLine("MOV [" + instruction[1] + "+X], A");
                    _memory[instruction[1] + _regX] = _regA;
                    break;
                case "8D":
                    Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] +"], A");
                    _memory[(instruction[1] << 8 | instruction[2])] = _regA;
                    break;
                case "9D":
                    Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "+X], A");
                    _memory[(instruction[1] << 8 | instruction[2]) + _regX] = _regA;
                    break;
                case "99":
                    Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "+Y], A");
                    _memory[(instruction[1] << 8 | instruction[2]) + _regY] = _regA;
                    break;
                case "81":
                    Console.WriteLine("MOV [[" + instruction[1] + "+X]], A");
                    _memory[_memory[instruction[1] + _regX]] = _regA;
                    break;
                case "91":
                    Console.WriteLine("MOV [[" + instruction[1] + "]+Y], A");
                    _memory[_memory[instruction[1]] + _regY] = _regA;
                    break;
                case "86":
                    Console.WriteLine("MOV [" + instruction[1] + "]], X");
                    _memory[instruction[1]] = _regX;
                    break;
                case "96":
                    Console.WriteLine("MOV [" + instruction[1] + "]+Y], X");
                    _memory[instruction[1]+_regY] = _regX;
                    break;
                case "8E":
                    Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "], X");
                    _memory[instruction[1] << 8 | instruction[2]] = _regX;
                    break;
                case "84":
                    Console.WriteLine("MOV [" + instruction[1] + "], Y");
                    _memory[instruction[1] ] = _regY;
                    break;
                case "94":
                    Console.WriteLine("MOV [" + instruction[1] + "+X], Y");
                    _memory[instruction[1] + _regX] = _regY;
                    break;
                case "8C":
                    Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "], Y");
                    _memory[instruction[1] << 8 | instruction[2]] = _regY;
                    break;
                #endregion
                #region Push/Pull
                case "48":
                    Console.WriteLine("PUSH A");
                    _memory[(byte)(_regS + 0x100)] = _regA;
                    _regS = (byte)(_regS - 1);
                    break;
                case "08":
                    Console.WriteLine("PUSH P");
                    _memory[_regS + 0x100] = _regP;
                    _regS = (byte)(_regS - 1);
                    _flagB = true;
                    break;
                case "68":
                    Console.WriteLine("POP A");
                    _regS = (byte)(_regS + 1);
                    _regA = _memory[_regS + 0x100];
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "28":
                    Console.WriteLine("POP P");
                    _regS = (byte)(_regS + 1);
                    ByteToFlags(_memory[_regS + 0x100]);
                    break;
                #endregion
                #region Add memory to accumulator with carry
                case "69":
                    Console.WriteLine("ADC A," + instruction[1]);
                    byte regABuffer = _regA;
                    _regA = (byte)(_regA + instruction[1] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regABuffer, _regA);
                    break;
                #endregion
                #region Logical AND memory with accumulator
                case "29":
                    Console.WriteLine("AND A,"+instruction[1]);
                    _regA = (byte)(_regA & instruction[1]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "25":
                    Console.WriteLine("AND A,[" + instruction[1]+"]");
                    _regA = (byte)(_regA & _memory[instruction[1]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "35":
                    Console.WriteLine("AND A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA & _memory[instruction[1] + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "2D":
                    Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA & _memory[(instruction[1] << 8 | instruction[2])]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "3D":
                    Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA & _memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "39":
                    Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA & _memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "21":
                    Console.WriteLine("AND A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA & _memory[_memory[instruction[1] + _regX]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "31":
                    Console.WriteLine("AND A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA & _memory[_memory[instruction[1]] + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                #endregion
                #region Logical XOR memory with accumulator
                case "49":
                    Console.WriteLine("XOR A," + instruction[1]);
                    _regA = (byte)(_regA ^ instruction[1]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "45":
                    Console.WriteLine("XOR A,[" + instruction[1] + "]");
                    _regA = (byte)(_regA ^ _memory[instruction[1]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "55":
                    Console.WriteLine("XOR A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA ^ _memory[instruction[1] + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "4D":
                    Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA ^ _memory[(instruction[1] << 8 | instruction[2])]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "5D":
                    Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA ^ _memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "59":
                    Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA ^ _memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "41":
                    Console.WriteLine("XOR A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA ^ _memory[_memory[instruction[1] + _regX]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "51":
                    Console.WriteLine("XOR A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA ^ _memory[_memory[instruction[1]] + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                #endregion
                #region Logical OR memory with accumulator
                case "09":
                    Console.WriteLine("OR A," + instruction[1]);
                    _regA = (byte)(_regA | instruction[1]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "05":
                    Console.WriteLine("OR A,[" + instruction[1] + "]");
                    _regA = (byte)(_regA | _memory[instruction[1]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "15":
                    Console.WriteLine("OR A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA | _memory[instruction[1] + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "0D":
                    Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA | _memory[(instruction[1] << 8 | instruction[2])]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "1D":
                    Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA | _memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "19":
                    Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA | _memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "01":
                    Console.WriteLine("OR A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA | _memory[_memory[instruction[1] + _regX]]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                case "11":
                    Console.WriteLine("OR A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA | _memory[_memory[instruction[1]] + _regY]);
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    break;
                    #endregion
            }
            return true;
        }

        private void processAddFlags(byte value1, byte valueAdded, byte value2)
        {
            string byteAsString1 = Convert.ToString(value1, 2).PadLeft(8, '0');
            string valueAddedAsString = Convert.ToString(valueAdded, 2).PadLeft(8, '0');
            string byteAsString2 = Convert.ToString(value2, 2).PadLeft(8, '0');
            if (byteAsString1[0] == '1' && byteAsString2[0] == '0')
                _flagC = false;
            else
                _flagC = true;
            if (byteAsString1[0] == '0' && valueAddedAsString[0] == '0' && byteAsString2[0] == '1' ||
                byteAsString1[0] == '1' && valueAddedAsString[0] == '1' && byteAsString2[0] == '0')
                _flagV = true;
            else
                _flagV = false;


            
        }

        private void processFlags(byte value, bool n = false, bool z = false, bool c = false)
        {
            string byteAsString = Convert.ToString(value, 2).PadLeft(8, '0');
            if (z)
                if (value == 0)
                    _flagZ = true;
                else
                    _flagZ = false;
            if (n)
                _flagN = (byteAsString[0] == '0');

        }

        private void ByteToFlags(byte b)
        {
            _flagC = (b & (1 << 0)) != 0;
            _flagZ = (b & (1 << 1)) != 0;
            _flagI = (b & (1 << 2)) != 0;
            _flagD = (b & (1 << 3)) != 0;
            _flagB = (b & (1 << 4)) != 0;
            _flagU = (b & (1 << 5)) != 0;
            _flagV = (b & (1 << 6)) != 0;
            _flagN = (b & (1 << 7)) != 0;
        }

        

    }
}
