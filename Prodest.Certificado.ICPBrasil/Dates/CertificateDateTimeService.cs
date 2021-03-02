using System;

namespace Prodest.Certificado.ICPBrasil.Dates
{
    public class CertificateDateTimeService : ICertificateDateTimeService
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}