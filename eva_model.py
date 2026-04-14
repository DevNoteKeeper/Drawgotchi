# Evaluation section of model

import tensorflow as tf
import matplotlib.pyplot as plt
import os

try:
    import seaborn as sns
except ImportError:
    sns = None

from sklearn.metrics import confusion_matrix,f1_score, precision_score, recall_score
from train_model import load_data

# visualize training and validation loss to check for overfitting
def visualize_training(history):
    plt.figure(figsize=(5, 5))
    plt.plot(history.history['loss'], label='Train Loss', color='orange')
    plt.plot(history.history['val_loss'], label='Validation Loss', color='blue')
    plt.xlabel('Epochs')
    plt.ylabel('Loss')
    plt.legend()
    plt.title('Training and Validation Loss')
    plt.show()

# evaluate model performance with test data
def evaluate_model(m, test_ds, class_names):
    # check loss and accuracy
    loss, accuracy = m.evaluate(test_ds)
    print("Test loss:", loss)
    print("Test accuracy:", accuracy)

    # extract true label and predictions
    y_true = []
    y_pred = []

    # test_ds is grouped into batches, so labels/predictions are extracted batch-by-batch.
    for img, label in test_ds:
        y_true.extend(label.numpy())
        y_pred.extend(tf.argmax(m.predict(img, verbose=0), axis=1).numpy())

    # visualize confusion matrix to check correct and incorrect predictions
    cm = confusion_matrix(y_true, y_pred)
    plt.figure(figsize=(5, 5))
    if sns is not None:
        sns.heatmap(cm, annot=True, fmt='d', xticklabels=class_names, yticklabels=class_names)
    else:
        plt.imshow(cm, cmap='Blues')
        plt.colorbar()
        plt.xticks(range(len(class_names)), class_names, rotation=45, ha='right')
        plt.yticks(range(len(class_names)), class_names)
        for i in range(cm.shape[0]):
            for j in range(cm.shape[1]):
                plt.text(j, i, str(cm[i, j]), ha='center', va='center')
    plt.xlabel('Predicted')
    plt.ylabel('True')
    plt.title('Confusion Matrix')
    plt.tight_layout()
    plt.show()

    #calculate precision, recall, f1 to evaluate model performance in detail
    precision= precision_score(y_true, y_pred, average='macro', zero_division=0)
    recall= recall_score(y_true, y_pred, average='macro', zero_division=0)
    f1 = f1_score(y_true, y_pred, average='macro', zero_division=0)

    metrics_dict ={
        'accuracy': accuracy,
        'precision': precision,
        'recall': recall,
        'f1': f1,
    }

    for key, value in metrics_dict.items():
        print(f"{key}: {value}")

if __name__ == '__main__':
    if not os.path.exists("trained_model.keras"):
        print("No trained model found. Please run init.py to train the model first.")
        exit(1)
    _, val_ds, class_names = load_data()
    model = tf.keras.models.load_model("trained_model.keras")
    # visualize_training(model)
    evaluate_model(model, val_ds, class_names)