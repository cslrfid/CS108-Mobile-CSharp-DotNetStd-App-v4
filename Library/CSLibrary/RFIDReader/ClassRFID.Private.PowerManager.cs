/*
Copyright (c) 2018 Convergence Systems Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSLibrary.Constants;

namespace CSLibrary
{
	public partial class RFIDReader
	{
		/*		bool _currentPowerMode = true;


				internal void ResetToDefaultPowerMode()
				{
					_currentPowerMode = true;
				}


				/// <summary>
				/// mode = false, power saving mode
				///	mode = true, normal power mode (or inventory mode)
				/// </summary>
				/// <param name="mode"></param>
				internal void SetReaderPowerMode (bool mode)
				{
					CSLibrary.Debug.WriteLine("SetReaderPowerMode : " + mode.ToString());

					if (mode)
					{
						if (!_currentPowerMode)
						{
							_currentPowerMode = true;
							MacWriteRegister(MACREGISTER.HST_PWRMGMT, 0x00);
							//_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.SETPWRMGMTCFG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
						}
					}
					else
					{
						if (_currentPowerMode)
						{
							_currentPowerMode = false;
							MacWriteRegister(MACREGISTER.HST_PWRMGMT, 0x01);
							_deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.SETPWRMGMTCFG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
						}
					}
				}
		*/

		internal bool _SetRFIDToStandbyMode = true;

		internal void SetToStandbyMode()
		{
			if (_SetRFIDToStandbyMode)
			{
				MacWriteRegister(MACREGISTER.HST_PWRMGMT, 0x01);
				_deviceHandler.SendAsyncUrgent(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.SETPWRMGMTCFG), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE);
				_SetRFIDToStandbyMode = false;
			}
		}

	}
}

/*
using System;
using System.Collections.Generic;
using System.Text;

namespace CSLibrary.RFIDReader
{
	class ClassRFID
	{
	}
}
*/