using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IQrManagements
    {
        public Dictionary<Qr, byte[]> QrList();
        public Qr CreateQRCode(Qr qr);
        public void RemoveQRCode(Qr qr);
        public byte[] GetQrCode(Qr qr);
        public bool CheckQrCodeExist(Qr qr);

    }

    public class Qr
    {
        public Guid scheduleId { get; set; }
        public bool isCheckIn { get; set; }
    }
}
