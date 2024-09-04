using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PdfFlipBook.Helper
{
    public class JsonHelper
    {
        public T ReadJsonFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            var jsonContent = File.ReadAllText(filePath, Encoding.UTF8);
            var deserializedObject = JsonConvert.DeserializeObject<T>(jsonContent);
            return deserializedObject;
        }

        public void WriteJsonToFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();


            var jsonContent = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);

            if (append)
                File.AppendAllText(filePath, jsonContent + Environment.NewLine, Encoding.UTF8);
            else
                File.WriteAllText(filePath, jsonContent, Encoding.UTF8);
        }
    }
}
