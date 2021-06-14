using System;
using System.Collections.Generic;
using System.Text;

using CSLibrary;
using CSLibrary.Constants;
using CSLibrary.Structures;
using CSLibrary.Events;
using CSLibrary.Tools;

namespace CSLibrary
{
	public partial class RFIDReader
	{
		//MacWriteRegister(MACREGISTER.HST_ANT_DESC_SEL, 0);
		//MacReadRegister(MACREGISTER.HST_ANT_DESC_RFPOWER, ref pwrlvl);
		//FM13DT160_CMDCFGPAR = 0x117,
		//FM13DT160_REGADDRPAR = 0x118,
		//FM13DT160_WRITEPAR = 0x119,
		//FM13DT160_PWDPAR = 0x11a,
		//FM13DT160_STOBLOCKADDPAR = 0x11b,
		//FM13DT160_STARTADDRPAR = 0x11c,
		//FM13DT160_READWRITELENPAR = 0x11d,
		//FM13DT160_DATAPAR = 0x11e,

		void FM13DT160_ReadMemory(uint offset, uint size)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_STARTADDRPAR, (uint)offset);
			MacWriteRegister(MACREGISTER.FM13DT160_READWRITELENPAR, (uint)size);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTREADMEMORY), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_WriteMemory(uint offset, uint size, uint data)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_STARTADDRPAR, (uint)offset);
			MacWriteRegister(MACREGISTER.FM13DT160_READWRITELENPAR, (uint)size);
			MacWriteRegister(MACREGISTER.FM13DT160_DATAPAR, data);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTWRITEMEMORY), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		/// <summary>
		/// mode = user, unlock, stop logging
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		void FM13DT160_Auth(uint mode, uint password)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, (uint)mode);
			MacWriteRegister(MACREGISTER.FM13DT160_PWDPAR, password);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTAUTH), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		void FM13DT160_GetTemp(int mode, int flag, uint storeto)
		{
			uint value = (uint)(0x80 | (flag = 0x04) | (flag = 0x02) | (flag = 0x01));

			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, value);
			MacWriteRegister(MACREGISTER.FM13DT160_READWRITELENPAR, storeto);
			
			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTGETTEMP), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_StartLog()
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, 0x00);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTSTARTLOG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);

		}

		void FM13DT160_StopLog(uint password)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, 0x50);
			MacWriteRegister(MACREGISTER.FM13DT160_PWDPAR, password);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTSTOPLOG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_WriteReg(int offset, int value)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_REGADDRPAR, (uint)offset);
			MacWriteRegister(MACREGISTER.FM13DT160_WRITEPAR, (uint)value);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTWRITEREG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_ReadReg(int offset)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_REGADDRPAR, (uint)offset);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTREADREG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);

		}

		void FM13DT160_DeepSleep(bool enable)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, enable ? 1U : 0U);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTDEEPSLEEP), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);

		}

		void FM13DT160_OpModeChk(bool enable)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, enable ? 1U : 0U);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTOPMODECHK), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_InitialRegFile()
		{
			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTINITIALREGFILE), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}

		void FM13DT160_LedCtrl(bool enable)
		{
			MacWriteRegister(MACREGISTER.FM13DT160_CMDCFGPAR, enable ? 1U : 0U);

			_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMMFM13DTLEDCTRL), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
		}


#if nouse
		bool FM13DT160_GetTemp()
		{
/*
			set 117 = 0
			call 5c
			set 119 = 0
			set 1108 = c012
			call 59
			set 117 = 6
			call 56
			set 117 = 86
			call 56
*/
			FM13DT160_OpModeChk(false);
			FM13DT160_WriteReg(0xc012, 0);

			FM13DT160_GetTemp(int mode, int flag, int storeto); // 0x06
			FM13DT160_GetTemp(int mode, int flag, int storeto); // 0x86
		}

		bool FM13DT160_GetBat()
		{
			/*
			117 = 0
			call 5c
			119 = 08
			118 = c012
			call 59
			117 = 12
			call 56
			117 = 92
			call 56 */

			FM13DT160_OpModeChk(false);
			FM13DT160_WriteReg(0xc012, 0x08);

			FM13DT160_GetTemp(int mode, int flag, int storeto); // 0x12
			FM13DT160_GetTemp(int mode, int flag, int storeto); // 0x92
		}

		bool FM13DT160_StartLog()
		{
			/*
			117 = 0
			call 5c
			11c = b040
			11d = 0004
			11e = 4cb329d6
			call 54
			11c = b094
			11d = 0004
			11e = 03000000 --
			call 54
			11c = b0a4
			11d = 0004
			11e = 0a000100
			call 54
			118 = c084
			119 = 01     --
			call 59
			line 3638344
			118 = c085
			119 = 0014   --
			call 59
			118 = c099
			119 = 0
			call 59
			118 = c098
			119 = 00000100
			call 59
			117 = 0
			call 57
			118 = c084
			call 5a */

			FM13DT160_OpModeChk(false);
			FM13DT160_WriteMemory(0xb040, 4, 0x4cb329d6);
			FM13DT160_WriteMemory(0xb094, 4, 0x03);
			FM13DT160_WriteMemory(0xb0a4, 4, 0x0a000100);
			FM13DT160_WriteReg(0xc084, 0x1);
			FM13DT160_WriteReg(0xc085, 0x14);
			FM13DT160_WriteReg(0xc099, 0);
			FM13DT160_WriteReg(0xc098, 0x100);
			FM13DT160_StartLog();
			FM13DT160_ReadReg(0xc084);
		}
#endif






	}
}
