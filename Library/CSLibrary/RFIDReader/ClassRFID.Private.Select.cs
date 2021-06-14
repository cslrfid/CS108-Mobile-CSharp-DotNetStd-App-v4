﻿/*
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

namespace CSLibrary
{
    public partial class RFIDReader
    {
        private Structures.TagSelectedParms _tagSelectedParms;

        private void TagSelected()
        {
            try
            {
                _tagSelectedParms = (Structures.TagSelectedParms)m_rdr_opt_parms.TagSelected.Clone();

                UInt32 value = 0;

                MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                value |= 0x0001U; // Enable Verify after write
                MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                MacReadRegister(MACREGISTER.HST_QUERY_CFG, ref value);
                value &= ~0x0200U; // Enable Ucode Parallel encoding
                MacWriteRegister(MACREGISTER.HST_QUERY_CFG, value);

                SetOperationMode(CSLibrary.Constants.RadioOperationMode.NONCONTINUOUS);
                SetTagGroup(CSLibrary.Constants.Selected.ASSERTED, CSLibrary.Constants.Session.S0, CSLibrary.Constants.SessionTarget.A);
                SetSingulationAlgorithmParms(CSLibrary.Constants.SingulationAlgorithm.FIXEDQ, new CSLibrary.Structures.FixedQParms
                    (
                    m_rdr_opt_parms.TagSelected.Qvalue,//QValue
                    0x5, //Retry
                    (uint)((m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE) == CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE ? 1 : 0),//toggle
                    0)//repeatUntilNoUnit
                    );

                CSLibrary.Structures.SelectCriterion[] sel = new CSLibrary.Structures.SelectCriterion[1];
                sel[0] = new CSLibrary.Structures.SelectCriterion();
                sel[0].action = new CSLibrary.Structures.SelectAction(CSLibrary.Constants.Target.SELECTED,
                    (m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH) == CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH ?
                    CSLibrary.Constants.Action.DSLINVB_ASLINVA : CSLibrary.Constants.Action.ASLINVA_DSLINVB, 0);


                if (m_rdr_opt_parms.TagSelected.bank == CSLibrary.Constants.MemoryBank.EPC)
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        (uint)((m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK) == CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK ? 16 : 32 + m_rdr_opt_parms.TagSelected.epcMaskOffset),
                        m_rdr_opt_parms.TagSelected.epcMaskLength,
                        m_rdr_opt_parms.TagSelected.epcMask.ToBytes());
                }
                else
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        m_rdr_opt_parms.TagSelected.MaskOffset,
                        m_rdr_opt_parms.TagSelected.MaskLength,
                        m_rdr_opt_parms.TagSelected.Mask);
                }
                if ((m_Result = SetSelectCriteria(sel)) != CSLibrary.Constants.Result.OK)
                {
                    //goto EXIT;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                //CSLibrary.Diagnostics.CoreDebug.Logger.ErrorException("HighLevelInterface.TagSelected()", ex);
#endif
                m_Result = CSLibrary.Constants.Result.SYSTEM_CATCH_EXCEPTION;
            }
        }

        private void TagSelectedDYNQ()
        {
            try
            {
                _tagSelectedParms = (Structures.TagSelectedParms)m_rdr_opt_parms.TagSelected.Clone();

                UInt32 value = 0;

                MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                value |= 0x0001U; // Enable Verify after write
                MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                MacReadRegister(MACREGISTER.HST_QUERY_CFG, ref value);
                value &= ~0x0200U; // Enable Ucode Parallel encoding
                MacWriteRegister(MACREGISTER.HST_QUERY_CFG, value);

                SetOperationMode(CSLibrary.Constants.RadioOperationMode.NONCONTINUOUS);
                SetTagGroup(CSLibrary.Constants.Selected.ASSERTED, CSLibrary.Constants.Session.S0, CSLibrary.Constants.SessionTarget.A);
                {
                    CSLibrary.Structures.DynamicQParms param = new CSLibrary.Structures.DynamicQParms();

                    param.startQValue = m_rdr_opt_parms.TagSelected.Qvalue;
                    param.toggleTarget = (uint)((m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE) == CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE ? 1 : 0);
                    param.retryCount = 0x05;

                    SetSingulationAlgorithmParms(CSLibrary.Constants.SingulationAlgorithm.DYNAMICQ, param);
                }

                CSLibrary.Structures.SelectCriterion[] sel = new CSLibrary.Structures.SelectCriterion[1];
                sel[0] = new CSLibrary.Structures.SelectCriterion();
                sel[0].action = new CSLibrary.Structures.SelectAction(CSLibrary.Constants.Target.SELECTED,
                    (m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH) == CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH ?
                    CSLibrary.Constants.Action.DSLINVB_ASLINVA : CSLibrary.Constants.Action.ASLINVA_DSLINVB, 0);

                if (m_rdr_opt_parms.TagSelected.bank == CSLibrary.Constants.MemoryBank.EPC)
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        (uint)((m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK) == CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK ? 16 : 32 + m_rdr_opt_parms.TagSelected.epcMaskOffset),
                        m_rdr_opt_parms.TagSelected.epcMaskLength,
                        m_rdr_opt_parms.TagSelected.epcMask.ToBytes());
                }
                else
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        m_rdr_opt_parms.TagSelected.MaskOffset,
                        m_rdr_opt_parms.TagSelected.MaskLength,
                        m_rdr_opt_parms.TagSelected.Mask);
                }
                if ((m_Result = SetSelectCriteria(sel)) != CSLibrary.Constants.Result.OK)
                {
                    //goto EXIT;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                //CSLibrary.Diagnostics.CoreDebug.Logger.ErrorException("HighLevelInterface.TagSelected()", ex);
#endif
                m_Result = CSLibrary.Constants.Result.SYSTEM_CATCH_EXCEPTION;
            }
        }

        /// <summary>
        /// Only set first EPC ID and length (register 0x804-0x807)
        /// </summary>
        private void FastTagSelected()
        {
            uint registerValue;

            //SelectCriterion pCriterion = SC.pCriteria[index];
            //SelectMask pMask = pCriterion.mask;
            //SelectAction pAction = pCriterion.action;

            // Instruct the MAC as to which select mask we want to work with
            MacWriteRegister(MACREGISTER.HST_TAGMSK_DESC_SEL, 0);

            // Create the HST_TAGMSK_LEN register and write it to the MAC
            registerValue = (uint)(m_rdr_opt_parms.TagSelected.epcMaskLength);
            MacWriteRegister(MACREGISTER.HST_TAGMSK_LEN, registerValue);

            // Now write the MAC's mask registers
            WriteMacMaskRegisters((ushort)MACREGISTER.HST_TAGMSK_0_3, m_rdr_opt_parms.TagSelected.epcMaskLength, m_rdr_opt_parms.TagSelected.epcMask.ToBytes());
            // Set up the selection criteria

        }

        private void PreFilter()
        {
            try
            {
                UInt32 value = 0;

                MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                value |= 0x0001U; // Enable Verify after write
                MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                MacReadRegister(MACREGISTER.HST_QUERY_CFG, ref value);
                value &= ~0x0200U; // Enable Ucode Parallel encoding
                MacWriteRegister(MACREGISTER.HST_QUERY_CFG, value);

                CSLibrary.Structures.SelectCriterion[] sel = new CSLibrary.Structures.SelectCriterion[1];
                sel[0] = new CSLibrary.Structures.SelectCriterion();
                sel[0].action = new CSLibrary.Structures.SelectAction(CSLibrary.Constants.Target.SELECTED,
                    (m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH) == CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH ?
                    CSLibrary.Constants.Action.DSLINVB_ASLINVA : CSLibrary.Constants.Action.ASLINVA_DSLINVB, 0);

                //SetTagGroup(CSLibrary.Constants.Selected.ASSERTED, CSLibrary.Constants.Session.S0, CSLibrary.Constants.SessionTarget.A);
                SetTagGroup(CSLibrary.Constants.Selected.ASSERTED);

                if (m_rdr_opt_parms.TagSelected.bank == CSLibrary.Constants.MemoryBank.EPC)
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        (uint)((m_rdr_opt_parms.TagSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK) == CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK ? 16 : 32 + m_rdr_opt_parms.TagSelected.epcMaskOffset),
                        m_rdr_opt_parms.TagSelected.epcMaskLength,
                        m_rdr_opt_parms.TagSelected.epcMask.ToBytes());
                }
                else
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagSelected.bank,
                        m_rdr_opt_parms.TagSelected.MaskOffset,
                        m_rdr_opt_parms.TagSelected.MaskLength,
                        m_rdr_opt_parms.TagSelected.Mask);
                }
                if ((m_Result = SetSelectCriteria(sel)) != CSLibrary.Constants.Result.OK)
                {
                    //goto EXIT;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                //CSLibrary.Diagnostics.CoreDebug.Logger.ErrorException("HighLevelInterface.TagSelected()", ex);
#endif
                m_Result = CSLibrary.Constants.Result.SYSTEM_CATCH_EXCEPTION;
            }
        }

        private void SetMaskThreadProc()
        {
            try
            {
                //FireStateChangedEvent(RFState.BUSY);

                if (m_rdr_opt_parms.TagGeneralSelected.ParallelEncoding)
                {
                    UInt32 value = 0;

                    MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                    value &= ~0x0001U; // Disable Verify after write
                    MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                    MacReadRegister(MACREGISTER.HST_QUERY_CFG, ref value);
                    value |= 0x0200; // Enable Ucode Parallel encoding
                    MacWriteRegister(MACREGISTER.HST_QUERY_CFG, value);
                }
                else
                {
                    UInt32 value = 0;

                    MacReadRegister(MACREGISTER.HST_TAGACC_DESC_CFG, ref value);
                    value |= 0x0001U; // Enable Verify after write
                    MacWriteRegister(MACREGISTER.HST_TAGACC_DESC_CFG, value);

                    MacReadRegister(MACREGISTER.HST_QUERY_CFG, ref value);
                    value &= ~0x0200U; // Enable Ucode Parallel encoding
                    MacWriteRegister(MACREGISTER.HST_QUERY_CFG, value);
                }

                CSLibrary.Structures.SelectCriterion[] sel = new CSLibrary.Structures.SelectCriterion[1];
                sel[0] = new CSLibrary.Structures.SelectCriterion();
                sel[0].action = new CSLibrary.Structures.SelectAction(CSLibrary.Constants.Target.SELECTED,
                    (m_rdr_opt_parms.TagGeneralSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH) == CSLibrary.Constants.SelectMaskFlags.ENABLE_NON_MATCH ?
                    CSLibrary.Constants.Action.DSLINVB_ASLINVA : CSLibrary.Constants.Action.ASLINVA_DSLINVB, 0);


                if (m_rdr_opt_parms.TagGeneralSelected.bank == CSLibrary.Constants.MemoryBank.EPC)
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagGeneralSelected.bank,
                        (uint)((m_rdr_opt_parms.TagGeneralSelected.flags & CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK) == CSLibrary.Constants.SelectMaskFlags.ENABLE_PC_MASK ? 16 : 32 + m_rdr_opt_parms.TagSelected.epcMaskOffset),
                        m_rdr_opt_parms.TagGeneralSelected.epcMaskLength,
                        m_rdr_opt_parms.TagGeneralSelected.epcMask.ToBytes());
                }
                else
                {
                    sel[0].mask = new CSLibrary.Structures.SelectMask(
                        m_rdr_opt_parms.TagGeneralSelected.bank,
                        m_rdr_opt_parms.TagGeneralSelected.MaskOffset,
                        m_rdr_opt_parms.TagGeneralSelected.MaskLength,
                        m_rdr_opt_parms.TagGeneralSelected.Mask);
                }
                if ((m_Result = SetSelectCriteria(sel)) != CSLibrary.Constants.Result.OK)
                {
                    goto EXIT;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                //CSLibrary.Diagnostics.CoreDebug.Logger.ErrorException("HighLevelInterface.TagSelected()", ex);
#endif
                m_Result = CSLibrary.Constants.Result.SYSTEM_CATCH_EXCEPTION;
            }

            EXIT:

            var abc = 10;

            //FireStateChangedEvent(RFState.IDLE);
        }


    }
}
