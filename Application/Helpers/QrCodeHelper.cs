using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class QrCodeHelper
    {
        public static string GenerateQrBase64(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using Bitmap bitmap = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);

            return Convert.ToBase64String(ms.ToArray());
        }
    }

}
