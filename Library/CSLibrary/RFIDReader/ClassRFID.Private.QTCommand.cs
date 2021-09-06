using System;
using System.Collections.Generic;
using System.Text;

namespace CSLibrary
{
    public partial class RFIDReader
    {
        private void QT_CommandProc()
        {
            UInt32 value = 0;

            try
            {
                FireStateChangedEvent(CSLibrary.Constants.RFState.BUSY);

                // Set up the access password
                MacWriteRegister(MACREGISTER.HST_TAGACC_ACCPWD, m_rdr_opt_parms.QTCommand.accessPassword);

                //m_Result = RadioQT_Command(m_rdr_opt_parms.QTCommand.RW, m_rdr_opt_parms.QTCommand.TP, m_rdr_opt_parms.QTCommand.SR, m_rdr_opt_parms.QTCommand.MEM);

                MacReadRegister(MACREGISTER.HST_INV_CFG, ref value);
                value |= (1 << 14);
                MacWriteRegister(MACREGISTER.HST_INV_CFG, value);

                MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                value &= 0xFFC0000F;  // 1111111111 000000000000000000 1111
                if (m_rdr_opt_parms.QTCommand.RW != 0) value |= 1 << 4;
                if (m_rdr_opt_parms.QTCommand.TP != 0) value |= 1 << 5;
                if (m_rdr_opt_parms.QTCommand.SR != 0) value |= 1 << 21;
                if (m_rdr_opt_parms.QTCommand.MEM != 0) value |= 1 << 20;
                MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                //                m_pMac->WriteRegister(HST_CMD, CMD_CUSTOM_M4QT);
                //COMM_HostCommand(HST_CMD.CUSTOMM4QT);
                _deviceHandler.SendAsync(0, 0, DOWNLINKCMD.RFIDCMD, PacketData(0xf000, (UInt32)HST_CMD.CUSTOMM4QT), HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE_COMMANDENDRESPONSE, (UInt32)0xffffffff);
            }
            catch (System.Exception ex)
            {
#if DEBUG
//                CSLibrary.Diagnostics.CoreDebug.Logger.ErrorException("HighLevelInterface.QT_CommandProc()", ex);
#endif
            }

//            FireStateChangedEvent(RFState.IDLE);
        }
    }
}
