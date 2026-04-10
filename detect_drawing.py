
from pathlib import Path
import time

import cv2
import numpy as np
import tensorflow as tf


MODEL_PATH = Path("best_model.keras")
DATASET_DIR = Path("dataset")
TRAIN_DIR = Path("augmented_data")
WIN = "canvas"

# Must match train_model.py:
# image_size = (480, 310) -> (height, width)
IMAGE_SIZE = (96, 96)

# image_dataset_from_directory() defaults to alphabetical class order.
# Prefer augmented_data because train_model.py trains from that folder.
CLASS_SOURCE_DIR = TRAIN_DIR if TRAIN_DIR.exists() else DATASET_DIR
CLASS_NAMES = sorted(
    [p.name for p in CLASS_SOURCE_DIR.iterdir() if p.is_dir() and not p.name.startswith(".")]
)

CONFIDENCE_THRESHOLD = 0.55
BRUSH_THICKNESS = 12

canvas = None
drawing = False
prev_pos = None
last_stroke_time = 0.0
last_prediction_time = 0.0
prediction_label = "Draw something..."
prediction_conf = 0.0


def make_canvas():
    h = cv2.getWindowImageRect(WIN)[3] or 1080
    w = cv2.getWindowImageRect(WIN)[2] or 1920
    return np.full((h, w, 3), 255, dtype=np.uint8)


def mouse_cb(event, x, y, flags, param):
    global drawing, prev_pos, last_stroke_time, canvas

    if event == cv2.EVENT_LBUTTONDOWN:
        drawing = True
        prev_pos = (x, y)
        last_stroke_time = time.time()

    elif event == cv2.EVENT_LBUTTONUP:
        drawing = False
        prev_pos = None
        last_stroke_time = time.time()

    elif event == cv2.EVENT_MOUSEMOVE and drawing:
        if prev_pos is not None:
            cv2.line(
                canvas,
                prev_pos,
                (x, y),
                (0, 0, 0),
                thickness=BRUSH_THICKNESS,
                lineType=cv2.LINE_AA,
            )
        prev_pos = (x, y)
        last_stroke_time = time.time()


def has_ink(img):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    # Count dark pixels; helps avoid predicting on a blank white canvas
    return np.count_nonzero(gray < 245) > 200


def preprocess_for_model(img):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    resized = cv2.resize(
        gray,
        (IMAGE_SIZE[1], IMAGE_SIZE[0]),  # cv2 uses (width, height)
        interpolation=cv2.INTER_AREA,
    )
    # Keep pixel range [0, 255]. The model already has keras.layers.Rescaling(1./255).
    x = resized.astype("float32")
    x = x[None, ..., None]  # shape: (1, H, W, 1)
    return x


def predict_canvas(model, img):
    x = preprocess_for_model(img)
    raw = model.predict(x, verbose=0)[0]

    # If outputs already look like probabilities, avoid applying softmax twice.
    if np.all(raw >= 0.0) and np.all(raw <= 1.0) and np.isclose(np.sum(raw), 1.0, atol=1e-3):
        probs = raw
    else:
        probs = tf.nn.softmax(raw).numpy()

    idx = int(np.argmax(probs))
    conf = float(probs[idx])
    label = CLASS_NAMES[idx]

    if conf < CONFIDENCE_THRESHOLD:
        return "unknown", conf

    return label, conf



def main():
    global canvas, prediction_label, prediction_conf, last_prediction_time

    if not MODEL_PATH.exists():
        raise FileNotFoundError(f"Could not find {MODEL_PATH}")

    model = tf.keras.models.load_model(MODEL_PATH)

    if len(CLASS_NAMES) != int(model.output_shape[-1]):
        raise ValueError(
            "Class count mismatch: "
            f"model outputs {int(model.output_shape[-1])} classes, "
            f"but found {len(CLASS_NAMES)} folders in {CLASS_SOURCE_DIR}"
        )

    cv2.namedWindow(WIN, cv2.WINDOW_NORMAL)
    cv2.setWindowProperty(WIN, cv2.WND_PROP_FULLSCREEN, cv2.WINDOW_FULLSCREEN)
    cv2.setMouseCallback(WIN, mouse_cb)

    # Create an initial fullscreen canvas
    cv2.imshow(WIN, np.zeros((310, 480, 3), dtype=np.uint8))
    cv2.waitKey(1)
    canvas = make_canvas()

    while True:
        now = time.time()

        # Live prediction after the user pauses drawing
        if (
            has_ink(canvas)
            and not drawing
            and (now - last_stroke_time) > 0.4
            and (now - last_prediction_time) > 0.7
        ):
            prediction_label, prediction_conf = predict_canvas(model, canvas)
            last_prediction_time = now

        display = canvas.copy()
        cv2.putText(
            display,
            f"Prediction: {prediction_label} ({prediction_conf:.2f})",
            (10, 35),
            cv2.FONT_HERSHEY_SIMPLEX,
            0.9,
            (20, 60, 200),
            2,
            cv2.LINE_AA,
        )

        cv2.imshow(WIN, display)
        key = cv2.waitKey(16) & 0xFF

        if key in (ord("q"), 27):
            break
        elif key == ord("c"):
            canvas[:] = 255
            prediction_label = "cleared"
            prediction_conf = 0.0

    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
