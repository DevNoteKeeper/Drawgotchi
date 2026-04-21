# Python Module (AI Pipeline)

Handles doodle classification and HTTP API for Unity communication.

## Role

- Preprocesses tablet drawings (96x96 grayscale)
- CNN model predicts 6 classes
- HTTP server responds with predictions

## Classes

- 🥬 Vegetable
- 🥩 Meat
- 🍜 Noodle
- 🍞 Bread
- 🥤 Drink
- 🛏️ Bed

## Dataset

- ~600 original doodles (approximately 100 per class)
- ~2400 after augmentation (rotation, scaling, blur, noise)
- 80/20 train/validation split

## Model

Architecture: CNN
Input: 96x96x1 grayscale
Validation accuracy: 87.25%
Precision: 87.77% | Recall: 87.34% | F1: 87.53%
Confidence threshold: 0.55
