import tensorflow as tf
from tensorflow import keras

image_size = (480,310)

def load_data():
    train_ds = tf.keras.utils.image_dataset_from_directory(
        "dataset",
        validation_split=0.2,
        subset="training",
        seed=123,
        image_size=image_size,
        batch_size=32,
        color_mode="grayscale",
    )

    val_ds = tf.keras.utils.image_dataset_from_directory(
        "dataset",
        validation_split=0.2,
        subset="validation",
        seed=123,
        image_size=image_size,
        batch_size=32,
        color_mode="grayscale",
    )

    AUTOTUNE = tf.data.AUTOTUNE

    train_ds = train_ds.cache().shuffle(1000).prefetch(buffer_size=AUTOTUNE)
    val_ds = val_ds.cache().prefetch(buffer_size=AUTOTUNE)

    return train_ds, val_ds

def build_model(num_classes):
    model = keras.Sequential([
        keras.layers.Input(shape=(image_size[0], image_size[1], 1)),
        keras.layers.Rescaling(1./255),

        # keras.layers.RandomFlip("horizontal"),
        # keras.layers.RandomRotation(0.2),  # ±20% rotation
        # keras.layers.RandomZoom(0.2),      # ±20% zoom
        keras.layers.RandomTranslation(0.2, 0.2),  # Shifts (random movement)


        keras.layers.Conv2D(16, 3, padding='same', activation='relu'),
        keras.layers.MaxPooling2D(),
        keras.layers.Conv2D(32, 3, padding='same', activation='relu'),
        keras.layers.MaxPooling2D(),
        keras.layers.Conv2D(64, 3, padding='same', activation='relu'),
        keras.layers.MaxPooling2D(),
        keras.layers.Flatten(),
        keras.layers.Dense(256, activation='relu'),
        keras.layers.Dense(num_classes)
    ])

    model.compile(optimizer=keras.optimizers.Adam(learning_rate=0.001),
              loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=True),
              metrics=['accuracy'])
    return model

def train_model(model, train_ds, val_ds):
    model.fit(
        train_ds,
        validation_data=val_ds,
        epochs=30
    )

def confusion_matrix(model, val_ds):
    y_true = []
    y_pred = []

    for images, labels in val_ds:
        predictions = model.predict(images)
        predicted_labels = tf.argmax(predictions, axis=1)
        y_true.extend(labels.numpy())
        y_pred.extend(predicted_labels.numpy())

    cm = tf.math.confusion_matrix(y_true, y_pred)
    print("Confusion Matrix:")
    print(cm)


def main():
    train_ds, val_ds = load_data()
    model = build_model(6)
    train_model(model, train_ds, val_ds)
    confusion_matrix(model, val_ds)

    model.save("model.keras")

if __name__ == "__main__":
    main()