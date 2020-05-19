#if NETFRAMEWORK || NETSTANDARD2_0
using System;
using System.Text;
using static QRCoder.QRCodeGenerator;
using static QRCoder.SvgQRCode;

namespace QRCoder
{
    public class SvgQRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Constructor without params to be used in COM Objects connections
        /// </summary>
        public SvgQRCode() { }
        public SvgQRCode(QRCodeData data) : base(data) { }

        public string GetGraphic(int pixelsPerModule) => GetGraphic(pixelsPerModule * QrCodeData.ModuleMatrix.Count);

        public string GetGraphic(int size, string darkColorHex = "#000", string lightColorHex = "#fff", bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            var offset = drawQuietZones ? 0 : 4;
            var drawableModulesCount = QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
            var pixelsPerModule = Math.Min(size, size) / (double)drawableModulesCount;
            var qrSize = drawableModulesCount * pixelsPerModule;
            var svgSizeAttributes = (sizingMode == SizingMode.WidthHeightAttribute) ? $@"width=""{size}"" height=""{size}""" : $@"viewBox=""0 0 {size} {size}""";
            var svgFile = new StringBuilder($@"<svg version=""1.1"" baseProfile=""full"" shape-rendering=""crispEdges"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");
            svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{CleanSvgVal(qrSize)}"" height=""{CleanSvgVal(qrSize)}"" fill=""{lightColorHex}"" />");
            for (int xi = offset; xi < offset + drawableModulesCount; xi++)
            {
                for (int yi = offset; yi < offset + drawableModulesCount; yi++)
                {
                    if (QrCodeData.ModuleMatrix[yi][xi])
                    {
                        var x = (xi - offset) * pixelsPerModule;
                        var y = (yi - offset) * pixelsPerModule;
                        svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(pixelsPerModule)}"" height=""{CleanSvgVal(pixelsPerModule)}"" fill=""{darkColorHex}"" />");
                    }
                }
            }
            svgFile.Append(@"</svg>");
            return svgFile.ToString();
        }

        private string CleanSvgVal(double input)
        {
            //Clean double values for international use/formats
            return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public enum SizingMode
        {
            WidthHeightAttribute,
            ViewBoxAttribute
        }
    }

    public static class SvgQRCodeHelper
    {
        public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorHex, string lightColorHex, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            using (var qrCodeData = CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new SvgQRCode(qrCodeData))
                return qrCode.GetGraphic(pixelsPerModule * qrCodeData.ModuleMatrix.Count, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
        }
    }
}

#endif