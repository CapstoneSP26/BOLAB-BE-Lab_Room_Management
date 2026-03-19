using BookLAB.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Services
{
    /// <summary>
    /// Service for managing QR codes in the attendance system.
    /// Responsible for creating, storing, retrieving, and removing QR codes.
    /// </summary>
    public class QrManagements : IQrManagements
    {
        public QrManagements(QRCodeGenerator qRCodeGenerator,
            IHttpClientFactory httpClientFactory,
            ILogger<QrManagements> logger)
        {
            // Initialize dependencies injected via DI
            _qrCodeGenerator = qRCodeGenerator;
            _client = httpClientFactory.CreateClient("BackendApi"); // Named HttpClient with BaseAddress configured
            _logger = logger;

            // Initialize the static dictionary if not already created
            if (_qrList == null)
            {
                lock (_qrListLock)
                {
                    if (_qrList == null)
                    {
                        _qrList = new Dictionary<Qr, byte[]>();
                    }
                }
            }
        }

        // Static dictionary storing QR codes: key = Qr object, value = QR image bytes
        private static Dictionary<Qr, byte[]> _qrList;
        private static readonly object _qrListLock = new object(); // Lock for thread-safety
        private readonly QRCodeGenerator _qrCodeGenerator; // Used to generate QR codes
        private readonly HttpClient _client;               // HttpClient for building URLs
        private readonly ILogger<QrManagements> _logger;   // Logger for error reporting

        /// <summary>
        /// Returns the current dictionary of QR codes.
        /// Ensures initialization if not already created.
        /// </summary>
        public Dictionary<Qr, byte[]> QrList()
        {
            if (_qrList == null)
            {
                lock (_qrListLock)
                {
                    if (_qrList == null)
                    {
                        _qrList = new Dictionary<Qr, byte[]>();
                    }
                }
            }
            return _qrList;
        }

        /// <summary>
        /// Creates a new QR code for the given Qr object.
        /// Encodes a URL pointing to the attendance API with parameters.
        /// Stores the QR code image in memory and schedules removal after 5 minutes.
        /// </summary>
        /// <param name="qr">Qr object containing scheduleId and check-in flag.</param>
        /// <returns>The Qr object if successful, null if failed.</returns>
        public Qr CreateQRCode(Qr qr)
        {
            try
            {
                // If QR already exists, return it
                if (_qrList.ContainsKey(qr)) return qr;

                // Build the URL for attendance scanning
                var url = new Uri(_client.BaseAddress,
                    "api/attendances/scan-qrcode?" +
                    "qrId=" + Guid.NewGuid() +
                    "&scheduleId=" + qr.scheduleId.ToString() +
                    "&isCheckIn=" + qr.isCheckIn +
                    "&studentId=").ToString();

                // Generate QR code from the URL
                var qrcode = _qrCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var image = new PngByteQRCode(qrcode).GetGraphic(20); // Convert to PNG byte array

                // Store QR code in dictionary
                _qrList.Add(qr, image);

                // Schedule removal after 5 minutes
                Timer timer = new Timer((state) =>
                {
                    RemoveQRCode(qr);
                }, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);

                return qr;
            }
            catch (Exception e)
            {
                // Log error with context
                _logger.LogError(e, "Failed to generate QR code for ScheduleId {ScheduleId}, IsCheckIn {IsCheckIn}", qr.scheduleId, qr.isCheckIn);
                return null;
            }
        }

        /// <summary>
        /// Removes a QR code from the dictionary.
        /// </summary>
        public void RemoveQRCode(Qr qr)
        {
            if (_qrList.ContainsKey(qr))
            {
                _qrList.Remove(qr);
            }
        }

        /// <summary>
        /// Retrieves the QR code image bytes for the given Qr object.
        /// </summary>
        public byte[] GetQrCode(Qr qr)
        {
            if (_qrList.ContainsKey(qr))
            {
                return _qrList[qr];
            }
            return null;
        }

        /// <summary>
        /// Checks if a QR code exists for the given Qr object.
        /// </summary>
        public bool CheckQrCodeExist(Qr qr)
        {
            return _qrList.ContainsKey(qr);
        }
    }

}
