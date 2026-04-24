from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
import json

import cv2
import numpy as np
import tensorflow as tf

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["POST"],
    allow_headers=["*"],
)

MODEL_PATH = Path("trained_model.keras")
DATASET_DIR = Path("dataset")
TRAIN_DIR = Path("augmented_data")
IMAGE_SIZE = (96, 96)
CONFIDENCE_THRESHOLD = 0.55

CLASS_SOURCE_DIR = TRAIN_DIR if TRAIN_DIR.exists() else DATASET_DIR
CLASS_NAMES = sorted([p.name for p in CLASS_SOURCE_DIR.iterdir() if p.is_dir() and not p.name.startswith(".")])

model = tf.keras.models.load_model(MODEL_PATH)

def preprocess_for_model(img_bgr):
    gray = cv2.cvtColor(img_bgr, cv2.COLOR_BGR2GRAY)
    resized = cv2.resize(gray, (IMAGE_SIZE[1], IMAGE_SIZE[0]), interpolation=cv2.INTER_AREA)
    x = resized.astype("float32")
    x = x[None, ..., None]  # (1, H, W, 1)
    return x

def predict_canvas(img_bgr):
    x = preprocess_for_model(img_bgr)
    raw = model.predict(x, verbose=0)[0]

    if np.all(raw >= 0.0) and np.all(raw <= 1.0) and np.isclose(np.sum(raw), 1.0, atol=1e-3):
        probs = raw
    else:
        probs = tf.nn.softmax(raw).numpy()

    idx = int(np.argmax(probs))
    conf = float(probs[idx])
    label = CLASS_NAMES[idx]
    if conf < CONFIDENCE_THRESHOLD:
        label = "unknown"
    return label, conf

class Handler(BaseHTTPRequestHandler):
    def _send_json(self, code, payload):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Access-Control-Allow-Origin", "*")
        self.end_headers()
        self.wfile.write(body)

    def do_OPTIONS(self):
        self.send_response(200)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Methods", "POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type, Content-Length")
        self.end_headers()

    def do_POST(self):
        if self.path != "/predict":
            self._send_json(404, {"error": "Not found"})
            return

        try:
            length = int(self.headers.get("Content-Length", "0"))
            data = self.rfile.read(length)
            arr = np.frombuffer(data, dtype=np.uint8)
            img = cv2.imdecode(arr, cv2.IMREAD_COLOR)
            if img is None:
                self._send_json(400, {"error": "Invalid image"})
                return

            label, conf = predict_canvas(img)
            self._send_json(200, {"label": label, "confidence": conf})
        except Exception as e:
            self._send_json(500, {"error": str(e)})

if __name__ == "__main__":
    host, port = "0.0.0.0", 10000
    print(f"Listening on http://{host}:{port}")
    ThreadingHTTPServer((host, port), Handler).serve_forever()
