using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using System.Drawing;


namespace ConsoleAppQRCodeReader
{
    class Program
    {
        /// <summary>
        /// Функция для получения напрямую массива байтов из ZXing.Result.
        /// Стандартная реализация ZXing.Result отдает строку, но не байты, по которым строка была создана.
        /// 
        /// Алгоритм в функции был переписан из алгоритма для Java: https://stackoverflow.com/a/4981787/5909792
        /// </summary>
        /// <param name="result"></param>
        /// <returns>byte[]</returns>
        static byte[] GetBytesFromZXingResult(Result result)
        {
            var byteSegments = (IList<byte[]>)result.ResultMetadata[ResultMetadataType.BYTE_SEGMENTS];
            
            int totalLength = 0;
            foreach (byte[] bs in byteSegments)
            {
                totalLength += bs.Length;
            }

            byte[] resultBytes = new byte[totalLength];
            int i = 0;
            foreach (byte[] bs in byteSegments)
            {
                foreach (byte b in bs)
                {
                    resultBytes[i++] = b;
                }
            }

            return resultBytes;
        }

        static void Main(string[] args)
        {
            IBarcodeReader reader = new BarcodeReader();

            // Если из локального файла
            //var barcodeBitmap = (Bitmap) Image.FromFile("qr_code_1.png");

            // Берем картинки из ресурсов
            Bitmap[] barcodeBitmapList = { Properties.Resource.Image1, Properties.Resource.Image2 };

            foreach (var barcodeBitmap in barcodeBitmapList)
            {
                // Пытаемся разобрать qr-код
                var result = reader.Decode(barcodeBitmap);
                if (result == null)
                {
                    Console.WriteLine("Failure to parse qr-code");
                }
                else
                {
                    // По умолчанию, ZXing.Result возвращает уже строку по данным в QR-кода
                    Console.WriteLine("Result: " + result);
                    Console.WriteLine("result.Text: " + result.Text);

                    // Поле RawBytes, похоже означает саму структуру данных QR-кода, но не данные в нем:
                    Console.WriteLine("result.RawBytes: " + Encoding.UTF8.GetString(result.RawBytes));

                    // Применяем функцию чтобы получить байтовый массив данных в QR-коде, в обход поля ZXing.Result.Text
                    var resultBytes = GetBytesFromZXingResult(result);

                    Console.WriteLine("resultBytes: " + Encoding.UTF8.GetString(resultBytes));
                    Console.WriteLine();
                }
            }
        }
    }
}
