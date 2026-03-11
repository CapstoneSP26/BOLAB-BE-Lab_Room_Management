using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.Managements
{
    public class QrManagements
    {
        public QrManagements(QRCodeGenerator qRCodeGenerator)
        {

            _qrCodeGenerator = qRCodeGenerator;
            //_qrCodeGenerator = new QRCodeGenerator();
            
            if (_qrList == null)
            {
                lock (_qrListLock)
                {
                    if (_qrList == null)
                    {
                        _qrList = new Dictionary<Guid, byte[]>();
                    }
                }
            }
        }
        private static Dictionary<Guid, byte[]> _qrList;
        private static readonly object _qrListLock = new object();
        private readonly QRCodeGenerator _qrCodeGenerator;

        public Dictionary<Guid, byte[]> QrList()
        {
            if (_qrList == null)
            {
                lock (_qrListLock)
                {
                    if (_qrList == null)
                    {
                        _qrList = new Dictionary<Guid, byte[]>();
                    }
                }
            }

            return _qrList;
        }

        public Guid CreateQRCode(Qr qr)
        {
            try
            {
                if (_qrList.ContainsKey(qr.scheduleId)) return Guid.Empty;

                var qrcode = _qrCodeGenerator.CreateQrCode(qr.scheduleId.ToString() + Guid.NewGuid(), QRCodeGenerator.ECCLevel.Q);
                var image = new PngByteQRCode(qrcode).GetGraphic(20);

                _qrList.Add(qr.scheduleId, image);

                Timer timer = new Timer((state) =>
                {
                    RemoveQRCode(qr.scheduleId);
                }, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);

                return qr.scheduleId;
            }
            catch (Exception e)
            {
                return Guid.Empty;
            }

        }

        public void RemoveQRCode(Guid scheduleId)
        {
            if (_qrList.ContainsKey(scheduleId))
            {
                _qrList.Remove(scheduleId);
            }
        }

        public byte[] GetQrCode(Guid scheduleId)
        {
            if (_qrList.ContainsKey(scheduleId))
            {
                return _qrList[scheduleId];
            }
            return null;
        }

        public bool CheckQrCodeExist(Guid qrId)
        {
            return _qrList.ContainsKey(qrId);
        }
    }

    public class Qr
    {
        public Guid scheduleId { get; set; }
    }
}
