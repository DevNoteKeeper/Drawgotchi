import tensorflow as tf
from tensorflow import keras

image_size = (96, 96)

def load_data():
    train_ds = tf.keras.utils.image_dataset_from_directory(
        "augmented_data",
        validation_split=0.2,
        subset="training",
        seed=123,
        image_size=image_size,
        batch_size=8,
        color_mode="grayscale",
    )

    val_ds = tf.keras.utils.image_dataset_from_directory(
        "augmented_data",
        validation_split=0.2,
        subset="validation",
        seed=123,
        image_size=image_size,
        batch_size=8,
        color_mode="grayscale",
    )

    AUTOTUNE = tf.data.AUTOTUNE

    train_ds = train_ds.cache().shuffle(1000).prefetch(buffer_size=AUTOTUNE)
    val_ds = val_ds.cache().prefetch(buffer_size=AUTOTUNE)

    return train_ds, val_ds


def build_model(num_classes):
    model = keras.Sequential([
        keras.layers.Input(shape=(image_size[0], image_size[1], 1)),
        keras.layers.Rescaling(1. / 255),

        # More aggressive augmentation for tiny datasets
        keras.layers.RandomFlip("horizontal"),
        keras.layers.RandomRotation(0.15),
        keras.layers.RandomZoom(0.15),
        keras.layers.RandomTranslation(0.15, 0.15),

        keras.layers.Conv2D(32, 3, padding='same', activation='relu'),
        keras.layers.BatchNormalization(),
        keras.layers.Dropout(0.25),

        keras.layers.Conv2D(64, 3, padding='same', activation='relu'),
        keras.layers.MaxPooling2D(),
        keras.layers.Dropout(0.25),

        keras.layers.Conv2D(128, 3, padding='same', activation='relu'),
        keras.layers.MaxPooling2D(),
        keras.layers.Dropout(0.3),

        keras.layers.Flatten(),
        keras.layers.Dense(256, activation='relu'),
        keras.layers.BatchNormalization(),
        keras.layers.Dropout(0.5),
        keras.layers.Dense(num_classes)
    ])

    model.compile(optimizer=keras.optimizers.Adam(learning_rate=1e-3),
                  loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=True),
                  metrics=['accuracy'])
    return model


def train_model(model, train_ds, val_ds):
    model.fit(
        train_ds,
        validation_data=val_ds,
        epochs=300,
        callbacks=[
            keras.callbacks.EarlyStopping(
                monitor="val_accuracy",
                mode="max",
                patience=100,
                restore_best_weights=True,
                min_delta=0.001,
                verbose=1,
            ),
            keras.callbacks.ReduceLROnPlateau(
                monitor="val_loss",
                factor=0.5,
                patience=25,
                min_lr=1e-6,
                verbose=1,
            ),
            keras.callbacks.ModelCheckpoint(
                "best_model.keras",
                monitor="val_accuracy",
                mode="max",
                save_best_only=True,
            ),
        ]
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

    # model.save("model.keras")


if __name__ == "__main__":
    main()
