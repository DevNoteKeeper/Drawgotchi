# Training section of model
import tensorflow as tf


import keras_tuner as kt

# define augmentation layers to apply random transformations
# define as global variable to avoid recreating on every map call
augment_layer = tf.keras.Sequential([
        # Left/Right inversion and horizontal flip
        tf.keras.layers.RandomFlip("horizontal_vertical"),
        # +-30% rotation
        tf.keras.layers.RandomRotation(0.15),
        # +-20% zoom in/out
        tf.keras.layers.RandomZoom(0.2),
        # +-20% contrast
        tf.keras.layers.RandomContrast(0.2),
        # +-20% sharpness
        tf.keras.layers.RandomSharpness(0.2),
    ])


# Add original and multiple augmented versions of the same image to our dataset
def data_augmentation(img, label):
    augmented_list = [img]
    for _ in range(3):
        augmented_list.append(augment_layer(img, training=True))

    augmented_images = tf.concat(augmented_list, axis=0)
    repeated_labels = tf.concat([label] * 4, axis=0)
    return augmented_images, repeated_labels


def load_data():
    train_ds = tf.keras.utils.image_dataset_from_directory(
        'data',
        label_mode='categorical',
        image_size=(480, 310),
        validation_split=0.3,
        batch_size=16,
        subset='training',
        shuffle=True,
        seed=42,
        color_mode="grayscale"
    )

    remain_ds = tf.keras.utils.image_dataset_from_directory(
        'data',
        label_mode='categorical',
        image_size=(480, 310),
        validation_split=0.3,
        batch_size=16,
        subset='validation',
        shuffle=True,
        seed=42,
        color_mode="grayscale"
    )

    # Apply augmentation with originals
    train_ds = train_ds.map(data_augmentation)


    # split remain_images into val_ds and test_ds by batch count
    remain_card = remain_ds.cardinality().numpy()
    val_ds = remain_ds.take(remain_card // 2)
    test_ds = remain_ds.skip(remain_card // 2)

    return train_ds, val_ds, test_ds



# build cnn model with tunable hyperparameters
def build_model(hp):
    model = tf.keras.Sequential([
        # to extract image feature step by step
        # example: line/edge -> pattern -> ear shape/muffin shape
        # filter, [16,32, 64]: to detect simple to complex feature used 16-64
        tf.keras.layers.Conv2D(hp.Choice('filter_1', [16, 32, 64]), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        # randomly disable 30% of neurons to prevent overfitting
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),
        tf.keras.layers.Conv2D(hp.Choice('filter_2', [16, 32, 64]), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),
        tf.keras.layers.Conv2D(hp.Choice('filter_3', [16, 32, 64, 128]), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),

        # 2d to 1d to use dense
        tf.keras.layers.Flatten(),
        # combine feature
        # units, [64, 128]: to avoid overfitting with small dataset used 64-128
        tf.keras.layers.Dense(hp.Choice('units_1', [64, 128]), activation='relu'),

    ])

    second_layer = hp.Choice('units_2', [0, 64, 128])
    if second_layer > 0:
        model.add(tf.keras.layers.Dense(second_layer, activation='relu')),

    # final classification
    model.add(tf.keras.layers.Dense(2, activation="softmax"))
    # learning_rate, [0.01, 0.001, 0.0001]: to process aggressive to fine-tuned training
    model.compile(
        optimizer=tf.keras.optimizers.Adam(hp.Choice('learning_rate', [0.01, 0.001, 0.0001])),
        loss='categorical_crossentropy',
        metrics=['accuracy'])

    return model

# find the best optimization parameter using random search
def optimize_parameter(train_ds, val_ds):
    tuner = kt.RandomSearch(
        hypermodel = build_model,
        objective='val_accuracy',
        max_trials=10,
        project_name="muffin_chihuahua_tuning"
    )
    tuner.search(
        train_ds,
        validation_data=val_ds,
        epochs=10,
    )
    # return best parameter [0] -> 1st place
    return tuner.get_best_hyperparameters()[0]


# train data and monitor performance with validation data
def train_model(train_ds, val_ds, best_hp):
    model = tf.keras.Sequential([
        tf.keras.layers.Conv2D(best_hp.get('filter_1'), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),
        tf.keras.layers.Conv2D(best_hp.get('filter_2'), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),
        tf.keras.layers.Conv2D(best_hp.get('filter_3'), 3, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.001)),
        tf.keras.layers.Dropout(0.3),
        tf.keras.layers.MaxPooling2D(),

        tf.keras.layers.Flatten(),
        tf.keras.layers.Dense(best_hp.get('units_1'), activation='relu'),

    ])

    second_layer = best_hp.get('units_1')
    if second_layer > 0:
        tf.keras.layers.Dense(second_layer, activation='relu'),


    model.add(tf.keras.layers.Dense(2, activation="softmax"))

    model.compile(
        optimizer=tf.keras.optimizers.Adam(best_hp.get('learning_rate')),
        loss='categorical_crossentropy',
        metrics=['accuracy'])


    hist = model.fit(
        train_ds,
        validation_data=val_ds,
        callbacks=[tf.keras.callbacks.EarlyStopping(monitor='val_loss', patience=10, restore_best_weights=True)],
        epochs=30)

    return model, hist

# Training and optimization process
def do_training():
    train_ds, val_ds, test_ds = load_data()
    best_hp = optimize_parameter(train_ds, val_ds)
    model, history = train_model(train_ds, val_ds, best_hp)

    model.save('model.keras')

    return model, history, test_ds

# Allow you to only run training
if __name__ == '__main__':
    do_training()