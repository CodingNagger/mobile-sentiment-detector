using System;
using System.Linq;

namespace MobileSentimentDetector.Model;

public class SentimentPrediction
{
    public string Sentiment { get; set; }
    public float NegativeProbability { get; set; }
    public float PositiveProbability { get; set; }

    public static SentimentPrediction FromLogits(float[] logits)
    {
        if (logits.Length != 2)
            throw new ArgumentException("Expected 2 logits for binary sentiment classification");

        // Compute softmax probabilities
        float[] expLogits = logits.Select(x => MathF.Exp(x)).ToArray();
        float sumExpLogits = expLogits.Sum();
        float[] probabilities = expLogits.Select(x => x / sumExpLogits).ToArray();

        // Determine sentiment
        int predictedIndex = Array.IndexOf(logits, logits.Max()); // Higher logit wins
        string sentiment = predictedIndex == 0 ? "Negative" : "Positive";

        return new SentimentPrediction
        {
            Sentiment = sentiment,
            NegativeProbability = probabilities[0],
            PositiveProbability = probabilities[1]
        };
    }
}