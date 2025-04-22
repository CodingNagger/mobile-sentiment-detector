# Mobile Sentiment Detector

## Blog post link

https://www.codingnagger.com/2025/04/22/entering-the-world-of-machine-learning-through-practice/

## Command to export model

```powershell
docker run -it --rm -v ${PWD}/exported_model:/app/exported_model optimum-cli-exporter

# Then within CLI
optimum-cli export onnx --model distilbert-base-uncased-finetuned-sst-2-english --task text-classification --opset 14 /app/exported_model

# Then from outside
rm -rf MobileSentimentDetector/Model/Assets/*.*
cp -r exported_model/* MobileSentimentDetector/Model/Assets/
```

