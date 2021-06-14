//#if CS468
using System;
using System.Collections.Generic;
using System.Text;
#if NETCFDESIGNTIME

namespace CSLibrary
{
    using CSLibrary.Constants;
    using CSLibrary.Structures;
    /// <summary>
    /// AntennaType Converter
    /// </summary>
    public class AntennaTypeConverter
        :
        System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to
        ///     the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom
        (
            System.ComponentModel.ITypeDescriptorContext context,
            Type sourceType
        )
        {
            return
                   typeof(string) == sourceType
                || base.CanConvertFrom(context, sourceType);
        }
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        ///     context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertFrom
        (
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            Object value
        )
        {
            if (String.IsNullOrEmpty(value as string))
            {
                return null; // TODO : supply err msg
            }

            String[] antennaData = (value as String).Split(new Char[] { ',' });

            if (null == antennaData)
            {
                return null; // TODO : supply err msg ~ improper arg 
            }

            if (8 != antennaData.Length)
            {
                return null; // TODO : supply err msg ~ improper arg count
            }

            try
            {
                // TODO : split out parsing ? to better define which parms bad...

                Antenna antenna = new Antenna(UInt32.Parse(antennaData[0]));

                AntennaPortState state =
                    (AntennaPortState)Enum.Parse
                    (
                        typeof(AntennaPortState),
                        antennaData[1].ToUpper()
                    );

                antenna.State = state;
                antenna.PowerLevel = UInt32.Parse(antennaData[2]);
                antenna.DwellTime = UInt32.Parse(antennaData[3]);
                antenna.NumberInventoryCycles = UInt32.Parse(antennaData[4]);
                antenna.PhysicalTxPort = UInt32.Parse(antennaData[5]);

                // Currently Rx is explicitly tied to Tx so cannot be set - ignore val
                antenna.PhysicalRxPort        = UInt32.Parse( antennaData[ 6 ] );

                antenna.AntennaSenseThreshold = UInt32.Parse(antennaData[7]);

                return antenna;
            }
            catch (Exception)
            {
                // TODO : supply err msg ~ bad arg

                return null;
            }
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        ///     context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture
        /// is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo
        (
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            Type destinationType
        )
        {
            if (typeof(string) == destinationType)
            {
                Antenna antenna = value as Antenna;

                if (null == antenna)
                {
                    throw new ArgumentException("Expected a Antenna", "value");
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat
                (
                    "{0},{1},{2},{3},{4},{5},{6},{7}",
                    antenna.Port,
                    antenna.State,
                    antenna.PowerLevel,
                    antenna.DwellTime,
                    antenna.NumberInventoryCycles,
                    antenna.PhysicalTxPort,
                    antenna.PhysicalRxPort,
                    antenna.AntennaSenseThreshold
                );

                return sb.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }


    } // END class AntennaTypeConverter


} // END namespace CSLibrary
#endif
//#endif