using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MobileSentimentDetector.Model
{
    public class VocabularyReader
    {
        public static List<string> ReadResource(string resourceName)
        {
            var result = new List<string>();

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        result.Add(line);
                    }
                }
            }

            return result;
        }
    }
}