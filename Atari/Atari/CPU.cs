using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atari
{
    public static class CPU
    {

        //REGISTERS
        static byte _regA; // Accumulator
        static byte _regX; // Index Register X
        static byte _regY; // Index Register Y
        public static ushort _regPC { get; set; } // Program Counter
        static byte _regS; // Stack Pointer 
        static byte _regP; // Processor Status Register

        //REGISTER FLAGS
        static bool _flagC; // Carry
        static bool _flagZ; // Zero
        static bool _flagI; // IRQ Disable
        static bool _flagD; // Decimal Mode
        static bool _flagB; // Break Flag
        static bool _flagV; // Overflow
        static bool _flagN; // Negative (sign)
        static bool _flagU = true; // Unused Flag

        public static int clock = 0;

        static byte regBuffer;

        static CPU()
        {
            _regS = 0;
        }

        public static bool ProcessInstruction (byte[] instruction)
        {
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            var pText = (_flagN ? "1" : "0") + (_flagV ? "1" : "0") + (_flagU ? "1" : "0") + (_flagB ? "1" : "0") + (_flagD ? "1" : "0") + (_flagI ? "1" : "0") + (_flagZ ? "1" : "0") + (_flagC ? "1" : "0");
            _regP = Convert.ToByte(pText, 2);
            clock++;
            //Console.WriteLine("CURRENT INSTRUCTION");
            switch (instruction[0])
            {
                #region Register Immeditate To Register Transfer
                case 0xA8:
                    //Console.WriteLine("MOV Y,A");
                    _regY = _regA;
                    processFlags(_regY, true, true);
                    _regPC += 1;
                    break;
                case 0xAA:
                    //Console.WriteLine("MOV X,A");
                    _regX = _regA;
                    processFlags(_regX, true, true);
                    _regPC += 1;
                    break;
                case 0xBA:
                    //Console.WriteLine("MOV X,S");
                    _regX = _regS;
                    processFlags(_regX, true, true);
                    _regPC += 1;
                    break;
                case 0x98:
                    //Console.WriteLine("MOV A,Y");
                    _regA = _regY;
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x8A:
                    //Console.WriteLine("MOV A,X");
                    _regA = _regX;
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x9A:
                    //Console.WriteLine("MOV S,X");
                    _regS = (byte)(_regX);
                    _regPC += 1;
                    break;
                case 0xA9:
                    //Console.WriteLine("MOV A,"+instruction[1]);
                    _regA = instruction[1];
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0xA2:
                    //Console.WriteLine("MOV X," + instruction[1]);
                    _regX = instruction[1];
                    processFlags(_regX, true, true);
                    _regPC += 1;
                    break;
                case 0xA0:
                    //Console.WriteLine("MOV Y," + instruction[1]);
                    _regY = instruction[1];
                    processFlags(_regY, true, true);
                    _regPC += 1;
                    break;
                #endregion
                #region Load Register from Memory

                case 0xA5:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "]");
                    _regA = RAM.Memory[instruction[1]];
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0xB5:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "+X]");
                    _regA = RAM.Memory[instruction[1]+_regX];
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0xAD:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = RAM.Memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0xBD:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+X]");
                    _regA = RAM.Memory[(instruction[1] << 8 | instruction[2])+_regX];
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0xB9:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+Y]");
                    _regA = RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY];
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0xA1:
                    //Console.WriteLine("MOV A,[[" + instruction[1] + "+X]]");
                    _regA = RAM.Memory[RAM.Memory[instruction[1] + _regX]];
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0xB1:
                    //Console.WriteLine("MOV A,[[" + instruction[1] + "]+X]");
                    _regA = RAM.Memory[RAM.Memory[instruction[1]]+_regX];
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0xA6:
                    //Console.WriteLine("MOV X,[" + instruction[1] + "]");
                    _regX = RAM.Memory[instruction[1]];
                    processFlags(_regX, true, true);
                    _regPC += 2;
                    break;
                case 0xB6:
                    //Console.WriteLine("MOV X,[" + instruction[1] + "+Y]");
                    _regX = RAM.Memory[instruction[1] + _regY];
                    processFlags(_regX, true, true);
                    _regPC += 2;
                    break;
                case 0xAE:
                    //Console.WriteLine("MOV X,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regX = RAM.Memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regX, true, true);
                    _regPC += 3;
                    break;
                case 0xBE:
                    //Console.WriteLine("MOV A,[" + instruction[1] + "+" + instruction[2] + "+Y]");
                    _regX = RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY];
                    processFlags(_regX, true, true);
                    _regPC += 3;
                    break;
                case 0xA4:
                    //Console.WriteLine("MOV Y,[" + instruction[1] + "]");
                    _regY = RAM.Memory[instruction[1]];
                    processFlags(_regY, true, true);
                    _regPC += 2;
                    break;
                case 0xB4:
                    //Console.WriteLine("MOV Y,[" + instruction[1] + "+X]");
                    _regY = RAM.Memory[instruction[1] + _regX];
                    processFlags(_regY, true, true);
                    _regPC += 2;
                    break;
                case 0xAC:
                    //Console.WriteLine("MOV Y,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regY = RAM.Memory[instruction[1] << 8 | instruction[2]];
                    processFlags(_regY, true, true);
                    _regPC += 3;
                    break;
                case 0xBC:
                    //Console.WriteLine("MOV Y,[" + instruction[1] + "+" + instruction[2] + "+X]");
                    _regY = RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX];
                    processFlags(_regY, true, true);
                    _regPC += 3;
                    break;
                #endregion
                #region Store Register in Memory
                case 0x85:
                    //Console.WriteLine("MOV [" + instruction[1] + "], A");
                    RAM.Memory[instruction[1]] = _regA;
                    _regPC += 2;
                    break;
                case 0x95:
                    //Console.WriteLine("MOV [" + instruction[1] + "+X], A");
                    RAM.Memory[instruction[1] + _regX] = _regA;
                    _regPC += 2;
                    break;
                case 0x8D:
                    //Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] +"], A");
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = _regA;
                    _regPC += 3;
                    break;
                case 0x9D:
                    //Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "+X], A");
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = _regA;
                    _regPC += 3;
                    break;
                case 0x99:
                    //Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "+Y], A");
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY] = _regA;
                    _regPC += 3;
                    break;
                case 0x81:
                    //Console.WriteLine("MOV [[" + instruction[1] + "+X]], A");
                    RAM.Memory[RAM.Memory[instruction[1] + _regX]] = _regA;
                    _regPC += 2;
                    break;
                case 0x91:
                    //Console.WriteLine("MOV [[" + instruction[1] + "]+Y], A");
                    RAM.Memory[RAM.Memory[instruction[1]] + _regY] = _regA;
                    _regPC += 2;
                    break;
                case 0x86:
                    //Console.WriteLine("MOV [" + instruction[1] + "]], X");
                    RAM.Memory[instruction[1]] = _regX;
                    _regPC += 2;
                    break;
                case 0x96:
                    //Console.WriteLine("MOV [" + instruction[1] + "]+Y], X");
                    RAM.Memory[instruction[1]+_regY] = _regX;
                    _regPC += 2;
                    break;
                case 0x8E:
                    //Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "], X");
                    RAM.Memory[instruction[1] << 8 | instruction[2]] = _regX;
                    _regPC += 3;
                    break;
                case 0x84:
                    //Console.WriteLine("MOV [" + instruction[1] + "], Y");
                    RAM.Memory[instruction[1] ] = _regY;
                    _regPC += 2;
                    break;
                case 0x94:
                    //Console.WriteLine("MOV [" + instruction[1] + "+X], Y");
                    RAM.Memory[instruction[1] + _regX] = _regY;
                    _regPC += 2;
                    break;
                case 0x8C:
                    //Console.WriteLine("MOV [" + instruction[1] + "+" + instruction[2] + "], Y");
                    RAM.Memory[instruction[1] << 8 | instruction[2]] = _regY;
                    _regPC += 3;
                    break;
                #endregion
                #region Push/Pull
                case 0x48:
                    //Console.WriteLine("PUSH A");
                    RAM.Memory[(byte)(_regS + 0x100)] = _regA;
                    _regS = (byte)(_regS - 1);
                    _regPC += 1;
                    break;
                case 0x08:
                    //Console.WriteLine("PUSH P");
                    RAM.Memory[_regS + 0x100] = _regP;
                    _regS = (byte)(_regS - 1);
                    _flagB = true;
                    _regPC += 1;
                    break;
                case 0x68:
                    //Console.WriteLine("POP A");
                    _regS = (byte)(_regS + 1);
                    _regA = RAM.Memory[_regS + 0x100];
                    if (_regA == 0)
                        _flagZ = true;
                    else
                        _flagZ = false;
                    _flagN = (_regA & (1 << 7)) != 0;
                    _regPC += 1;
                    break;
                case 0x28:
                    //Console.WriteLine("POP P");
                    _regS = (byte)(_regS + 1);
                    ByteToFlags(RAM.Memory[_regS + 0x100]);
                    _regPC += 1;
                    break;
                #endregion
                #region Add memory to accumulator with carry
                case 0x69:
                    //Console.WriteLine("ADC A," + instruction[1]);
                    regBuffer = _regA;
                    _regA = (byte)(_regA + instruction[1] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, instruction[1], _regA);
                    _regPC += 2;
                    break;
                case 0x65:
                    //Console.WriteLine("ADC A,[" + RAM.Memory[instruction[1]] + "]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[instruction[1]] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[instruction[1]], _regA);
                    _regPC += 2;
                    break;
                case 0x75:
                    //Console.WriteLine("ADC A,[" + RAM.Memory[instruction[1]] + "+X]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[instruction[1] + _regX] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[instruction[1] + _regX], _regA);
                    _regPC += 2;
                    break;
                case 0x6D:
                    //Console.WriteLine("ADC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[(instruction[1] << 8 | instruction[2])] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2])], _regA);
                    _regPC += 3;
                    break;
                case 0x7D:
                    //Console.WriteLine("ADC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "+X]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], _regA);
                    _regPC += 3;
                    break;
                case 0x79:
                    //Console.WriteLine("ADC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "+Y]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY], _regA);
                    _regPC += 3;
                    break;
                case 0x61:
                    //Console.WriteLine("ADC A,[[" + RAM.Memory[instruction[1]] + "+X]]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[RAM.Memory[instruction[1] + _regX]] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[RAM.Memory[instruction[1] + _regX]], _regA);
                    _regPC += 2;
                    break;
                case 0x71:
                    //Console.WriteLine("ADC A,[[" + RAM.Memory[instruction[1]] + "]+Y]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA + RAM.Memory[RAM.Memory[instruction[1]] + _regY] + Convert.ToByte(_flagC));
                    processFlags(_regA, true, true);
                    processAddFlags(regBuffer, RAM.Memory[RAM.Memory[instruction[1]] + _regY], _regA);
                    _regPC += 2;
                    break;

                #endregion
                #region Subtract memory from accumulator with carry
                case 0xE9:
                    //Console.WriteLine("SBC A," + instruction[1]);
                    regBuffer = _regA;
                    _regA = (byte)(_regA - instruction[1] + Convert.ToByte(_flagC) - 1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, instruction[1], _regA);
                    _regPC += 2;
                    break;
                case 0xE5:
                    //Console.WriteLine("SBC A,[" + RAM.Memory[instruction[1]] + "]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[instruction[1]] + Convert.ToByte(_flagC) -1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[instruction[1]], _regA);
                    _regPC += 2;
                    break;
                case 0xF5:
                    //Console.WriteLine("SBC A,[" + RAM.Memory[instruction[1]] + "+X]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[instruction[1] + _regX] + Convert.ToByte(_flagC) -1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[instruction[1] + _regX], _regA);
                    _regPC += 2;
                    break;
                case 0xED:
                    //Console.WriteLine("SBC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[(instruction[1] << 8 | instruction[2])] + Convert.ToByte(_flagC) -1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2])], _regA);
                    _regPC += 3;
                    break;
                case 0xFD:
                    //Console.WriteLine("SBC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "+X]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] + Convert.ToByte(_flagC) -1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], _regA);
                    _regPC += 3;
                    break;
                case 0xF9:
                    //Console.WriteLine("SBC A,[" + RAM.Memory[instruction[1]] + "+" + RAM.Memory[instruction[2]] + "+Y]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY] + Convert.ToByte(_flagC) -1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY], _regA);
                    _regPC += 3;
                    break;
                case 0xE1:
                    //Console.WriteLine("SBC A,[[" + RAM.Memory[instruction[1]] + "+X]]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[RAM.Memory[instruction[1] + _regX]] + Convert.ToByte(_flagC) - 1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[RAM.Memory[instruction[1] + _regX]], _regA);
                    _regPC += 2;
                    break;
                case 0xF1:
                    //Console.WriteLine("SBC A,[[" + RAM.Memory[instruction[1]] + "]+Y]");
                    regBuffer = _regA;
                    _regA = (byte)(_regA - RAM.Memory[RAM.Memory[instruction[1]] + _regY] + Convert.ToByte(_flagC) - 1);
                    processFlags(_regA, true, true);
                    processSubtractFlags(regBuffer, RAM.Memory[RAM.Memory[instruction[1]] + _regY], _regA);
                    _regPC += 2;
                    break;

                #endregion
                #region Logical AND memory with accumulator
                case 0x29:
                    //Console.WriteLine("AND A,"+instruction[1]);
                    _regA = (byte)(_regA & instruction[1]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x25:
                    //Console.WriteLine("AND A,[" + instruction[1]+"]");
                    _regA = (byte)(_regA & RAM.Memory[instruction[1]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x35:
                    //Console.WriteLine("AND A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA & RAM.Memory[instruction[1] + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x2D:
                    //Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA & RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x3D:
                    //Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA & RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x39:
                    //Console.WriteLine("AND A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA & RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x21:
                    //Console.WriteLine("AND A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA & RAM.Memory[RAM.Memory[instruction[1] + _regX]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x31:
                    //Console.WriteLine("AND A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA & RAM.Memory[RAM.Memory[instruction[1]] + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                #endregion
                #region Logical XOR memory with accumulator
                case 0x49:
                    //Console.WriteLine("XOR A," + instruction[1]);
                    _regA = (byte)(_regA ^ instruction[1]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x45:
                    //Console.WriteLine("XOR A,[" + instruction[1] + "]");
                    _regA = (byte)(_regA ^ RAM.Memory[instruction[1]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x55:
                    //Console.WriteLine("XOR A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA ^ RAM.Memory[instruction[1] + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x4D:
                    //Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA ^ RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x5D:
                    //Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA ^ RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x59:
                    //Console.WriteLine("XOR A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA ^ RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x41:
                    //Console.WriteLine("XOR A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA ^ RAM.Memory[RAM.Memory[instruction[1] + _regX]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x51:
                    //Console.WriteLine("XOR A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA ^ RAM.Memory[RAM.Memory[instruction[1]] + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                #endregion
                #region Logical OR memory with accumulator
                case 0x09:
                    //Console.WriteLine("OR A," + instruction[1]);
                    _regA = (byte)(_regA | instruction[1]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x05:
                    //Console.WriteLine("OR A,[" + instruction[1] + "]");
                    _regA = (byte)(_regA | RAM.Memory[instruction[1]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x15:
                    //Console.WriteLine("OR A,[" + instruction[1] + "+X]");
                    _regA = (byte)(_regA | RAM.Memory[instruction[1] + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x0D:
                    //Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + "]");
                    _regA = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x1D:
                    //Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    _regA = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x19:
                    //Console.WriteLine("OR A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    _regA = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 3;
                    break;
                case 0x01:
                    //Console.WriteLine("OR A,[[" + instruction[1] + "+X]]");
                    _regA = (byte)(_regA | RAM.Memory[RAM.Memory[instruction[1] + _regX]]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                case 0x11:
                    //Console.WriteLine("OR A,[[" + instruction[1] + "]+Y]");
                    _regA = (byte)(_regA | RAM.Memory[RAM.Memory[instruction[1]] + _regY]);
                    processFlags(_regA, true, true);
                    _regPC += 2;
                    break;
                #endregion
                #region Compare
                case 0xC9:
                    //Console.WriteLine("CMP A," + instruction[1]);
                    regBuffer = (byte)(_regA - instruction[1]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xC5:
                    //Console.WriteLine("CMP A,[" + instruction[1] + "]");
                    regBuffer = (byte)(_regA | RAM.Memory[instruction[1]]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xD5:
                    //Console.WriteLine("CMP A,[" + instruction[1] + "+X]");
                    regBuffer = (byte)(_regA | RAM.Memory[instruction[1] + _regX]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xCD:
                    //Console.WriteLine("CMP A,[" + instruction[1] + "+" + instruction[2] + "]");
                    regBuffer = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xDD:
                    //Console.WriteLine("CMP A,[" + instruction[1] + "+" + instruction[2] + " +X]");
                    regBuffer = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xD9:
                    //Console.WriteLine("CMP A,[" + instruction[1] + "+" + instruction[2] + " +Y]");
                    regBuffer = (byte)(_regA | RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regY]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xC1:
                    //Console.WriteLine("CMP A,[[" + instruction[1] + "+X]]");
                    regBuffer = (byte)(_regA | RAM.Memory[RAM.Memory[instruction[1] + _regX]]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xD1:
                    //Console.WriteLine("CMP A,[[" + instruction[1] + "]+Y]");
                    regBuffer = (byte)(_regA | RAM.Memory[RAM.Memory[instruction[1]] + _regY]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xE0:
                    //Console.WriteLine("CMP X," + instruction[1]);
                    regBuffer = (byte)(_regX - instruction[1]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xE4:
                    //Console.WriteLine("CMP X,[" + instruction[1] + "]");
                    regBuffer = (byte)(_regX | RAM.Memory[instruction[1]]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xEC:
                    //Console.WriteLine("CMP X,[" + instruction[1] + "+" + instruction[2] + "]");
                    regBuffer = (byte)(_regX | RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xC0:
                    //Console.WriteLine("CMP Y," + instruction[1]);
                    regBuffer = (byte)(_regY - instruction[1]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xC4:
                    //Console.WriteLine("CMP Y,[" + instruction[1] + "]");
                    regBuffer = (byte)(_regY | RAM.Memory[instruction[1]]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 2;
                    break;
                case 0xCC:
                    //Console.WriteLine("CMP Y,[" + instruction[1] + "+" + instruction[2] + "]");
                    regBuffer = (byte)(_regY | RAM.Memory[(instruction[1] << 8 | instruction[2])]);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                #endregion
                #region Increment
                case 0xE6:
                    //Console.WriteLine("INC [" + instruction[1] + "]");
                    RAM.Memory[instruction[1]] = (byte)(RAM.Memory[instruction[1]] + 1);
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0xF6:
                    //Console.WriteLine("INC [" + instruction[1] + "+X]");
                    RAM.Memory[instruction[1] + _regX] = (byte)(RAM.Memory[instruction[1] + _regX] + 1);
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0xEE:
                    //Console.WriteLine("INC [" + instruction[1] + "+" + instruction[2] + "]");
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] + 1);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xFE:
                    //Console.WriteLine("INC [" + instruction[1] + "+" + instruction[2] + "] + X");
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] + 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                case 0xE8:
                    //Console.WriteLine("INC X");
                    _regX = (byte)(_regX + 1);
                    processFlags(_regX, true, true);
                    _regPC += 1;
                    break;
                case 0xC8:
                    //Console.WriteLine("INC Y");
                    _regY = (byte)(_regY + 1);
                    processFlags(_regY, true, true);
                    _regPC += 1;
                    break;
                #endregion
                #region Decrement
                case 0xC6:
                    //Console.WriteLine("DEC [" + instruction[1] + "]");
                    RAM.Memory[instruction[1]] = (byte)(RAM.Memory[instruction[1]] - 1);
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0xD6:
                    //Console.WriteLine("DEC [" + instruction[1] + "+X]");
                    RAM.Memory[instruction[1] + _regX] = (byte)(RAM.Memory[instruction[1] + _regX] - 1);
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0xCE:
                    //Console.WriteLine("DEC [" + instruction[1] + "+" + instruction[2] + "]");
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] - 1);
                    processFlags(regBuffer, true, true);
                    calcCarryFromSubtract(regBuffer, instruction[1]);
                    _regPC += 3;
                    break;
                case 0xDE:
                    //Console.WriteLine("DEC [" + instruction[1] + "+" + instruction[2] + "] + X");
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] - 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                case 0xCA:
                    //Console.WriteLine("DEC X");
                    _regX = (byte)(_regX - 1);
                    processFlags(_regX, true, true);
                    _regPC += 1;
                    break;
                case 0x88:
                    //Console.WriteLine("DEC Y");
                    _regY = (byte)(_regY - 1);
                    processFlags(_regY, true, true);
                    _regPC += 1;
                    break;
                #endregion
                #region Shift Left Logical/Arithmetic
                case 0x0A:
                    //Console.WriteLine("SHL A");
                    _flagC = Convert.ToString(_regA, 2).PadLeft(8, '0')[0] == 1;
                    _regA = (byte)(_regA << 1);
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x06:
                    //Console.WriteLine("SHL [" + instruction[1] + "]");
                    _flagC = Convert.ToString(RAM.Memory[instruction[1]], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[instruction[1]] = (byte)(RAM.Memory[instruction[1]] << 1);
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0x16:
                    //Console.WriteLine("SHL [" + instruction[1] + " + X]");
                    _flagC = Convert.ToString(RAM.Memory[instruction[1] + _regX], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[instruction[1] + _regX] = (byte)(RAM.Memory[instruction[1] + _regX] << 1);
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0x0E:
                    //Console.WriteLine("SHL [" + instruction[1] + instruction[2] + "]");
                    _flagC = Convert.ToString(RAM.Memory[(instruction[1] << 8 | instruction[2])], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] << 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2])], true, true);
                    break;
                case 0x1E:
                    //Console.WriteLine("SHL [" + instruction[1] + instruction[2] + " + X]");
                    _flagC = Convert.ToString(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] << 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                #endregion
                #region Shift Right Logical/Arithmetic
                case 0x4A:
                    //Console.WriteLine("SHR A");
                    _flagC = false;
                    _regA = (byte)(_regA >> 1);
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x46:
                    //Console.WriteLine("SHR [" + instruction[1] + "]");
                    _flagC = false;
                    RAM.Memory[instruction[1]] = (byte)(RAM.Memory[instruction[1]] >> 1);
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0x56:
                    //Console.WriteLine("SHR [" + instruction[1] + " + X]");
                    _flagC = false;
                    RAM.Memory[instruction[1] + _regX] = (byte)(RAM.Memory[instruction[1] + _regX] >> 1);
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0x4E:
                    //Console.WriteLine("SHR [" + instruction[1] + instruction[2] + "]");
                    _flagC = false;
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] >> 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2])], true, true);
                    _regPC += 3;
                    break;
                case 0x5E:
                    //Console.WriteLine("SHR [" + instruction[1] + instruction[2] + " + X]");
                    _flagC = false;
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)(RAM.Memory[(instruction[1] << 8 | instruction[2])] >> 1);
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                #endregion
                #region Shift Left through carry
                case 0x2A:
                    //Console.WriteLine("ROL A");
                    _flagC = Convert.ToString(_regA, 2).PadLeft(8, '0')[0] == 1;
                    _regA = (byte)((_regA << 1) + Convert.ToInt32(_flagC)) ;
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x26:
                    //Console.WriteLine("ROL [" + instruction[1] + "]");
                    _flagC = Convert.ToString(RAM.Memory[instruction[1]], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[instruction[1]] = (byte)((RAM.Memory[instruction[1]] << 1) + Convert.ToInt32(_flagC));
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0x36:
                    //Console.WriteLine("ROL [" + instruction[1] + " + X]");
                    _flagC = Convert.ToString(RAM.Memory[instruction[1] + _regX], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[instruction[1] + _regX] = (byte)((RAM.Memory[instruction[1] + _regX] << 1) + Convert.ToInt32(_flagC));
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0x2E:
                    //Console.WriteLine("ROL [" + instruction[1] + instruction[2] + "]");
                    _flagC = Convert.ToString(RAM.Memory[(instruction[1] << 8 | instruction[2])], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)((RAM.Memory[(instruction[1] << 8 | instruction[2])] << 1) + Convert.ToInt32(_flagC));
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2])], true, true);
                    _regPC += 3;
                    break;
                case 0x3E:
                    //Console.WriteLine("ROL [" + instruction[1] + instruction[2] + " + X]");
                    _flagC = Convert.ToString(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], 2).PadLeft(8, '0')[0] == 1;
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)((RAM.Memory[(instruction[1] << 8 | instruction[2])] << 1) + Convert.ToInt32(_flagC));
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                #endregion
                #region Shift Right through carry
                case 0x6A:
                    //Console.WriteLine("ROR A");
                    _flagC = (_regA & (1 << 7)) != 0;
                    _regA = (byte)((_regA >> 1) | (Convert.ToByte(_flagC) << 8));
                    processFlags(_regA, true, true);
                    _regPC += 1;
                    break;
                case 0x66:
                    //Console.WriteLine("ROR [" + instruction[1] + "]");
                    _flagC = (RAM.Memory[instruction[1]] & (1 << 7)) != 0;
                    RAM.Memory[instruction[1]] = (byte)((RAM.Memory[instruction[1]] >> 1) | (Convert.ToByte(_flagC) << 8));
                    processFlags(RAM.Memory[instruction[1]], true, true);
                    _regPC += 2;
                    break;
                case 0x76:
                    //Console.WriteLine("ROR [" + instruction[1] + " + X]");
                    _flagC = (RAM.Memory[instruction[1] + _regX] & (1 << 7)) != 0; ;
                    RAM.Memory[instruction[1] + _regX] = (byte)((RAM.Memory[instruction[1] + _regX] >> 1) | (Convert.ToByte(_flagC) << 8));
                    processFlags(RAM.Memory[instruction[1] + _regX], true, true);
                    _regPC += 2;
                    break;
                case 0x6E:
                    //Console.WriteLine("ROR [" + instruction[1] + instruction[2] + "]");
                    _flagC = (RAM.Memory[(instruction[1] << 8 | instruction[2])] & (1 << 7)) != 0; ; 
                    RAM.Memory[(instruction[1] << 8 | instruction[2])] = (byte)((RAM.Memory[(instruction[1] << 8 | instruction[2])] >> 1) | (Convert.ToByte(_flagC) << 8));
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2])], true, true);
                    _regPC += 3;
                    break;
                case 0x7E:
                    //Console.WriteLine("ROR [" + instruction[1] + instruction[2] + " + X]");
                    _flagC = (RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] & (1 << 7)) != 0; 
                    RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX] = (byte)((RAM.Memory[(instruction[1] << 8 | instruction[2])] >> 1) | (Convert.ToByte(_flagC) << 8));
                    processFlags(RAM.Memory[(instruction[1] << 8 | instruction[2]) + _regX], true, true);
                    _regPC += 3;
                    break;
                #endregion
                #region CPU Jump and Control
                case 0x4C:
                    //Console.WriteLine("JMP " + instruction[1] + instruction[2]);
                    _regPC = (ushort)(instruction[1] << 8 | instruction[2]);
                    _regPC += 3;
                    break;
                case 0x6C:
                    //Console.WriteLine("JMP [" + instruction[1] + instruction[2] + "]");
                    _regPC = (ushort)(RAM.Memory[instruction[1] << 8 | instruction[2]]);
                    _regPC += 3;
                    break;
                #endregion
                default:
                    //Console.WriteLine("UNKNOWN INSTRUCTION");
                    _regPC += 1;
                    break;
            }

            //watch.Stop();

            //Console.WriteLine("-----------------");
            //Console.WriteLine("FLAG VALUES");
            //Console.WriteLine("CNVZ");
            //Console.WriteLine(Convert.ToString(Convert.ToInt32(_flagC)) + Convert.ToString(Convert.ToInt32(_flagN)) + Convert.ToString(Convert.ToInt32(_flagV)) + Convert.ToString(Convert.ToInt32(_flagZ)));
            //Console.WriteLine("-----------------");
            //Console.WriteLine("REGISTRY VALUES");
            //Console.WriteLine("A " + _regA);
            //Console.WriteLine("P " + _regP);
            //Console.WriteLine("PC " + _regPC);
            //Console.WriteLine("S " + _regS);
            //Console.WriteLine("X " + _regX);
            //Console.WriteLine("Y " + _regY);
            //Console.WriteLine(watch.ElapsedMilliseconds);
            //System.Threading.Thread.Sleep(30);
            //Console.Clear();
            
            return true;

        }

        private static void processSubtractFlags(byte value1, byte valueAdded, byte value2)
        {
            string byteAsString1 = Convert.ToString(value1, 2).PadLeft(8, '0');
            string valueAddedAsString = Convert.ToString(valueAdded, 2).PadLeft(8, '0');
            string byteAsString2 = Convert.ToString(value2, 2).PadLeft(8, '0');
            calcCarryFromSubtract(value1, valueAdded);
            if (byteAsString1[0] == '0' && valueAddedAsString[0] == '0' && byteAsString2[0] == '1' ||
                byteAsString1[0] == '1' && valueAddedAsString[0] == '1' && byteAsString2[0] == '0')
                _flagV = true;
            else
                _flagV = false;
        }

        private static void processAddFlags(byte value1, byte valueAdded, byte value2)
        {
            string byteAsString1 = Convert.ToString(value1, 2).PadLeft(8, '0');
            string valueAddedAsString = Convert.ToString(valueAdded, 2).PadLeft(8, '0');
            string byteAsString2 = Convert.ToString(value2, 2).PadLeft(8, '0');
            if (byteAsString1[0] == '1' && byteAsString2[0] == '0' ||
                valueAddedAsString[0] == '1' && byteAsString2[0] == '0')
                _flagC = false;
            else
                _flagC = true;
            if (byteAsString1[0] == '0' && valueAddedAsString[0] == '0' && byteAsString2[0] == '1' ||
                byteAsString1[0] == '1' && valueAddedAsString[0] == '1' && byteAsString2[0] == '0')
                _flagV = true;
            else
                _flagV = false;


            
        }

        private static void processFlags(byte value, bool n = false, bool z = false, bool c = false)
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

        private static void calcCarryFromSubtract(byte value1, byte valueAdded)
        {
            if (valueAdded > value1)
                _flagC = true;
            else
                _flagC = false;
        }

        private static void ByteToFlags(byte b)
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
