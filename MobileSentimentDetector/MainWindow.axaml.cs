using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using MobileSentimentDetector.Model;
using Microsoft.ML.OnnxRuntime;

namespace MobileSentimentDetector;

public class ModelInput
{
    [ColumnName("input_ids")] public long[] InputIds { get; set; }
    [ColumnName("attention_mask")] public long[] AttentionMask { get; set; }
    [ColumnName("token_type_ids")] public long[] TokenTypeIds { get; set; }
}

public class ModelOutput
{
    [ColumnName("logits")] public float[] Logits { get; set; }
}

public partial class MainWindow : Window
{
    private PredictionEngine<ModelInput, ModelOutput> pipeline;

    public MainWindow()
    {

        Logger.TryGet(LogEventLevel.Fatal, LogArea.Control)?.Log(this, "Avalonia Infrastructure");
        Console.WriteLine("System Diagnostics Console.WriteLine");

        InitializeComponent();
        pipeline = CreatePredictionPipeline();
    }

    public PredictionEngine<ModelInput, ModelOutput> CreatePredictionPipeline()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        foreach (var name in resourceNames)
            Console.WriteLine(name);

        try
        {
            var sessionOptions = new SessionOptions();
            Console.WriteLine("ONNX Runtime initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ONNX Runtime initialization failed: {ex}");
        }

        string resourceName = "MobileSentimentDetector.Model.Assets.model.onnx";
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
                throw new Exception("Resource not found: " + resourceName);

            var mlContext = new MLContext();

            // Save the ONNX model to a temporary file or memory stream
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                // Define the ONNX model input and output columns
                var onnxPredictionPipeline = mlContext.Transforms.ApplyOnnxModel(
                    outputColumnNames: new[] { "logits" }, // Replace with your model's output column name
                    inputColumnNames: new[] { "input_ids", "attention_mask" },   // Replace with your model's input column name
                    modelBytes: memoryStream,
                    fallbackToCpu: true);

                // Create a transformer from the pipeline
                var transformer = onnxPredictionPipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<ModelInput>()));
                var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(transformer);

                return predictionEngine;
            }
        }
    }

    public void DetectSentiment(object sender, RoutedEventArgs args)
    {
        try
        {
            sentimentPlaceholder.Text = Detect(input.Text);
        }
        catch (Exception exception)
        {
            sentimentPlaceholder.Text = exception.ToString();
            Console.WriteLine($"{exception}");
        }
    }

    string Detect(string sentence)
    {
        // Create Tokenizer and tokenize the sentence.
        var tokenizer = new MobileBertTokenizer();

        // Get the sentence tokens.
        var tokens = tokenizer.Tokenize(sentence);
        Console.WriteLine(String.Join(", ", tokens));

        // Encode the sentence and pass in the count of the tokens in the sentence. // had to fix to set count to 128 instead
        var encoded = tokenizer.Encode(tokens.Count, sentence);

        // Break out encoding to InputIds, AttentionMask from list of (input_id, attention_mask, type_id).
        const int maxSequenceLength = 128;
        var bertInput = new ModelInput()
        {
            InputIds = encoded.Select(t => t.InputIds).ToArray(),
            AttentionMask = encoded.Select(t => t.AttentionMask).ToArray(),
            TokenTypeIds = encoded.Select(t => t.TokenTypeIds).ToArray()
        };

        var prediction = pipeline.Predict(bertInput);

        Console.WriteLine(JsonSerializer.Serialize(prediction.Logits));

        return SentimentPrediction.FromLogits(prediction.Logits).Sentiment;
    }
}

