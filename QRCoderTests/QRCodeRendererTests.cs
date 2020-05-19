using System;
using Xunit;
using QRCoder;
using Shouldly;
using QRCoderTests.XUnitExtenstions;
using System.IO;
using System.Security.Cryptography;

namespace QRCoderTests
{

    public class QRCodeRendererTests
    {
#if !NETCOREAPP
        [Fact]
        [Category("QRRenderer/QRCode")]
        public void Can_create_standard_qrcode_graphic()
        {
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var bmp = new PngByteQRCode(data).GetGraphic(10);
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(bmp);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("c362846367cf0a9999e53b50cfcecfe2");
        }
#endif
    }
}



