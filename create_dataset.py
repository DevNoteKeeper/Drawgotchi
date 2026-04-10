from pathlib import Path
import numpy as np
import cv2

DATASET_DIR = Path("dataset")
WIN         = "canvas"
label       = "bed"
canvas      = None
drawing     = False
prev_pos    = None
entering_label = False
label_input = ""


def make_canvas():
    h = cv2.getWindowImageRect(WIN)[3] or 1080
    w = cv2.getWindowImageRect(WIN)[2] or 1920
    return np.full((h, w, 3), 255, dtype=np.uint8)


def save():
    out = DATASET_DIR / label
    out.mkdir(parents=True, exist_ok=True)
    count = len(list(out.glob("*.png")))
    path  = out / f"{count+1:06d}.png"
    cv2.imwrite(str(path), canvas)
    print(f"[Saved] {path}")


def mouse_cb(event, x, y, flags, param):
    global drawing, prev_pos
    if event == cv2.EVENT_LBUTTONDOWN:
        drawing  = True
        prev_pos = (x, y)
    elif event == cv2.EVENT_LBUTTONUP:
        drawing  = False
        prev_pos = None
    elif event == cv2.EVENT_MOUSEMOVE and drawing:
        if prev_pos:
            cv2.line(canvas, prev_pos, (x, y), (0, 0, 0),
                     thickness=6, lineType=cv2.LINE_AA)
        prev_pos = (x, y)


# ── Setup ─────────────────────────────────────────────────────────────────────
cv2.namedWindow(WIN, cv2.WINDOW_NORMAL)
cv2.setWindowProperty(WIN, cv2.WND_PROP_FULLSCREEN, cv2.WINDOW_FULLSCREEN)
cv2.setMouseCallback(WIN, mouse_cb)

# Get actual fullscreen size after a first render
cv2.imshow(WIN, np.zeros((310, 480, 3), dtype=np.uint8))
cv2.waitKey(1)
rect   = cv2.getWindowImageRect(WIN)
H, W   = rect[3], rect[2]
canvas = np.full((H, W, 3), 255, dtype=np.uint8)

# ── Main loop ──────────────────────────────────────────────────────────────────
while True:
    display = canvas.copy()
    if entering_label:
        cv2.putText(display, f"label: {label_input} ", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.7, (200, 50, 50), 2)
        cv2.imshow(WIN, display)

        key = cv2.waitKey(16) & 0xFF
        if key == 13:    # Enter — confirm
            label = label_input or label
            entering_label = False
        elif key == 8 and label_input or key == 127:    # Backspace or Delete
            label_input = label_input[:-1]
        elif 32 <= key < 127:    # Printable ASCII
            label_input += chr(key).lower()
    else:
        cv2.putText(display, f"Label: {label} | (S)ave (C)lear (1-6)Label (Q)uit",
                    (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (200, 50, 50), 2)
        cv2.imshow(WIN, display)

        key = cv2.waitKey(16) & 0xFF

        if key in (ord('q'), 27):
            break
        elif key in (ord('s'), 13):
            save()
            canvas[:] = 255
        elif key == ord('c'):
            canvas[:] = 255
        elif key == ord('1'):
            label = "bed"
        elif key == ord('2'):
            label = "bread"
        elif key == ord('3'):
            label = "meat"
        elif key == ord('4'):
            label = "drink"
        elif key == ord('5'):
            label = "noodle"
        elif key == ord('6'):
            label = "vegetable"


cv2.destroyAllWindows()