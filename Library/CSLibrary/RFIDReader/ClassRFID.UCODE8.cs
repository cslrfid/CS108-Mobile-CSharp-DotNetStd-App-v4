using System;
using System.Collections.Generic;
using System.Text;

namespace CSLibrary
{
	public partial class RFIDReader
	{
		public bool SetBrandID(bool enable)
		{
			//Get 901 and Set bit 27

			UInt32 value = 0;

			MacReadRegister(MACREGISTER.HST_INV_CFG, ref value);

			if (enable)
				value |= (1 << 27);
			else
				value &= ~(1U << 27);

			_deviceHandler.rfid.MacWriteRegister(RFIDReader.MACREGISTER.HST_INV_CFG, value);

			return true;
		}
	}
}
