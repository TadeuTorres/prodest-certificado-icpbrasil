using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Prodest.Certificado.ICPBrasil.Certificados
{
    internal class Asn1Helper
    {
        private byte[] rawData;
        public readonly List<Tag> TagList;

        public Asn1Helper(ref byte[] rawData)
        {
            this.rawData = rawData;
            TagList = new List<Tag>();
            LoadTagList();
        }

        private void LoadTagList()
        {
            for (var offset = 0; offset < rawData.Length;)
            {
                TagList.Add(new Tag(ref rawData, ref offset));
            }
        }
    }

#pragma warning disable S1939 // Inheritance list should not be redundant

    internal enum TagId
#pragma warning restore S1939 // Inheritance list should not be redundant
    {
        Rfc822Name = 1,
        Integer = 2,
        BitString = 3,
        OctetString = 4,
        Null = 5,
        ObjectIdentifier = 6,
        Utf8String = 12,
        Sequence = 16,
        Set = 17,
        PrintableString = 19,
        T61String = 20,
        Ia5String = 22,
        UtcTime = 23
    }

    internal class Tag
    {
        public TagId TagId { get; }

        private int LengthOctets { get; }

        private int StartContents { get; }

        public Tag(ref byte[] rawdata, ref int offset)
        {
            LengthOctets = 0;

            // pode estar em formato Short ou Long
            // se ((rawData[offset] & 0x1f) == 0x1f)
            // formato Long não é usado nos certificados da ICP-Brasil
            // formato Short
            TagId = (TagId)(rawdata[offset] & 0x1f);
            offset++;

            // Octetos de tamanho
            if ((rawdata[offset] & 0x80) == 0x00)
            { // Formato Short: tamanho de até 127 bytes
                LengthOctets = (rawdata[offset++] & 0x7f);
            }
            else
            { // Formato Long: tamanho em 2 até 127 octetos
                var lenOctetos = rawdata[offset++] & 0x7f;
                LengthOctets = CalculaBase256(rawdata, ref offset, lenOctetos);
            }
            StartContents = offset;

            switch (TagId)
            {
                case TagId.ObjectIdentifier:
                case TagId.PrintableString:
                case TagId.OctetString:
                case TagId.Utf8String:
                case TagId.BitString:
                case TagId.Ia5String:
                case TagId.Integer:
                case TagId.Rfc822Name:
                case TagId.T61String:
                case TagId.UtcTime:
                    offset += LengthOctets;
                    break;

                case TagId.Null:
                case TagId.Sequence:
                case TagId.Set:
                    break;
            }
        }

        public string Format(byte[] rawdata)
        {
            switch (TagId)
            {
                case TagId.ObjectIdentifier:
                    return CalculaOid(rawdata, StartContents, LengthOctets);

                case TagId.Ia5String:
                case TagId.T61String:
                case TagId.PrintableString:
                case TagId.UtcTime:
                case TagId.OctetString:
                case TagId.Utf8String:
                case TagId.Rfc822Name:
                    return (new ASCIIEncoding()).GetString(rawdata, StartContents, LengthOctets);

                default:
                    return TagId.ToString();
            }
        }

        private static int CalculaBase256(IReadOnlyList<byte> rawdata, ref int offset, int length)
        {
            if (rawdata == null || rawdata.Count < offset + length) return 0;
            var tamanho = rawdata[offset++];

            for (var i = 1; i < length; i++)
            {
                tamanho <<= 8;
                tamanho += rawdata[offset++];
            }

            return tamanho;
        }

        private static string CalculaOid(IReadOnlyList<byte> rawdata, int offset, int length)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.CurrentCulture
                , "{0}.{1}", rawdata[offset] / 40, rawdata[offset] % 40);
            offset++;
            length--;

            for (var i = offset; i < (offset + length); i++)
            {
                var auxValue = rawdata[i] & 0x7f;

                if (
                    (rawdata[i] & 0x80) == 0x80
                    && i < offset + length)
                {
                    auxValue = (rawdata[i++] & 0x7f) << 7;
                    var auxValue2 = (rawdata[i] & 0x7f);

                    while (
                        (rawdata[i] & 0x80) == 0x80
                        && i < offset + length)
                    {
                        auxValue += auxValue2;
                        auxValue <<= 7;
                        auxValue2 = rawdata[++i] & 0x7f;
                    }
                    sb.AppendFormat(CultureInfo.CurrentCulture
                        , ".{0}", auxValue + auxValue2);
                }
                else
                {
                    sb.AppendFormat(CultureInfo.CurrentCulture
                        , ".{0}", auxValue);
                }
            }
            return sb.ToString();
        }
    }
}